using System;
using System.Collections.Generic;
using System.Linq;
using Kohl.Framework.Lists;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Configuration.Files.Main.Settings
{
	using Sql;
	
    public partial class Settings
    {
        /// <summary>
        ///     Gets alphabeticaly sorted array of tags resolved from Tags store
        /// </summary>
        public static string[] Tags (bool isDatabaseFavorite)
        {
            if (!isDatabaseFavorite)
            {
                List<string> tags = GetSection().Tags.ToList();
                tags.Sort();
                return tags.ToArray();
            }

            string[] groups;

            using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
            {
                groups =
                (from g in dc.Groups
                 orderby g.Name
                select g.Name).ToArray<string>();
            }

            return groups;
        }

        public static void AddTags(List<String> tags, bool isDatabaseFavorite)
        {
            List<String> addedTags = AddTagsToSettings(tags, isDatabaseFavorite);
            DataDispatcher.Instance.ReportTagsAdded(addedTags);
        }

        private static List<string> AddTagsToSettings(List<String> tags, bool isDatabaseFavorite)
        {
            if (isDatabaseFavorite)
                return tags;

            List<String> addedTags = new List<string>();
            foreach (String tag in tags)
            {
                if (String.IsNullOrEmpty(tag))
                    continue;

                String addedTag = AddTagToSettings(tag, isDatabaseFavorite);
                if (!String.IsNullOrEmpty(addedTag))
                    addedTags.Add(addedTag);
            }
            return addedTags;
        }

        /// <summary>
        ///     Adds tag to the tags collection if it already isnt there.
        ///     If the tag is in database, than it returns empty string, otherwise the commited tag.
        /// </summary>
        private static String AddTagToSettings(String tag, bool isDatabaseFavorite)
        {
            if (AutoCaseTags)
                tag = ToTitleCase(tag);
            if (Tags(isDatabaseFavorite).Contains(tag))
                return String.Empty;

            if (!isDatabaseFavorite)
            {
                GetSection().Tags.AddByName(tag);
                SaveImmediatelyIfRequested();
            }
            else
            {
                using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                {
                    dc.Groups.AddObject(new Sql.Groups() { Name = tag });
                    dc.SaveChanges();
                }
            }

            return tag;
        }

        public static void DeleteTags(List<String> tagsToDelete, bool isDatabaseFavorite)
        {
            List<String> deletedTags = DeleteTagsFromSettings(tagsToDelete, isDatabaseFavorite);
            DataDispatcher.Instance.ReportTagsDeleted(deletedTags);
        }

        private static List<String> DeleteTagsFromSettings(List<String> tagsToDelete, bool isDatabaseFavorite)
        {
            List<String> deletedTags = new List<String>();
            foreach (String tagTodelete in tagsToDelete)
            {
                if (GetNumberOfFavoritesUsingTag(tagTodelete, isDatabaseFavorite) > 0)
                    continue;

                deletedTags.Add(DeleteTagFromSettings(tagTodelete, isDatabaseFavorite));
            }
            return deletedTags;
        }

        /// <summary>
        ///     Removes the tag from settings, but doesnt send the Tag removed event
        /// </summary>
        private static String DeleteTagFromSettings(String tagToDelete, bool isDatabaseFavorite)
        {
            if (AutoCaseTags)
                tagToDelete = ToTitleCase(tagToDelete);

            if (!isDatabaseFavorite)
            {
                GetSection().Tags.DeleteByName(tagToDelete);
                SaveImmediatelyIfRequested();
            }
            else
            {
                using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                {
                    Sql.Groups group = dc.Groups.Where(x => x.Name == tagToDelete).FirstOrDefault();

                    dc.Groups.DeleteObject(group);
                    dc.SaveChanges();
                }
            }

            return tagToDelete;
        }

        private static Int32 GetNumberOfFavoritesUsingTag(String tagToRemove, bool isDatabaseFavorite)
        {
            SortableList<FavoriteConfigurationElement> favorites = GetFavorites(isDatabaseFavorite).ToList();
            return favorites.Count(favorite => favorite.TagList.Contains(tagToRemove));
        }

        public static void RebuildTagIndex(bool isDatabaseFavorite)
        {
            String[] oldTags = Tags(isDatabaseFavorite);
            ClearTags(isDatabaseFavorite);
            ReCreateAllTags(isDatabaseFavorite);
            DataDispatcher.Instance.ReportTagsRecreated(Tags(isDatabaseFavorite).ToList(), oldTags.ToList());
        }

        private static void ReCreateAllTags(bool isDatabaseFavorite)
        {
            FavoriteConfigurationElementCollection favorites = GetFavorites(isDatabaseFavorite);
            foreach (FavoriteConfigurationElement favorite in favorites)
            {
                // dont report update here
                AddTagsToSettings(favorite.TagList, favorite.IsDatabaseFavorite);
            }
        }

        /// <summary>
        ///     Clears all tags from database, but doesnt sed the tags changed event
        /// </summary>
        private static void ClearTags(bool isDatabaseFavorite)
        {
            foreach (String tag in Tags(isDatabaseFavorite))
            {
                DeleteTagFromSettings(tag, isDatabaseFavorite);
            }
        }
    }
}