using MSTSCLib;

namespace Terminals.Connections
{
    using System;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using Terminals.Properties;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	
	using Terminals.Configuration.Files.Main.Favorites;
	
	using Kohl.Framework.Info;
	using Kohl.Framework.Localization;
	using Kohl.Framework.Logging;
	
    using AxMSTSCLib;

    public class RDPConnection : Connection.Connection
    {
        public Control RdpClient
        {
            get
            {
                return this.client;
            }
        }

		private static object connectLock = new object();
		
        /// <summary>
        /// Checks, if the connection port is available. Simulates reconnect feature of the RDP client.
        /// Doens use the port scanner, because it needs administrative priviledges 
        /// </summary>
        internal class ConnectionStateDetector
        {
            /// <summary>
            /// Try reconnect max. 1 hour. Consider provide application configuration option for this value.
            /// </summary>
            private const int RECONNECT_MAX_DURATION = 1000 * 3600;

            /// <summary>
            /// Once per 20 seconds
            /// </summary>
            private const int TIMER_INTERVAL = 1000 * 20;

            private int retriesCount;
            private readonly System.Threading.Timer retriesTimer;
            private string serverName;
            private int port;

            private readonly object activityLock = new object();
			
            private bool disabled;
            private bool isRunning;

            internal bool IsRunning
            {
                get
                {
                    lock (this.activityLock)
                    {
                        return this.isRunning;
                    }
                }
            }

            private bool CanTest
            {
                get
                {
                    lock (this.activityLock)
                    {
                        return this.isRunning && !this.disabled;
                    }
                }
            }

            /// <summary>
            /// Connection to the favorite target service should be available again
            /// </summary>
            internal event EventHandler Reconnected;

            /// <summary>
            /// Detector stoped to try reconnect, because maximum amount of retries exceeded.
            /// </summary>
            internal event EventHandler ReconnectExpired;

            internal ConnectionStateDetector()
            {
                this.retriesTimer = new System.Threading.Timer(TryReconnection);
            }

            private void TryReconnection(object state)
            {
                if (!this.CanTest)
                    return;

                this.retriesCount++;
                bool success = this.TryReconnection();

                if (success)
                {
                    this.ReportReconnected();
                    return;
                }

                if (this.retriesCount > (RECONNECT_MAX_DURATION / TIMER_INTERVAL))
                    this.ReconnectionFail();
            }

            private bool TryReconnection()
            {
                try
                {
                    // simulate reconnect, cant use port scanned, because it requires admin priviledges
                    var portClient = new System.Net.Sockets.TcpClient(this.serverName, this.port);
                    return true;
                }
                catch // exception is not necessary, simply is has to work
                {
                    return false;
                }
            }

            private void ReconnectionFail()
            {
                if (this.ReconnectExpired != null)
                    this.ReconnectExpired(this, EventArgs.Empty);
            }

            private void ReportReconnected()
            {
                if (this.Reconnected != null)
                    this.Reconnected(this, EventArgs.Empty);
            }

            internal void AssignFavorite(FavoriteConfigurationElement favorite)
            {
                this.serverName = favorite.ServerName;
                this.port = favorite.Port;
            }

            internal void Start()
            {
                lock (this.activityLock)
                {
                    if (this.disabled)
                        return;

                    this.isRunning = true;
                    this.retriesCount = 0;
                    this.retriesTimer.Change(0, TIMER_INTERVAL);
                }
            }

            internal void Stop()
            {
                lock (this.activityLock)
                {
                    this.isRunning = false;
                    this.retriesTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
            }

            /// <summary>
            /// Fill space between disconnected request from GUI and real disconnect of the client.
            /// </summary>
            internal void Disable()
            {
                lock (this.activityLock)
                {
                    this.disabled = true;
                }
            }

            public override string ToString()
            {
                return string.Format("ConnectionStateDetector:IsRunning={0},Disabled={1}", this.isRunning, this.disabled);
            }
        }
		
        /// <summary>
        /// Translates error codes to client messages
        /// </summary>
        internal static class RdpClientErrorMessages
        {
            public static string ToDisconnectMessage(AxMsRdpClient7NotSafeForScripting client, int reason)
            {
                switch (reason)
                {
                    case 1:
                    case 2:
                    case 3:
                        // These are normal disconnects and not considered errors.
                        return String.Empty;

                    default:
                        return client.GetErrorDescription((uint)reason, (uint)client.ExtendedDisconnectReason);
                }
            }

