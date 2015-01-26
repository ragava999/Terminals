namespace Terminals.Configuration.Files.Main.Groups
{
    using System;
    using System.Configuration;

    public class GroupConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public new GroupConfigurationElement this[string Name]
        {
            get { return (GroupConfigurationElement) this.BaseGet(Name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new GroupConfigurationElement();
        }
        
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new GroupConfigurationElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((GroupConfigurationElement) element).Name;
        }

        public void Add(GroupConfigurationElement item)
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