using System;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Forms.Controls
{
    public partial class TabColorPreferences : UserControl
    {
        public TabColorPreferences()
        {
            this.InitializeComponent();

        }

        public void FillControls(FavoriteConfigurationElement favorite)
        {
            this.txtActive.Text = favorite.TabColor;
        }

        public void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.TabColor = this.txtActive.Text;
        }

        private void BtnActiveClick(object sender, EventArgs e)
        {
            this.colorDialog1.Color = FavoriteConfigurationElement.TranslateColor(this.txtActive.Text);
            DialogResult result = this.colorDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.txtActive.Text = FavoriteConfigurationElement.GetDisplayColor(this.colorDialog1.Color);
            }
        }
    }
}