			// http://msdn.microsoft.com/en-us/library/windows/desktop/aa382176(v=vs.85).aspx
			// IMsTscAxEvents::OnFatalError
            public static string ToFatalErrorMessage(int errorCode)
            {
                switch (errorCode)
                {
                    case 0:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_0");
                    case 1:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_1");
                    case 2:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_2");
                    case 3:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_3");
                    case 4:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_4");
                    case 5:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_5");
                    case 6:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_6");
                    case 7:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_7");
                    case 100:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_8");
                    default:
                        return Localization.Text("Connection.RDPConnection.OnFatalError_9");
                }
            }

            // http://msdn.microsoft.com/en-us/library/windows/desktop/dd919969%28v=vs.85%29.aspx
            // IMsTscAxEvents::OnLogonError method
            public static string ToLogonMessage(int errorCode)
            {
                switch (errorCode)
                {
                    case -5:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_1");
                    case -2:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_2");
                    case -3:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_3");
                    case -6:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_4");
                    case -7:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_5");
                    case -4:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_6");
                    case -1:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_7");
                    case 0:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_8");
                    case 2:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_9");
                    case 1:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_10");
                    case 3:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_11");
                    case -1073741714:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_12");
                    case -1073741715:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_13");
                    case -1073741276:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_14");
                    default:
                        return Localization.Text("Connection.RDPConnection.OnLogonError_15");
                }
            }

			// http://msdn.microsoft.com/en-us/library/windows/desktop/aa382819(v=vs.85).aspx
			// IMsTscAxEvents::OnWarning
            public static string ToWarningMessage(int warningCode)
            {
                switch (warningCode)
                {
                    case 1:
                        return Localization.Text("Connection.RDPConnection.OnWarning_1");
                    default:
                        return Localization.Text("Connection.RDPConnection.OnWarning_2");
                }
            }
        }

        protected override Image[] images
        {
            get { return new Image[] { Resources.RDP }; }
        }

        public override ushort Port
        {
            get { return 3389; }
        }

        public bool FullScreen
        {
            get
            {
                try
                {
                    if (this.client == null)
                        return false;

                    return this.client.FullScreen;
                }
				catch (Exception ex)
                {
					Log.Error("Error getting full screen status " + ex.Message);
                    return false;
                }

            }
            set
            {
                try
                {
                    if (this.client != null)
                        this.client.FullScreen = value;
                }
                catch (Exception ex)
				{ 
					Log.Error("Error switching to full screen " + ex.Message);
				}
            }
        }

        private readonly ReconnectingControl reconecting = new ReconnectingControl();
        private readonly ConnectionStateDetector connectionStateDetector = new ConnectionStateDetector();

        private IMsRdpClientNonScriptable4 nonScriptable;
        private RdpClientControl client = null;

		/// <summary>
		/// http://www.codeproject.com/Tips/109917/Fix-the-focus-issue-on-RDP-Client-from-the-AxInter
		/// </summary>
		internal class RdpClientControl: AxMsRdpClient7NotSafeForScripting
		{
			protected override void WndProc(ref System.Windows.Forms.Message m)
		    {
		        //Fix for the missing focus issue on the rdp client component
				// https://www.autoitscript.com/autoit3/docs/appendix/WinMsgCodes.htm
		        if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
		            this.Focus();

		        base.WndProc(ref m);
		    }
		}
		
        public delegate void Disconnected(RDPConnection Connection);
        public event Disconnected OnDisconnected;
        public delegate void ConnectionEstablish(RDPConnection Connection);
        public event ConnectionEstablish OnConnected;

        public override bool Connected
        {
            get
            {
                // dont let the connection to close with running reconnection
                return Convert.ToBoolean(this.client.Connected) || this.connectionStateDetector.IsRunning;
            }
        }

