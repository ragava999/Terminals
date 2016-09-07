using System;
using System.Windows.Forms;
using Kohl.Framework.Info;

using Terminals.Configuration.Files.Credentials;
using Terminals.Forms.Controls;

namespace Terminals.Forms.Credentials
{
    public partial class ManageCredentialForm : Form
    {
        private string editedCredentialName = "";
        private string lastString = CredentialPanel.HIDDEN_PASSWORD;
        private bool reset;
        private Timer timer;

        public ManageCredentialForm(CredentialSet editedCredential)
        {
            this.InitializeComponent();

            this.FillControlsFromCredential(editedCredential);

            this.Text = AssemblyInfo.Title + " - " +
				"Credential manager";

            this.txtPassword.PasswordChar = CredentialPanel.HIDDEN_PASSWORD_CHAR;
            
            if (Terminals.Configuration.Files.Main.Settings.Settings.CredentialStore == Terminals.Configuration.Files.Main.CredentialStoreType.KeePass)
            {
            	SaveButton_cred.Enabled = false;
            }
        }

        private void FillControlsFromCredential(CredentialSet editedCredential)
        {
            if (editedCredential != null)
            {
                this.txtName.Text = editedCredential.Name;
                this.txtDomain.Text = editedCredential.Domain;
                this.txtUserName.Text = editedCredential.Username;
                if (!string.IsNullOrEmpty(editedCredential.Password))
                    this.txtPassword.Text = CredentialPanel.HIDDEN_PASSWORD;
                this.editedCredentialName = editedCredential.Name;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtName.Text) || string.IsNullOrEmpty(this.txtUserName.Text))
            {
				MessageBox.Show("You must enter both a name and a user name for the credential.",
					"Credential manager",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txtName.Focus();
                return;
            }

            if (this.reset)
                this.ResetLastString();

            if (this.UpdateCredential())
            {
                StoredCredentials.Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool UpdateCredential()
        {
            CredentialSet conflicting = StoredCredentials.GetByName(this.txtName.Text);
            CredentialSet oldItem = StoredCredentials.GetByName(this.editedCredentialName);

            if (conflicting != null && this.editedCredentialName != this.txtName.Text)
            {
                return this.UpdateConflicting(conflicting, oldItem);
            }

            this.UpdateOldOrCreateNew(oldItem);
            return true;
        }

        private void UpdateOldOrCreateNew(CredentialSet oldItem)
        {
            if (oldItem == null || this.editedCredentialName != this.txtName.Text)
            {
                CredentialSet newCredential = this.CreateNewCredential();
                StoredCredentials.Add(newCredential);
            }
            else
            {
                this.UpdateFromControls(oldItem);
            }
        }

        private bool UpdateConflicting(CredentialSet conflicting, CredentialSet oldItem)
        {
            DialogResult result =
				MessageBox.Show("The credential name you entered already exists.\nDo you want to overwrite it?",
			"Credential manager",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
                return false;

            if (oldItem != null)
            {
                StoredCredentials.Remove(oldItem);
            }

            this.UpdateFromControls(conflicting);
            return true;
        }

        private void UpdateFromControls(CredentialSet toUpdate)
        {
            toUpdate.Domain = this.txtDomain.Text;
            toUpdate.Name = this.txtName.Text;
            toUpdate.Username = this.txtUserName.Text;
            if (this.txtPassword.Text != CredentialPanel.HIDDEN_PASSWORD)
                toUpdate.SecretKey = this.txtPassword.Text;
        }

        private CredentialSet CreateNewCredential()
        {
            CredentialSet newItem = new CredentialSet();
            this.UpdateFromControls(newItem);
            return newItem;
        }

        private void ResetLastString()
        {
            this.txtPassword.PasswordChar = CredentialPanel.HIDDEN_PASSWORD_CHAR;

            this.reset = false;

            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer = null;
            }

            if (string.IsNullOrEmpty(this.lastString))
            {
                this.lastString = StoredCredentials.GetByName(this.editedCredentialName).SecretKey;
            }

            this.txtPassword.Text = this.lastString;
        }

        private void LblPasswordDoubleClick(object sender, EventArgs e)
        {
            if (this.txtPassword.PasswordChar == '\0')
            {
                this.ResetLastString();
            }
            else
            {
                this.lastString = this.txtPassword.Text;

                if (this.lastString == CredentialPanel.HIDDEN_PASSWORD)
				{
					this.txtPassword.Text = StoredCredentials.GetByName(this.editedCredentialName).SecretKey;

					if (!string.IsNullOrEmpty(StoredCredentials.GetByName(this.editedCredentialName).SecretKey))
						Clipboard.SetText(StoredCredentials.GetByName(this.editedCredentialName).SecretKey);
				}
				
                this.txtPassword.PasswordChar = '\0';
                this.reset = true;
                this.timer = new Timer();
                this.timer.Tick += delegate { this.ResetLastString(); };
                this.timer.Interval = 3000;
                this.timer.Enabled = true;
                this.timer.Start();
            }
        }
    }
}