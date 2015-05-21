using Terminals.Connection.Manager;

namespace Terminals.Connections
{
    // .NET namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows.Forms;
    using Microsoft.Win32;
	using Terminals.CommandLine;
    
    #if GECKO
	// Gecko namespaces
	using Gecko;
	using Gecko.DOM;
	using Gecko.IO;
    #endif


    // Terminals and framework namespaces
    using Kohl.Framework.Info;

    using Configuration.Files.Main.Favorites;
    using Connection;
    using Properties;

    [PermissionSet(SecurityAction.PermitOnly, Name = "FullTrust")]
    public partial class MiniBrowser : UserControl, IDisposable
    {
        public static readonly string[] FieldConstants =
        {
            ConnectionManager.ParsingConstants.UserName,
            ConnectionManager.ParsingConstants.Password,
            ConnectionManager.ParsingConstants.DomainName,
            ConnectionManager.ParsingConstants.UserNameWithDomain,
            ConnectionManager.ParsingConstants.GetDateTimeFormat("en-GB"),
            ConnectionManager.ParsingConstants.GetDateTimeFormat("en-US"),
            ConnectionManager.ParsingConstants.GetDateTimeFormat("de-AT"),
            ConnectionManager.ParsingConstants.Click,
            ConnectionManager.ParsingConstants.Redirect,
            ConnectionManager.ParsingConstants.Script
        };

        private BrowserCredentials browserCredential = new BrowserCredentials();
        private BrowserType browserType = BrowserType.InternetExplorer;

        
        #if GECKO
        private GeckoWebBrowser firefox;
        #endif
        
        private string homeUrl = string.Empty;
        private IE internetExplorer;

        private int repeatedClickCount;

        public MiniBrowser()
        {
            this.InitializeComponent();
        }

        public BrowserType BrowserType
        {
            get { return this.browserType; }
            set
            {
                this.browserType = value;
                this.InitializeBrowser();
            }
        }

        public BrowserCredentials BrowserCredential
        {
            get { return this.browserCredential; }
            set
            {
                if (value != null)
                {
                    this.browserCredential = value;
                }
            }
        }

        public HtmlFormField[] HtmlFormFields { private get; set; }

        public string Home { private get; set; }

        public new void Dispose()
        {
            if (this.internetExplorer != null)
            {
                this.internetExplorer.Stop();
                this.internetExplorer.Dispose();
                this.internetExplorer = null;
                base.Dispose();
            }
            #if GECKO
            else if (this.firefox != null)
            {
                this.firefox.Stop();
                this.firefox.Dispose();
                this.firefox = null;
                base.Dispose();
            }
            #endif
        }

        public event Action<object, string> UrlActivated;

        public event EventHandler BackActivated
        {
            add { this.backButton.Click += value; }
            remove { this.backButton.Click -= value; }
        }

