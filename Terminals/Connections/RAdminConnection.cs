using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Kohl.Framework.Info;

using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Properties;

namespace Terminals.Connections
{
    public class RAdminConnection : ExternalConnection
    {
        private bool connected = false;

        public override bool Connected
        {
            get { return this.connected; }
        }
		
        protected override Image[] images
        {
            get { return new Image[] { Resources.RADMIN }; }
        }

        public override ushort Port
        {
            get { return Settings.RAdminDefaultPort; }
        }

        #region Public Enums (1)

        public enum ColorDepth
        {
            Bits1,
            Bits2,
            Bits4,
            Bits8,
            Bits16,
            Bits24
        }

        #endregion

        #region Private Fields (2)

        private IntPtr hLoginDialog = IntPtr.Zero;
        private IntPtr hSessionDukeessionDialog = IntPtr.Zero;

        #endregion

        #region Overrides - Inherited Properties (6)

        protected override string WorkingDirectory
        {
            get { return string.Empty; }
        }

        protected override string ProgramPath
        {
            get { return Settings.RAdminProgramPath; }
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
                string connection = "/connect:" + this.Favorite.ServerName + ":" + this.Favorite.Port;

                if (!string.IsNullOrEmpty(this.PhonebookPath))
                {
                    connection += " /pbpath\"" + this.PhonebookPath + "\"";
                }

                if (this.Through && !string.IsNullOrEmpty(this.ThroughServerName))
                {
                    connection += " through:" + this.ThroughServerName + ":" +
                                  (string.IsNullOrEmpty(this.ThroughPort)
                                       ? this.Favorite.Port.ToString()
                                       : this.ThroughPort);
                }

                if (!this.StandardConnectionMode)
                {
                    if (this.TelnetMode)
                    {
                        connection += " /telnet";
                    }

                    if (this.ViewOnlyMode)
                    {
                        connection += " /noinput";
                    }

                    if (this.FileTransferMode)
                    {
                        connection += " /file";
                    }

                    if (this.Shutdown)
                    {
                        connection += " /shutdown";
                    }

                    if (this.ChatMode)
                    {
                        connection += " /chat";
                    }

                    if (this.VoiceChatMode)
                    {
                        connection += " /voice";
                    }

                    if (this.SendTextMessageMode)
                    {
                        connection += " /message";
                    }
                }

                if (this.UseFullScreen)
                {
                    connection += " /fullscreen";
                }

                if (this.Updates > 0 && this.Updates < 100)
                {
                    connection += " /updates:" + this.Updates.ToString();
                }

                switch (this.ColorMode)
                {
                    case ColorDepth.Bits24:
                        connection += " /24bpp";
                        break;
                    case ColorDepth.Bits16:
                        connection += " /16bpp";
                        break;
                    case ColorDepth.Bits8:
                        connection += " /8bpp";
                        break;
                    case ColorDepth.Bits4:
                        connection += " /4bpp";
                        break;
                    case ColorDepth.Bits2:
                        connection += " /2bpp";
                        break;
                    case ColorDepth.Bits1:
                        connection += " /1bpp";
                        break;
                }

                return connection;
            }
        }

        #endregion

        #region RAdmin Properties (15)

        /// <summary>
        ///     The path to the RAdmin phonebook file.
        /// </summary>
        private string PhonebookPath
        {
            get { return this.Favorite.RAdminPhonebookPath; }
        }

        /// <summary>
        ///     Allow to connect using an intermediate server and/or port.
        /// </summary>
        private bool Through
        {
            get { return this.Favorite.RAdminThrough; }
        }

        /// <summary>
        ///     Connect using an intermediate server.
        /// </summary>
        private string ThroughServerName
        {
            get { return this.Favorite.RAdminThroughServerName; }
        }

        /// <summary>
        ///     Connect using an intermediate port.
        /// </summary>
        private string ThroughPort
        {
            get { return this.Favorite.RAdminThroughPort; }
        }

