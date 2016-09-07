using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kohl.Framework.Info;
using Terminals.Configuration.Files.Credentials;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Configuration.Files.Main.Settings
{
    /// <summary>
    ///     Provides unified creation of Favorites
    /// </summary>
    public static class FavoritesFactory
    {
        private const string DISCOVERED_CONNECTIONS = "Discovered Connections";

        public const String TerminalsReleasesFavoriteName = "TerminalsNews";

        public static FavoriteConfigurationElement GetOrCreateReleaseFavorite()
        {
            List<FavoriteConfigurationElement> favorites = Settings.GetFavorites(false).ToList();
            FavoriteConfigurationElement release = favorites
                .FirstOrDefault(candidate => candidate.Name == TerminalsReleasesFavoriteName);

            if (release == null)
            {
                release = new FavoriteConfigurationElement(TerminalsReleasesFavoriteName);
                release.Url = Url.GitHubLatestRelease_Link;
                release.Tags = "Terminals";
                release.Protocol = "HTTP";
                Settings.AddFavorite(release);
            }
            return release;
        }

        public static FavoriteConfigurationElement GetOrCreateQuickConnectFavorite(String server,
                                                                                   Boolean ConnectToConsole, Int32 port,
                                                                                   string protocol, string url, bool isDatabaseFavorite)
        {
            FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(isDatabaseFavorite);
            FavoriteConfigurationElement favorite = favorites[server];
            if (favorite != null)
            {
                favorite.ConnectToConsole = ConnectToConsole;
            }
            else //create a temporaty favorite and connect to it
            {
                favorite = new FavoriteConfigurationElement();
                favorite.ConnectToConsole = ConnectToConsole;
                favorite.ServerName = server;
                favorite.Name = server;
                favorite.Protocol = protocol;

                if (!string.IsNullOrEmpty(url))
                    favorite.Url = url;

                if (port != 0)
                    favorite.Port = port;
            }
            return favorite;
        }

        public static FavoriteConfigurationElement GetFavoriteUpdatedCopy(String connectionName,
                                                                          Boolean forceConsole, Boolean forceNewWindow,
                                                                          CredentialSet credential, bool isDatabaseFavorite)
        {
            FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(isDatabaseFavorite);
            FavoriteConfigurationElement favorite = favorites[connectionName];
            if (favorite == null)
                return null;

            favorite = (FavoriteConfigurationElement) favorite.Clone();

            if (forceConsole)
                favorite.ConnectToConsole = true;

            if (forceNewWindow)
                favorite.NewWindow = true;

            if (credential != null)
            {
                favorite.XmlCredentialSetName = credential.Name;
                favorite.UserName = credential.Username;
                favorite.DomainName = credential.Domain;
                favorite.EncryptedPassword = credential.Password;
            }
            return favorite;
        }

        public static FavoriteConfigurationElement CreateNewFavorite(string favoriteName, string server, string protocol,
                                                                     string domain, string userName, int port)
        {
            FavoriteConfigurationElement newFavorite = new FavoriteConfigurationElement();
            newFavorite.Name = favoriteName;
            newFavorite.ServerName = server;
            newFavorite.UserName = userName;
            newFavorite.DomainName = domain;
            newFavorite.Tags = DISCOVERED_CONNECTIONS;
            newFavorite.Port = port;
            newFavorite.Protocol = protocol;
            return newFavorite;
        }

        public static FavoriteConfigurationElement CreateNewFavorite(string favoriteName, string server, string protocol,
                                                                     int port)
        {
            string name = GetHostName(server, favoriteName);
            string domainName = GetCurrentDomainName(server);
            return CreateNewFavorite(name, server, protocol, domainName, Environment.UserName, port);
        }

        private static string GetCurrentDomainName(string server)
        {
            if (Environment.UserDomainName != Environment.MachineName)
                return Environment.UserDomainName;

            return server;
        }

        private static string GetHostName(string server, string name)
        {
            try
            {
                IPAddress address;
                if (IPAddress.TryParse(server, out address))
                    name = Dns.GetHostEntry(address).HostName;

                return name;
            }
            catch //lets not log dns lookups!
            {
                return name;
            }
        }
    }
}