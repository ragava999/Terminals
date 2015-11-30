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
    	public static readonly string MasterPasswordCaption = "";
    	
    	static MasterPasswordOptionPanel()
    	{
    		// This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;
            
    		MasterPasswordCaption = "By setting your Master Password allows " + Kohl.Framework.Info.AssemblyInfo.Title + " to store your connection information in a much more secure manner.  Although it is not required, it is highly recommended";
    	}
    	
        public MasterPasswordOptionPanel()
        {      	
            this.InitializeComponent();

            this.lblPasswordsMatch.Text = string.Empty;
            this.ConfirmPasswordTextBox.PasswordChar = this.PasswordTextbox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
            this.lblMasterPasswordCaption.Text = MasterPasswordCaption + ".";
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