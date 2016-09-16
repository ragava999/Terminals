namespace Terminals.Configuration.Files.Main.Keys
{
    using System;
    using System.Configuration;

    [Serializable]
    public class KeyConfigElement : ConfigurationElement
    {
        // Constructor allowing name, and key to be specified.
        public KeyConfigElement(String newName, String newKey)
        {
            this.Name = newName;
            this.Key = newKey;
        }

        // Default constructor, will use default values as defined
        // below.
        public KeyConfigElement()
        {
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            private set { this["name"] = value; }
        }

        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get { return (string) this["key"]; }
            private set { this["key"] = value; }
        }
    }
}