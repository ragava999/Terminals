namespace Terminals.Plugins.AutoIt
{
    using MainSettings = Configuration.Files.Main.Settings;

    static class Settings
    {
        public static string AutoItProgramPath(string text = null, string defaultValue = null)
        {
            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(defaultValue))
                return MainSettings.Settings.GetPluginOption<string>("AUTOIT_ProgramPath");

            MainSettings.Settings.SetPluginOption("AUTOIT_ProgramPath", text, defaultValue);
            return null;
        }
    }
}