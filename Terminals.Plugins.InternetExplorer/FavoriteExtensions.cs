namespace Terminals.Plugins.InternetExplorer
{
    using Configuration.Files.Main.Favorites;

    static class FavoriteExtensions
    {
        public static string InternetExplorerUrl(this FavoriteConfigurationElement favorite, string text = null)
        {
            if (string.IsNullOrEmpty(text))
                return favorite.GetPluginValue<string>("InternetExplorerUrl");

            favorite.SetPluginValue("InternetExplorerUrl", text);
            return null;
        }
    }
}
