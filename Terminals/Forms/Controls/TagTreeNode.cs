using System;
using System.Linq;
using System.Windows.Forms;

namespace Terminals.Forms.Controls
{
    /// <summary>
    ///     Tree node for tags, this simulates lazy loading using dummy node,
    ///     until first expansion, where favorite nodes should replace the dummy node
    /// </summary>
    public class TagTreeNode : TreeNode
    {
        public const string DUMMY_NODE = null;

        public TagTreeNode(string tagName, string imageKey) : this(tagName)
        {
            this.ImageKey = imageKey;
            this.SelectedImageKey = imageKey;
        }

        public TagTreeNode(String tagName) : base(tagName, 0, 1)
        {
            this.Nodes.Add(String.Empty, DUMMY_NODE);
            this.Name = tagName;
        }

        /// <summary>
        ///     Gets the value indicating lazy loading not performed yet,
        ///     e.g. node contains only dummy node and contains no favorite nodes
        /// </summary>
        public Boolean NotLoadedYet
        {
            get
            {
                return this.Nodes.Count == 1 &&
                       String.IsNullOrEmpty(this.Nodes[0].Name);
            }
        }

        public bool ContainsFavoriteNode(string favoriteName)
        {
            return this.Nodes.Cast<FavoriteTreeNode>()
                       .Any(treeNode => treeNode.Favorite.Name == favoriteName);
        }
    }
}