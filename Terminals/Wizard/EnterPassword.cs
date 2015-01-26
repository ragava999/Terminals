using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Wizard
{
    public partial class EnterPassword : UserControl
    {
        public EnterPassword()
        {
            this.InitializeComponent();

            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (this.DesignMode)
                return;

            this.EnableMasterPassword.Checked = true;
            this.EnableMasterPassword.Enabled = true;
            this.panel1.Enabled = true;

            if (Settings.IsMasterPasswordDefined)
            {
                this.EnableMasterPassword.Checked = true;
                this.EnableMasterPassword.Enabled = false;
                this.panel1.Enabled = false;
            }

            this.confirmTextBox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
            this.masterPasswordTextbox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
        }

        public bool StorePassword
        {
            get
            {
                if (this.EnableMasterPassword.Checked && this.masterPasswordTextbox.Text != "") return true;
                return false;
            }
        }

        public string Password
        {
            get
            {
                if (this.masterPasswordTextbox.Text == this.confirmTextBox.Text)
                {
                    return this.masterPasswordTextbox.Text;
                }
                return "";
            }
        }

        private void confirmTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ErrorLabel.Text = this.masterPasswordTextbox.Text != this.confirmTextBox.Text ? "Passwords do not match!" : "Passwords match!";

            this.progressBar1.Value = PasswordStrength.Strength(this.masterPasswordTextbox.Text);
            if (this.progressBar1.Value <= 10)
            {
                this.progressBar1.ForeColor = Color.Red;
            }
            else if (this.progressBar1.Value <= 50)
            {
                this.progressBar1.ForeColor = Color.Yellow;
            }
            else if (this.progressBar1.Value <= 75)
            {
                this.progressBar1.ForeColor = Color.Green;
            }
            else if (this.progressBar1.Value <= 100)
            {
                this.progressBar1.ForeColor = Color.Blue;
            }
        }

        private void EnableMasterPassword_CheckedChanged(object sender, EventArgs e)
        {
            this.panel1.Enabled = this.EnableMasterPassword.Checked;
        }
    }
}