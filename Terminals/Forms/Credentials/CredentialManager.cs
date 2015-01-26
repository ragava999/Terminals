using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Kohl.Framework.Localization;
using Terminals.Configuration.Files.Credentials;

namespace Terminals.Forms.Credentials
{
    public partial class CredentialManager : Form
    {
        public CredentialManager()
        {
            this.InitializeComponent();
            this.Text = AssemblyInfo.Title() + " - " +
                        Localization.Text("Credentials.Credential.CredentialManager_Caption");
            StoredCredentials.Instance.CredentialsChanged += this.CredentialsChanged;
            Localization.SetLanguage(this);
        }

        private void CredentialsChanged(object sender, EventArgs e)
        {
            this.BindList();
        }

        private void BindList()
        {
            this.CredentialsListView.Items.Clear();
            List<CredentialSet> credentials = StoredCredentials.Instance.Items;

            foreach (CredentialSet credential in credentials)
            {
                ListViewItem item = new ListViewItem(credential.Name);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, credential.Username));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, credential.Domain));
                this.CredentialsListView.Items.Add(item);
            }

            // Auto resize the form to prevent showing a horizontal scroll bar.
            // Only show the vertical or nothing.
            if (this.CredentialsListView.Items.Count > 10)
            {
                this.Size = new Size(483, 250);
            }
            else
            {
                this.panel1.Location = new Point(350, 0);
                this.panel1.Size = new Size(100, 212);
                this.Size = new Size(466, 250);
            }
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
            if (this.CredentialsListView.SelectedItems != null && this.CredentialsListView.SelectedItems.Count > 0)
            {
                string name = this.CredentialsListView.SelectedItems[0].Text;
                return StoredCredentials.Instance.GetByName(name);
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
                        string.Format(Localization.Text("Credentials.Credential.CredentialManager.DeleteButton_Click"),
                                      toRemove.Name),
                        Localization.Text("Credentials.Credential.CredentialManager_Caption"), MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    StoredCredentials.Instance.Remove(toRemove);
                    StoredCredentials.Instance.Save();
                    this.BindList();
                }
            }
        }

        private void CredentialManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            StoredCredentials.Instance.CredentialsChanged -= this.CredentialsChanged;
        }
    }
}