using Terminals.Connection.Manager;

namespace Terminals.Connections
{
    // .NET namespace
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    // ICA namespace
    using AxWFICALib;

    // Terminals and framework namespaces
    using Kohl.Framework.Info;
    using Kohl.Framework.Localization;
    using Kohl.Framework.Logging;
    using Configuration.Files.Main.Favorites;
    using Connection;
    using Properties;

    public class ICAConnection : Connection
    {
        private bool connected;

        private AxICAClient iIcaClient;

        protected override Image[] images
        {
            get { return new Image[] {Resources.CITRIX}; }
        }

        public override ushort Port
        {
            get { return 1494; }
        }

        public override bool Connected
        {
            get { return this.connected; }
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
            this.iIcaClient.DesiredHRes = size.Width;
            this.iIcaClient.DesiredVRes = size.Height;
        }

        // http://blogs.citrix.com/2010/03/02/fun-with-the-ica-client-object-ico-and-net-console-applications/
        public override bool Connect()
        {
            this.connected = false;

            try
            {
                this.iIcaClient = new AxICAClient();

                // Register the event callbacks
                ((Control) this.iIcaClient).DragEnter += this.ICAConnection_DragEnter;
                ((Control) this.iIcaClient).DragDrop += this.ICAConnection_DragDrop;
                this.iIcaClient.OnDisconnect += this.iIcaClient_OnDisconnect;

                // Embed the control
                this.Embed(this.iIcaClient);

                // Set the color mode
                iIcaClient.InvokeIfNecessary(() => this.iIcaClient.Address = this.Favorite.ServerName);

                switch (this.Favorite.Colors)
                {
                    case Colors.Bit16:
                        this.iIcaClient.SetProp("DesiredColor", "16");
                        break;
                    case Colors.Bits32:
                        this.iIcaClient.SetProp("DesiredColor", "32");
                        break;
                    case Colors.Bits8:
                        this.iIcaClient.SetProp("DesiredColor", "16");
                        break;
                    default:
                        this.iIcaClient.SetProp("DesiredColor", "24");
                        break;
                }

                // Set the application name
                this.iIcaClient.Application = AssemblyInfo.Title();

                // Set the path to the config files
                this.iIcaClient.AppsrvIni = this.Favorite.IcaServerIni;
                this.iIcaClient.WfclientIni = this.Favorite.IcaClientIni;

                // Set the encryption level
                this.iIcaClient.Encrypt = this.Favorite.IcaEnableEncryption;
                string specifiedLevel = this.Favorite.IcaEncryptionLevel.Trim();

                if (specifiedLevel.Contains(" "))
                {
                    string encryptLevel = specifiedLevel.Substring(0, specifiedLevel.IndexOf(" ")).Trim();
                    if (!string.IsNullOrEmpty(encryptLevel)) this.iIcaClient.EncryptionLevelSession = encryptLevel;
                }

                //Set credentials
                this.iIcaClient.Domain = this.Favorite.Credential.DomainName;
                this.iIcaClient.Username = this.Favorite.Credential.UserName;
                this.iIcaClient.SetProp("ClearPassword", this.Favorite.Credential.Password);

                // Set the server
                this.iIcaClient.Address = this.Favorite.ServerName;

                this.ChangeDesktopSize();

                //Set to false to embed the session, rather than to launch externally.
                this.iIcaClient.Launch = false;

                // Diable inter process communication
                this.iIcaClient.IPCLaunch = false;

                //Enable seamless
                this.iIcaClient.TWIMode = true;

                //Set Server locator
                this.iIcaClient.BrowserProtocol = "UDP";
                this.iIcaClient.TCPBrowserAddress = this.Favorite.ServerName;
                    // this is a metaframe server that has a UDP listener running.

                // Set the protocol and timeout
                this.iIcaClient.TransportDriver = "TCP/IP";
                this.iIcaClient.SessionExitTimeout = 120;

                // Fit to window = 3, 2 ... Size, 1 ... Percent, 0 ... Disabled (default)
                this.iIcaClient.SetProp("ScalingMode", "3");

                if (this.Favorite.IcaApplicationName != "")
                {
                    // Start setting properties
                    this.iIcaClient.InitialProgram = "#" + this.Favorite.IcaApplicationName;
                }

                this.InvokeIfNecessary(() => this.Text = Localization.Text("Connections.ICAConnection.Connect"));
                
                this.iIcaClient.Connect();

                this.InvokeIfNecessary(() => this.iIcaClient.Focus());
                
                return this.connected = true;
            }
            catch (Exception ex)
            {
                Log.Fatal(string.Format(Localization.Text("Connections.HTTPConnection.Connect_Error2"), this.Favorite.Protocol), ex);
                return this.connected = false;
            }
        }

        private void iIcaClient_OnDisconnect(object sender, EventArgs e)
        {
            Log.Fatal(string.Format(Localization.Text("Connections.ICAConnection_ConnectionLost"), this.Favorite.Name));
            this.connected = false;

            this.CloseTabPage();
        }

        private void ICAConnection_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            string desktopShare = this.GetDesktopShare();
            
            if (String.IsNullOrEmpty(desktopShare))
            {
                MessageBox.Show(this, Localization.Text("Connections.ICAConnection.ICAConnection_DragDrop"),
                                AssemblyInfo.Title(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
                this.SHCopyFiles(files, desktopShare);
        }

        private void ICAConnection_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop, false) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public override void Disconnect()
        {
            try
            {
                this.connected = false;
                this.iIcaClient.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(
                    string.Format(Localization.Text("Connection.ExternalConnection.Disconnect"), this.Favorite.Protocol,
                                  this.Favorite.Name), ex);
            }
        }

        private void SHCopyFiles(string[] sourceFiles, string destinationFolder)
        {
            ShellFileOperation fo = new ShellFileOperation();
            List<string> destinationFiles = new List<string>();

            foreach (string sourceFile in sourceFiles)
            {
                destinationFiles.Add(Path.Combine(destinationFolder, Path.GetFileName(sourceFile)));
            }

            fo.InvokeOperation(this.Handle, FileOperations.Copy, sourceFiles, destinationFiles.ToArray());
        }
    }
}