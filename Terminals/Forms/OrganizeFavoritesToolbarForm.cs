using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms
{
    public partial class OrganizeFavoritesToolbarForm : Form
    {
        public OrganizeFavoritesToolbarForm()
        {
            this.InitializeComponent();
            foreach (string favoriteButton in Settings.FavoritesToolbarButtons)
            {
                this.lvFavoriteButtons.Items.Add(favoriteButton);
            }
        }

        private void lvFavoriteButtons_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem selectedItem = null;
            if (this.lvFavoriteButtons.SelectedItems.Count == 1)
            {
                selectedItem = this.lvFavoriteButtons.SelectedItems[0];
            }
            bool isSelected = (selectedItem != null);
            this.tsbMoveToFirst.Enabled = (isSelected && selectedItem.Index > 0);
            this.tsbMoveUp.Enabled = (isSelected && selectedItem.Index > 0);
            this.tsbMoveDown.Enabled = (isSelected && selectedItem.Index < this.lvFavoriteButtons.Items.Count - 1);
            this.tsbMoveToLast.Enabled = (isSelected && selectedItem.Index < this.lvFavoriteButtons.Items.Count - 1);
        }

        private void tsbMoveToFirst_Click(object sender, EventArgs e)
        {
            string name = this.lvFavoriteButtons.SelectedItems[0].Text;
            this.lvFavoriteButtons.SelectedItems[0].Remove();
            ListViewItem listViewItem = this.lvFavoriteButtons.Items.Insert(0, name);
            listViewItem.Selected = true;
        }

        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            string name = this.lvFavoriteButtons.SelectedItems[0].Text;
            int index = this.lvFavoriteButtons.SelectedItems[0].Index;
            this.lvFavoriteButtons.SelectedItems[0].Remove();
            ListViewItem listViewItem = this.lvFavoriteButtons.Items.Insert(index - 1, name);
            listViewItem.Selected = true;
        }

        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            string name = this.lvFavoriteButtons.SelectedItems[0].Text;
            int index = this.lvFavoriteButtons.SelectedItems[0].Index;
            this.lvFavoriteButtons.SelectedItems[0].Remove();
            ListViewItem listViewItem = this.lvFavoriteButtons.Items.Insert(index + 1, name);
            listViewItem.Selected = true;
        }

        private void tsbMoveToLast_Click(object sender, EventArgs e)
        {
            string name = this.lvFavoriteButtons.SelectedItems[0].Text;
            this.lvFavoriteButtons.SelectedItems[0].Remove();
            ListViewItem listViewItem = this.lvFavoriteButtons.Items.Add(name);
            listViewItem.Selected = true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            List<string> names = (from ListViewItem item in this.lvFavoriteButtons.Items select item.Text).ToList();
            Settings.UpdateFavoritesToolbarButtons(names);
        }
    }
}