using System;
using System.Collections.Generic;
using System.Linq;
using Kohl.Framework.ExtensionMethods;
using Kohl.Framework.Lists;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Configuration.Files.Main.Settings
{
    using Sql;

    public static partial class Settings
    {
        /// <summary>
        ///     Gets the default tag name for favorites without any tag
        /// </summary>
        public const String UNTAGGED_NODENAME = "Untagged";

        public static void EditFavorite(string oldName, FavoriteConfigurationElement favorite)
        {
            if (favorite == null)
                return;

            StartDelayedUpdate();
            EditFavoriteInSettings(favorite, oldName);

            SaveAndFinishDelayedUpdate();
            DataDispatcher.Instance.ReportFavoriteUpdated(oldName, favorite);
        }

        public static void ApplyCredentialsForAllSelectedFavorites(List<FavoriteConfigurationElement> selectedFavorites, string credentialName)
        {
            StartDelayedUpdate();

            foreach (FavoriteConfigurationElement favorite in selectedFavorites)
            {
                favorite.XmlCredentialSetName = credentialName;
                UpdateFavorite(favorite.Name, favorite);
            }

            SaveAndFinishDelayedUpdate();
        }

        public static void SetPasswordToAllSelectedFavorites(List<FavoriteConfigurationElement> selectedFavorites, string newPassword)
        {
            StartDelayedUpdate();
            foreach (FavoriteConfigurationElement favorite in selectedFavorites)
            {
                favorite.Password = newPassword;
                UpdateFavorite(favorite.Name, favorite);
            }
            SaveAndFinishDelayedUpdate();
        }

        public static void ApplyDomainNameToAllSelectedFavorites(List<FavoriteConfigurationElement> selectedFavorites, string newDomainName)
        {
            StartDelayedUpdate();
            foreach (FavoriteConfigurationElement favorite in selectedFavorites)
            {
                favorite.DomainName = newDomainName;
                UpdateFavorite(favorite.Name, favorite);
            }
            SaveAndFinishDelayedUpdate();
        }

        public static void ApplyUserNameToAllSelectedFavorites(List<FavoriteConfigurationElement> selectedFavorites, string newUserName)
        {
            StartDelayedUpdate();
            foreach (FavoriteConfigurationElement favorite in selectedFavorites)
            {
                favorite.UserName = newUserName;
                UpdateFavorite(favorite.Name, favorite);
            }
            SaveAndFinishDelayedUpdate();
        }


        public static void DeleteFavorites(List<FavoriteConfigurationElement> favorites)
        {
            if (favorites == null || favorites.Count == 0)
                return;

            List<String> deletedTags = new List<string>();
            StartDelayedUpdate();

            foreach (FavoriteConfigurationElement favorite in favorites)
            {
                DeleteFavoriteFromSettings(favorite.Name, favorite.IsDatabaseFavorite);
                List<String> difference = DeleteTagsFromSettings(favorite.TagList, favorite.IsDatabaseFavorite);
                deletedTags.AddRange(difference);
            }

            SaveAndFinishDelayedUpdate();
            DataDispatcher.Instance.ReportTagsDeleted(deletedTags);
            DataDispatcher.Instance.ReportFavoritesDeleted(favorites);
        }

        public static void AddFavorite(FavoriteConfigurationElement favorite)
        {
            AddFavoriteToSettings(favorite);
            List<String> addedTags = AddTagsToSettings(favorite.TagList, favorite.IsDatabaseFavorite);
            DataDispatcher.Instance.ReportTagsAdded(addedTags);
            DataDispatcher.Instance.ReportFavoriteAdded(favorite);
        }

        /// <summary>
        ///     Replaces the favorite stored with <paramref name="oldName" /> by copy of the <paramref name="favorite" />
        /// </summary>
        /// <param name="oldName"> favorite.Name value, before the favorite was changed </param>
        /// <param name="favorite"> Not null updated connection favorite </param>
        private static void UpdateFavorite(string oldName, FavoriteConfigurationElement favorite)
        {
            EditFavoriteInSettings(favorite, oldName);
            DataDispatcher.Instance.ReportFavoriteUpdated(oldName, favorite);
        }

        private static void EditFavoriteInSettings(FavoriteConfigurationElement favorite, string oldName)
        {
            if (!favorite.IsDatabaseFavorite)
            {
                TerminalsConfigurationSection section = GetSection();
                section.Favorites[oldName] = favorite.Clone() as FavoriteConfigurationElement;
                SaveImmediatelyIfRequested();
            }
            else
            {
                using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                {
                    Sql.Connections connection = favorite.ToConnection(dc, dc.Connections.Where(x => x.Name == oldName).FirstOrDefault());
                    dc.SaveChanges();
                }
            }
        }

        private static void DeleteFavoriteFromSettings(string name, bool isDatabaseFavorite)
        {
            if (!isDatabaseFavorite)
            {
                GetSection().Favorites.Remove(name);
                SaveImmediatelyIfRequested();
            }
            else
            {
                using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                {
                    Sql.Connections connection = dc.Connections.Where(x => x.Name == name).FirstOrDefault();

                    if (connection != null)
                        dc.Connections.DeleteObject(connection);

                    dc.SaveChanges();
                }
            }

            DeleteFavoriteButton(name);
        }

        /// <summary>
        ///     Adds favorite to the database, but doesnt fire the changed event
        /// </summary>
        private static void AddFavoriteToSettings(FavoriteConfigurationElement favorite)
        {
            if (!favorite.IsDatabaseFavorite)
            {
                GetSection().Favorites.Add(favorite);
                SaveImmediatelyIfRequested();
            }
            else
            {
                using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                {
                    Connections connection = favorite.ToConnection(dc);

                    if (!dc.Connections.Contains(connection))
                        dc.Connections.AddObject(connection);

                    dc.SaveChanges();
                }
            }
        }

        /// <summary>
        ///     Adds all favorites as new in the configuration as a batch and saves configuration after all are imported.
        /// </summary>
        /// <param name="favorites"> Not null collection of favorites to import </param>
        public static void AddFavorites(List<FavoriteConfigurationElement> favorites)
        {
            if (favorites == null || favorites.Count == 0)
                return;

            StartDelayedUpdate();
            List<String> tagsToAdd = new List<string>();
            foreach (FavoriteConfigurationElement favorite in favorites)
            {
				try
				{
					if (favorite == null)
						continue;

					if (favorite.Tags == null)
						favorite.Tags = "";

					if (string.IsNullOrEmpty(favorite.Name))
					{
						string guid = Guid.NewGuid().ToString();
						favorite.Name = guid;
						Kohl.Framework.Logging.Log.Error ("Imported favorite has no name, choosing a guid instead '" + guid + "'.");
					}

	                AddFavoriteToSettings(favorite);
	                List<String> difference = favorite.TagList.GetMissingSourcesInTarget(tagsToAdd);
	                tagsToAdd.AddRange(difference);
				}
				catch (Exception ex)
				{
					Kohl.Framework.Logging.Log.Error ("Unable to add imported favorite " + favorite.Name + ".", ex);
				}
            }

            List<String> addedTags = AddTagsToSettings(tagsToAdd, favorites[0].IsDatabaseFavorite);
            SaveAndFinishDelayedUpdate();

            DataDispatcher.Instance.ReportTagsAdded(addedTags);
            DataDispatcher.Instance.ReportFavoritesAdded(favorites);
        }

        // Default favorites exist only in the XML file
        #region Default favorites
        public static FavoriteConfigurationElement GetDefaultFavorite()
        {
            TerminalsConfigurationSection section = GetSection();
            if (section != null && section.DefaultFavorite.Count > 0)
                return section.DefaultFavorite[0];
            return null;
        }

        public static void SaveDefaultFavorite(FavoriteConfigurationElement favorite)
        {
            FavoriteConfigurationElementCollection defaultFav = GetSection().DefaultFavorite;
            defaultFav.Clear();
            defaultFav.Add(favorite);
            SaveImmediatelyIfRequested();
        }

        public static void RemoveDefaultFavorite()
        {
            FavoriteConfigurationElementCollection defaultFav = GetSection().DefaultFavorite;
            defaultFav.Clear();
            SaveImmediatelyIfRequested();
        }
        #endregion

        public static FavoriteConfigurationElementCollection GetFavorites(bool isDatabaseFavorite)
        {
            if (!isDatabaseFavorite)
            {
                TerminalsConfigurationSection section = GetSection();
                if (section != null)
                    return section.Favorites;
            }
            else
            {
                FavoriteConfigurationElementCollection collection = new FavoriteConfigurationElementCollection();

                using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                {
                	if (dc == null)
                		return null;
                	
                    foreach (var connection in dc.Connections)
                    {
                        FavoriteConfigurationElement favorite = connection.ToFavorite();
                        collection.Add(favorite);
                    }
                }

                return collection;
            }

            return null;
        }

        /// <summary>
        ///     Gets all favorites, which contain required tag in not sorted collection.
        ///     If, the tag is empty, than returns "Untagged" favorites.
        /// </summary>
        private static List<FavoriteConfigurationElement> GetFavoritesByTag(String tag, bool isDatabaseFavorite)
        {
        	FavoriteConfigurationElementCollection result = GetFavorites(isDatabaseFavorite);
        	
        	if (result == null)
        		return new List<FavoriteConfigurationElement>();
        	
            if (String.IsNullOrEmpty(tag) || tag == UNTAGGED_NODENAME)
            {
                return result.ToList()
                             .Where(favorite => String.IsNullOrEmpty(favorite.Tags))
                             .ToList();
            }

            return result.ToList()
                         .Where(favorite => favorite.TagList.Contains(tag, StringComparer.CurrentCultureIgnoreCase))
                         .ToList();
        }

        /// <summary>
        ///     Gets all favorites, which contain required tag in collection sorted by default sort property.
        ///     If, the tag is empty, than returns "Untagged" favorites.
        /// </summary>
        public static SortableList<FavoriteConfigurationElement> GetSortedFavoritesByTag(string tag, bool isDatabaseFavorite)
        {
            List<FavoriteConfigurationElement> tagFavorites = GetFavoritesByTag(tag, isDatabaseFavorite);
            return OrderByDefaultSorting(tagFavorites);
        }

        public static SortableList<FavoriteConfigurationElement> OrderByDefaultSorting(List<FavoriteConfigurationElement> source)
        {        	
            IOrderedEnumerable<FavoriteConfigurationElement> sorted;
            switch (DefaultSortProperty)
            {
                case SortProperties.ServerName:
                    sorted = source.OrderBy(favorite => favorite.ServerName)
                                   .ThenBy(favorite => favorite.Name);
                    break;

                case SortProperties.Protocol:
                    sorted = source.OrderBy(favorite => favorite.Protocol)
                                   .ThenBy(favorite => favorite.Name);
                    break;
                case SortProperties.ConnectionName:
                    sorted = source.OrderBy(favorite => favorite.Name);
                    break;
                default:
                    return new SortableList<FavoriteConfigurationElement>(source);
            }

            return new SortableList<FavoriteConfigurationElement>(sorted);
        }

        public static FavoriteConfigurationElement GetOneFavorite(string connectionName, bool isDatabaseFavorite)
        {
            return GetFavorites(isDatabaseFavorite)[connectionName];
        }

        private static void UpdateFavoritePasswordsByNewKeyMaterial(string newKeyMaterial)
        {
            foreach (FavoriteConfigurationElement favorite in GetFavorites(false))
            {
                favorite.UpdatePasswordsByNewKeyMaterial(newKeyMaterial);
            }
        }
    }
}