        public event EventHandler ForwardActivated
        {
            add { this.forwardButton.Click += value; }
            remove { this.forwardButton.Click -= value; }
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        public void SetAddress(string address)
        {
            this.urlTextBox.Text = address;
        }

        public void SetCanGoBack(bool can_go_back)
        {
            this.backButton.Enabled = can_go_back;
        }

        public void SetCanGoForward(bool can_go_forward)
        {
            this.forwardButton.Enabled = can_go_forward;
        }

        public void SetIsLoading(bool is_loading)
        {
            this.goButton.Text = is_loading
                                     ? "Stop"
                                     : "Go";
            this.goButton.Image = is_loading ? Resources.red : Resources.green;

            if (!is_loading)
            {
                this.FillFormFields();
            }
        }

        ~MiniBrowser()
        {
            this.Dispose();
        }

        #if IEWORKAROUND
        [System.Runtime.InteropServices.DllImport("urlmon.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
		private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);
		const int URLMON_OPTION_USERAGENT = 0x10000001;
		const int URLMON_OPTION_USERAGENT_REFRESH = 0x10000002;
		
        /// <summary>
	    /// Fires before navigation occurs in the given object (on either a window or frameset element).
	    /// </summary>
	    /// <param name="pDisp">Object that evaluates to the top level or frame WebBrowser object corresponding to the navigation.</param>
	    /// <param name="url">String expression that evaluates to the URL to which the browser is navigating.</param>
	    /// <param name="Flags">Reserved. Set to zero.</param>
	    /// <param name="TargetFrameName">String expression that evaluates to the name of the frame in which the resource will be displayed, or Null if no named frame is targeted for the resource.</param>
	    /// <param name="PostData">Data to send to the server if the HTTP POST transaction is being used.</param>
	    /// <param name="Headers">Value that specifies the additional HTTP headers to send to the server (HTTP URLs only). The headers can specify such things as the action required of the server, the type of data being passed to the server, or a status code.</param>
	    /// <param name="Cancel">Boolean value that the container can set to True to cancel the navigation operation, or to False to allow it to proceed.</param>
	    private delegate void BeforeNavigate2(object pDisp, ref dynamic url, ref dynamic Flags, ref dynamic TargetFrameName, ref dynamic PostData, ref dynamic Headers, ref bool Cancel);
	
	    // bool renavigating = false;
	    
	    const string IE10 = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
	    const string IE11 = "Mozilla/5.0 (compatible, MSIE 11, Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";
	   #endif
	   
	   public enum BrowserEmulationVersion
		{
		  Default = 0,
		  Version7 = 7000,
		  Version8 = 8000,
		  Version8Standards = 8888,
		  Version9 = 9000,
		  Version9Standards = 9999,
		  Version10 = 10000,
		  Version10Standards = 10001,
		  Version11 = 11000,
		  Version11Edge = 11001
		}
	   
	   
	   // Code taken from http://www.codeproject.com/Articles/793687/Configuring-the-emulation-mode-of-an-Internet-Expl
	   // Copyright belongs to the orignal author
	   private const string InternetExplorerRootKey = @"Software\Microsoft\Internet Explorer";
	   private const string BrowserEmulationKey = InternetExplorerRootKey + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
	   
	   public static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion)
		{
		  bool result;
		
		  result = false;
		
		  try
		  {
		    RegistryKey key;
		
		    key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
		
		    if (key != null)
		    {
		      string programName;
		
		      programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
		
		      if (browserEmulationVersion != BrowserEmulationVersion.Default)
		      {
		        // if it's a valid value, update or create the value
		        key.SetValue(programName, (int)browserEmulationVersion, RegistryValueKind.DWord);
		      }
		      else
		      {
		        // otherwise, remove the existing value
		        key.DeleteValue(programName, false);
		      }
		
		      result = true;
		    }
		  }
		  catch (Exception ex)
		  {
		  	Kohl.Framework.Logging.Log.Error("Unable to set browser emulation mode", ex);
		  }
		
		  return result;
		}
		
	   public static int GetInternetExplorerMajorVersion()
		{
		  int result;
		
		  result = 0;
		
		  try
		  {
		    RegistryKey key;
		
		    key = Registry.LocalMachine.OpenSubKey(InternetExplorerRootKey);
		
		    if (key != null)
		    {
		      object value;
		
		      value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);
		
		      if (value != null)
		      {
		        string version;
		        int separator;
		
		        version = value.ToString();
		        separator = version.IndexOf('.');
		        if (separator != -1)
		        {
		          int.TryParse(version.Substring(0, separator), out result);
		        }
		      }
		    }
		  }
		  catch (Exception ex)
		  {
		  	Kohl.Framework.Logging.Log.Error("Unable to set browser emulation mode", ex);
		  }
		
		  return result;
		}
	   
