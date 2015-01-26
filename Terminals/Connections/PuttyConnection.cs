using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Kohl.Framework.Localization;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Properties;

namespace Terminals.Connections
{
    public class PuttyConnection : ExternalConnection
    {
        private Options options;

        private bool connected = false;

        public override bool IsPortRequired
        {
            get
            {
                return false;
            }
        }

        #region Overrides - Inherited Properties (6)

        protected override string WorkingDirectory
        {
            get { return string.Empty; }
        }

        protected override string ProgramPath
        {
            get { return Settings.PuttyProgramPath; }
        }

        protected override SleepType SleepMethod
        {
            get { return SleepType.NoSleep; }
        }

        protected override bool UseShellExecute
        {
            get { return false; }
        }

        protected override int Sleep
        {
            get { return 0; }
        }

        protected override string Arguments
        {
            get
            {
                if (this.options == null)
                {
                    return string.Empty;
                }

                return this.options.Arguments;
            }
        }

        protected override bool EnableRedrawExternalWindow
        {
            get { return true; }
        }

        #endregion

        #region Overrides - Inherited Methods (2)

        private readonly string windowTitle = Guid.NewGuid().ToString();
        private IntPtr hConfigDialog = IntPtr.Zero;
        private IntPtr hPuttyConsole = IntPtr.Zero;
        private IntPtr hPuttyWarning = IntPtr.Zero;
        private IntPtr panelHandle = IntPtr.Zero;
        private Thread puttyWarning;

