using System;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Panels.OptionPanels
{
    public partial class DefaultPasswordOptionPanel : IOptionPanel
    {
        public DefaultPasswordOptionPanel()
        {
            this.InitializeComponent();
        }

        public override void LoadSettings()
        {
            this.domainTextbox.Text = Settings.DefaultDomain;
            this.usernameTextbox.Text = Settings.DefaultUsername;
            this.passwordTextBox.Text = Settings.DefaultPassword;
        }

        public override void SaveSettings()
        {
            Settings.DefaultDomain = this.domainTextbox.Text;
            Settings.DefaultUsername = this.usernameTextbox.Text;
            if (!String.IsNullOrEmpty(this.passwordTextBox.Text))
                Settings.DefaultPassword = this.passwordTextBox.Text;
        }

        public IHostingForm IHostingForm { get; set; }
    }
}