        /// <summary>
        ///     This is the RAdmin default connection mode.
        /// </summary>
        private bool StandardConnectionMode
        {
            get { return this.Favorite.RAdminStandardConnectionMode; }
        }

        /// <summary>
        ///     Connect using the RAdmin telnet mode,
        ///     which is not equal to the telnet protocol
        ///     specification (i.e RAdmin specific!)
        /// </summary>
        private bool TelnetMode
        {
            get { return this.Favorite.RAdminTelnetMode; }
        }

        /// <summary>
        ///     Connect using the view only mode.
        /// </summary>
        private bool ViewOnlyMode
        {
            get { return this.Favorite.RAdminViewOnlyMode; }
        }

        /// <summary>
        ///     Connect using the file transfer mode.
        /// </summary>
        private bool FileTransferMode
        {
            get { return this.Favorite.RAdminFileTransferMode; }
        }

        /// <summary>
        ///     Shutdown the remote computer.
        /// </summary>
        private bool Shutdown
        {
            get { return this.Favorite.RAdminShutdown; }
        }

        /// <summary>
        ///     Connect using chat mode.
        /// </summary>
        private bool ChatMode
        {
            get { return this.Favorite.RAdminChatMode; }
        }

        /// <summary>
        ///     Connect using voice chat mode.
        /// </summary>
        private bool VoiceChatMode
        {
            get { return this.Favorite.RAdminVoiceChatMode; }
        }

        /// <summary>
        ///     Connect using send text message mode.
        /// </summary>
        private bool SendTextMessageMode
        {
            get { return this.Favorite.RAdminSendTextMessageMode; }
        }

        /// <summary>
        ///     Connect using the full screen mode.
        /// </summary>
        private bool UseFullScreen
        {
            get { return this.Favorite.RAdminUseFullScreen; }
        }

        /// <summary>
        ///     Connect using the specified number of screen updates per second.
        /// </summary>
        private int Updates
        {
            get { return this.Favorite.RAdminUpdates; }
        }

        /// <summary>
        ///     Connect using the specified color depth.
        /// </summary>
        private ColorDepth ColorMode
        {
            get
            {
                try
                {
                    return (ColorDepth)Enum.Parse(typeof(ColorDepth), this.Favorite.RAdminColorMode, true);
                }
                catch
                {
                    return ColorDepth.Bits24;
                }
            }
        }

        #endregion

        #region Private Methods (5)

        private IntPtr GetLoginDialog(int timeout)
        {
            // ***** FIND THE LOGIN WINDOW In RADMIN *****
            const string className = "#32770";
            string windowTitle = "Windows security: " + this.Favorite.ServerName; // "Connection info";
            IntPtr hDialog = GetDialog(this.HWnd, className, windowTitle, true, 30000);

            return hDialog;
        }

        private void SendLoginRequest(IntPtr hDialog)
        {
            // ***** FIND THE TEXTBOXES AND FILL THEM WITH OUR CREDENTIALS *****
            string text = string.Empty;
            IntPtr hTextBox = IntPtr.Zero;
            string className = "Edit";

            int counter = 0;

            do
            {
                switch (counter)
                {
                    case 0:
                        text = this.Favorite.Credential.UserName;
                        break;
                    case 1:
                        text = this.Favorite.Credential.Password;
                        break;
                    case 2:
                        text = this.Favorite.Credential.DomainName;
                        break;
                    default:
                        text = string.Empty;
                        break;
                }

                hTextBox = FindWindowEx(hDialog, hTextBox, className, IntPtr.Zero);
                SendMessage(hTextBox, (uint)WindowsMessages.WM_SETTEXT, IntPtr.Zero, text);

                counter++;
            } while (hTextBox != IntPtr.Zero);

            hTextBox = IntPtr.Zero;

            // ***** FIND THE OK BUTTON AND CLICK IT *****
            IntPtr hButton = IntPtr.Zero;
            className = "Button";

            counter = 0;
            do
            {
                hButton = FindWindowEx(hDialog, hButton, className, IntPtr.Zero);
                if (counter == 1)
                {
                    SendMessage(hButton, (uint)WindowsMessages.WM_LBUTTONDOWN, IntPtr.Zero, default(string));
                    SendMessage(hButton, (uint)WindowsMessages.WM_LBUTTONUP, IntPtr.Zero, default(string));
                }
                counter++;
            } while (hButton != IntPtr.Zero);

            hButton = IntPtr.Zero;
        }