        public override bool Connect()
        {
            this.connected = false;

            this.options = new Options(this.Favorite.ServerName, this.Favorite.PuttyConnectionType, this.Favorite.Port, this.Favorite.PuttyProxyHost, this.Favorite.PuttyProxyPort, this.Favorite.PuttyProxyType, this.Favorite.PuttyEnableX11)
                               {
                                   ShowOptionsAndWaitForUserInput = this.Favorite.PuttyShowOptions,
                                   LoginName = { Value = (this.Favorite.PuttyDontAddDomainToUserName ? this.Favorite.Credential.UserName : this.Favorite.Credential.UserNameWithDomain) },
                                   Password = {Value = this.Favorite.Credential.Password},
                                   Compression = {Value = this.Favorite.PuttyCompression},
                                   Verbose = {Value = this.Favorite.PuttyVerbose},
                                   CloseWindowOnExit = {Value = this.Favorite.PuttyCloseWindowOnExit}
                               };

            return this.connected = base.Connect();
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static string GetWindowText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetText(IntPtr hWnd)
        {
            const int length = 0;
            StringBuilder sb = new StringBuilder(length + 1);
            SendMessage(hWnd, (int) WindowsMessages.WM_GETTEXT, length + 1, sb);
            return sb.ToString();
        }

        // Aquire checkbox state
        public const long BM_GETCHECK = 0xf0;
        
        // Checkbox is checked
        public const long BST_CHECKED = 0x1;
        
        // Checkbox is greyed out
        public const long BST_INDETERMINATE = 0x2;
        
        // Checkbox is unchecked
        public const long BST_UNCHECKED = 0x0;
      
        // Set the check state of the checkbox
        public const long BM_SETCHECK = 0xF1;

        [DllImport("user32.dll")]
        public static extern uint IsDlgButtonChecked(IntPtr hDlg, int nIdButton);

        [DllImport("user32.dll")]
        public static extern uint CheckDlgButton(IntPtr hDlg, int nIdButton, long check);

        public static string GetText(IntPtr iptrHWndDialog, int iControlId)
        {
            IntPtr iptrHWndControl = GetDlgItem(iptrHWndDialog, iControlId);
            HandleRef hrefHWndTarget = new HandleRef(null, iptrHWndControl);
            const int length = 1000;
            StringBuilder sb = new StringBuilder(length + 1);
            SendMessage(hrefHWndTarget, (int)WindowsMessages.WM_GETTEXT, length + 1, sb);
            return sb.ToString();
        }

        private static void SetText(IntPtr iptrHWndDialog, int iControlId, string strTextToSet)
        {
            IntPtr iptrHWndControl = GetDlgItem(iptrHWndDialog, iControlId);
            HandleRef hrefHWndTarget = new HandleRef(null, iptrHWndControl);
            SendMessage(hrefHWndTarget, (uint) WindowsMessages.WM_SETTEXT, IntPtr.Zero, strTextToSet);
        }

        public static void SelectOption(IntPtr iptrHWndDialog, int iControlId)
        {
            IntPtr iptrHWndControl = GetDlgItem(iptrHWndDialog, iControlId);
            HandleRef hrefHWndTarget = new HandleRef(null, iptrHWndControl);
            SendMessage(hrefHWndTarget, (uint) WindowsMessages.WM_LBUTTONDOWN, IntPtr.Zero, null);
            SendMessage(hrefHWndTarget, (uint) WindowsMessages.WM_LBUTTONUP, IntPtr.Zero, null);
        }

        public static void ClickButton(IntPtr iptrHWndDialog, int iControlId)
        {
            IntPtr iptrHWndControl = GetDlgItem(iptrHWndDialog, iControlId);
            HandleRef hrefHWndTarget = new HandleRef(null, iptrHWndControl);
            SendMessage(hrefHWndTarget, (uint)WindowsMessages.BM_CLICK, IntPtr.Zero, null);
        }

        public static void SetCheckState(IntPtr iptrHWndDialog, int iControlId, bool value)
        {
        	// Abort if the check state doesn't divergate from the desired value
        	if ((IsDlgButtonChecked(iptrHWndDialog, iControlId) == BST_CHECKED) == value)
        		return;
        	
        	CheckDlgButton(iptrHWndDialog, iControlId, (value ? BST_CHECKED : BST_UNCHECKED));
        }
        
        private static bool SetConfigOptions(IntPtr hConfigDialog, Options options, bool pressOKButton = false)
        {
            if (hConfigDialog == IntPtr.Zero)
            {
                return false;
            }

            // Set the options ...
            SelectOption(hConfigDialog, options.CloseWindowOnExit.Identifier[options.CloseWindowOnExit.Value]);

            // Sending empty strings will force PUTTY to stay unresponsive.
            if (!string.IsNullOrEmpty(options.HostName.Value))
                SetText(hConfigDialog, options.HostName.Identifier, options.HostName.Value);

			// Select the proxy page
			SelectTreeViewItem(hConfigDialog, 4, 2);
			
			// Proxy
            SetText(hConfigDialog, options.ProxyHost.Identifier, options.ProxyHost.Value);
			
			// Port
			SetText(hConfigDialog, options.ProxyPort.Identifier, options.ProxyPort.Value.ToString());
			
			// Socks 5
			SelectOption(hConfigDialog, options.ProxyType.Identifier[options.ProxyType.Value]);
			
            // Select the Connection ... SSH ... X11
			SelectTreeViewItem(hConfigDialog, 4, 5, 4);
			
			// X11 forwarding
			SetCheckState(hConfigDialog, options.EnableX11.Identifier, options.EnableX11.Value);
            
			// Press the button to connect
			if (pressOKButton)
				ClickButton(hConfigDialog, 0x3F1);
			
            return true;
        }
		
		private const int TV_FIRST = 0x1100;
		private const int TVGN_ROOT = 0x0;
		private const int TVGN_NEXT = 0x1;
		private const int TVGN_CHILD = 0x4;
		private const int TVGN_FIRSTVISIBLE = 0x5;
		private const int TVGN_NEXTVISIBLE = 0x6;
		private const int TVGN_CARET = 0x9;
		private const int TVM_SELECTITEM = (TV_FIRST + 11);
		private const int TVM_GETNEXTITEM = (TV_FIRST + 10);
		private const int TVM_GETITEM = (TV_FIRST + 12);
		
		//Send Message API
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);
		