        public override bool Connect()
        {
            try
            {
                if (!this.InitializeClientControl())
                    return false;

                this.ConfigureCientUserControl();

                try
                {
                    this.client.ConnectingText = "Connecting. Please wait...";
                    this.client.DisconnectedText = "Disconnecting...";
					
                    this.ConfigureColorsDepth();
                    this.ConfigureRedirectedDrives();
                    this.ConfigureInterface();
                    this.ConfigureStartBehaviour();
                    this.ConfigureTimeouts();
                    this.ConfigureRedirectOptions();
                    this.ConfigureConnectionBar();
                    this.ConfigureTsGateway();
                    this.ConfigureSecurity();
                    this.ConfigureConnection();
                    this.AssignEventHandlers();

                    this.client.FullScreen = true;
                }
                catch (Exception exc)
                {
                    Log.Info("There was an exception setting an RDP Value.", exc);
                }

                if (this.Favorite.DesktopSize != DesktopSize.FitToWindow)
                    InvokeIfNecessary(() =>
                    {
                        this.ChangeDesktopSize();
                    });
                else
                    InvokeIfNecessary(() =>
                    {
						lock(connectLock)
						{	
                        	this.client.Width = this.ParentForm.Width;
                       		this.client.Height = this.ParentForm.Height;
						}
                    });
				
                // if next line fails on Protected memory access exception,
                // some string property is set to null, which leads to this exception
                this.client.Connect();

                if (this.Favorite.DesktopSize == DesktopSize.FitToWindow)
                    InvokeIfNecessary(() =>
                    {
                        this.ChangeDesktopSize();
                    });

                return true;
            }
            catch (Exception exc)
            {
                Log.Fatal("Connecting to RDP", exc);
                return false;
            }
        }

        private bool InitializeClientControl()
        {
            try
            {
				this.client = new RdpClientControl();
            }
            catch (Exception exception)
            {
                string message = "Please update your RDP client to at least version 6.";
                Log.Info(message, exception);
                MessageBox.Show(message);
                return false;
            }
            return true;
        }

        public new bool Focus()
        {
            try
            {
				base.Focus();
				reconecting.Focus();
				
                if (this.client == null)
                    return false;

				return base.Focus() && ((Control)this.client).Focus();
            }
			catch (Exception ex)
            {
				Log.Error("Error setting focus " + ex.Message); 
                return false;
            }
        }

        private static readonly object lock_controlcreation = new object();

        private void ConfigureCientUserControl()
        {
            lock (lock_controlcreation)
            {
                ((System.ComponentModel.ISupportInitialize)(this.client)).BeginInit();

                this.Embed(client);

                this.client.CreateControl();

                this.InvokeIfNecessary(() =>
                {
                    nonScriptable = this.client.GetOcx() as IMsRdpClientNonScriptable4;
                    ((System.ComponentModel.ISupportInitialize)(this.client)).EndInit();
                });
            }

            this.InvokeIfNecessary(() =>
            {
                if (this.Favorite.DesktopSize == DesktopSize.AutoScale)
                    this.client.Dock = DockStyle.Fill;
            });

            this.ConfigureReconnect();
        }

        private void ConfigureReconnect()
        {
            // if not added to the client control controls collection, then it isnt visible
            var clientControl = (Control)this.client;
           
            this.InvokeIfNecessary(() =>
            {
                clientControl.Controls.Add(this.reconecting);
                this.reconecting.Hide();
                this.reconecting.AbortReconnectRequested += new EventHandler(this.Recoonecting_AbortReconnectRequested);
            });
            
            this.connectionStateDetector.AssignFavorite(this.Favorite);
            this.connectionStateDetector.ReconnectExpired += ConnectionStateDetectorOnReconnectExpired;
            this.connectionStateDetector.Reconnected += ConnectionStateDetectorOnReconnected;
        }

        private void ConnectionStateDetectorOnReconnected(object sender, EventArgs eventArgs)
        {
            if (this.reconecting.InvokeRequired)
                this.reconecting.Invoke(new EventHandler(this.ConnectionStateDetectorOnReconnected), new object[] { sender, eventArgs });
            else
                this.Reconnect();
        }

        private void Reconnect()
        {
            if (!this.reconecting.Reconnect)
                return;
            this.StopReconnect();
            this.reconecting.Reconnect = false;
            this.client.Connect();
        }

        private void ConnectionStateDetectorOnReconnectExpired(object sender, EventArgs eventArgs)
        {
            this.CancelReconnect();
        }

        private void Recoonecting_AbortReconnectRequested(object sender, EventArgs e)
        {
            this.CancelReconnect();
        }

        private void CancelReconnect()
        {
            if (this.reconecting.InvokeRequired)
            {
                this.reconecting.Invoke(new Action(this.CancelReconnect));
            }
            else
            {
                this.StopReconnect();
                this.FinishDisconnect();
            }
        }

