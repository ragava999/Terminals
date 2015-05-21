using System;
using System.Drawing;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;
using Terminals.Forms.Controls;

namespace Terminals.Panels.OptionPanels
{
    public partial class MasterPasswordOptionPanel : IOptionPanel
    {
        public MasterPasswordOptionPanel()
        {
            this.InitializeComponent();

            this.lblPasswordsMatch.Text = string.Empty;

            

            this.txtKeePassPassword.PasswordChar = this.ConfirmPasswordTextBox.PasswordChar = this.PasswordTextbox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
        }

        private bool PasswordsMatch
        {
            get { return this.PasswordTextbox.Text.Equals(this.ConfirmPasswordTextBox.Text); }
        }

        private bool PasswordsAreEntered
        {
            get
            {
                return !String.IsNullOrEmpty(this.PasswordTextbox.Text) &&
                       !String.IsNullOrEmpty(this.ConfirmPasswordTextBox.Text);
            }
        }

        public override void LoadSettings()
        {
            this.chkPasswordProtectTerminals.Checked = Settings.IsMasterPasswordDefined;
            this.PasswordTextbox.Enabled = Settings.IsMasterPasswordDefined;
            this.ConfirmPasswordTextBox.Enabled = Settings.IsMasterPasswordDefined;
            this.txtKeePassPassword.Text = Settings.KeePassPassword;
            this.txtKeePassPath.Text = Settings.KeePassPath;
            
            this.FillTextBoxesByMasterPassword(Settings.IsMasterPasswordDefined);
        }

        public override void SaveSettings()
        {
            if (!this.chkPasswordProtectTerminals.Checked && Settings.IsMasterPasswordDefined)
            {
                Settings.UpdateMasterPassword(string.Empty); // remove password
            }
            else // new password is defined
            {
                if (this.PasswordsMatch &&
                    !string.IsNullOrEmpty(this.PasswordTextbox.Text) &&
                    this.PasswordTextbox.Text != CredentialPanel.HIDDEN_PASSWORD)
                {
                    Settings.UpdateMasterPassword(this.PasswordTextbox.Text);
                }
            }
 
            bool restart = false;
            if (Settings.KeePassPassword != txtKeePassPassword.Text || Settings.KeePassPath != txtKeePassPath.Text)
            {
            	restart = true;
            }
            
            Settings.KeePassPassword = txtKeePassPassword.Text;
            Settings.KeePassPath = txtKeePassPath.Text;
            
            if (restart)
            {
            	MessageBox.Show("Your Credential configuration has changed. Please close your existing connections and restart Terminals.");
            }
        }

        public IHostingForm IHostingForm { get; set; }

        private void FillTextBoxesByMasterPassword(bool isMasterPasswordDefined)
        {
            if (isMasterPasswordDefined)
            {
                this.PasswordTextbox.Text = CredentialPanel.HIDDEN_PASSWORD;
                this.ConfirmPasswordTextBox.Text = this.PasswordTextbox.Text;
            }
            else
            {
                this.PasswordTextbox.Text = String.Empty;
                this.ConfirmPasswordTextBox.Text = String.Empty;
            }
        }

        private void chkPasswordProtectTerminals_CheckedChanged(object sender, EventArgs e)
        {
            this.PasswordTextbox.Enabled = this.chkPasswordProtectTerminals.Checked;
            this.ConfirmPasswordTextBox.Enabled = this.chkPasswordProtectTerminals.Checked;
            this.lblPasswordsMatch.Visible = this.chkPasswordProtectTerminals.Checked;
            this.lblPasswordsMatch.Text = String.Empty;
        }

        private void PasswordTextbox_TextChanged(object sender, EventArgs e)
        {
            this.CheckPasswords();
        }

        private void ConfirmPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            this.CheckPasswords();
        }

        private void CheckPasswords()
        {
            if (this.PasswordsAreEntered)
            {
                if (this.PasswordsMatch)
                {
                    this.lblPasswordsMatch.Text =
						"Password matches.";
                    this.lblPasswordsMatch.ForeColor = Color.Green;
                }
                else
                {
                    this.lblPasswordsMatch.Text =
						"Passwords do not match.";
                    this.lblPasswordsMatch.ForeColor = Color.Red;
                }
            }
        }
    }
}