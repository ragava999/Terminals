using Kohl.Framework.Info;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Terminals.Configuration.Files.Credentials;

namespace Terminals.Forms.Credentials
{
    public partial class CredentialManager : Form
    {
        public CredentialManager()
        {
            this.InitializeComponent();
            this.Text = AssemblyInfo.Title + " - " +
                        "Credential manager";
            StoredCredentials.CredentialsChanged += this.CredentialsChanged;

            if (Terminals.Configuration.Files.Main.Settings.Settings.CredentialStore == Terminals.Configuration.Files.Main.CredentialStoreType.KeePass)
            {
                AddButton.Enabled = false;
                DeleteButton.Enabled = false;
            }
        }

        private void CredentialsChanged(object sender, EventArgs e)
        {
            this.BindList();
        }

        private void BindList()
        {
            CredentialsGrid.AutoGenerateColumns = false;
            CredentialsGrid.DataSource          = StoredCredentials.Items.ToArray();
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CredentialManager_Load(object sender, EventArgs e)
        {
            this.BindList();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            this.EditCredential(null);
        }

        private CredentialSet GetSelectedItemCredentials()
        {
            if (CredentialsGrid.SelectedRows.Count > 0)
            {
                return (CredentialSet)CredentialsGrid.SelectedRows[0].DataBoundItem;
            }

            return null;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            CredentialSet selected = this.GetSelectedItemCredentials();
            if (selected != null)
            {
                this.EditCredential(selected);
            }
        }

        private void EditCredential(CredentialSet selected)
        {
            ManageCredentialForm mgr = new ManageCredentialForm(selected);
            if (mgr.ShowDialog() == DialogResult.OK)
                this.BindList();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            CredentialSet toRemove = this.GetSelectedItemCredentials();
            if (toRemove != null)
            {
                if (
                    MessageBox.Show(
                        string.Format("Are you sure you want to delete credential {0}?",
                                      toRemove.Name), "Credential manager", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    StoredCredentials.Remove(toRemove);
                    StoredCredentials.Save();
                    this.BindList();
                }
            }
        }

        private void CredentialManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            StoredCredentials.CredentialsChanged -= this.CredentialsChanged;
        }
    }
}