		public static bool SetBrowserEmulationVersion()
		{
		  int ieVersion;
		  BrowserEmulationVersion emulationCode;
		
		  ieVersion = GetInternetExplorerMajorVersion();
		
		  Kohl.Framework.Logging.Log.Info("Setting browser emulation mode to version " + ieVersion);
		  
		  if (ieVersion >= 11)
		  {
		    emulationCode = BrowserEmulationVersion.Version11;
		  }
		  else
		  {
		    switch (ieVersion)
		    {
		      case 10:
		        emulationCode = BrowserEmulationVersion.Version10;
		        break;
		      case 9:
		        emulationCode = BrowserEmulationVersion.Version9;
		        break;
		      case 8:
		        emulationCode = BrowserEmulationVersion.Version8;
		        break;
		      default:
		        emulationCode = BrowserEmulationVersion.Version7;
		        break;
		    }
		  }
		
		  return SetBrowserEmulationVersion(emulationCode);
		}
	   
        private void InitializeBrowser()
        {
            this.Dispose();

            switch (this.browserType)
            {
                case BrowserType.InternetExplorer:
            		this.SetIsLoading(true);
            		
            		SetBrowserEmulationVersion();
            		
                    this.internetExplorer = new IE
                                                {
                                                    Dock = DockStyle.Fill,
                                                    // Never change this to true
                                                    // There's a bug in IE!
                                                    ScriptErrorsSuppressed = false
                                                };

                    this.internetExplorer.DocumentCompleted += this.WebBrowser_DocumentCompleted;
                    
                    #if IEWORKAROUND
                    dynamic activeXIE = this.internetExplorer.ActiveXInstance;
                    activeXIE.BeforeNavigate2 += new BeforeNavigate2((object pDisp,
						ref dynamic url,
						ref dynamic Flags,
						ref dynamic TargetFrameName,
						ref dynamic PostData,
						ref dynamic Headers,
						ref bool Cancel) =>
						{	
							UrlMkSetSessionOption(URLMON_OPTION_USERAGENT_REFRESH, null, 0, 0);
							UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, IE10, IE10.Length, 0);
    						
							// The following code causes problems with AJAX - please don't resend the headers
							/*
							if (!renavigating)
							{
								const string agent = IE11;
							    Headers += string.Format("User-Agent: {0}\r\n", agent);
							    renavigating = true;
							    Cancel = true;
							    this.internetExplorer.Navigate((string)url, (string)TargetFrameName, (byte[])PostData, (string)Headers);
							}
							else
							{
							    renavigating = false;
							}
							*/        
						});
                   #endif 
                   
                    this.browserContainer.Controls.Add(this.internetExplorer);
                    break;
                #if GECKO
                case BrowserType.Firefox:
                    // Workaround
                    this.SyncCerts();

                    Xpcom.Initialize();

                    // Set some preferences
                    nsIPrefBranch pref = Xpcom.GetService<nsIPrefBranch>("@mozilla.org/preferences-service;1");

                    // Show same page as firefox does for unsecure SSL/TLS connections ...
                    pref.SetIntPref("browser.ssl_override_behavior", 1);
                    pref.SetIntPref("security.OCSP.enabled", 0);
                    pref.SetBoolPref("security.OCSP.require", false);
                    pref.SetBoolPref("extensions.hotfix.cert.checkAttributes", true);
                    pref.SetBoolPref("security.remember_cert_checkbox_default_setting", true);
                    pref.SetBoolPref("services.sync.prefs.sync.security.default_personal_cert", true);
                    pref.SetBoolPref("browser.xul.error_pages.enabled", true);
                    pref.SetBoolPref("browser.xul.error_pages.expert_bad_cert", false);

                    // disable caching of http documents
                    pref.SetBoolPref("network.http.use-cache", false);

                    // disalbe memory caching
                    pref.SetBoolPref("browser.cache.memory.enable", false);                    

                    // Desktop Notification
                    pref.SetBoolPref("notification.feature.enabled", true);
                    
                    // WebSMS
                    pref.SetBoolPref("dom.sms.enabled", true);
                    pref.SetCharPref("dom.sms.whitelist", "");

                    // WebContacts
                    pref.SetBoolPref("dom.mozContacts.enabled", true);
                    pref.SetCharPref("dom.mozContacts.whitelist", "");

                    pref.SetBoolPref("social.enabled", false);

                    // WebAlarms
                    pref.SetBoolPref("dom.mozAlarms.enabled", true);

                    // WebSettings
                    pref.SetBoolPref("dom.mozSettings.enabled", true);
                    
                    pref.SetBoolPref("network.jar.open-unsafe-types", true);
                    pref.SetBoolPref("security.warn_entering_secure",    false);
                    pref.SetBoolPref("security.warn_entering_weak",      false);
                    pref.SetBoolPref("security.warn_leaving_secure", false);
                    pref.SetBoolPref("security.warn_viewing_mixed", false);
                    pref.SetBoolPref("security.warn_submit_insecure", false);
                    pref.SetIntPref("security.ssl.warn_missing_rfc5746",  1);
                    pref.SetBoolPref("security.ssl.enable_false_start", false);
                    pref.SetBoolPref("security.enable_ssl3",             true);
                    pref.SetBoolPref("security.enable_tls", true);
                    pref.SetBoolPref("security.enable_tls_session_tickets", true);
                    pref.SetIntPref("privacy.popups.disable_from_plugins", 2);

                    // don't store passwords
                    pref.SetIntPref("security.ask_for_password", 1);
                    pref.SetIntPref("security.password_lifetime", 0);
                    pref.SetBoolPref("signon.prefillForms", false);
                    pref.SetBoolPref("signon.rememberSignons", false);
                    pref.SetBoolPref("browser.fixup.hide_user_pass", false);
                    pref.SetBoolPref("privacy.item.passwords", true);

                    this.firefox = new GeckoWebBrowser {Dock = DockStyle.Fill, AllowDrop = true};
                    this.SetIsLoading(true);
                    this.firefox.DocumentCompleted += this.FireFox_DocumentCompleted;
                    this.browserContainer.Controls.Add(this.firefox);
                    
                    break;
                #endif
            }

            
        }
        
