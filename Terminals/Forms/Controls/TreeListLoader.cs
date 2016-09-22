namespace Terminals.Forms.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Terminals.Configuration.Files.Main.Favorites;
    using Terminals.Configuration.Files.Main.Settings;
    using Terminals.Configuration.Files.Main.Tags;

    /// <summary>
    ///     Fills tree list with favorites
    /// </summary>
    public class TreeListLoader
    {
        private readonly TreeViewBase treeList;

        /// <summary>
        ///     gets or sets virtual tree node for favorites, which have no tag defined
        /// </summary>
        private TagTreeNode unTaggedNode;

        public TreeListLoader(TreeViewBase treeListToFill)
        {
            this.treeList = treeListToFill;

            FavoritesDataDispatcher.Instance.TagsChanged += this.OnTagsCollectionChanged;
            FavoritesDataDispatcher.Instance.FavoritesChanged += this.OnFavoritesCollectionChanged;
        }

        /// <summary>
        ///     Unregisters the Data dispatcher eventing.
        ///     Call this to release the treeview, otherwise it will result in memory gap.
        /// </summary>
        public void UnregisterEvents()
        {
            FavoritesDataDispatcher.Instance.TagsChanged -= this.OnTagsCollectionChanged;
            FavoritesDataDispatcher.Instance.FavoritesChanged -= this.OnFavoritesCollectionChanged;
        }

        private void OnFavoritesCollectionChanged(FavoritesChangedEventArgs args)
        {
            if (this.IsOrphan())
                return;

            string selectedTagName = this.treeList.FindSelectedTagNodeName();
            string selectedFavorite = this.treeList.GetSelectedFavoriteNodeName();
            this.RemoveFavorites(args.Removed);
            this.UpdateFavorites(args.Updated);
            this.AddNewFavorites(args.Added);
            selectedFavorite = args.GetUpdatedFavoriteName(selectedFavorite);
            this.treeList.RestoreSelectedFavorite(selectedTagName, selectedFavorite);
        }

        /// <summary>
        ///     This prevents performance problems, when someone forgets to unregister.
        ///     Returns true, if the associated treeview is already dead; otherwise false.
        /// </summary>
        private Boolean IsOrphan()
        {
            if (this.treeList.IsDisposed)
            {
                this.UnregisterEvents();
                return true;
            }

            return false;
        }

        private void RemoveFavorites(List<FavoriteConfigurationElement> removedFavorites)
        {
            foreach (FavoriteConfigurationElement favorite in removedFavorites)
            {
                foreach (String tag in favorite.TagList)
                {
                    TagTreeNode tagNode = this.treeList.Nodes[tag] as TagTreeNode;
                    RemoveFavoriteFromTagNode(tagNode, favorite.Name);
                }

                RemoveFavoriteFromTagNode(this.unTaggedNode, favorite.Name);
            }
        }

        private void UpdateFavorites(Dictionary<String, FavoriteConfigurationElement> updatedFavorites)
        {
            foreach (KeyValuePair<string, FavoriteConfigurationElement> updateArg in updatedFavorites)
            {
                foreach (TagTreeNode tagNode in this.treeList.Nodes)
                {
                    RemoveFavoriteFromTagNode(tagNode, updateArg.Key);
                }

                this.AddFavoriteToAllItsTagNodes(updateArg.Value);
            }
        }

        private void AddNewFavorites(List<FavoriteConfigurationElement> addedFavorites)
        {
            foreach (FavoriteConfigurationElement favorite in addedFavorites)
            {
                this.AddFavoriteToAllItsTagNodes(favorite);
            }
        }

        public TagTreeNode GetTagTreeNode(string name)
        {
            if (string.IsNullOrEmpty(name) || this.treeList == null || this.treeList.Nodes == null || this.treeList.Nodes[name] == null)
                return null;

            if (!(this.treeList.Nodes[name] is TagTreeNode))
                return null;

            return this.treeList.Nodes[name] as TagTreeNode;
        }

        private void AddFavoriteToAllItsTagNodes(FavoriteConfigurationElement favorite)
        {
            foreach (string tag in favorite.TagList)
            {
                TagTreeNode tagNode = GetTagTreeNode(tag);
                AddNewFavoriteNodeToTagNode(favorite, tagNode);
            }

            if (String.IsNullOrEmpty(favorite.Tags))
            {
                AddNewFavoriteNodeToTagNode(favorite, this.unTaggedNode);
            }
        }

        private static void RemoveFavoriteFromTagNode(TagTreeNode tagNode, String favoriteName)
        {
            if (tagNode != null && !tagNode.NotLoadedYet)
                tagNode.Nodes.RemoveByKey(favoriteName);
        }

        private static void AddNewFavoriteNodeToTagNode(FavoriteConfigurationElement favorite, TagTreeNode tagNode)
        {
            if (tagNode != null && !tagNode.NotLoadedYet) // add only to expanded nodes
            {
                FavoriteTreeNode favoriteTreeNode = new FavoriteTreeNode(favorite);
                int index = FindFavoriteNodeInsertIndex(tagNode.Nodes, favorite);
                InsertNodePreservingOrder(tagNode.Nodes, index, favoriteTreeNode);
            }
        }

        /// <summary>
        ///     Identify favorite index position in nodes collection by default sorting order.
        /// </summary>
        /// <param name="nodes"> Not null nodes collection of FavoriteTreeNodes to search in. </param>
        /// <param name="favorite"> Not null favorite to identify in nodes collection. </param>
        /// <returns> -1, if the tag should be added to the end of tag nodes, otherwise found index. </returns>
        public static int FindFavoriteNodeInsertIndex(TreeNodeCollection nodes, FavoriteConfigurationElement favorite)
        {
            for (int index = 0; index < nodes.Count; index++)
            {
                FavoriteTreeNode comparedNode = nodes[index] as FavoriteTreeNode;
                if (comparedNode.CompareByDefaultFavoriteSorting(favorite) > 0)
                    return index;
            }

            return -1;
        }

        private void OnTagsCollectionChanged(TagsChangedArgs args)
        {
            if (this.IsOrphan())
                return;

            this.RemoveUnusedTagNodes(args.Removed);
            this.AddMissingTagNodes(args.Added);
        }

        private void AddMissingTagNodes(List<String> newTags)
        {
            foreach (String newTag in newTags)
            {
                int index = FindTagNodeInsertIndex(this.treeList.Nodes, newTag);
                this.CreateAndAddTagNode(newTag, index);
            }
        }

        /// <summary>
        ///     Finds the index for the node to insert in nodes collection
        ///     and skips nodes before the startIndex.
        /// </summary>
        /// <param name="nodes"> Not null nodes collection to search in. </param>
        /// <param name="newTag"> Not empty new tag to add. </param>
        /// <returns> -1, if the tag should be added to the end of tag nodes, otherwise found index. </returns>
        private static int FindTagNodeInsertIndex(TreeNodeCollection nodes, string newTag)
        {
            // Skips first "Untagged" node to keep it first.
            for (int index = 1; index < nodes.Count; index++)
            {
                if (nodes[index].Text.CompareTo(newTag) > 0)
                    return index;
            }

            return -1;
        }

        private void RemoveUnusedTagNodes(List<String> removedTags)
        {
            foreach (String obsoletTag in removedTags)
            {
                this.treeList.Nodes.RemoveByKey(obsoletTag);
            }
        }

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
            InsertNodePreservingOrder(this.treeList.Nodes, index, tagNode);
            return tagNode;
        }

        private static void InsertNodePreservingOrder(TreeNodeCollection nodes, int index, TreeNode tagNode)
        {
            if (index < 0)
                nodes.Add(tagNode);
            else
                nodes.Insert(index, tagNode);
        }
        public void ClearTags()
        {
            if (this.treeList == null || this.treeList.Nodes == null)
                return;

            this.treeList.Nodes.Clear();
        }

        public void LoadTags(bool isDatabaseFavorite)
        {
            ClearTags();

            this.unTaggedNode = this.CreateAndAddTagNode(Settings.UNTAGGED_NODENAME);

            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            // because of designer
            if (Settings.Tags(false) == null)
                return;

            int counter = 0;

            if (!isDatabaseFavorite)
                foreach (string tagName in Settings.Tags(false))
                {
                    this.CreateAndAddTagNode(tagName);

                    counter++;

                    if (Program.LimitTags != 0 && Program.LimitTags == counter)
                        break;
                }
            else
                using (Terminals.Configuration.Sql.TerminalsObjectContext dc = Terminals.Configuration.Sql.TerminalsObjectContext.Create())
                {
                    if (dc == null)
                        return;

                    // Filter all other tags
                    foreach (var group in dc.Groups)
                    {
                        this.CreateAndAddTagNode(group.Name);

                        counter++;

                        if (Program.LimitTags != 0 && Program.LimitTags == counter)
                            break;
                    }
                }
        }

        public void LoadFavorites(TagTreeNode tagNode, bool isDatabaseFavorite, string filter = null)
        {
            if (tagNode.NotLoadedYet)
            {
                tagNode.Nodes.Clear();
                AddFavoriteNodes(tagNode, isDatabaseFavorite, filter);
            }
        }

        private static void AddFavoriteNodes(TagTreeNode tagNode, bool isDatabaseFavorite, string filter = null)
        {
            List<FavoriteConfigurationElement> tagFavorites = Settings.GetSortedFavoritesByTag(tagNode.Text, isDatabaseFavorite);

            int counter = 0;

            foreach (FavoriteConfigurationElement favorite in tagFavorites)
            {
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
    }
}