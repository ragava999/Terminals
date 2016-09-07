namespace Terminals.Configuration.Files.Main.Favorites
{
    using System;
    using System.ComponentModel;
    using System.Configuration;

    /// <summary>
    /// The PluginConfiguration Configuration Element.
    /// </summary>
    [Serializable]
    public class PluginConfiguration : ConfigurationElement
    {
        #region Constructor (2)
        public PluginConfiguration()
        {
        }
        
        public PluginConfiguration(string name)
        {
            this.Name = name;
        }
        #endregion

        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [global::System.ComponentModel.DescriptionAttribute("The Name.")]
        [ConfigurationProperty("name", IsRequired = true, IsKey = true, IsDefaultCollection = true)]
        public virtual string Name
        {
            get
            {
                return ((string)(base["name"]));
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("value", IsRequired = false)]
        private string value
        {
            get
            {
                return ((base["value"].ToString()));
            }
            set
            {
                base["value"] = value;
            }
        }

        [ConfigurationProperty("defaultValue", IsRequired = false)]
        private string defaultValue
        {
            get
            {
                return ((base["defaultValue"].ToString()));
            }
            set
            {
                base["defaultValue"] = value;
            }
        }

        public void SetValue<T>(T value, T defaultValue = default(T))
        {
            if (defaultValue != null)
                this.defaultValue = defaultValue.ToString();
        	
            // Only set the value if it differs from the default value
            if (value != null && !System.Collections.Generic.EqualityComparer<T>.Default.Equals(value, defaultValue))
                this.value = value.ToString();
        }

        public T GetValue<T>()
        {
            if (value == null || (value is string && (string)value == string.Empty))
                return Parse<T>(defaultValue);

            return Parse<T>(value);
        }

        public static T Parse<T>(string value)
        {
            // or ConvertFromInvariantString if you are doing serialization
            //return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
        }
    }
}