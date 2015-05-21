namespace Terminals.Plugins.Text
{
    using Configuration.Files.Main.Favorites;

    static class FavoriteExtensions
    {
        public static string TinyMceText(this FavoriteConfigurationElement favorite, string text = null)
        {
            if (string.IsNullOrEmpty(text))
                return favorite.GetPluginValue<string>("TEXT_Html");

            favorite.SetPluginValue("TEXT_Html", text);
            return null;
        }

        public static bool ShowTinyMceInConnectionMode(this FavoriteConfigurationElement favorite, bool? value = null)
        {
            if (!value.HasValue)
                return favorite.GetPluginValue<bool>("TEXT_ShowTinyMceInConnectionMode");

            favorite.SetPluginValue("TEXT_ShowTinyMceInConnectionMode", value.Value);
            return true;
        }

        public static bool ShowTinyMceInEditMode(this FavoriteConfigurationElement favorite, bool? value = null)
        {
            if (!value.HasValue)
                return favorite.GetPluginValue<bool>("TEXT_ShowTinyMceInEditMode");

            favorite.SetPluginValue("TEXT_ShowTinyMceInEditMode", value.Value);
            return true;
        }
    }
}
