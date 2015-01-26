using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Kohl.Framework.Converters;
using Kohl.Framework.Localization;

using Terminals.Configuration.Files.Main.Favorites;
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

        /*
        /// <summary>
        ///     Creates the and add tag node in tree list on proper position defined by index.
        ///     This allowes the tag nodes to keep ordered by name.
        /// </summary>
        /// <param name="tag"> The tag name to create. </param>
        /// <param name="index"> The index on which node would be inserted. If negative number, than it is added to the end. </param>
        /// <returns> Not null, newly creted node </returns>
        public TagTreeNode CreateAndAddTagNode(String tag, int index = -1)
        {
            TagTreeNode tagNode = new TagTreeNode(tag);
            InsertNodePreservingOrder(this.Nodes, index, tagNode);
            return tagNode;
        }

        private static void InsertNodePreservingOrder(TreeNodeCollection nodes, int index, TreeNode tagNode)
        {
            if (index < 0)
                nodes.Add(tagNode);
            else
                nodes.Insert(index, tagNode);
        }

        private void OnTreeViewExpand(object sender, TreeViewEventArgs e)
        {
            TagTreeNode tagNode = e.Node as TagTreeNode;
            LoadAllFavoritesUnderTag(tagNode);
        }

        public void LoadAllFavoritesUnderTag(TagTreeNode tagNode, string filter = null)
        {
            if (tagNode.NotLoadedYet)
            {
                tagNode.Nodes.Clear();
                AddFavoriteNodes(tagNode, filter);
            }
        }

        public override void Load(string filter = null)
        {
            try
            {
                // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
                // designer for this class.
                if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                {
                    // Clear all the tags
                    ClearTags();

                    if (!string.IsNullOrEmpty(filter))
                    {
                        // Filter the untagged node
                        //CreateAndAddTagNode(Settings.UNTAGGED_NODENAME);
                        FilterTags(Settings.UNTAGGED_NODENAME, filter);

                        TerminalsDataContext dc = new TerminalsDataContext();

                        // Filter all other tags
                        foreach (var group in dc.Groups)
                        {
                            CreateAndAddTagNode(group.Name);
                        }
                    }
                    else
                    {
                        // Don't filter load all tags
                        this.CreateAndAddTagNode(Settings.UNTAGGED_NODENAME);

                        // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
                        // designer for this class.
                        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                            return;

                        // because of designer
                        if (Settings.Tags(true) == null)
                            return;

                        int counter = 0;

                        TerminalsDataContext dc = new TerminalsDataContext();

                        foreach (var group in dc.Groups)
                        {
                            this.CreateAndAddTagNode(group.Name);

                            counter++;

                            if (Program.LimitTags != 0 && Program.LimitTags == counter)
                                break;
                        }
                    }
                }
            }
            catch
            {
                Kohl.Framework.Logging.Log.Error("Error loading database tree view.");
            }
        }

        private void FilterTags(string tagName, string filter)
        {
            TagTreeNode node = CreateAndAddTagNode(tagName);
            LoadAllFavoritesUnderTag(node, filter);
            if (node.Nodes.Count == 0)
                node.Remove();
        }

        public void ClearTags()
        {
            if (this == null || this.Nodes == null)
                return;

            this.Nodes.Clear();
        }

        private static void AddFavoriteNodes(TagTreeNode tagNode, string filter = null)
        {
            TerminalsDataContext dc = new TerminalsDataContext();

            int counter = 0;

            foreach (var connection in dc.ConnectionsInGroups.Where(item => item.Group.Name == tagNode.Name).Select(item => item.Connection))
            {
                FavoriteConfigurationElement favorite = new FavoriteConfigurationElement(connection.Name) { Protocol = connection.Protocol, Notes = connection.Notes ?? string.Empty, IsDatabaseFavorite = true  };

                if (string.IsNullOrEmpty(filter) || favorite.Name.ToUpper().Contains(filter.ToUpper()) || favorite.Name.ToUpper() == filter.ToUpper())
                {
                    FavoriteTreeNode favoriteTreeNode = new FavoriteTreeNode(favorite);

                    tagNode.Nodes.Add(favoriteTreeNode);

                    counter++;

                    if (Program.LimitFavorites != 0 && Program.LimitFavorites == counter)
                        break;
                }
            }
        }

        */
    }
}