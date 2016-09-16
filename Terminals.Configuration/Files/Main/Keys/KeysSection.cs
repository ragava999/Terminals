using System;
using System.Configuration;

namespace Terminals.Configuration.Files.Main.Keys
{
    /// <summary>
    ///     SSHKeysSection is a special configuration section for SSH keys.
    /// </summary>
    [Serializable]
    public class KeysSection : ConfigurationSection
    {
        // Declare a collection element represented
        // in the configuration file by the sub-section
        // <keys> <add .../> </keys> 
        // Note: the "IsDefaultCollection = false" 
        // instructs the .NET Framework to build a nested 
        // section like <keys> ...</keys>.
        [ConfigurationProperty("keys", IsDefaultCollection = false)]
        public KeysCollection Keys
        {
            get { return (KeysCollection) base["keys"]; }
        }

        public void AddKey(string name, string key)
        {
            this.Keys.Add(new KeyConfigElement(name, key));
        }
    }
}