using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Terminals.ExportImport.Import
{
    /// <remarks />
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class vRDConfigurationFileCredentialsFolder
    {
        private vRDConfigurationFileCredentialsFolderCredentials[] credentialsField;

        private string sortedField;

        /// <remarks />
        [XmlElement("Credentials", Form = XmlSchemaForm.Unqualified)]
        public vRDConfigurationFileCredentialsFolderCredentials[] Credentials
        {
            get { return this.credentialsField; }
            set { this.credentialsField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Sorted
        {
            get { return this.sortedField; }
            set { this.sortedField = value; }
        }
    }
}