		private static void SelectTreeViewItem(IntPtr dialog, int mainItemNumber, int subItemNumber = -1, int subSubItemNumber = -1)
		{
			int treeItem = 0;
			IntPtr systreeView = FindWindowEx(dialog, IntPtr.Zero, "SysTreeView32", IntPtr.Zero);
			
			treeItem = SendMessage((int)systreeView, TVM_GETNEXTITEM, TVGN_ROOT, IntPtr.Zero);

			// -1 is needed because we have root itself
			for (int i = 0; i < mainItemNumber-1; i++)
				treeItem = SendMessage((int)systreeView, TVM_GETNEXTITEM, TVGN_NEXT, (IntPtr)treeItem);
			
			if (subItemNumber > 0)
			{
				treeItem = SendMessage((int)systreeView, TVM_GETNEXTITEM, TVGN_CHILD, (IntPtr)treeItem);
				for (int i = 0; i < subItemNumber-1; i++)
					treeItem = SendMessage((int)systreeView, TVM_GETNEXTITEM, TVGN_NEXT, (IntPtr)treeItem);
			}
						
			if (subSubItemNumber > 0)
			{
				SendMessage((int)systreeView, TVM_SELECTITEM, TVGN_CARET, (IntPtr)treeItem);
				treeItem = SendMessage((int)systreeView, TVM_GETNEXTITEM, TVGN_CHILD, (IntPtr)treeItem);
				for (int i = 0; i < subSubItemNumber-1; i++)
					treeItem = SendMessage((int)systreeView, TVM_GETNEXTITEM, TVGN_NEXT, (IntPtr)treeItem);
			}
			
			SendMessage((int)systreeView, TVM_SELECTITEM, TVGN_CARET, (IntPtr)treeItem);
		}

