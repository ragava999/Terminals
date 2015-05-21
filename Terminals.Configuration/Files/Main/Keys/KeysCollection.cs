namespace Terminals.Configuration.Files.Main.Keys
{
    using System;
    using System.Configuration;

    [ConfigurationCollection(typeof (KeyConfigElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class KeysCollection : ConfigurationElementCollection
    {
        public new KeyConfigElement this[string Name]
        {
            get { return (KeyConfigElement) this.BaseGet(Name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyConfigElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return (element as KeyConfigElement).Name;
        }

        public void Add(KeyConfigElement key)
        {
            this.BaseAdd(key, false);
        }
    }
}