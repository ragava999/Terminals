namespace Terminals.Configuration.Files.Main.MRU
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Most-recently-used (MRU) lists.
    /// </summary>
    [Serializable]
    public class MRUItemConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        private new MRUItemConfigurationElement this[string name]
        {
            get { return (MRUItemConfigurationElement) this.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MRUItemConfigurationElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new MRUItemConfigurationElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((MRUItemConfigurationElement) element).Name;
        }

        private MRUItemConfigurationElement ItemByName(string name)
        {
            return (MRUItemConfigurationElement) this.BaseGet(name);
        }

        private void Add(MRUItemConfigurationElement item)
        {
            this.BaseAdd(item);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            this.BaseAdd(element, false);
        }

        private void Remove(string name)
        {
            this.BaseRemove(name);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public List<string> ToList()
        {
            return this.Cast<MRUItemConfigurationElement>()
                       .Select(configurationElement => configurationElement.Name)
                       .ToList();
        }

        public string[] ToSortedArray()
        {
            List<string> domainNames = this.ToList();
            domainNames.Sort();
            return domainNames.ToArray();
        }

        public void AddByName(string name)
        {
            MRUItemConfigurationElement configurationElement = this.ItemByName(name);
            if (configurationElement == null)
            {
                this.Add(new MRUItemConfigurationElement(name));
            }
        }

        public void DeleteByName(string name)
        {
            MRUItemConfigurationElement configurationElement = this.ItemByName(name);
            if (configurationElement != null)
            {
                this.Remove(name);
            }
        }

        public void EditByName(string oldName, string newName)
        {
            MRUItemConfigurationElement configurationElement = this.ItemByName(oldName);
            if (configurationElement != null)
            {
                this[oldName].Name = newName;
            }
        }
    }
}