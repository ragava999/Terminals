using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms
{
    public partial class AddConnectionForm : Form
    {
        private List<string> connections;

        public AddConnectionForm(bool saveInDB)
        {
            this.InitializeComponent();
            FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(saveInDB);
            foreach (FavoriteConfigurationElement favorite in favorites)
            {
                this.lvFavorites.Items.Add(favorite.Name);
            }
        }

        public List<string> Connections
        {
            get { return this.connections; }
        }

        private void lvFavorites_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            this.SetButtonOkState();
        }

        private void SetButtonOkState()
        {
            this.btnOk.Enabled = this.lvFavorites.CheckedItems.Count > 0;
        }

        private void txtServerName_TextChanged(object sender, EventArgs e)
        {
            this.SetButtonOkState();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.connections = new List<string>();
            foreach (ListViewItem item in this.lvFavorites.CheckedItems)
            {
                this.connections.Add(item.Text);
            }
        }
    }
}