        private void StopReconnect()
        {
            this.connectionStateDetector.Stop();
            this.reconecting.Hide();

            if (this.reconecting.Disable)
                Terminals.Configuration.Files.Main.Settings.Settings.AskToReconnect = false;
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
            try
            {
                switch (desktopSize)
                {
                    case DesktopSize.Custom:
                        this.client.AdvancedSettings3.SmartSizing = false;
                        this.client.FullScreen = false;
                        break;

                    case DesktopSize.AutoScale:
                    case DesktopSize.FitToWindow:
                        this.client.AdvancedSettings3.SmartSizing = true;
                        this.client.FullScreen = false;
                        break;

                    case DesktopSize.FullScreen:
                        this.client.FullScreen = true;
                        this.client.AdvancedSettings3.SmartSizing = false;
                        size = this.MaxSize;
                        break;
                }

                client.InvokeIfNecessary(() =>
                {
                    this.client.Height = size.Height;
                    this.client.Width = size.Width;
                });
            }
            catch (Exception ex)
            {
                Log.Error(Localization.Text("Connections.RDPConnection.ChangeDesktopSize"), ex);
            }
        }

        private void ConfigureColorsDepth()
        {
            switch (this.Favorite.Colors)
            {
                case Colors.Bits8:
                    this.client.ColorDepth = 8;
                    break;
                case Colors.Bit16:
                    this.client.ColorDepth = 16;
                    break;
                case Colors.Bits24:
                    this.client.ColorDepth = 24;
                    break;
                case Colors.Bits32:
                    this.client.ColorDepth = 32;
                    break;
            }
        }

        private void ConfigureRedirectedDrives()
        {
            if (this.Favorite.RedirectedDrives.Count > 0 && this.Favorite.RedirectedDrives[0].Equals("true"))
                this.client.AdvancedSettings2.RedirectDrives = true;
            else
            {
                this.InvokeIfNecessary(() =>
                {
                    for (int i = 0; i < nonScriptable.DriveCollection.DriveCount; i++)
                    {
                        IMsRdpDrive drive = nonScriptable.DriveCollection.get_DriveByIndex((uint)i);
                        foreach (string str in this.Favorite.RedirectedDrives)
                        {
                            if (drive.Name.IndexOf(str) > -1)
                                drive.RedirectionState = true;
                        }
                    }
                });
            }
        }

        private void ConfigureInterface()
        {
            //advanced settings
            //bool, 0 is false, other is true
            if (this.Favorite.AllowBackgroundInput)
                this.client.AdvancedSettings.allowBackgroundInput = -1;

            if (this.Favorite.BitmapPeristence)
                this.client.AdvancedSettings.BitmapPeristence = -1;

            if (this.Favorite.EnableCompression)
                this.client.AdvancedSettings.Compress = -1;

            if (this.Favorite.AcceleratorPassthrough)
                this.client.AdvancedSettings2.AcceleratorPassthrough = -1;

            if (this.Favorite.DisableControlAltDelete)
                this.client.AdvancedSettings2.DisableCtrlAltDel = -1;

            if (this.Favorite.DisplayConnectionBar)
                this.client.AdvancedSettings2.DisplayConnectionBar = true;

            if (this.Favorite.DoubleClickDetect)
                this.client.AdvancedSettings2.DoubleClickDetect = -1;

            if (this.Favorite.DisableWindowsKey)
                this.client.AdvancedSettings2.EnableWindowsKey = -1;

            if (this.Favorite.EnableEncryption)
                this.client.AdvancedSettings2.EncryptionEnabled = -1;

            this.ConfigureAzureService();

            this.ConfigureCustomReconnect();
        }

