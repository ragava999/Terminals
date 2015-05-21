namespace Terminals.Plugins.AutoIt.Connection
{
    // .NET namespaces
    using System;
    using System.Drawing;
    using System.IO;
    using System.Diagnostics;
    using System.Windows.Forms;

    // Terminals namespaces
    using Configuration.Files.Main.Favorites;
    using Kohl.Framework.Info;
    using Kohl.Framework.Logging;
    using MainSettings = Configuration.Files.Main.Settings;

    public partial class AutoItConnection : Terminals.Connection.Connection, Terminals.Connection.IAfterConnectSupport
    {
        public bool IsInAfterConnectMode { get; set; }

        public bool IsAfterConnectEnabled
        {
            get
            {
                if (Terminals.Connection.Manager.ConnectionManager.GetProtocolName(typeof(AutoItEditorConnection)).ToUpper() == Favorite.Protocol.ToUpper())
                    return false;

                // Run the connection only if a script exists -> normal EXIT -> condition OK
                if (string.IsNullOrEmpty(Script))
                    return false;

                // We want to run a connection buth only
                // if the program path has been defined
                // if not -> Exception
                if (string.IsNullOrEmpty(ProgramPath) || !File.Exists(ProgramPath))
                {
                    Log.Fatal("Error connection can't be executed. The path to the autoit exe hasn't been specified in the options.");
                    return false;
                }


                // Everything ok -> run the connection
                return true;
            }
        }

        Process process = new Process();
        
        private string includeFileName = null;
        private string scriptFileName = null;

        private bool connected = false;

        private const int GWL_STYLE = (-16);
        private const int WS_MAXIMIZE = 0x01000000;
        private const int WS_VISIBLE = 0x10000000;

        public override bool IsPortRequired
        {
            get
            {
                return false;
            }
        }

        protected override Image[] images
        {
            get { return new Image[] { Properties.Resources.AutoIt }; }
        }

        protected string ProgramPath
        {
            get { return Settings.AutoItProgramPath(); }
        }

        protected string Script
        {
            get { return Favorite.AutoItScript(); }
            set { Favorite.AutoItScript(value); }
        }

        public AutoItConnection()
        {
            InitializeComponent();
        }

        public override bool Connected
        {
            get { return connected; }
        }

        private void CleanUp()
        {
            if (includeFileName != null || File.Exists(includeFileName))
                File.Delete(includeFileName);

            if (scriptFileName != null || File.Exists(scriptFileName))
                File.Delete(scriptFileName);
        }

        string disconnectFile = null;
        int handle = 0;

        public override bool Connect()
        {
            string autoItExe = ProgramPath;

            if (string.IsNullOrEmpty(autoItExe) || !File.Exists(autoItExe))
            {
                Log.Fatal("Can't find the auto it exe file, please set the correct path in your autoit 'options'.");
                return connected = false;
            }

            try
            {
                CleanUp();

                includeFileName = Path.Combine(AssemblyInfo.DirectoryConfigFiles, Path.GetFileName(Path.GetTempFileName()));
      
                this.TerminalTabPage.InvokeIfNecessary(new MethodInvoker(delegate { handle = this.TerminalTabPage.Handle.ToInt32(); }));

                if (handle > 0)
                {
                    disconnectFile = System.IO.Path.Combine(AssemblyInfo.DirectoryConfigFiles, handle.ToString() + ".disconnect");
                    FileSystemWatcher fsw = new FileSystemWatcher(Path.GetDirectoryName(disconnectFile), "*.disconnect");
                    fsw.Created += OnDisconnectFile_Created;
                    fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
                    fsw.EnableRaisingEvents = true;
                }

                string includeFileContent = "; #STRUCTURE# ===================================================================================================================\n";
                includeFileContent += "; Name...........: " + AssemblyInfo.Title() + ".au3\n";
                includeFileContent += "; Description ...: Returns the favorite's properties.\n";
                includeFileContent += "; Author ........: " + AssemblyInfo.Author + "\n";
                includeFileContent += "; Remarks .......: This file will be generated dynamically by " + AssemblyInfo.Title() + ". Please don't change this file.\n";
                includeFileContent += "; Related .......: " + AssemblyInfo.Url + "\n";
                includeFileContent += "; Usage   .......: #include <" + AssemblyInfo.Title() + ".au3>\n";
                includeFileContent += "; ===============================================================================================================================\n";
                includeFileContent += "Global Const $Terminals_Protocol           = \"" + this.Favorite.Protocol + "\"\n";
                includeFileContent += "Global Const $Terminals_ServerName         = \"" + this.Favorite.ServerName + "\"\n";
                includeFileContent += "Global Const $Terminals_Port               = " + this.Favorite.Port + "\n";
                includeFileContent += "Global Const $Terminals_ConnectionName     = \"" + this.Favorite.Name + "\"\n";
                includeFileContent += "\n";
                includeFileContent += "Global Const $Terminals_CredentialName     = \"" + this.Favorite.XmlCredentialSetName + "\"\n";
                includeFileContent += "Global Const $Terminals_User               = \"" + this.Favorite.Credential.UserName + "\"\n";
                includeFileContent += "Global Const $Terminals_Domain             = \"" + this.Favorite.Credential.DomainName + "\"\n";
                includeFileContent += "Global Const $Terminals_Password           = \"" + this.Favorite.Credential.Password + "\"\n";
                includeFileContent += "\n";
                includeFileContent += "; The handle to the control, where the windows can be embedded.\n";
                includeFileContent += "Global Const $Terminals_ConnectionHWND     = " + handle + "\n";
                includeFileContent += "\n";
                includeFileContent += "; Terminals' process ID.\n";
                includeFileContent += "Global Const $Terminals_ProcessId          = " + System.Diagnostics.Process.GetCurrentProcess().Id + "\n";
                includeFileContent += "\n";
                includeFileContent += "Global Const $Terminals_Version            = \"" + AssemblyInfo.Version + "\"\n";
                includeFileContent += "\n";
                includeFileContent += "Global Const $Terminals_Directory          = \"" + AssemblyInfo.Directory + "\"\n";
                includeFileContent += "\n";
                includeFileContent += "Global Const $Terminals_CurrentUser        = \"" + UserInfo.UserNameAlias + "\"\n";
                includeFileContent += "Global Const $Terminals_CurrentUserDomain  = \"" + UserInfo.UserDomain + "\"\n";
                includeFileContent += "Global Const $Terminals_CurrentUserSID     = \"" + UserInfo.UserSid + "\"\n";
                includeFileContent += "Global Const $Terminals_MachineDomain      = \"" + MachineInfo.MachineDomain + "\"";

                includeFileContent += "\n\n";

                includeFileContent += "Func Embed($hWnd)\n";
                includeFileContent += "\tDllCall(\"user32.dll\", \"int\", \"SetParent\", \"hwnd\", $hwnd, \"int\", $Terminals_ConnectionHWND)\n";
                includeFileContent += "\tDllCall(\"user32.dll\", \"int\", \"SetWindowLong\", \"hwnd\", $hWnd, \"int\", " + GWL_STYLE + ", \"int\", " + (WS_VISIBLE + WS_MAXIMIZE).ToString() + ")\n";
                includeFileContent += "\tDllCall(\"user32.dll\", \"int\", \"MoveWindow\", \"hWnd\", $hWnd, \"int\", 0, \"int\", 0, \"int\", " + TerminalTabPage.Size.Width + ", \"int\", " + TerminalTabPage.Size.Height + ", \"int\", True)\n";
                includeFileContent += "EndFunc";
                includeFileContent += "\n\n";

                includeFileContent += "Func Disconnect()\n";
                includeFileContent += "\tFileWrite(\"" + disconnectFile + "\", \"0\")\n";
                includeFileContent += "EndFunc";
                includeFileContent += "\n\n";

                using (StreamWriter stream = new StreamWriter(includeFileName, false, System.Text.Encoding.UTF8))
                {
                    stream.Write(includeFileContent);
                    stream.Flush();
                    stream.Close();
                }

                string scriptFileContent = "#include \"" + includeFileName + "\"" + Environment.NewLine + this.Script;

                scriptFileName = Path.Combine(AssemblyInfo.DirectoryConfigFiles, Path.GetFileName(Path.GetTempFileName()));

                using (StreamWriter stream = new StreamWriter(scriptFileName, false, System.Text.Encoding.UTF8))
                {
                    stream.Write(scriptFileContent);
                    stream.Flush();
                    stream.Close();
                }

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = ProgramPath;
                process.StartInfo.Arguments = scriptFileName;

                /*
                // If we want to run the program as a differnt user
                if (!IsInAfterConnectMode && !string.IsNullOrEmpty(this.Favorite.Credential.UserName) && !string.IsNullOrEmpty(this.Favorite.Credential.Password))
                {
                    // Only use the domain if there is one
                    if (!string.IsNullOrEmpty(this.Favorite.Credential.DomainName))
                    {
                        process.StartInfo.Domain = this.Favorite.Credential.DomainName;
                    }

                    process.StartInfo.UserName = this.Favorite.Credential.UserName;

                    // Create a secure string for the runas password
                    System.Security.SecureString sec = new System.Security.SecureString();

                    foreach (char character in this.Favorite.Credential.Password)
                    {
                        sec.AppendChar(character);
                    }

                    process.StartInfo.Password = sec;
                }
                */
                process.Start();
            }
            catch (Exception ex)
            {
                Log.Fatal("Script results: " + "Error executing your autoit script.", ex);
                return connected = false;
            }

            return connected = true;
        }

        void OnDisconnectFile_Created(object sender, FileSystemEventArgs e)
        {
            int hwnd = 0;
            if (Path.GetExtension(e.FullPath).ToUpper() == ".DISCONNECT")
                if (Int32.TryParse(Path.GetFileNameWithoutExtension(e.FullPath), out hwnd))
                {
                    if (hwnd == handle)
                    {
                        File.Delete(e.FullPath);
                        Disconnect();
                    }
                }
        }

        public override void Disconnect()
        {
            if (process != null)
            {
                if (!process.HasExited)
                    try
                    {
                        process.Kill();
                    }
                    catch { }
            }

            this.CloseTabPage(false);
            CleanUp();
            connected = false;
			
			this.Dispose(true);
        }

        protected override void ChangeDesktopSize(DesktopSize size, System.Drawing.Size siz)
        {
            
        }
    }
}