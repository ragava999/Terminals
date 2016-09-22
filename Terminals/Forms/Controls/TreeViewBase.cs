using System.Linq;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Forms.Controls
{
    public class TreeViewBase : TreeView
    {
        public virtual void LoadAllFavoritesUnderTag(TagTreeNode tagNode, string filter = null)
        {
        }

        public virtual void Load(string filter = null)
        {
        }

        public string GetSelectedFavoriteNodeName()
        {
            FavoriteConfigurationElement selectedFavorite = this.SelectedFavorite;

            if (selectedFavorite != null)
                return selectedFavorite.Name;

            return string.Empty;
        }

        public void RestoreSelectedFavorite(string tagNodeName, string favoriteName)
        {
            if (string.IsNullOrEmpty(tagNodeName) || string.IsNullOrEmpty(favoriteName))
                return;

            TreeNode tagNode = this.GetTagNodeByName(tagNodeName);
            TreeNode favoriteNode = FindFavoriteNodeByName(tagNode, favoriteName);

            // tag node has been removed, try find another one
            if (tagNode == null || favoriteNode == null)
                tagNode = this.FindFirstTagNodeContainingFavorite(favoriteName);

            this.SelectedNode = FindFavoriteNodeByName(tagNode, favoriteName);
        }

        private TreeNode FindFirstTagNodeContainingFavorite(string favoriteName)
        {
            return (from TreeNode tagNode in this.Nodes let favoriteNode = FindFavoriteNodeByName(tagNode, favoriteName) where favoriteNode != null select tagNode).FirstOrDefault();
        }

        private static FavoriteTreeNode FindFavoriteNodeByName(TreeNode tagNode, string favoriteName)
        {
            if (tagNode == null)
                return null;

            return tagNode.Nodes.OfType<FavoriteTreeNode>().FirstOrDefault(favoriteNode => favoriteNode.Favorite.Name.Equals(favoriteName));
        }

        public FavoriteConfigurationElement SelectedFavorite
        {
            get
            {
                FavoriteTreeNode selectedFavoriteNode = this.SelectedNode as FavoriteTreeNode;

                if (selectedFavoriteNode != null)
                    return selectedFavoriteNode.Favorite;

                return null;
            }
        }

        public TagTreeNode GetTagNodeByName(string tagName)
        {
            return this.Nodes.Cast<TagTreeNode>().FirstOrDefault(tagNode => tagNode.Name == tagName);
        }

        public string FindSelectedTagNodeName()
        {
            if (this.SelectedNode == null || this.SelectedNode.Parent == null)
                return string.Empty;

            TreeNode tagNode = this.SelectedNode.Parent;
            return tagNode.Name;
        }
    }
}