        private IntPtr GetConnectInfoDialog()
        {
            return this.GetConnectInfoDialog(this.HWnd);
        }

        private IntPtr GetConnectInfoDialog(IntPtr hWnd)
        {
            // ***** FIND THE CONNECTION INFORMATION DIALOG, EMBED IT AND HIDE THE CANCEL BUTTON *****
            string className = "#32770";
            const string windowTitle = "Connection info";
            IntPtr hDialog = GetDialog(hWnd, className, windowTitle, true, 1500);

            if (hDialog != IntPtr.Zero)
            {
                IntPtr hButton = IntPtr.Zero;
                className = "Button";

                do
                {
                    hButton = FindWindowEx(hDialog, hButton, className, IntPtr.Zero);

                    if (hButton != IntPtr.Zero)
                    {
                        EnableWindow(hButton, false);
                        ShowWindow(hButton, ShowWindowCommands.Hide);
                        break;
                    }
                } while (true);

                hButton = IntPtr.Zero;
            }

            return hDialog;
        }

        public override void Disconnect()
        {
            if (!connected)
                return;

            System.Diagnostics.Process.GetProcessById(process.Id).Kill();
        	
        	if (this.HWnd != IntPtr.Zero)
            {
                // close the window using API
                SendMessage(this.HWnd, 0x0112 /* WM_SYSCOMMAND */, 0xF060 /* SC_CLOSE */, null);
            }
        	
            System.Collections.Generic.List<IntPtr> list = GetChildWindows(this.TerminalTabPage.Controls[0].Handle);
            foreach (IntPtr ptr in list)
            {
                // close the window using API
                SendMessage(ptr, 0x0112 /* WM_SYSCOMMAND */, 0xF060 /* SC_CLOSE */, null);
            }
            
            InvokeIfNecessary(() => base.Disconnect());
        }
        
        private IntPtr GetSessionDukeessionDialog(int attempts = 1000)
        {
            // ***** FIND THE CONNECTION INFORMATION DIALOG, EMBED IT AND HIDE THE CANCEL BUTTON *****
            string windowTitle = this.Favorite.ServerName + " - Full Control";

            if (!this.StandardConnectionMode)
            {
                if (this.TelnetMode)
                {
                    windowTitle = this.Favorite.ServerName + " - Telnet";
                }

                if (this.ViewOnlyMode)
                {
                    windowTitle = this.Favorite.ServerName + " - View Only";
                }

                if (this.FileTransferMode)
                {
                    windowTitle = this.Favorite.ServerName + " - File Transfer";
                }

                if (this.Shutdown)
                {
                }

                if (this.ChatMode)
                {
                }

                if (this.VoiceChatMode)
                {
                    windowTitle = this.Favorite.ServerName + " - Voice Chat []";
                }

                if (this.SendTextMessageMode)
                {
                    windowTitle = this.Favorite.ServerName + " - New Message";
                }
            }

            IntPtr hDialog = GetDialog(this.HWnd, null, windowTitle, true, attempts);

            return hDialog;
        }
        #endregion