        /// <summary>
        ///     Syncs the cert_override.txt from FireFox, Mozilla, etc.
        /// </summary>
        private void SyncCerts()
        {
            string geckodir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                           "Geckofx\\DefaultProfile\\");
            string gecko = geckodir + "cert_override.txt";

            string geckoContent = File.Exists(gecko) ? File.ReadAllText(gecko) : null;

            // Get every cert_override.txt file for every Mozilla app
            string mozilla = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                          "Mozilla");
            string result = "";
            foreach (string file in this.FindFiles(mozilla, "cert_override.txt"))
            {
                result += Environment.NewLine;
                StreamReader stream = new StreamReader(file);
                while (stream.Peek() >= 0)
                {
                    string line = stream.ReadLine();
                    if (line.StartsWith("#"))
                        continue;

                    if (string.IsNullOrEmpty(geckoContent) || !geckoContent.Contains(line))
                    {
                        result += Environment.NewLine + line;
                    }
                }
            }

            if (!string.IsNullOrEmpty(result.Trim()))
            {
                try
                {
                    if (!Directory.Exists(geckodir))
                        Directory.CreateDirectory(geckodir);
                }
                catch
                {
                }

                try
                {
                    File.WriteAllText(gecko, result, Encoding.UTF8);
                }
                catch
                {
                }
            }
        }

        private IEnumerable<string> FindFiles(string sDir, string file)
        {
            if (!string.IsNullOrEmpty(sDir) && !string.IsNullOrEmpty(file) && new DirectoryInfo(sDir).Exists)
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, file))
                    {
                        yield return f;
                    }
                    foreach (string f in this.FindFiles(d, file)) yield return f;
                }
            }
        }

        #if GECKO
        private void FireFox_DocumentCompleted(object sender, EventArgs e)
        {
            this.SetIsLoading(false);

            GeckoWebBrowser browser = sender as GeckoWebBrowser;

            if (browser.Url.ToString() == "about:blank")
            {
                return;
            }

            // don't set the url in the textbox if it contains javascript code
            if (browser != null && browser.Url != null && !string.IsNullOrWhiteSpace(browser.Url.ToString()))
            	if (!browser.Url.ToString().Trim().StartsWith("javascript:", StringComparison.InvariantCultureIgnoreCase))
					urlTextBox.Text = browser.Url.ToString();
			
            this.FillFormFields();
        }
        #endif

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {        	
            if (e.Url.AbsoluteUri.Equals("about:blank"))
            {
                return;
            }

            this.SetIsLoading(false);
            
            // don't set the url in the textbox if it contains javascript code
            if (e != null && e.Url != null && !string.IsNullOrWhiteSpace(e.Url.AbsoluteUri))
            	if (!e.Url.AbsoluteUri.Trim().StartsWith("javascript:", StringComparison.InvariantCultureIgnoreCase))
					urlTextBox.Text = e.Url.AbsoluteUri;
			
            this.FillFormFields();
        }

        private void FillFormFields()
        {
            if (this.HtmlFormFields != null)
            {
                // Skip certificate warnings in IE.
                if (this.browserType == BrowserType.InternetExplorer)
                {
                    if (internetExplorer.Document.Body.InnerHtml.Contains("overridelink") && internetExplorer.Document.Body.InnerHtml.Contains("'infoBlockID'"))
                    {
                        this.internetExplorer.Document.GetElementById("overridelink").InvokeMember("click");
                        Kohl.Framework.Logging.Log.Info("Certificate warning has been skipped: " + this.urlTextBox.Text);
                        return;
                    }
                }

                // If there is not authentication return configured
                if (this.browserCredential.Authentication == BrowserAuthentication.None)
                {
                    Kohl.Framework.Logging.Log.Info("Ignoring form fields for URL: " + this.urlTextBox.Text);
                    return;
                }

                // TODO: KOHL> Add PowerShell script that gets some input parameters -> btw this is needed for the generic connection too! -> maybe implement an afterwards script, before connect, and timer configureable script! Explicitly let the user choose between ID, Name and e.g. CSS class -> don't use any automagic. React on document.onready, load-events, button clicks, explicit page urls and titles etc. to be more dynamic
                foreach (HtmlFormField htmlFormField in this.HtmlFormFields)
                {
                    this.FillFormField(htmlFormField.Key, htmlFormField.Value);
                }
            }
        }

        private bool redirected = false;
        string prevScript = "";

        private void FillFormField(string id, string value)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(value))
                return;

            // Only fill out the form field if we have an id
            if (!string.IsNullOrEmpty(id) || value == ConnectionManager.ParsingConstants.Click)
            {
                #region IE
                if (this.browserType == BrowserType.InternetExplorer)
                {
                    if (this.internetExplorer.Document == null)
                        return;

                    HtmlElement element = this.internetExplorer.Document.GetElementById(id);

                    // The element hasn't been found -> try to find it by name
                    if (element == null)
                    {
                        HtmlElementCollection collection = this.internetExplorer.Document.All;

                        foreach (HtmlElement e in collection)
                        {
                            if (e.Name == id)
                            {
                                element = e;
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(id) && element != null)
                    {
                        if (value != ConnectionManager.ParsingConstants.Click & value != ConnectionManager.ParsingConstants.Redirect && value != ConnectionManager.ParsingConstants.Script)
                        {
                            element.SetAttribute("value", this.ParseValue(value));
                        }
                        else if (value == ConnectionManager.ParsingConstants.Click)
                        {
                            // Prevent the browser from looping
                            // e.g. a GMail user has entered a wrong pwd
                            // the browser would now loop until the site
                            // (which will never change) chages.
                            if (this.repeatedClickCount >= 1)
                            {
                                return;
                            }

                            this.repeatedClickCount++;

                            element.InvokeMember("click");
                        }
                    }
                    else if (value == ConnectionManager.ParsingConstants.Click)
                    {
                        HtmlElementCollection collection = this.internetExplorer.Document.All;

                        if (collection == null)
                        {
                            return;
                        }

                        for (int i = 0; i < collection.Count; i++)
                        {
                            element = collection[i];

                            string type = element.GetAttribute("type");

                            if (!string.IsNullOrEmpty(type))
                            {
                                if (element.GetAttribute("type").ToLowerInvariant() == "submit")
                                {
                                    // Prevent the browser from looping
                                    // e.g. a GMail user has entered a wrong pwd
                                    // the browser would now loop until the site
                                    // (which will never change) chages.
                                    if (this.repeatedClickCount >= 1)
                                    {
                                        return;
                                    }

                                    this.repeatedClickCount++;

                                    element.InvokeMember("click");
                                }
                            }
                        }
                    }
                    else if (value == ConnectionManager.ParsingConstants.Redirect & redirected == false) 
                    {
                        redirected = true;
                        this.Home = id;
                        this.internetExplorer.Navigate(id);
                    }
                    else if (value == ConnectionManager.ParsingConstants.Script)
                    {
                        string script = id;
                        string functionName = GetScriptFunctionName(ref script);

                        HtmlElement head = internetExplorer.Document.GetElementsByTagName("head")[0];
                        HtmlElement scriptElement = internetExplorer.Document.CreateElement("script");
                        scriptElement.SetAttribute("text", script);
                        head.AppendChild(scriptElement);
                       
                        this.internetExplorer.Document.InvokeScript(functionName);
                    }
                }
                #endregion
                #region FF
                #if GECKO
                else if (this.browserType == BrowserType.Firefox)
                {
                    if (this.firefox.Document != null)
                    {
                        GeckoElement element = this.firefox.Document.GetElementById(id);

                        bool useName = false;

                        // The element hasn't been found -> try to find it by name
                        if (element == null)
                        {
                            GeckoElementCollection collection = this.firefox.Document.GetElementsByName(id);

                            if (collection != null && collection.Length >= 1)
                                element = collection[0];

                            useName = true;

                            if (element == null && value != ConnectionManager.ParsingConstants.Redirect && value != ConnectionManager.ParsingConstants.Click && value != ConnectionManager.ParsingConstants.Script)
                            {
                                return;
                            }
                        }

                        if (value != ConnectionManager.ParsingConstants.Click && value != ConnectionManager.ParsingConstants.Redirect && value != ConnectionManager.ParsingConstants.Script)
                        {
                            // The below code doesn't work
                            // element.SetAttribute("value", this.ParseValue(value));

                            using (AutoJSContext context = new AutoJSContext(firefox.Window.JSContext))
                            {
                                string result;
                                
                                // by id
                                if(!useName)
                                    context.EvaluateScript(string.Format("document.getElementById('{0}').value = '{1}';", id, this.ParseValue(value)), out result);
                                // by name
                                else
                                    context.EvaluateScript(string.Format("document.GetElementsByName('{0}')[0].value = '{1}';", id, this.ParseValue(value)), out result);

                                if (result.Equals("undefined"))
                                {
                                    return;
                                }
                            }

                        }
                        else if (value == ConnectionManager.ParsingConstants.Click)
                        {
                            // Prevent the browser from looping
                            // e.g. a GMail user has entered a wrong pwd
                            // the browser would now loop until the site
                            // (which will never change) chages.
                            if (this.repeatedClickCount >= 1)
                            {
                                return;
                            }

                            this.repeatedClickCount++;

                            if (element == null)
                            {
                                IEnumerable<GeckoHtmlElement> collection = this.firefox.Document.GetElementsByTagName("input");

                                if (collection == null)
                                {
                                    return;
                                }

                                foreach (GeckoHtmlElement el in collection)
                                {
                                    string type = el.GetAttribute("type");

                                    if (!string.IsNullOrEmpty(type))
                                    {
                                        if (el.GetAttribute("type").ToLowerInvariant() == "submit")
                                        {
                                            // Prevent the browser from looping
                                            // e.g. a GMail user has entered a wrong pwd
                                            // the browser would now loop until the site
                                            // (which will never change) chages.
                                            if (this.repeatedClickCount >= 1)
                                            {
                                                return;
                                            }

                                            this.repeatedClickCount++;

                                            (el).Click();
                                        }
                                    }
                                }
                            }
                            else
                                ((GeckoInputElement)element).Click();
                        }
                        else if (value == ConnectionManager.ParsingConstants.Redirect & redirected == false)
                        {
                            redirected = true;
                            this.Home = id;
                            this.firefox.Navigate(id);
                        }
                        else if (value == ConnectionManager.ParsingConstants.Script)
                        {
                            string script = id;
                            //string functionName = GetScriptFunctionName(ref script);

                            if (prevScript == script)
                            {
                                prevScript = null;
                                return;
                            }

                            if (string.IsNullOrEmpty(prevScript) || prevScript != script)
                                prevScript = script;

                            using (AutoJSContext context = new AutoJSContext(firefox.Window.JSContext))
                            {                               
                                string result;
                                context.EvaluateScript(script, out result);
                            }
                        }
                    }
                }
                #endif
                #endregion
            }
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

        private string ParseValue(string value)
        {
            return ConnectionManager.ParseValue(this.BrowserCredential, value);
        }

        //
        // Summary:
        //     Loads the document at the specified Uniform Resource Locator (URL) into the
        //     System.Windows.Forms.WebBrowser control, replacing the previous document.
        //
        // Parameters:
        //   urlString:
        //     The URL of the document to load.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     This System.Windows.Forms.WebBrowser instance is no longer valid.
        //
        //   System.InvalidOperationException:
        //     A reference to an implementation of the IWebBrowser2 interface could not
        //     be retrieved from the underlying ActiveX WebBrowser control.
        public void Navigate(string urlString)
        {
        	this.SetIsLoading(true);
        	
            urlString = ParseValue(urlString);

            if (!string.IsNullOrEmpty(urlString))
                this.urlTextBox.Text = urlString;

            if (string.IsNullOrEmpty(this.homeUrl))
                this.homeUrl = this.urlTextBox.Text;

            string url = this.urlTextBox.Text;
            
            if (this.browserType == BrowserType.InternetExplorer)
            {
                 if (this.BrowserCredential.Authentication == BrowserAuthentication.Basic)
                 {
                     Uri uri = new Uri(url);

                     string authHdr = "Authorization: Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(this.browserCredential.UserName + ":" + this.browserCredential.Password)) + "\r\n";

                     this.internetExplorer.Navigate(uri, null, null, authHdr);
                 }
                 else
                    this.internetExplorer.Navigate(url);
            }
            #if GECKO
            else if (this.browserType == BrowserType.Firefox)
            {
                MimeInputStream headers = null;

                if (this.BrowserCredential.Authentication == BrowserAuthentication.Basic)
                {
                    UriBuilder uriSite = new UriBuilder(url);
                    uriSite.UserName = BrowserCredential.UserName;
                    uriSite.Password = BrowserCredential.Password;
                    this.firefox.Navigate(uriSite.Uri.ToString(), GeckoLoadFlags.BypassCache, null, null, null);
                    return;
                }

                this.firefox.Navigate(url, GeckoLoadFlags.BypassCache, null, null, headers);
            }
            #endif
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (this.browserType == BrowserType.InternetExplorer)
                this.internetExplorer.GoBack();
            #if GECKO
            else
                this.firefox.GoBack();
            #endif
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Home))
            {
                this.Home = this.homeUrl;
            }

            if (this.browserType == BrowserType.InternetExplorer)
                this.internetExplorer.Navigate(this.Home);
           	#if GECKO
            else
                this.firefox.Navigate(this.Home);
            #endif
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            if (this.browserType == BrowserType.InternetExplorer)
                this.internetExplorer.GoForward();
            #if GECKO
            else
                this.firefox.GoForward();
            #endif
        }

        private void HandleGoButtonClick(object sender, EventArgs e)
        {
            this.Navigate(urlTextBox.Text);
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            this.Navigate(urlTextBox.Text);
        }
    }
}