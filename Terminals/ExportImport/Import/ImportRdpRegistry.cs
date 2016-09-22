using Kohl.Framework.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Manager;
using Terminals.Connections;

namespace Terminals.ExportImport.Import
{
    /// <summary>
    ///     Loads favorites from windows registry used by the native Windows remote desktop client
    /// </summary>
    public static class ImportRdpRegistry
    {
        /// <summary>
        ///     Gets the last recently used connection registry path, relative to the HKCU
        ///     HKEY_CURRENT_USER\Software\Microsoft\Terminal Server Client\Servers
        /// </summary>
        private const string REGISTRY_KEY = @"Software\Microsoft\Terminal Server Client\Servers";

        /// <summary>
        ///     Reads favorites from the registry. Reads serverName, domain and user name.
        /// </summary>
        /// <returns> Not null collection of favorites. Empty collection by exception. </returns>
        public static List<FavoriteConfigurationElement> Import()
        {
            try
            {
                RegistryKey favoritesKey = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY);

                if (favoritesKey != null)
                    return ImportFavoritesFromSubKeys(favoritesKey);
            }
            catch (Exception e)
            {
                Log.Error("The RDP import from the registry failed.", e);
            }

            return new List<FavoriteConfigurationElement>();
        }

        private static List<FavoriteConfigurationElement> ImportFavoritesFromSubKeys(RegistryKey favoritesKey)
        {
            return (from favoriteKeyName in favoritesKey.GetSubKeyNames() let favoriteKey = favoritesKey.OpenSubKey(favoriteKeyName) where favoriteKey != null select ImportRdpKey(favoriteKey, favoriteKeyName)).ToList();
        }

        private static FavoriteConfigurationElement ImportRdpKey(RegistryKey favoriteKey, string favoriteName)
        {
            string userKey = favoriteKey.GetValue("UsernameHint").ToString();
            int slashIndex = userKey.LastIndexOf('\\');
            string domainName = userKey.Substring(0, slashIndex);
            string userName = userKey.Substring(slashIndex + 1, userKey.Length - slashIndex - 1);
            return FavoritesFactory.CreateNewFavorite(favoriteName, favoriteName, typeof(RDPConnection).GetProtocolName(), domainName, userName, ConnectionManager.GetPort(typeof(RDPConnection).GetProtocolName()));
        }
    }
}