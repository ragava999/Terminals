namespace Terminals.Configuration.Files.Main.Favorites
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Favorites changed event arguments container, informing about changes in Favorites collection
    /// </summary>
    public class FavoritesChangedEventArgs : EventArgs
    {
        public FavoritesChangedEventArgs()
        {
            this.Added = new List<FavoriteConfigurationElement>();
            this.Removed = new List<FavoriteConfigurationElement>();
            this.Updated = new Dictionary<string, FavoriteConfigurationElement>();
        }

        /// <summary>
        ///     All favorites actualy no longer stored in favorites persisted database
        /// </summary>
        public List<FavoriteConfigurationElement> Added { get; private set; }

        /// <summary>
        ///     Newly added favorites to the persisted database
        /// </summary>
        public List<FavoriteConfigurationElement> Removed { get; private set; }

        /// <summary>
        ///     Already stored favorites, where at least one prperty was changed.
        ///     Dictionary key is old name before the favorite changes occured.
        ///     Usefull when tracking rename changes.
        /// </summary>
        public Dictionary<string, FavoriteConfigurationElement> Updated { get; private set; }

        /// <summary>
        ///     Gets the value idicating if there are any added or reomoved items to report.
        /// </summary>
        public Boolean IsEmpty
        {
            get
            {
                return this.Added.Count == 0 &&
                       this.Removed.Count == 0 &&
                       this.Updated.Count == 0;
            }
        }

        public override string ToString()
        {
            return String.Format("FavoritesChangedEventArgs:Added={0};Removed={1};Changed={2}", this.Added.Count, this.Removed.Count, this.Updated.Count);
        }

        public string GetUpdatedFavoriteName(string oldName)
        {
            if (this.Updated.ContainsKey(oldName))
                return this.Updated[oldName].Name;

            return oldName;
        }
    }
}