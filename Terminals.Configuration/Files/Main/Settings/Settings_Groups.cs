using Terminals.Configuration.Files.Main.Groups;

namespace Terminals.Configuration.Files.Main.Settings
{
    public static partial class Settings
    {
        public static GroupConfigurationElementCollection GetGroups()
        {
            return GetSection().Groups;
        }

        public static void DeleteGroup(string name)
        {
            GetSection().Groups.Remove(name);
            SaveImmediatelyIfRequested();
        }

        public static void AddGroup(GroupConfigurationElement group)
        {
            GetSection().Groups.Add(group);
            SaveImmediatelyIfRequested();
        }
    }
}