        /// <summary>
        /// The ActiveX component requires a UTF-8 encoded string, but .NET uses
        /// UTF-16 encoded strings by default.  The following code converts
        /// the UTF-16 encoded string so that the byte-representation of the
        /// LoadBalanceInfo string object will "appear" as UTF-8 to the Active component.
        /// Furthermore, since the final string still has to be shoehorned into
        /// a UTF-16 encoded string, I pad an extra space in case the number of
        /// bytes would be odd, in order to prevent the byte conversion from
        /// mangling the string at the end.  The space is ignored by the RDP
        /// protocol as long as it is inserted at the end.
        /// Finally, it is required that the LoadBalanceInfo setting is postfixed
        /// with \r\n in order to work properly.  Note also that \r\n MUST be
        /// the last two characters, so the space padding has to be inserted first.
        /// The following code has been tested with Windows Azure connections
        /// only - I am aware there are other types of RDP connections that
        /// require the LoadBalanceInfo parameter which I have not tested
        /// (e.g., Multi-Server Terminal Services Gateway), that may or may not
        /// work properly.
        ///
        /// Sources:
        ///  1. http://stackoverflow.com/questions/13536267/how-to-connect-to-azure-vm-with-remote-desktop-activex
        ///  2. http://social.technet.microsoft.com/Forums/windowsserver/en-US/e68d4e9a-1c8a-4e55-83b3-e3b726ff5346/issue-with-using-advancedsettings2loadbalanceinfo
        ///  3. Manual comparison of raw packets between Windows RDP client and Terminals using WireShark.
        /// </summary>
        private void ConfigureAzureService()
        {
            if (!string.IsNullOrEmpty(this.Favorite.LoadBalanceInfo))
            {
                var lbTemp = this.Favorite.LoadBalanceInfo;

                if (lbTemp.Length % 2 == 1)
                    lbTemp += " ";

                lbTemp += "\r\n";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(lbTemp);
                string lbFinal = System.Text.Encoding.Unicode.GetString(bytes);
                this.client.AdvancedSettings2.LoadBalanceInfo = lbFinal;
            }
        }

        private void ConfigureCustomReconnect()
        {
            this.client.AdvancedSettings3.EnableAutoReconnect = false;
            this.client.AdvancedSettings3.MaxReconnectAttempts = 0;
            this.client.AdvancedSettings3.keepAliveInterval = 0;
        }

        private void ConfigureStartBehaviour()
        {
            if (this.Favorite.GrabFocusOnConnect)
                this.client.AdvancedSettings2.GrabFocusOnConnect = true;

            if (this.Favorite.EnableSecuritySettings)
            {
                if (this.Favorite.SecurityFullScreen)
                    this.client.SecuredSettings2.FullScreen = -1;

                this.client.SecuredSettings2.StartProgram = this.Favorite.SecurityStartProgram;
                this.client.SecuredSettings2.WorkDir = this.Favorite.SecurityWorkingFolder;
            }
        }

        private void ConfigureTimeouts()
        {
            try
            {
                this.client.AdvancedSettings2.MinutesToIdleTimeout = this.Favorite.IdleTimeout;

                int timeout = this.Favorite.OverallTimeout;
                if (timeout > 600)
                    timeout = 10;
                if (timeout <= 0)
                    timeout = 10;
                this.client.AdvancedSettings2.overallConnectionTimeout = timeout;
                timeout = this.Favorite.ConnectionTimeout;
                if (timeout > 600)
                    timeout = 10;
                if (timeout <= 0)
                    timeout = 10;

                this.client.AdvancedSettings2.singleConnectionTimeout = timeout;

                timeout = this.Favorite.ShutdownTimeout;
                if (timeout > 600)
                    timeout = 10;
                if (timeout <= 0)
                    timeout = 10;
                this.client.AdvancedSettings2.shutdownTimeout = timeout;
            }
            catch (Exception exc)
            {
                Log.Error("Error when trying to set timeout values.", exc);
            }
        }

        private void ConfigureRedirectOptions()
        {
            this.client.AdvancedSettings3.RedirectPorts = this.Favorite.RedirectPorts;
            this.client.AdvancedSettings3.RedirectPrinters = this.Favorite.RedirectPrinters;
            this.client.AdvancedSettings3.RedirectSmartCards = this.Favorite.RedirectSmartCards;
            this.client.AdvancedSettings3.PerformanceFlags = this.Favorite.PerformanceFlags;
            this.client.AdvancedSettings6.RedirectClipboard = this.Favorite.RedirectClipboard;
            this.client.AdvancedSettings6.RedirectDevices = this.Favorite.RedirectDevices;
        }

        private void ConfigureConnectionBar()
        {
            this.client.AdvancedSettings6.ConnectionBarShowMinimizeButton = false;
            this.client.AdvancedSettings6.ConnectionBarShowPinButton = false;
            this.client.AdvancedSettings6.ConnectionBarShowRestoreButton = false;
            this.client.AdvancedSettings3.DisplayConnectionBar = this.Favorite.DisplayConnectionBar;
        }

