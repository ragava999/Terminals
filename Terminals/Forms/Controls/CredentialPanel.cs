using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Terminals.Configuration.Files.Credentials;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Forms.Credentials;
using System.Linq;

namespace Terminals.Forms.Controls
{
    public partial class CredentialPanel : FavoritePanel
    {
        public const char HIDDEN_PASSWORD_CHAR = '●';
        public const String HIDDEN_PASSWORD = "●●●●●●●●●●●●●●●●";

        public CredentialPanel()
        {
            this.InitializeComponent();
            this.txtPassword.PasswordChar = HIDDEN_PASSWORD_CHAR;
        }

        public CredentialSet SelectedCredentialSet
        {
            get { return (this.CredentialDropdown.SelectedItem as CredentialSet); }
        }

        public string FavoritePassword { private get; set; }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.DomainName = this.cmbDomains.Text;
            favorite.UserName = this.cmbUsers.Text;
            if (this.chkSavePassword.Checked)
            {
                favorite.Password = this.txtPassword.Text != HIDDEN_PASSWORD ? this.txtPassword.Text : this.FavoritePassword;
            }
            else
            {
                favorite.Password = String.Empty;
            }
        }

        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            this.cmbDomains.Text = favorite.Credential.DomainName;
            this.cmbUsers.Text = favorite.Credential.UserName;

            if (!favorite.Credential.IsSetPassword && favorite.Credential.IsSetEncryptedPassword)
            {
                MessageBox.Show("There was an issue decrypting your password.\n\nPlease provide a new password and save the favorite.", AssemblyInfo.Title(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.txtPassword.Text = "";
                this.FavoritePassword = "";
                this.txtPassword.Focus();
                favorite.Password = "";
            }

            if (favorite.Credential.IsSetPassword)
            {
                this.txtPassword.Text = HIDDEN_PASSWORD;
                this.chkSavePassword.Checked = true;
            }
            else
            {
                this.txtPassword.Text = String.Empty;
                this.chkSavePassword.Checked = false;
            }

            this.FavoritePassword = favorite.Credential.Password;
        }

        public void LoadMRUs()
        {
            this.cmbDomains.Items.AddRange(Settings.MRUDomainNames);
            this.cmbUsers.Items.AddRange(Settings.MRUUserNames);
        }

        public void SaveMRUs()
        {
            Settings.AddDomainMRUItem(this.cmbDomains.Text);
            Settings.AddUserMRUItem(this.cmbUsers.Text);
        }

        private void CredentialDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CredentialsPanel.Enabled = true;
            CredentialSet set = (this.CredentialDropdown.SelectedItem as CredentialSet);

            if (set != null)
            {
                this.CredentialsPanel.Enabled = false;
                this.cmbDomains.Text = set.Domain;
                this.cmbUsers.Text = set.Username;
                this.txtPassword.Text = set.SecretKey;
                this.chkSavePassword.Checked = true;
            }
        }

        private void CredentialManagerPicturebox_Click(object sender, EventArgs e)
        {
            String cred = String.Empty;
            if (this.CredentialDropdown.SelectedItem.GetType() != typeof(string))
                cred = ((CredentialSet)this.CredentialDropdown.SelectedItem).Name;

            CredentialManager mgr = new CredentialManager();
            mgr.ShowDialog();
            this.FillCredentials(cred);
        }

        public void FillCredentials(String credentialName)
        {
            this.CredentialDropdown.Items.Clear();
            List<CredentialSet> creds = StoredCredentials.Items;
            this.CredentialDropdown.Items.Add("(custom)");

            Int32 selIndex = 0;
            if (creds != null)
            {
                foreach (CredentialSet item in creds)
                {
                    Int32 index = this.CredentialDropdown.Items.Add(item);
                    if (!String.IsNullOrEmpty(credentialName) && credentialName == item.Name)
                        selIndex = index;
                }
            }

            this.CredentialDropdown.SelectedIndex = selIndex;
        }
    }
}