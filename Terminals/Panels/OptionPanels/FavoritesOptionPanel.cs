using System;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Panels.OptionPanels
{
    public partial class FavoritesOptionPanel : IOptionPanel
    {
        public FavoritesOptionPanel()
        {
            this.InitializeComponent();
        }

        public override void LoadSettings()
        {
            this.chkAutoCaseTags.Checked = Settings.AutoCaseTags;
            this.chkAutoExapandTagsPanel.Checked = Settings.AutoExapandTagsPanel;
            this.chkEnableFavoritesPanel.Checked = Settings.EnableFavoritesPanel;
            this.chkAutoSetTag.Checked = Settings.AutoSetTag;
            this.chkHideFavoritesFromQuickMenu.Checked = Settings.HideFavoritesFromQuickMenu;

            switch (Settings.DefaultSortProperty)
            {
                case SortProperties.ServerName:
                    this.ServerNameRadio.Checked = true;
                    break;
                case SortProperties.ConnectionName:
                    this.ConnectionNameRadioButton.Checked = true;
                    break;
                case SortProperties.Protocol:
                    this.ProtocolRadionButton.Checked = true;
                    break;
                case SortProperties.None:
                    this.NoneRadioButton.Checked = true;
                    break;
            }

            // Register event handlers to check if the user has changed any values. ->
            // If the user has changed the values reload the favorites -> due to new sort order.
            this.NoneRadioButton.CheckedChanged += this.FavoriteSortOrder_CheckedChanged;
            this.ProtocolRadionButton.CheckedChanged += this.FavoriteSortOrder_CheckedChanged;
            this.ConnectionNameRadioButton.CheckedChanged += this.FavoriteSortOrder_CheckedChanged;
            this.ServerNameRadio.CheckedChanged += this.FavoriteSortOrder_CheckedChanged;

            this.FavoritesFont.Font = Settings.FavoritesFont;
            this.FavoritesFont.BackColor = Settings.FavoritesFontBackColor;
            this.FavoritesFont.ForeColor = Settings.FavoritesFontForeColor;
            this.ImageHeight.Value = Settings.FavoritesImageHeight;
            this.ImageWidth.Value = Settings.FavoritesImageWidth;
        }

        /// <summary>
        /// Setting this value to true causes a reload of the favorites tree view.
        /// </summary>
        public bool Changed { get; private set; }

        public override void SaveSettings()
        {
            Settings.AutoSetTag = this.chkAutoSetTag.Checked;
            Settings.AutoCaseTags = this.chkAutoCaseTags.Checked;
            Settings.AutoExapandTagsPanel = this.chkAutoExapandTagsPanel.Checked;
            Settings.EnableFavoritesPanel = this.chkEnableFavoritesPanel.Checked;

            if (Settings.HideFavoritesFromQuickMenu != this.chkHideFavoritesFromQuickMenu.Checked)
                Changed = true;

            Settings.HideFavoritesFromQuickMenu = this.chkHideFavoritesFromQuickMenu.Checked;

            if (this.ServerNameRadio.Checked)
                Settings.DefaultSortProperty = SortProperties.ServerName;
            else if (this.NoneRadioButton.Checked)
                Settings.DefaultSortProperty = SortProperties.None;
            else if (this.ConnectionNameRadioButton.Checked)
                Settings.DefaultSortProperty = SortProperties.ConnectionName;
            else
                Settings.DefaultSortProperty = SortProperties.Protocol;

            // Don't overwrite a value of true that has already been set.
            if (Changed != true)
                if (Settings.FavoritesFont != this.FavoritesFont.Font || Settings.FavoritesFontBackColor != this.FavoritesFont.BackColor || Settings.FavoritesFontForeColor != this.FavoritesFont.ForeColor || Settings.FavoritesImageHeight != Convert.ToInt32(this.ImageHeight.Value) || Settings.FavoritesImageWidth != Convert.ToInt32(this.ImageWidth.Value))
                    Changed = true;
                else
                    Changed = false;
            
            Settings.FavoritesFont = this.FavoritesFont.Font;
            Settings.FavoritesFontBackColor = this.FavoritesFont.BackColor;
            Settings.FavoritesFontForeColor = this.FavoritesFont.ForeColor;
            Settings.FavoritesImageHeight = Convert.ToInt32(this.ImageHeight.Value);
            Settings.FavoritesImageWidth = Convert.ToInt32(this.ImageWidth.Value);
        }

        public new IHostingForm IHostingForm { get; set; }

        private void FavoriteSortOrder_CheckedChanged(object sender, EventArgs e)
        {
            Changed = true;
        }
    }
}