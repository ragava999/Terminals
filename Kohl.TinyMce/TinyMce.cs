namespace Kohl.TinyMce
{
    using ICSharpCode.SharpZipLib.Zip;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// TinyMCE editor encapsulated in a manged assembly.
    /// </summary>
    public class TinyMce : UserControl
    {
        public new event EventHandler TextChanged;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        // The subdirectory in ApplicationData folder - name can be changed.
        private readonly string workpath;

        // The instance to the Windows.Forms.WebBrowser.
        private WebBrowser webBrowser;

        //easilly navigate into resources
        private readonly string resource;

        private string mceSettings = "";
        private string fileName = "", additionalCssFile = "";

        private bool isTextEditable = true;

        public bool IsTextEditable
        {
            get
            {
                if (this.InvokeRequired)
                    return (bool)this.Invoke(new Func<bool>(delegate { return isTextEditable; }));

                return isTextEditable;
            }
            set
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(delegate { isTextEditable = value; }));
                else
                    isTextEditable = value;
            }
        }

        public new string Text
        {
            get
            {
                if (this.InvokeRequired)
                    return (string)this.Invoke(new Func<string>(delegate { return base.Text; }));

                return base.Text;
            }
            set
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(delegate { base.Text = value; }));
                else
                    base.Text = value;
            }
        }

        /// <summary>
        /// TinyMCE editor encapsulated in a manged assembly.
        /// </summary>
        public TinyMce()
        {
            this.resource = this.GetType().Namespace;
            this.workpath = Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            InitializeComponent();

            if (webBrowser.InvokeRequired)
                webBrowser.Invoke(new MethodInvoker(delegate { webBrowser.Navigate("about:blank"); }));
            else
                webBrowser.Navigate("about:blank");

            this.TextChanged += (sender, args) => { };
        }

        private static readonly object preparationLock = new object();
        private static readonly object setTextLock = new object();

        /// <summary>
        /// Prepare tinyMCE package
        /// </summary>
        public void PrepareTinyMce(bool removeSaveButton = false)
        {
            lock (preparationLock)
            {
                if (!string.IsNullOrEmpty(mceSettings))
                    return;

                mceSettings = GetWorkPath("tinyMCE.txt");
                string workingDirectory = GetWorkPath("tinyMCE");

                if (!Directory.Exists(GetWorkPath()))
                {
                    Directory.CreateDirectory(GetWorkPath());
                }


                //const string tinyMceDirectory = "tinyMCE\\tinyMCE\\jscripts\\tiny_mce";
                const string tinyMceDirectory = "tinymce\\js\\tinymce";

                // Extract the tiny mce editor
                if (!Directory.Exists(workingDirectory) || !File.Exists(GetWorkPath(tinyMceDirectory) + "\\tiny_mce.js"))
                {
                    Directory.CreateDirectory(workingDirectory);

                    FastZip fz = new FastZip();

                    Application.DoEvents();

                    //using (Stream strFile = GetResourceFile(resource + ".resources.tinymce_3.5.8.zip"))
                    using (Stream strFile = GetResourceFile(resource + ".resources.tinymce_4.0.28.zip"))
                        fz.ExtractZip(strFile, workingDirectory, FastZip.Overwrite.Never, null, null, null, false, false);

                    Application.DoEvents();
                }

                //set the settings 
                using (StreamReader sr = new StreamReader(GetResourceFile(resource + ".resources.tinyMCE.txt")))
                    mceSettings = sr.ReadToEnd();

                if (removeSaveButton && !string.IsNullOrWhiteSpace(mceSettings))
                {
                    // 	toolbar1: "save | newdocument fullpage | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | inserttime preview | forecolor backcolor | styleselect formatselect fontselect fontsizeselect",
                    mceSettings = System.Text.RegularExpressions.Regex.Replace(mceSettings, @"(\s*toolbar[1-9][0-9]*[\w]*\s*:\s*""[\s|\w]*)(save[\s|]*)", "$1", System.Text.RegularExpressions.RegexOptions.Compiled);
                }
            }
        }

        public Stream GetResourceFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return null;

            try
            {
                return this.GetType().Assembly.GetManifestResourceStream(file);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Render(bool? isTextEditable = null)
        {
            PrepareTinyMce();
            SetText(isTextEditable);
        }

        public void SetText(string text, string fileName, string additionalCssFile = null, bool isTextEditable = true)
        {
            this.Text = text;
            this.fileName = fileName;
            this.additionalCssFile = additionalCssFile;
            SetText(isTextEditable);
        }

        System.Threading.Thread thread = null;

        private void SetText(bool? edit)
        {
            string sDoc = null;

            lock (setTextLock)
            {
                if (edit.HasValue)
                    isTextEditable = edit.Value;

                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = resource;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<HTML><HEAD>");
                if (isTextEditable && !string.IsNullOrEmpty(mceSettings))
                {
                    sb.AppendLine(this.mceSettings);
                    sb.AppendLine("</HEAD><BODY><FONT FACE=\"Arial\" SIZE=\"-1\">");
                    sb.AppendLine("<form method=\"\" action=\"\" onsubmit=\"return false;\">");
                    sb.AppendLine("<textarea id=\"elm1\" name=\"elm1\" style=\"width:100%\">");
                }
                else
                {
                    sb.AppendLine("</HEAD><BODY><FONT FACE=\"Arial\" SIZE=\"-1\">");
                }
                sb.AppendLine(this.Text);
                if (isTextEditable && !string.IsNullOrEmpty(mceSettings))
                {
                    sb.AppendLine("</textarea>");
                }
                sb.AppendLine("</FONT></BODY></HTML>");

                //get CSS to local folder
                if (File.Exists(additionalCssFile))
                {
                    File.Copy(additionalCssFile, GetWorkPath(fileName + ".css"), true);
                    additionalCssFile = fileName + ".css";
                }
                string t = sb.Replace("[CSS]", additionalCssFile).ToString();
                sDoc = GetWorkPath(fileName + ".html");

                if (Directory.Exists(GetWorkPath()))
                {
                    using (StreamWriter stream = new StreamWriter(sDoc, false, System.Text.Encoding.GetEncoding(1250)))
                    {
                        stream.Write(t);
                    }

                    if (thread == null)
                    {
                        thread = new System.Threading.Thread((System.Threading.ThreadStart)delegate
                        {
                            do
                            {
                                lock (locker)
                                {
                                    if (webBrowser == null || webBrowser.Disposing || webBrowser.IsDisposed || this.IsDisposed || this.Disposing)
                                        continue;

                                    System.Threading.Thread.Sleep(250);
                                    Application.DoEvents();

                                    try
                                    {
                                        if (webBrowser.InvokeRequired)
                                            webBrowser.Invoke(new MethodInvoker(delegate { SetText(); }));
                                        else
                                            SetText();

                                        Application.DoEvents();
                                    }
                                    catch (Exception ex)
                                    {
                                        string message = ex.Message;
                                    }
                                }
                            } while (true);
                        });

                        thread.Start();
                    }
                }
            }

            if (Directory.Exists(GetWorkPath()))
            {
                try
                {
                    if (webBrowser.InvokeRequired)
                        webBrowser.Invoke(new MethodInvoker(delegate { webBrowser.Navigate(sDoc); }));
                    else
                        webBrowser.Navigate(sDoc);
                }
                catch (Exception e)
                {
                    string s = e.Message;
                }
            }
        }

        public void Save()
        {
            object result = InvokeScript("if (typeof tinyMCE != 'undefined') { return tinyMCE.activeEditor.getContent();}");

            if (result != null)
                this.Text = result.ToString();
        }

        private object InvokeScript(string script)
        {
            HtmlDocument document = null;

            if (webBrowser.InvokeRequired)
                webBrowser.Invoke(new MethodInvoker(delegate { document = webBrowser.Document; }));
            else
                document = webBrowser.Document;

            HtmlElement head = document.GetElementsByTagName("head")[0];
            HtmlElement scriptElement = document.CreateElement("script");

            string fn = GetScriptFunctionName(ref script);

            scriptElement.SetAttribute("text", script);
            head.AppendChild(scriptElement);

            return document.InvokeScript(fn);
        }

        private string GetScriptFunctionName(ref string script)
        {
            string functionName = "FN_" + Guid.NewGuid().ToString().Replace("-", "");

            if (!script.ToUpper().Contains("FUNCTION"))
            {
                script = "function " + functionName + "() {" + script + "}";
            }
            else
            {
                int startIndex = script.ToUpper().IndexOf("FUNCTION");
                int endIndex = script.ToUpper().IndexOf("(", startIndex) - 1;

                functionName = script.Substring(startIndex, endIndex);
            }

            return functionName;
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                lock (locker)
                {
                    if (webBrowser == null || webBrowser.Disposing || webBrowser.IsDisposed || this.IsDisposed || this.Disposing)
                        continue;

                    System.Threading.Thread.Sleep(250);
                    Application.DoEvents();

                    try
                    {
                        if (webBrowser.InvokeRequired)
                            webBrowser.Invoke(new MethodInvoker(delegate { SetText(); }));
                        else
                            SetText();

                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Get the correct internal path where tinyMCE and work file will be stored
        /// </summary>
        /// <param name="subPath"></param>
        /// <returns></returns>
        private string GetWorkPath(string subPath = null)
        {
            if (!string.IsNullOrEmpty(subPath))
                subPath = Path.Combine(workpath, subPath);
            else
                subPath = workpath;

            return Path.Combine(Path.GetTempPath(), subPath);
        }

        public virtual void OnTextChanged(object sender, EventArgs e)
        {
            if (this.TextChanged != null)
                this.TextChanged(sender, e);
        }

        private static readonly object locker = new object();
        private static readonly object disposeLocker = new object();

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (locker)
            {
                if (webBrowser == null || webBrowser.Disposing || webBrowser.IsDisposed || this.IsDisposed || this.Disposing)
                    return;

                try
                {
                    if (webBrowser.InvokeRequired)
                        webBrowser.Invoke(new MethodInvoker(delegate { SetText(); }));
                    else
                        SetText();
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            }
        }

        private void SetText()
        {
            if (webBrowser == null || webBrowser.Document == null)
                return;

            HtmlDocument document = null;

            if (webBrowser.InvokeRequired)
                webBrowser.Invoke(new MethodInvoker(delegate { document = webBrowser.Document; }));
            else
                document = webBrowser.Document;

            dynamic domdocument = document.DomDocument;

            dynamic element = null;

            try
            {
                element = domdocument.getElementById("elm1");
            }
            catch
            {
                return;
            }

            if (element != null)
            {
                string innerText = string.Empty;

                // Check if an inner Text exists
                try
                {
                    innerText = element.innerText;
                }
                catch
                {
                    return;
                }

                if (this.Text != innerText)
                {
                    string oldText = this.Text;
                    this.Text = innerText;
                    this.OnTextChanged(oldText, EventArgs.Empty);
                }
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            lock (disposeLocker)
            {
                if (disposing)
                {
                    if (thread != null)
                    {
                        if (thread.IsAlive)
                            thread.Abort();

                        thread = null;
                    }

                    string path = GetWorkPath();

                    if (Directory.Exists(path))
                        try
                        {
                            Directory.Delete(path, true);
                        }
                        catch
                        { }

                    if (webBrowser != null && !webBrowser.Disposing && !webBrowser.IsDisposed)
                    {
                        if (webBrowser.InvokeRequired)
                            webBrowser.Invoke(new MethodInvoker(() => webBrowser.Dispose()));
                        else
                            webBrowser.Dispose();

                        webBrowser = null;
                    }

                    if (components != null)
                    {
                        components.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(284, 262);
            this.webBrowser.TabIndex = 0;
            // 
            // TinyMce
            // 
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate { this.Controls.Add(this.webBrowser); }));
            else
                this.Controls.Add(this.webBrowser);

            this.Name = "TinyMce";
            this.Size = new System.Drawing.Size(284, 262);
            this.ResumeLayout(false);
        }
        #endregion
    }
}
