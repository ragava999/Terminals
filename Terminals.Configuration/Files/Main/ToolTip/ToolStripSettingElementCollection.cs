using System;
using System.Configuration;

namespace Terminals.Configuration.Files.Main.ToolTip
{
    public class ToolStripSettingElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public new string AddElementName
        {
            get { return base.AddElementName; }
            set { base.AddElementName = value; }
        }

        public new string ClearElementName
        {
            get { return base.ClearElementName; }
            set { base.AddElementName = value; }
        }

        public new string RemoveElementName
        {
            get { return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }

        public ToolStripSettingElement this[int index]
        {
            get { return (ToolStripSettingElement) this.BaseGet(index); }
            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new ToolStripSettingElement this[string Name]
        {
            get { return (ToolStripSettingElement) this.BaseGet(Name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ToolStripSettingElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new ToolStripSettingElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ToolStripSettingElement) element).Name;
        }

        public int IndexOf(ToolStripSettingElement item)
        {
            return this.BaseIndexOf(item);
        }

        public ToolStripSettingElement ItemByName(string name)
        {
            return (ToolStripSettingElement) this.BaseGet(name);
        }

        public void Add(ToolStripSettingElement item)
        {
            this.BaseAdd(item);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            this.BaseAdd(element, false);
        }

        public void Remove(ToolStripSettingElement item)
        {
            if (this.BaseIndexOf(item) >= 0)
                this.BaseRemove(item.Name);
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            this.BaseRemove(name);
        }

        public void Clear()
        {
            this.BaseClear();
        }
    }
}