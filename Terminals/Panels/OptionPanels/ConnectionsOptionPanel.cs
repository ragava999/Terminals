using System;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Panels.OptionPanels
{
    public partial class ConnectionsOptionPanel : IOptionPanel
    {
        private Int32 timeout = 5;

        public ConnectionsOptionPanel()
        {
            this.InitializeComponent();
        }

        public override void LoadSettings()
        {
            this.validateServerNamesCheckbox.Checked = Settings.ForceComputerNamesAsURI;
            this.warnDisconnectCheckBox.Checked = Settings.WarnOnConnectionClose;
            this.txtDefaultDesktopShare.Text = Settings.DefaultDesktopShare;
            this.PortscanTimeoutTextBox.Text = Settings.PortScanTimeoutSeconds.ToString();
            this.chkAskReconnectRdp.Checked = Settings.AskToReconnect;
        }

        public override void SaveSettings()
        {
            Settings.ForceComputerNamesAsURI = this.validateServerNamesCheckbox.Checked;
            Settings.WarnOnConnectionClose = this.warnDisconnectCheckBox.Checked;
            Settings.DefaultDesktopShare = this.txtDefaultDesktopShare.Text;
            Settings.AskToReconnect = chkAskReconnectRdp.Checked;

            Int32.TryParse(this.PortscanTimeoutTextBox.Text, out this.timeout);
            if (Settings.PortScanTimeoutSeconds <= 0 || Settings.PortScanTimeoutSeconds >= 60)
                this.timeout = 5;
            Settings.PortScanTimeoutSeconds = this.timeout;
        }

        public new IHostingForm IHostingForm { get; set; }
    }
}