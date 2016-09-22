using Kohl.Framework.Converters;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Configuration.Sql;
using Terminals.Connection.Manager;

namespace Terminals.Forms.Controls
{
    public partial class DBTreeView : TreeViewBase
    {
        private readonly TreeListLoader loader;

        public DBTreeView()
        {
            this.ImageList = new ImageList();

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

                    using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                    {
                        // Filter all other tags
                        foreach (var group in dc.Groups)
                        {
                            FilterTags(group.Name, filter);
                        }
                    }
                }
                else
                {
                    // Don't filter load all tags
                    this.loader.LoadTags(true);
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
                this.loader.LoadFavorites(tagNode, true, filter);
        }

        private void OnTreeViewExpand(object sender, TreeViewEventArgs e)
        {
            TagTreeNode tagNode = e.Node as TagTreeNode;
            LoadAllFavoritesUnderTag(tagNode);
        }
    }
}