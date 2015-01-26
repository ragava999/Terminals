using System;
using System.Drawing;
using System.Windows.Forms;
using AxMicrosoft.VMRCClientControl.Interop;
using Kohl.Framework.Localization;
using Kohl.Framework.Logging;
using Microsoft.VMRCClientControl.Interop;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection;
using Terminals.Connection.Manager;
using Terminals.Properties;

namespace Terminals.Connections
{
    public class VMRCConnection : Connection.Connection
    {
        private bool connected;
        private AxVMRCClientControl vmrc;

        protected override Image[] images
        {
            get { return new Image[] {Resources.VMRC}; }
        }

        public override ushort Port
        {
            get { return 5900; }
        }

        public override bool Connected
        {
            get { return this.connected; }
        }

        public bool ViewOnlyMode
        {
            get
            {
                if (this.vmrc != null && this.connected) return this.vmrc.ViewOnlyMode;
                return false;
            }
            set { if (this.vmrc != null && this.connected) this.vmrc.ViewOnlyMode = value; }
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
        }

        public override bool Connect()
        {
            this.connected = false;

            try
            {
                this.vmrc = new AxVMRCClientControl();

                try
                {
                    this.Embed(this.vmrc);
                }
                catch (Exception ex)
                {
                    Log.Fatal(Localization.Text("Connections.VMRCConnection.Connect_Fatal"), ex);
                    return false;
                }

                Size size = GetSize();

                if (this.vmrc.InvokeRequired)
                    this.vmrc.Invoke(new MethodInvoker(delegate
                    {
                        this.vmrc.UserName = this.Favorite.Credential.UserName;
                        this.vmrc.ServerAddress = this.Favorite.ServerName;
                        this.vmrc.ServerPort = this.Favorite.Port;
                        this.vmrc.UserDomain = this.Favorite.Credential.DomainName;
                        this.vmrc.UserPassword = this.Favorite.Credential.Password;
                        this.vmrc.AdministratorMode = this.Favorite.VmrcAdministratorMode;
                        this.vmrc.ReducedColorsMode = this.Favorite.VmrcReducedColorsMode;
                        this.vmrc.Size = size;
                        this.vmrc.ShrinkEnabled = true;
                        this.vmrc.CtlAutoSize = true;
                        this.vmrc.AllowDrop = true;
                    }));
                else
                {
                    this.vmrc.UserName = this.Favorite.Credential.UserName;
                    this.vmrc.ServerAddress = this.Favorite.ServerName;
                    this.vmrc.ServerPort = this.Favorite.Port;
                    this.vmrc.UserDomain = this.Favorite.Credential.DomainName;
                    this.vmrc.UserPassword = this.Favorite.Credential.Password;
                    this.vmrc.AdministratorMode = this.Favorite.VmrcAdministratorMode;
                    this.vmrc.ReducedColorsMode = this.Favorite.VmrcReducedColorsMode;
                    this.vmrc.Size = size;
                    this.vmrc.ShrinkEnabled = true;
                    this.vmrc.CtlAutoSize = true;
                    this.vmrc.AllowDrop = true;
                }

                this.vmrc.OnStateChanged += this.vmrc_OnStateChanged;
                this.vmrc.OnSwitchedDisplay += this.vmrc_OnSwitchedDisplay;

                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(delegate { this.Text = Localization.Text("Connections.VMRCConnection.Connect_Info"); }));
                else
                    this.Text = Localization.Text("Connections.VMRCConnection.Connect_Info");

                this.vmrc.Connect();

                return this.connected = true;
            }
            catch (Exception exc)
            {
                Log.Fatal(
                    string.Format(Localization.Text("Connections.HTTPConnection.Connect_Error2"), this.Favorite.Protocol),
                    exc);
                return this.connected = false;
            }
        }

        private void vmrc_OnSwitchedDisplay(object sender, _IVMRCClientControlEvents_OnSwitchedDisplayEvent e)
        {
            this.Text = e.displayName;
        }

        private void vmrc_OnStateChanged(object sender, _IVMRCClientControlEvents_OnStateChangedEvent e)
        {
            if (e.state == VMRCState.vmrcState_Connecting)
                return;

            if (e.state == VMRCState.vmrcState_Connected)
                this.connected = true;
            else
            {
                if (e.state == VMRCState.vmrcState_ConnectionFailed)
                    Log.Fatal(string.Format(Localization.Text("Connections.VMRCConnection.OnStateChanged_Fatal1"),
                                            this.Favorite.Name));

                if (e.state == VMRCState.vmrcState_NotConnected)
                    Log.Fatal(string.Format(Localization.Text("Connections.VMRCConnection.OnStateChanged_Fatal2"),
                                            this.Favorite.Name));

                this.connected = false;

                this.CloseTabPage();
            }
        }

        public void AdminDisplay()
        {
            if (this.vmrc != null && this.connected)
            {
                this.vmrc.AdministratorMode = true;
                this.vmrc.AdminDisplay();
            }
        }

        public override void Disconnect()
        {
            this.connected = false;
            try
            {
                this.vmrc.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(
                    string.Format(Localization.Text("Connection.ExternalConnection.Disconnect"), this.Favorite.Protocol,
                                  this.Favorite.Name), ex);
            }
        }
    }
}