        protected override void PerformPostAction(Process process)
        {
            this.connected = false;

            if (this.options.ShowOptionsAndWaitForUserInput || this.options.SetAdditionalGuiOptions)
            {
                // Window title: PuTTY Configuration
                this.hConfigDialog = GetDialog(this.HWnd, "PuTTYConfigBox", "PuTTY Configuration");

                if (this.hConfigDialog == IntPtr.Zero)
                {
                    this.Disconnect();
                }

                Application.DoEvents();
				
				InvokeIfNecessary(() => { this.TerminalTabPage.Controls.Clear(); });
				
				Control panelControl = this.EmbedWindow(this.hConfigDialog);
				
				InvokeIfNecessary(() => { panelHandle = panelControl.Handle; });
                BringWindowToTop(this.hConfigDialog);

                /* Give the GUI some time to be painted */
                Thread.Sleep(100);

				bool autoClickOKButtonInConfigDialog = false;
				
				//if (this.options.ShowOptionsAndWaitForUserInput && this.options.SetAdditionalGuiOptions)
				//	autoClickOKButtonInConfigDialog = false;
				
				//if (!this.options.ShowOptionsAndWaitForUserInput && this.options.SetAdditionalGuiOptions)
				//	autoClickOKButtonInConfigDialog = true;
				
				autoClickOKButtonInConfigDialog |= !this.options.ShowOptionsAndWaitForUserInput && this.options.SetAdditionalGuiOptions;
				
                // Set the options and remain open
				SetConfigOptions(this.hConfigDialog, this.options, autoClickOKButtonInConfigDialog);
								
                // wait for any of both threads, first one wins
                while (this.hConfigDialog == IntPtr.Zero)
                {
                    Application.DoEvents();
                    Thread.Sleep(30);
                }
                
                SetWindowText(this.hConfigDialog, this.windowTitle);

                this.hConfigDialog = FindWindowEx(this.panelHandle, IntPtr.Zero, null, IntPtr.Zero);
                this.HWnd = this.hConfigDialog;

                Log.Info(Localization.Text("Connection.PuttyConnection.PerformPostAction_Info1"));
                
                // wait for any of both threads, first one wins
                do
                {
                    Application.DoEvents();
                    Thread.Sleep(30);
                    this.HWnd = this.hConfigDialog;
                    this.hConfigDialog = FindWindowEx(this.panelHandle, IntPtr.Zero, null, IntPtr.Zero);
                    
                    // If the tab page has been removed from its parent control
                    if (this.TerminalTabPage.Parent == null)
                    {
                    	// this forces a disconnect because the value of the connected-Property will remain the same (i.e. "false").
                    	string error = "The user has aborted the session manually before a connection was created.";
                    	Log.Warn(Localization.Text(error));
                    	this.Disconnect();
                    	throw new Exception(error);
                    	//return;
                    }
                    
                } while (this.hConfigDialog != IntPtr.Zero);

                Log.Info(Localization.Text("Connection.PuttyConnection.PerformPostAction_Info2"));

                /* Give the GUI some time to be painted */
                Thread.Sleep(120);
            }
            
            // Start two threads and wait either for the security warning
            // or the putty session window to appear.
            this.puttyWarning = new Thread((ThreadStart)delegate
                                               {
                                                   Thread.Sleep(120);
                                                   this.hPuttyWarning = GetDialog(IntPtr.Zero, "#32770",
                                                                                  "PuTTY Security Alert");
                                                   /* Give the GUI some time to be painted */
                                                   Thread.Sleep(120); /* Click the no button */
                                                   SelectOption(this.hPuttyWarning, 7);
                                                   Log.Debug("PuTTY Connection: Ignored security warning.");
                                               });

            Thread puttyConsole = new Thread((ThreadStart)delegate
                                                 {
                                                     Thread.Sleep(120);
                                                     this.hPuttyConsole = GetDialog(IntPtr.Zero, "PuTTY", null);
                                                 });

            this.puttyWarning.Start();
            puttyConsole.Start();

            // wait for any of both threads, first one wins
            while (this.hPuttyConsole == IntPtr.Zero)
            {
                Application.DoEvents();
                Thread.Sleep(30);
            }

            // stop both threads, we'll only need one IntPtr
            puttyConsole.Abort();

            // Embed the application console window
            Application.DoEvents();
            this.EmbedWindow(this.hPuttyConsole);
            BringWindowToTop(this.hPuttyConsole);
            Log.Info(Localization.Text("Connection.PuttyConnection.PerformPostAction_Info3"));

            this.hConfigDialog = FindWindowEx(this.panelHandle, IntPtr.Zero, null, IntPtr.Zero);
            this.HWnd = this.hConfigDialog;

            new Thread((ThreadStart)delegate
                           {
                               int pwdtimeout = this.Favorite.PuttyPasswordTimeout;
                               const int pollingInterval = 10;

                               string text = "PUTTY";

                               int counter = 0;

                               bool set = false;

                               do
                               {
                                   counter++;
                                   text = GetWindowText(this.hPuttyConsole);

                                   if (counter >= (pwdtimeout/pollingInterval))
                                   {
                                       // Putty wants us to enter the password
                                       if (this.options.ConnectionType.Value != PuttyConnectionType.Ssh)
                                       {
                                           if (!set)
                                           {
                                               Log.Info(
                                                   Localization.Text(
                                                       "Connection.PuttyConnection.PerformPostAction_Info4"));
                                               // Make sure that our window is active
                                               SetForegroundWindow(this.hPuttyConsole);
                                               Thread.Sleep(100);
                                               SendKeys.SendWait(this.Favorite.Credential.Password + "{ENTER}");
                                               set = true;
                                           }
                                           break;
                                       }
                                   }

                                   Application.DoEvents();
                                   Thread.Sleep(pollingInterval);
                               } while (text.ToUpper().Contains("PUTTY"));

                               if (string.IsNullOrEmpty(text))
                               {
                                   // ERROR .. PWD wrong, etc ...
                                   Log.Error(Localization.Text("Connection.PuttyConnection.PerformPostAction_Error"));
                                   this.Disconnect();
                                   return;
                               }

                               Log.Info(Localization.Text("Connection.PuttyConnection.PerformPostAction_Info"));
                           }).Start();

            this.connected = true;
        }

        #endregion

        protected override Image[] images
        {
            get { return new Image[] {Resources.PUTTY}; }
        }

        private static int GetDefaultPort(PuttyConnectionType connectionType)
        {
            switch (connectionType)
            {
                    // Return the SSH port
                case PuttyConnectionType.Raw:
                case PuttyConnectionType.Ssh:
                    return 22;
                    // Return the RLogin port
                case PuttyConnectionType.Rlogin:
                    return 513;
                    // Return the telnet port in any other case
                default:
                    return 23;
            }
        }

