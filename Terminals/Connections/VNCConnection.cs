using System;
using System.Drawing;
using System.Windows.Forms;

using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Properties;
using VncSharp;

namespace Terminals.Connections
{
    public class VNCConnection : Connection.Connection
    {
        private bool connected;
        private RemoteDesktop rd;
        private string vncPassword = "";

        protected override Image[] images
        {
            get { return new Image[] {Resources.VNC}; }
        }

        public override ushort Port
        {
            get { return 5900; }
        }

        public override bool Connected
        {
            get { return this.connected; }
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
        }

        public void SendSpecialKeys(SpecialKeys Keys)
        {
            this.rd.SendSpecialKeys(Keys);
        }

        public override bool Connect()
        {
            this.connected = false;

            try
            {
                this.rd = new RemoteDesktop
                              {
                                  AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                  AutoSize = true,
                                  AutoScroll = true
                              };

                this.vncPassword = this.Favorite.Credential.Password;

                if (string.IsNullOrEmpty(this.vncPassword)) return false;

                this.Embed(this.rd);

                this.rd.VncPort = this.Favorite.Port;
                this.rd.ConnectComplete += this.rd_ConnectComplete;
                this.rd.ConnectionLost += this.rd_ConnectionLost;
                this.rd.GetPassword = this.VNCPassword;

                if (this.InvokeRequired)
					this.Invoke(new MethodInvoker(delegate { this.Text = "Connecting to VNC server ..."; }));
                else
					this.Text = "Connecting to VNC server ...";

                if (this.rd.InvokeRequired)
                    this.rd.Invoke(new MethodInvoker(delegate { this.rd.Connect(this.Favorite.ServerName, this.Favorite.VncDisplayNumber, this.Favorite.VncViewOnly, this.Favorite.VncAutoScale);  }));
                else
                    this.rd.Connect(this.Favorite.ServerName, this.Favorite.VncDisplayNumber, this.Favorite.VncViewOnly, this.Favorite.VncAutoScale);

                this.rd.BringToFront();
                return this.connected = true;
            }
            catch (Exception exc)
            {
				Log.Error("Error occured while connecting to the VNC server.", exc);
                return this.connected = false;
            }
        }

        private void rd_ConnectionLost(object sender, EventArgs e)
        {
            this.connected = false;

            this.CloseTabPage();
        }

        private string VNCPassword()
        {
            return this.vncPassword;
        }

        private void rd_ConnectComplete(object sender, ConnectEventArgs e)
        {
            // Update Form to match geometry of remote desktop
            //ClientSize = new Size(e.DesktopWidth, e.DesktopHeight);
            try
            {
                this.connected = true;
                RemoteDesktop remoteDesktop = (RemoteDesktop) sender;
                remoteDesktop.Visible = true;
                remoteDesktop.BringToFront();
                remoteDesktop.FullScreenUpdate();
                remoteDesktop.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.Fatal(
					string.Format("Terminals was unable to create the {0} connection.", this.Favorite.Protocol),
                    ex);
            }
            // Change the Form's title to match desktop name
        }

        public override void Disconnect()
        {
            try
            {
                this.connected = false;
                this.rd.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(
					string.Format("Unable to disconnect form the {0} connection named \"{1}\".", this.Favorite.Protocol,
                                  this.Favorite.Name), ex);
            }
        }
    }
}