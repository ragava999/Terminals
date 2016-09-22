using Terminals.Connection.Manager;

namespace Terminals.Forms.Controls
{
    using Configuration.Files.Main;
    using Configuration.Files.Main.Favorites;
    using Configuration.Files.Main.Settings;
    using System.Windows.Forms;

    public class FavoriteTreeNode : TreeNode
    {
        public FavoriteTreeNode(FavoriteConfigurationElement favorite) : base(favorite.Name)
        {
            this.Name = favorite.Name;
            this.Favorite = favorite;
            this.Tag = favorite;

            this.ImageKey = ConnectionImageHandler.GetTreeviewImageListKey(favorite);
            this.SelectedImageKey = this.ImageKey;

            new System.Threading.Thread(new System.Threading.ThreadStart(new MethodInvoker(delegate
            {
                this.ToolTipText = favorite.GetToolTipText();
            }))).Start();
        }

        /// <summary>
        ///     Gets or sets the corresponding connection favorite
        /// </summary>
        public FavoriteConfigurationElement Favorite { get; private set; }

        /// <summary>
        ///     Returns text compareto method values selecting property to compare
        ///     depending on Settings default sort property value
        /// </summary>
        /// <param name="target"> not null favorite to compare with </param>
        /// <returns> result of CompareTo method </returns>
        public int CompareByDefaultFavoriteSorting(FavoriteConfigurationElement target)
        {
            return this.CompareByDefaultSorting(target, this.Favorite);
        }

        /// <summary>
        ///     Returns text compareto method values selecting property to compare
        ///     depending on Settings default sort property value
        /// </summary>
        /// <param name="target"> not null favorite to compare with </param>
        /// <param name="source"></param>
        /// <returns> result of String CompareTo method </returns>
        private int CompareByDefaultSorting(FavoriteConfigurationElement target, FavoriteConfigurationElement source)
        {
            switch (Settings.DefaultSortProperty)
            {
                case SortProperties.ServerName:
                    return source.ServerName.CompareTo(target.ServerName);
                case SortProperties.Protocol:
                    return source.Protocol.CompareTo(target.Protocol);
                case SortProperties.ConnectionName:
                    return source.Name.CompareTo(target.Name);
                default:
                    return -1;
            }
        }
    }
}