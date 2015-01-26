namespace Terminals.Forms.Controls
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Kohl.Framework.Converters;

    using Configuration.Files.Main.Favorites;
    using Configuration.Files.Main.Settings;
    using Connection;
    using Kohl.Framework.Localization;
    using Terminals.Connection.Manager;

    /// <summary>
    ///     Treeview in main window to present favorites organized by Tags
    /// </summary>
    public class FavoritesTreeView : TreeViewBase
    {
        private readonly TreeListLoader loader;

        public FavoritesTreeView()
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                this.ImageList = ConnectionImageHandler.GetProtocolImageList();
                this.ImageIndex = 0;
                this.LineColor = Color.Black;
                this.SelectedImageIndex = 0;
                this.ShowNodeToolTips = true;
                this.AfterExpand += this.OnTreeViewExpand;

                this.ImageList.ImageSize = new Size(Settings.FavoritesImageWidth, Settings.FavoritesImageHeight);
                this.ImageList = ConnectionImageHandler.GetProtocolImageList();
                this.ImageList.ImageSize = new Size(Settings.FavoritesImageWidth, Settings.FavoritesImageHeight);
                this.Font = FontParser.FromString(Settings.FavoritesFont);
                this.BackColor = ColorParser.FromString(Settings.FavoritesFontBackColor);
                this.ForeColor = ColorParser.FromString(Settings.FavoritesFontForeColor);

                this.loader = new TreeListLoader(this);
            }
        }

        private void FilterTags(string tagName, string filter)
        {
            TagTreeNode node = this.loader.CreateAndAddTagNode(tagName);
            LoadAllFavoritesUnderTag(node, filter);
            if (node.Nodes.Count == 0)
                node.Remove();
        }

        public override void Load(string filter = null)
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    // Clear all the tags
                    this.loader.ClearTags();

                    // Filter the untagged node
                    FilterTags(Settings.UNTAGGED_NODENAME, filter);

                    // Filter all other tags
                    foreach (string tagName in Settings.Tags(false))
                    {
                        FilterTags(tagName, filter);
                    }
                }
                else
                {
                    // Don't filter load all tags
                    this.loader.LoadTags(false);
                }
            }
        }

        public void UnregisterEvents()
        {
            this.loader.UnregisterEvents();
        }

        public override void LoadAllFavoritesUnderTag(TagTreeNode tagNode, string filter = null)
        {
            if (tagNode != null)
                this.loader.LoadFavorites(tagNode, false, filter);
        }

        private void OnTreeViewExpand(object sender, TreeViewEventArgs e)
        {
            TagTreeNode tagNode = e.Node as TagTreeNode;
            LoadAllFavoritesUnderTag(tagNode);
        }
    }
}