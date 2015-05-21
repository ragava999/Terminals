using System;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Groups;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms
{
    public partial class OrganizeGroupsForm : Form
    {
        public OrganizeGroupsForm()
        {
            this.InitializeComponent();
            this.LoadGroups();
        }

        public bool SaveInDB { get; set; }

        private void LoadGroups()
        {
            this.lvGroups.BeginUpdate();
            try
            {
                this.lvGroups.Items.Clear();
                GroupConfigurationElementCollection groups = Settings.GetGroups();
                foreach (GroupConfigurationElement group in groups)
                {
                    ListViewItem item = this.lvGroups.Items.Add(group.Name);
                    item.Name = group.Name;
                    item.Tag = group;
                }
                if (this.lvGroups.Items.Count > 0)
                {
                    this.lvGroups.Items[0].Focused = true;
                    this.lvGroups.Items[0].Selected = true;
                }
            }
            finally
            {
                this.lvGroups.EndUpdate();
            }
        }

        private void LoadConnections(GroupConfigurationElement group)
        {
            this.lvConnections.BeginUpdate();
            try
            {
                this.lvConnections.Items.Clear();
                foreach (FavoriteAliasConfigurationElement favorite in group.FavoriteAliases)
                {
                    ListViewItem item = this.lvConnections.Items.Add(favorite.Name);
                    item.Name = favorite.Name;
                }
                if (this.lvConnections.Items.Count > 0)
                {
                    this.lvConnections.Items[0].Focused = true;
                    this.lvConnections.Items[0].Selected = true;
                }
            }
            finally
            {
                this.lvConnections.EndUpdate();
            }
        }

        private GroupConfigurationElement GetSelectedGroup()
        {
            if (this.lvGroups.SelectedItems.Count > 0)
                return (GroupConfigurationElement) this.lvGroups.SelectedItems[0].Tag;
            return null;
        }

        private void tsbDeleteGroup_Click(object sender, EventArgs e)
        {
            GroupConfigurationElement group = this.GetSelectedGroup();
            if (group != null)
            {
                Settings.DeleteGroup(group.Name);
                this.LoadGroups();
            }
        }

        private void lvGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            GroupConfigurationElement group = this.GetSelectedGroup();
            if (group != null)
            {
                this.LoadConnections(group);
            }
        }

        private void tsbAddConnection_Click(object sender, EventArgs e)
        {
            GroupConfigurationElement group = this.GetSelectedGroup();
            if (group != null)
            {
                AddConnectionForm frmAddConnection = new AddConnectionForm(SaveInDB);
                if (frmAddConnection.ShowDialog() == DialogResult.OK)
                {
                    foreach (string connection in frmAddConnection.Connections)
                    {
                        group.FavoriteAliases.Add(new FavoriteAliasConfigurationElement(connection));
                        this.lvConnections.Items.Add(connection);
                    }
                    Settings.DeleteGroup(group.Name);
                    Settings.AddGroup(group);
                }
            }
        }

        private void tsbDeleteConnection_Click(object sender, EventArgs e)
        {
            GroupConfigurationElement group = this.GetSelectedGroup();
            if (group != null)
            {
                if (this.lvConnections.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in this.lvConnections.SelectedItems)
                    {
                        group.FavoriteAliases.Remove(item.Text);
                        Settings.DeleteGroup(group.Name);
                        Settings.AddGroup(group);
                        item.Remove();
                    }
                }
            }
        }

        private void tsbAddGroup_Click(object sender, EventArgs e)
        {
            using (NewGroupForm frmNewGroup = new NewGroupForm())
            {
                if (frmNewGroup.ShowDialog() == DialogResult.OK)
                {
                    GroupConfigurationElement serversGroup = new GroupConfigurationElement
                                                                 {
                                                                     Name = frmNewGroup.txtGroupName.Text,
                                                                     FavoriteAliases = new FavoriteAliasConfigurationElementCollection()
                                                                 };
                    Settings.AddGroup(serversGroup);
                    this.LoadGroups();
                }
            }
        }
    }
}