namespace Terminals.Network.DNS
{
    using System;
    using System.Management;
    using Kohl.Framework.Logging;

    public class Adapter
    {
        public ManagementObject PropertyData { private get; set; }

        public String[] DNSServerSearchOrder
        {
            get { return this.ToStringArray("DNSServerSearchOrder"); }
        }

        public Boolean IPEnabled
        {
            get { return this.ToBoolean("IPEnabled"); }
        }

        private Boolean ToBoolean(String property)
        {
            try
            {
                object value = this.PropertyData.Properties[property].Value;
                return (value == null) ? false : Convert.ToBoolean(value);
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return false;
            }
        }

        private String[] ToStringArray(String property)
        {
            try
            {
                object value = this.PropertyData.Properties[property].Value;
                return (value == null) ? new String[] {} : (String[]) value;
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return new String[] {};
            }
        }
    }
}