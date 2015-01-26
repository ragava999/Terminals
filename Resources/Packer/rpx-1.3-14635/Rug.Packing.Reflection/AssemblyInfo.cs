using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Packing.Reflection
{
    public class AssemblyInfo : MarshalByRefObject
    {
        #region Private Members
        
        private string m_Title;
        private string m_Description;
        private string m_Configuration;
        private string m_Company;
        private string m_Product;
        private string m_Copyright;
        private string m_Trademark;
        private string m_Culture;
        private string m_Version;
        private string m_FileVersion;
        private bool m_PassArgs;

        #endregion

        #region Properties
        
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        public string Configuration
        {
            get { return m_Configuration; }
            set { m_Configuration = value; }
        }

        public string Company
        {
            get { return m_Company; }
            set { m_Company = value; }
        }

        public string Product
        {
            get { return m_Product; }
            set { m_Product = value; }
        }

        public string Copyright
        {
            get { return m_Copyright; }
            set { m_Copyright = value; }
        }

        public string Trademark
        {
            get { return m_Trademark; }
            set { m_Trademark = value; }
        }

        public string Culture
        {
            get { return m_Culture; }
            set { m_Culture = value; }
        }

        public string Version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }

        public string FileVersion
        {
            get { return m_FileVersion; }
            set { m_FileVersion = value; }
        }

        public bool PassArgs
        {
            get { return m_PassArgs; }
            set { m_PassArgs = value; }
        }

        #endregion

        #region ICloneable Members

        public static AssemblyInfo Clone(AssemblyInfo other)
        {
            AssemblyInfo info = new AssemblyInfo();

            info.Title = other.Title;
            info.Description = other.Description;
            info.Configuration = other.Configuration;
            info.Company = other.Company;
            info.Product = other.Product;
            info.Copyright = other.Copyright;
            info.Trademark = other.Trademark;
            info.Culture = other.Culture;
            info.Version = other.Version;
            info.FileVersion = other.FileVersion;
            info.PassArgs = other.PassArgs;

            return info;
        }

        #endregion
    }

}
