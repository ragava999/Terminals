using System;
using System.Collections.Generic;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Configuration.Files.Main.Settings
{
    public class ConfigurationChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Use Create methods instead
        /// </summary>
        private ConfigurationChangedEventArgs()
        {
        }

        public List<FavoriteConfigurationElement> OldFavorites { get; private set; }
        public List<FavoriteConfigurationElement> NewFavorites { get; private set; }

        public List<String> OldTags { get; private set; }
        public List<String> NewTags { get; private set; }

        public List<String> OldFavoriteButtons { get; private set; }
        public List<String> NewFavoriteButtons { get; private set; }

        public static ConfigurationChangedEventArgs CreateFromSettings(
            TerminalsConfigurationSection oldSettings,
            TerminalsConfigurationSection newSettings)
        {
            ConfigurationChangedEventArgs args = new ConfigurationChangedEventArgs();

            args.OldFavorites = oldSettings.Favorites.ToList();
            args.NewFavorites = newSettings.Favorites.ToList();
            args.OldTags = oldSettings.Tags.ToList();
            args.NewTags = newSettings.Tags.ToList();
            args.OldFavoriteButtons = oldSettings.FavoritesButtons.ToList();
            args.NewFavoriteButtons = newSettings.FavoritesButtons.ToList();

            return args;
        }

        public static ConfigurationChangedEventArgs CreateFromButtons(
            List<string> oldFavoriteButtons, List<string> newFavoriteButtons)
        {
            ConfigurationChangedEventArgs args = new ConfigurationChangedEventArgs();

            args.OldFavorites = new List<FavoriteConfigurationElement>();
            args.NewFavorites = new List<FavoriteConfigurationElement>();
            args.OldTags = new List<string>();
            args.NewTags = new List<string>();
            args.OldFavoriteButtons = oldFavoriteButtons;
            args.NewFavoriteButtons = newFavoriteButtons;

            return args;
        }
    }
}