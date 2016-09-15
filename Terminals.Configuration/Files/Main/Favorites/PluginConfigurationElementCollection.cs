namespace Terminals.Configuration.Files.Main.Favorites
{
    using System;
	using System.Linq;

    /// <summary>
    /// A collection of PluginConfiguration instances.
    /// </summary>
    [Serializable]
	[global::System.Configuration.ConfigurationCollection(typeof(global::System.Configuration.ConfigurationElementCollection))]
    public class PluginConfigurationElementCollection : global::System.Configuration.ConfigurationElementCollection
    {
        #region Constructor (1)
        public PluginConfigurationElementCollection() : base(StringComparer.CurrentCultureIgnoreCase)
        {
            
        }
        #endregion

        #region Constants (1)
        /// <summary>
        /// The XML name of the individual <see cref="PluginConfiguration"/> instances in this collection.
        /// </summary>
        internal const string PluginConfigurationPropertyName = "plugin";
		/// <summary>
        /// The XML name of the collection <see cref="PluginConfigurationElementCollection"/> itself.
        /// </summary>
		internal readonly string PluginConfigurationCollectionPropertyName = "pluginOptions";
        #endregion

        #region Overrides (5)
        /// <summary>
        /// Gets the type of the <see cref="global::System.Configuration.PluginConfigurationElementCollection"/>.
        /// </summary>
        /// <returns>The <see cref="global::System.Configuration.PluginConfigurationElementCollectionType"/> of this collection.</returns>
        public override global::System.Configuration.ConfigurationElementCollectionType CollectionType
        {
            get
            {
				return global::System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap; // BasicMapAlternate;
            }
        }

        /// <summary>
        /// Gets the name used to identify this collection of elements
        /// </summary>
        protected override string ElementName
        {
            get
            {
				return PluginConfigurationElementCollection.PluginConfigurationPropertyName;
            }
        }

        /// <summary>
        /// Indicates whether the specified <see cref="Configuration.ConfigurationElement"/> exists in the <see cref="global::System.Configuration.PluginConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="elementName">The name of the element to verify.</param>
        /// <returns>
        /// <see langword="true"/> if the element exists in the collection; otherwise, <see langword="false"/>.
        /// </returns>
        protected override bool IsElementName(string elementName)
        {
            return (elementName == PluginConfigurationElementCollection.PluginConfigurationPropertyName);
        }
        
        /// <summary>
        /// Gets the element key for the specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="global::System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="global::System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(global::System.Configuration.ConfigurationElement element)
        {
            return ((PluginConfiguration)(element)).Name;
        }

        /// <summary>
        /// Creates a new <see cref="PluginConfiguration"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="PluginConfiguration"/>.
        /// </returns>
        protected override global::System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new PluginConfiguration();
        }
        #endregion

        #region Indexer (2)
        /// <summary>
        /// Gets the <see cref="PluginConfiguration"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="PluginConfiguration"/> to retrieve.</param>
        public PluginConfiguration this[int index]
        {
            get
            {
                return ((PluginConfiguration)(base.BaseGet(index)));
            }
        }

        /// <summary>
        /// Gets the <see cref="PluginConfiguration"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="PluginConfiguration"/> to retrieve.</param>
        public PluginConfiguration this[object name]
        {
            get
            {
                return ((PluginConfiguration)(base.BaseGet(name)));
            }
        }
        #endregion

        #region Add (1)
        /// <summary>
        /// Adds the specified <see cref="PluginConfiguration"/> to the <see cref="global::System.Configuration.PluginConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="pluginConfiguration">The <see cref="PluginConfiguration"/> to add.</param>
        public void Add(PluginConfiguration pluginConfiguration)
        {
            base.BaseAdd(pluginConfiguration);
        }
        #endregion

        #region Remove (1)
        /// <summary>
        /// Removes the specified <see cref="PluginConfiguration"/> from the <see cref="global::System.Configuration.PluginConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="pluginConfiguration">The <see cref="PluginConfiguration"/> to remove.</param>
        public void Remove(PluginConfiguration pluginConfiguration)
        {
            base.BaseRemove(this.GetElementKey(pluginConfiguration));
        }
        #endregion

        #region GetItem (2)
        /// <summary>
        /// Gets the <see cref="PluginConfiguration"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="PluginConfiguration"/> to retrieve.</param>
        public PluginConfiguration GetItemAt(int index)
        {
            return ((PluginConfiguration)(base.BaseGet(index)));
        }

        /// <summary>
        /// Gets the <see cref="PluginConfiguration"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="PluginConfiguration"/> to retrieve.</param>
        public PluginConfiguration GetItemByKey(float name)
        {
            return ((PluginConfiguration)(base.BaseGet(((object)(name)))));
        }
        #endregion

        #region IsReadOnly override (1)
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion

		protected override object OnRequiredPropertyNotFound(string name)
		{
			return base.OnRequiredPropertyNotFound(name);
		}

        #region DeserializeElement (1)
		/// <summary>
		/// Default deserzialization perfectly works on .NET and mono for Windows, but not on any mono platforms other than windows. To prevent the mono framework on non Windows platforms to throw errors kick into the deserialization and override the default one.
		/// </summary>
		/// <param name="reader">Reader.</param>
		/// <param name="serializeCollectionKey">If set to <c>true</c> serialize collection key.</param>
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
		{
			if (reader.Name.ToLowerInvariant() == this.PluginConfigurationCollectionPropertyName.ToLowerInvariant())
			{
				Kohl.Framework.Logging.Log.Info("Reading plugin options.");

				while (reader.Read())
				{
					if (reader.Name.ToLowerInvariant() == PluginConfigurationPropertyName.ToLowerInvariant())
					{
						if (!this.BaseGetAllKeys().Contains(reader[0].ToString()))
						{
							PluginConfiguration config = new PluginConfiguration(reader[0].ToString());

							if (reader.AttributeCount == 3)
								config.SetValue(reader[1].ToString(), reader[2].ToString());
							else if (reader.AttributeCount == 2)
								config.SetValue(reader[1].ToString());

							this.Add(config);
						}
					}
					if (reader.Name.ToLowerInvariant() == this.PluginConfigurationCollectionPropertyName.ToLowerInvariant())
						return;
				}
			}
		}
        #endregion
    }
}