using System.Configuration;

namespace Terminals.Configuration.Files.Main.Favorites
{
    public class FavoriteAliasConfigurationElement : ConfigurationElement
    {
        public FavoriteAliasConfigurationElement()
        {
        }

        public FavoriteAliasConfigurationElement(string name)
        {
            this.Name = name;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            private set { this["name"] = value; }
        }
    }
}