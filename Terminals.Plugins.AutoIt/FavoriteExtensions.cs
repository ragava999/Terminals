namespace Terminals.Plugins.AutoIt
{
    using Configuration.Files.Main.Favorites;

    static class FavoriteExtensions
    {
        public static string AutoItScript(this FavoriteConfigurationElement favorite, string text = null)
        {
            if (string.IsNullOrEmpty(text))
                return favorite.GetPluginValue<string>("AUTOIT_Script");

            favorite.SetPluginValue("AUTOIT_Script", text);
            return null;
        }
    }
}