        private class Options
        {
            private readonly PuttyConnectionType connectionType;
            private bool showOptions;

            public Options(string hostName, PuttyConnectionType connectionType, int port, string proxyHost, int proxyPort, PuttyProxyType proxyType, bool enableX11)
            {
                this.connectionType = connectionType;

				// Create the command line string for the Putty verbose option and leave any other property unchanged.
                this.Verbose = new Property<bool, int>
                                   {
                                       GetCmdLine =
                                           () => this.Verbose.Value ? "-v" : string.Empty
                                   };
				
				// Create the command line string  for the Putty liogin name option and leave any other property unchanged.
                this.LoginName = new Property<string, int>
                                     {
                                         GetCmdLine =
                                             () => string.IsNullOrEmpty(this.LoginName.Value)
                                                       ? string.Empty
                                                       : "-l " + this.LoginName.Value
                                     };
				
				// Create the command line string for the Putty password option and leave any other property unchanged.
                this.Password = new Property<string, int>
                                    {
                                        GetCmdLine =
                                            () => string.IsNullOrEmpty(this.Password.Value)
                                                      ? string.Empty
                                                      : "-pw " + this.Password.Value
                                    };
				
				// Create the command line string for the Putty compression option and leave any other property unchanged.
                this.Compression = new Property<bool, int>
                                       {
                                           GetCmdLine =
                                               () => this.Compression.Value
                                                         ? "-C"
                                                         : string.Empty
                                       };

				// Set the hostname
                this.HostName = new Property<string, int> {ClassName = "EDIT", Identifier = 1044, Value = hostName};

                this.EnableX11 = new Property<bool, int> {ClassName = "BUTTON", Identifier = 0x413, Value = enableX11};
                
				this.ProxyHost = new Property<string, int> {ClassName = "EDIT", Identifier = 0x41B, Value = proxyHost};
								
				this.ProxyPort = new Property<int, int> {ClassName = "EDIT", Identifier = 0x41D, Value = proxyPort};
				
				this.ProxyType = new Property<PuttyProxyType, Dictionary<PuttyProxyType, int>> {ClassName = "EDIT", Value = proxyType};
				
                this.ProxyType.Identifier = new Dictionary<PuttyProxyType, int> {
                                                           {
                                                               PuttyProxyType.HTTP,
                                                               0x417
                                                           },
                                                           {
                                                               PuttyProxyType.Local,
                                                               0x419
                                                           },
                                                           {
                                                               PuttyProxyType.None,
                                                               0x414
                                                           },
                                                           {
                                                               PuttyProxyType.SOCKS4,
                                                               0x415
                                                           },
                                                           {
                                                               PuttyProxyType.SOCKS5,
                                                               0x416
                                                           },
                                                           {
                                                               PuttyProxyType.Telnet,
                                                               0x418
                                                           }};
				
                this.ConnectionType = new Property<PuttyConnectionType, int[]>
                                          {
                                              ClassName = "BUTTON",
                                              Identifier = new[] {1049, 1050, 1051, 1052},
                                              Value = connectionType,
                                              GetCmdLine = delegate
                                                               {
                                                                   // serial port is not supported by putty command line
                                                                   switch (this.ConnectionType.Value)
                                                                   {
                                                                       case PuttyConnectionType.Telnet:
                                                                           return "-telnet";
                                                                       case PuttyConnectionType.Rlogin:
                                                                           return "-rlogin";
                                                                       case PuttyConnectionType.Raw:
                                                                           return "-raw";
                                                                       default:
                                                                           return "-ssh";
                                                                   }
                                                               }
                                          };

                if (port < 1 || port > 65535)
                {
                    port = GetDefaultPort(connectionType);
                }

                this.Port = new Property<int, int>
                                {
                                    ClassName = "EDIT",
                                    Identifier = 1046,
                                    Value = port,
                                    GetCmdLine = () => "-P " + this.Port.Value
                                };
                
				this.CloseWindowOnExit = new Property<PuttyCloseWindowOnExit, Dictionary<PuttyCloseWindowOnExit, int>>();
                
                this.CloseWindowOnExit.Identifier = new Dictionary<PuttyCloseWindowOnExit, int> {
                                                           {
                                                               PuttyCloseWindowOnExit.OnlyOnCleanExit,
                                                               1065
                                                           },
                                                           {
                                                               PuttyCloseWindowOnExit.Always,
                                                               1063
                                                           },
                                                           {
                                                               PuttyCloseWindowOnExit.Never,
                                                               1064
                                                           }};
            }

