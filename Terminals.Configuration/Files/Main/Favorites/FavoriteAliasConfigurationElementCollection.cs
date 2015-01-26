namespace Terminals.Configuration.Files.Main.Favorites
{
    using System;
    using System.Configuration;

    public class FavoriteAliasConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FavoriteAliasConfigurationElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new FavoriteAliasConfigurationElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((FavoriteAliasConfigurationElement) element).Name;
        }

        public void Add(FavoriteAliasConfigurationElement item)
        {
            this.BaseAdd(item);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            this.BaseAdd(element, false);
        }
        
        public void Remove(string name)
        {
            this.BaseRemove(name);
        }
    }
}