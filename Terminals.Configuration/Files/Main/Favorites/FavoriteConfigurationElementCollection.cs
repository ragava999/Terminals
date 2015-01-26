namespace Terminals.Configuration.Files.Main.Favorites
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Kohl.Framework.Lists;

    public class FavoriteConfigurationElementCollection : ConfigurationElementCollection
    {
        public FavoriteConfigurationElementCollection() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public FavoriteConfigurationElement this[int index]
        {
            get { return (FavoriteConfigurationElement) this.BaseGet(index); }
        }

        public new FavoriteConfigurationElement this[string name]
        {
            get { return (FavoriteConfigurationElement) this.BaseGet(name); }
            set
            {
                if (this.BaseGet(name) != null)
                {
                    this.BaseRemove(name);
                }
                this.BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FavoriteConfigurationElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new FavoriteConfigurationElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((FavoriteConfigurationElement) element).Name;
        }

        public void Add(FavoriteConfigurationElement item)
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

        public void Clear()
        {
            this.BaseClear();
        }

        public SortableList<FavoriteConfigurationElement> ToListOrderedByDefaultSorting()
        {
            SortableList<FavoriteConfigurationElement> source = this.ToList();
            return OrderByDefaultSorting(source);
        }

        internal static SortableList<FavoriteConfigurationElement> OrderByDefaultSorting(List<FavoriteConfigurationElement> source)
        {
            IOrderedEnumerable<FavoriteConfigurationElement> sorted;
            switch (Settings.Settings.DefaultSortProperty)
            {
                case SortProperties.ServerName:
                    sorted = source.OrderBy(favorite => favorite.ServerName)
                                   .ThenBy(favorite => favorite.Name);
                    break;

                case SortProperties.Protocol:
                    sorted = source.OrderBy(favorite => favorite.Protocol)
                                   .ThenBy(favorite => favorite.Name);
                    break;
                case SortProperties.ConnectionName:
                    sorted = source.OrderBy(favorite => favorite.Name);
                    break;
                default:
                    return new SortableList<FavoriteConfigurationElement>(source);
            }

            return new SortableList<FavoriteConfigurationElement>(sorted);
        }

        public SortableList<FavoriteConfigurationElement> ToList()
        {
            SortableList<FavoriteConfigurationElement> favorites = new SortableList<FavoriteConfigurationElement>();
            foreach (FavoriteConfigurationElement favorite in this)
            {
                favorites.Add(favorite);
            }

            return favorites;
        }
    }
}