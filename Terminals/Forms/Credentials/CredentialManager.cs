using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Kohl.Framework.Info;

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
                        
            if (Terminals.Configuration.Files.Main.Settings.Settings.KeePassUse)
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
            this.CredentialsListView.Items.Clear();
            List<CredentialSet> credentials = StoredCredentials.Items;   
            
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
                return StoredCredentials.GetByName(name);
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