        private void ConfigureTsGateway()
        {
            // Terminal Server Gateway Settings
            this.client.TransportSettings.GatewayUsageMethod = (uint)this.Favorite.TsgwUsageMethod;
            this.client.TransportSettings.GatewayCredsSource = (uint)this.Favorite.TsgwCredsSource;
            this.client.TransportSettings.GatewayHostname = this.Favorite.TsgwHostname;
            this.client.TransportSettings2.GatewayDomain = this.Favorite.TsgwDomain;
            this.client.TransportSettings2.GatewayProfileUsageMethod = 1;

            if (this.Favorite.TsgwSeparateLogin)
            {
                this.client.TransportSettings2.GatewayUsername = this.Favorite.TsgwUsername;
                this.client.TransportSettings2.GatewayPassword = this.Favorite.TsgwPassword;
            }
            else
            {
                this.client.TransportSettings2.GatewayUsername = this.Favorite.Credential.UserName;
                this.client.TransportSettings2.GatewayPassword = this.Favorite.Credential.Password;
            }
        }

        private void ConfigureSecurity()
        {
            if (this.Favorite.EnableTlsAuthentication)
                this.client.AdvancedSettings5.AuthenticationLevel = 2;

            InvokeIfNecessary(() =>
            {
                this.nonScriptable.EnableCredSspSupport = this.Favorite.EnableNlaAuthentication;
            });

            this.client.SecuredSettings2.AudioRedirectionMode = (int)this.Favorite.Sounds;

            this.client.UserName = this.Favorite.Credential.UserName;
            this.client.Domain = this.Favorite.Credential.DomainName;
            try
            {
                if (!String.IsNullOrEmpty(this.Favorite.Password))
                {
                    InvokeIfNecessary(() =>
                    {
                        if (this.nonScriptable != null)
                            this.nonScriptable.ClearTextPassword = this.Favorite.Password;
                    });
                }
            }
            catch (Exception exc)
            {
                Log.Error("Error when trying to set the ClearTextPassword on the nonScriptable mstsc object", exc);
            }
        }

        private void ConfigureConnection()
        {
            this.client.Server = this.Favorite.ServerName;
            this.client.AdvancedSettings3.RDPPort = this.Favorite.Port;
            this.client.AdvancedSettings3.ContainerHandledFullScreen = -1;
            // Use ConnectToServerConsole or ConnectToAdministerServer based on implementation
            this.client.AdvancedSettings7.ConnectToAdministerServer = this.Favorite.ConnectToConsole;
            this.client.AdvancedSettings3.ConnectToServerConsole = this.Favorite.ConnectToConsole;
        }

        private void AssignEventHandlers()
        {
            this.client.OnRequestGoFullScreen += new EventHandler(this.client_OnRequestGoFullScreen);
            this.client.OnRequestLeaveFullScreen += new EventHandler(this.client_OnRequestLeaveFullScreen);
            this.client.OnDisconnected += new IMsTscAxEvents_OnDisconnectedEventHandler(this.client_OnDisconnected);
            this.client.OnWarning += new IMsTscAxEvents_OnWarningEventHandler(this.client_OnWarning);
            this.client.OnFatalError += new IMsTscAxEvents_OnFatalErrorEventHandler(this.client_OnFatalError);
            this.client.OnLogonError += new IMsTscAxEvents_OnLogonErrorEventHandler(this.client_OnLogonError);
            this.client.OnConnected += new EventHandler(this.client_OnConnected);
            // assign the drag and drop event handlers directly throws an exception
            var clientControl = (Control)this.client;

            if (clientControl.InvokeRequired)
                clientControl.Invoke(new MethodInvoker(delegate
                {
                    clientControl.DragEnter += new DragEventHandler(this.client_DragEnter);
                    clientControl.DragDrop += new DragEventHandler(this.client_DragDrop);
                }));
            else
            {
                clientControl.DragEnter += new DragEventHandler(this.client_DragEnter);
                clientControl.DragDrop += new DragEventHandler(this.client_DragDrop);
            }

        }

        public override void Disconnect()
        {
            try
            {
                this.connectionStateDetector.Disable();
                this.client.Disconnect();
                Log.Info("Disconnected from RDP session.");
            }
            catch (Exception ex)
            {
                Log.Error(Localization.Text("Connection.ExternalConnection.Disconnect"), ex);
            }
        }
        