			public bool SetAdditionalGuiOptions
			{
				get { return (!string.IsNullOrWhiteSpace(this.ProxyHost.Value) && this.ProxyType.Value != PuttyProxyType.None) || string.IsNullOrWhiteSpace(this.HostName.Value) || this.EnableX11.Value; }
            }
			
            public bool ShowOptionsAndWaitForUserInput
            {
                get { return this.showOptions; }
                set
                {
                    this.IncludeHost = !value;
                    this.showOptions = value;
                }
            }

            /// <summary>
            ///     Session ... Host name
            /// </summary>
            public Property<string, int> LoginName { get; private set; }

            /// <summary>
            ///     This option is not visible in the GUI, is only intended to be used for automation.
            /// </summary>
            public Property<string, int> Password { get; private set; }

            private bool IncludeHost { get; set; }

            /// <summary>
            ///     Connection ... SSH ... Enable Compression
            /// </summary>
            public Property<bool, int> Compression { get; private set; }

            public string Arguments
            {
                get
                {
                    string connection = string.Empty;

                    connection += " " + this.Verbose.GetCmdLine();
            
                    connection += " " + this.LoginName.GetCmdLine();

                    connection += " " + this.ConnectionType.GetCmdLine();

                    if (this.IncludeHost && !SetAdditionalGuiOptions)
                    {
                        connection += " " + this.HostName.Value;
                    }

                    connection += " " + this.Port.GetCmdLine();

                    if (this.connectionType == PuttyConnectionType.Ssh)
                    {
                        connection += " " + this.Password.GetCmdLine();
                        connection += " " + this.Compression.GetCmdLine();
                    }

                    return connection;
                }
            }

            // This can be set by supplying command line options
            public Property<string, int> HostName { get; private set; }
			
			public Property<int, int> ProxyPort { get; set; }
			
			public Property<string, int> ProxyHost { get; set; }

			public Property<bool, int> EnableX11 { get; set; }
			
			public Property<PuttyProxyType, Dictionary<PuttyProxyType, int>> ProxyType { get; set; }
			
            private Property<int, int> Port { get; set; }
            
			public Property<PuttyConnectionType, int[]> ConnectionType { get; private set; }

            // this can only be set via API
            public Property<PuttyCloseWindowOnExit, Dictionary<PuttyCloseWindowOnExit, int>> CloseWindowOnExit { get; private set; }

            /// <summary>
            ///     This option applies to most PuTTY tools and is not visible in the GUI.
            /// </summary>
            public Property<bool, int> Verbose { get; private set; }
        }

        public class Property<TValue, TIdentifier>
        {
            public delegate string Action();

            // The equevalent commandline argument used to set the value
            // e.g. Port (Class Name: Edit, ID: 1046, Text: 22)
            //      PUTTY.EXE -P 22
            public Action GetCmdLine = () => string.Empty;

            public Property()
            {
                this.Identifier = default(TIdentifier);
                this.ClassName = null;
                this.Value = default(TValue);
            }

            // Control ID, e.g. 1044
            public TIdentifier Identifier { get; set; }

            // Class Name, e.g. EDIT, STATIC, COMBOBOX, BUTTON
            public string ClassName { private get; set; }

            // The text of an edit field, or the caption of an label, the selected text or hole list of a combobox, etc.
            public TValue Value { get; set; }

            // Defines the method signature used to get our command line
            // in our case it will be an anonymous method.
        }
    }
}