        #region Overrides - Inherited Methods (1)
        protected override void PerformPostAction(Process process)
        {
            this.connected = false;

            Thread.Sleep(100);
            Application.DoEvents();

            IntPtr hConnectInfoDialog = IntPtr.Zero;

            for (int i = 0; i <= 3; i++)
                if (hConnectInfoDialog == IntPtr.Zero)
                    hConnectInfoDialog = this.GetConnectInfoDialog();

            Thread.Sleep(50);
            Application.DoEvents();

            // Nice to have feature -> Display connection status
            if (hConnectInfoDialog != IntPtr.Zero)
            {
                Application.DoEvents();
                this.EmbedWindow(hConnectInfoDialog);
                Application.DoEvents();
            }

            Thread sessionDukeessionDialogThread =
                new Thread((ThreadStart)
                    delegate { this.hSessionDukeessionDialog = this.GetSessionDukeessionDialog(30000); });
            Thread loginDialogThread =
                new Thread((ThreadStart)delegate { this.hLoginDialog = this.GetLoginDialog(30000); });

            loginDialogThread.Start();
            sessionDukeessionDialogThread.Start();

            // wait for any of both threads, first one wins
            while (this.hSessionDukeessionDialog == IntPtr.Zero && this.hLoginDialog == IntPtr.Zero)
            {
                Application.DoEvents();
                Thread.Sleep(30);

                if (!loginDialogThread.IsAlive && !sessionDukeessionDialogThread.IsAlive)
                {
                    // we have reached the timeout
                    // neither a login dialog nor a RAdmin session dialog has been found
                    // e.g.
                    // Reason: Bad IP or DNS ...
                    this.Disconnect();
                    this.connected = false;
                    return;
                }
            }

            // stop both threads, we'll only need one IntPtr
            loginDialogThread.Abort();
            sessionDukeessionDialogThread.Abort();

            // Check if we need to perform a login or if we are able to use Windows Integrated Security (our current logon credentials)
            if (this.hSessionDukeessionDialog == IntPtr.Zero || this.hLoginDialog != IntPtr.Zero)
            {
                // Login screen has appeared or main screen hasn't been shown to us ->
                // RAdmin wants us to supply our credentials
                // Has the user valid credentials?
                if (!this.Favorite.Credential.IsSetUserName)
                {
                    this.Disconnect();
					string error = "Please enter the user name in your RAdmin connection properties.";
                    MessageBox.Show(error, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(error);
                    this.Disconnect();
                    this.connected = false;
                    return;
                }

                if (!this.Favorite.Credential.IsSetPassword)
                {
                    this.Disconnect();
					string error = "Please enter the password in your RAdmin connection properties.";
                    MessageBox.Show(error, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(error);
                    this.Disconnect();
                    this.connected = false;
                    return;
                }

                if (!this.Favorite.Credential.IsSetDomainName)
                {
					Log.Warn("Attention: The domain hasn't been set in your RAdmin connection properties. The connection will be established without the domain name.");
                }

                Application.DoEvents();
                // Send the login request
                this.SendLoginRequest(this.hLoginDialog);
                Application.DoEvents();
            }

            // Hide the login window
            if (this.hLoginDialog != IntPtr.Zero)
            {
                ShowWindow(this.hLoginDialog, ShowWindowCommands.Hide);
            }

            // Get the RAdmin session dialog
            this.hSessionDukeessionDialog = this.GetSessionDukeessionDialog(30000);

            if (this.hSessionDukeessionDialog != IntPtr.Zero)
            {
                // --> SUCCESS !!!
                // Display our RAdmin session
                this.HWndRedrawWindow = this.hSessionDukeessionDialog;
                this.EmbedWindow(this.hSessionDukeessionDialog);
                this.connected = false;
                return;
            }

            // --> ERROR
            // No error has been catched by SessionDuke -> Display generic one.
            if (this.TerminalTabPage.Controls.Count < 1)
            {
                // Password incorrect, TCP error, etc.
                this.Disconnect();
                this.connected = false;
            }
            this.connected = true;
        }
        #endregion
    }
}