        private void client_OnConnected(object sender, EventArgs e)
        {
            if (this.OnConnected != null)
                this.OnConnected(this);
        }

        private void client_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string desktopShare = this.GetDesktopShare();

            if (String.IsNullOrEmpty(desktopShare))
            {
                MessageBox.Show(this, Localization.Text("Connections.ICAConnection.ICAConnection_DragDrop"),
                                AssemblyInfo.Title(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                this.SHCopyFiles(files, desktopShare);
            }
        }

        private void SHCopyFiles(string[] sourceFiles, string destinationFolder)
        {
            ShellFileOperation fo = new ShellFileOperation();

            fo.InvokeOperation(this.Handle, FileOperations.Copy, sourceFiles, sourceFiles.Select(sourceFile => Path.Combine(destinationFolder, Path.GetFileName(sourceFile))).ToArray());
        }

        private void client_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private static void PostLeavingFullScreenMessage(Control source)
        {
            PostMessage(new HandleRef(source, source.Handle), WM_LEAVING_FULLSCREEN, IntPtr.Zero, IntPtr.Zero);
        }

        private void client_OnRequestLeaveFullScreen(object sender, EventArgs e)
        {
            ParentForm.UpdateControls();
            PostLeavingFullScreenMessage(this);
        }

        private void client_OnRequestGoFullScreen(object sender, EventArgs e)
        {
            ParentForm.UpdateControls();
        }

        private void client_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            if (DecideToReconnect(e))
            {
                this.TryReconnect();
            }
            else
            {
                this.ShowDisconnetMessageBox(e);
                this.FinishDisconnect();
            }
        }

        private static bool DecideToReconnect(IMsTscAxEvents_OnDisconnectedEvent e)
        {
            // 516 reason in case of reconnect expired
            // 2308 connection lost
            // 2 - regular logoff also in case of forced reboot or shutdown
            if (e.discReason != 2308 && e.discReason != 2)
                return false;

            return Terminals.Configuration.Files.Main.Settings.Settings.AskToReconnect;
        }

        private void TryReconnect()
        {
            this.reconecting.Show();
            this.reconecting.BringToFront();
            this.connectionStateDetector.Start();
        }

        private void FinishDisconnect()
        {
            this.CloseTabPage();
            this.FireDisconnected();
        }

        private void ShowDisconnetMessageBox(IMsTscAxEvents_OnDisconnectedEvent e)
        {
            int reason = e.discReason;
            string error = RdpClientErrorMessages.ToDisconnectMessage(this.client, reason);

            if (!string.IsNullOrEmpty(error))
            {
                string message = String.Format("Error connecting to {0}\n\n{1}", this.client.Server, error);
                MessageBox.Show(this, message, AssemblyInfo.TitleVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void FireDisconnected()
        {
            if (this.OnDisconnected != null)
                this.OnDisconnected(this);
        }

        private void client_OnFatalError(object sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            int errorCode = e.errorCode;
            string message = RdpClientErrorMessages.ToFatalErrorMessage(errorCode);
            string finalMsg = string.Format(Localization.Text("Connection.RDPConnection.OnFatalError"), errorCode, message);
            Log.Fatal(errorCode.ToString());
			
			if (!string.IsNullOrEmpty(finalMsg) && !string.IsNullOrEmpty(finalMsg.Trim()))
            	MessageBox.Show(finalMsg);
			
            Log.Fatal(finalMsg);
        }

        private void client_OnWarning(object sender, IMsTscAxEvents_OnWarningEvent e)
        {
            int warningCode = e.warningCode;
            string message = RdpClientErrorMessages.ToWarningMessage(warningCode);
            string finalMsg = string.Format(Localization.Text("Connection.RDPConnection.OnWarning"), warningCode, message);
            Log.Warn(warningCode.ToString());
            Log.Warn(finalMsg);
        }

        private void client_OnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            int errorCode = e.lError;
			
            string message = RdpClientErrorMessages.ToLogonMessage(errorCode);
            string finalMsg = string.Format(Localization.Text("Connection.RDPConnection.OnLogonError"), errorCode, message);
            
			if (errorCode != -2)
				Log.Error(finalMsg);
			else
				Log.Debug(finalMsg);
        }  
    }
}