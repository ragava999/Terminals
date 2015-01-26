using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Kohl.Framework.ExtensionMethods;
using Terminals.Configuration.Files.Credentials;
using Terminals.Configuration.Files.History;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Configuration.Files.Main.Tags;

namespace Terminals.Configuration.Files.Main.Favorites
{
    /// <summary>
    ///     Central point, which distributes informations about changes in Tags and Favorites collections
    /// </summary>
    public sealed class DataDispatcher
    {
        #region Thread safe singleton with lazy loading

        private DataDispatcher()
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                Settings.Settings.ConfigurationChanged += this.OnConfigFileReloaded;
        }

        /// <summary>
        ///     Gets the thread safe singleton instance of the dispatcher
        /// </summary>
        public static DataDispatcher Instance
        {
            get { return Nested.Instance; }
        }

        private static class Nested
        {
            private static DataDispatcher instance;

            public static DataDispatcher Instance
            {
                get
                {
                    // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
                    // designer for this class.
                    if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                        if (instance == null)
                            instance = new DataDispatcher();

                    return instance;
                }
            }
        }

        #endregion

        public event TagsChangedEventHandler TagsChanged;
        public event FavoritesChangedEventHandler FavoritesChanged;

        /// <summary>
        ///     Because filewatcher is created before the main form in GUI thread.
        ///     This lets to fire the file system watcher events in GUI thread.
        /// </summary>
        public static void AssignSynchronizationObject(ISynchronizeInvoke synchronizer)
        {
            Settings.Settings.AssignSynchronizationObject(synchronizer);
            ConnectionHistory.Instance.AssignSynchronizationObject(synchronizer);
            StoredCredentials.Instance.AssignSynchronizationObject(synchronizer);
        }

        private void OnConfigFileReloaded(ConfigurationChangedEventArgs args)
        {
            this.MergeTags(args);
            this.MergeFavorites(args);
        }

        private void MergeTags(ConfigurationChangedEventArgs args)
        {
            List<string> oldTags = args.OldTags;
            List<string> newTags = args.NewTags;
            List<string> deletedTags = oldTags.GetMissingSourcesInTarget(newTags);
            List<string> addedTags = newTags.GetMissingSourcesInTarget(oldTags);
            TagsChangedArgs tagsArgs = new TagsChangedArgs(addedTags, deletedTags);
            this.FireTagsChanged(tagsArgs);
        }

        private void MergeFavorites(ConfigurationChangedEventArgs args)
        {
            List<FavoriteConfigurationElement> oldFavorites = args.OldFavorites;
            List<FavoriteConfigurationElement> newFavorites = args.NewFavorites;
            List<FavoriteConfigurationElement> missingFavorites = GetMissingFavorites(newFavorites, oldFavorites);
            List<FavoriteConfigurationElement> redundantFavorites = GetMissingFavorites(oldFavorites, newFavorites);

            FavoritesChangedEventArgs favoriteArgs = new FavoritesChangedEventArgs();
            favoriteArgs.Added.AddRange(missingFavorites);
            favoriteArgs.Removed.AddRange(redundantFavorites);
            this.FireFavoriteChanges(favoriteArgs);
        }

        public static List<FavoriteConfigurationElement> GetMissingFavorites(
            List<FavoriteConfigurationElement> newFavorites,
            List<FavoriteConfigurationElement> oldFavorites)
        {
            return newFavorites.Where(
                newFavorite => oldFavorites.FirstOrDefault(oldFavorite => oldFavorite.Name == newFavorite.Name) == null)
                               .ToList();
        }

        public void ReportFavoriteAdded(FavoriteConfigurationElement addedFavorite)
        {
            FavoritesChangedEventArgs args = new FavoritesChangedEventArgs();
            args.Added.Add(addedFavorite);
            this.FireFavoriteChanges(args);
        }

        public void ReportFavoritesAdded(List<FavoriteConfigurationElement> addedFavorites)
        {
            FavoritesChangedEventArgs args = new FavoritesChangedEventArgs();
            args.Added.AddRange(addedFavorites);
            this.FireFavoriteChanges(args);
        }

        public void ReportFavoriteUpdated(string oldName, FavoriteConfigurationElement changedFavorite)
        {
            FavoritesChangedEventArgs args = new FavoritesChangedEventArgs();
            args.Updated.Add(oldName, changedFavorite);
            this.FireFavoriteChanges(args);
        }

        public void ReportFavoriteDeleted(FavoriteConfigurationElement deletedFavorite)
        {
            FavoritesChangedEventArgs args = new FavoritesChangedEventArgs();
            args.Removed.Add(deletedFavorite);
            this.FireFavoriteChanges(args);
        }

        public void ReportFavoritesDeleted(List<FavoriteConfigurationElement> deletedFavorites)
        {
            FavoritesChangedEventArgs args = new FavoritesChangedEventArgs();
            args.Removed.AddRange(deletedFavorites);
            this.FireFavoriteChanges(args);
        }

        private void FireFavoriteChanges(FavoritesChangedEventArgs args)
        {
            Debug.WriteLine(args.ToString());
            if (this.FavoritesChanged != null && !args.IsEmpty)
            {
                this.FavoritesChanged(args);
            }
        }

        public void ReportTagsAdded(List<String> addedsTag)
        {
            TagsChangedArgs args = new TagsChangedArgs();
            args.Added.AddRange(addedsTag);
            this.FireTagsChanged(args);
        }

        public void ReportTagsDeleted(List<String> deletedTags)
        {
            TagsChangedArgs args = new TagsChangedArgs();
            args.Removed.AddRange(deletedTags);
            this.FireTagsChanged(args);
        }

        public void ReportTagsRecreated(List<String> addedTags, List<String> deletedTags)
        {
            TagsChangedArgs args = new TagsChangedArgs(addedTags, deletedTags);
            this.FireTagsChanged(args);
        }

        private void FireTagsChanged(TagsChangedArgs args)
        {
            Debug.WriteLine(args.ToString());
            if (this.TagsChanged != null && !args.IsEmpty)
            {
                this.TagsChanged(args);
            }
        }
    }
}