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
    public class vRDConfigurationFileConnectionsFolder
    {
        private Connection[] connectionField;
        private string credentialsField;
        private string descriptionField;

        private vRDConfigurationFileConnectionsFolderFolder[] folderField;

        private string nameField;

        private string sortedField;

        /// <remarks />
        [XmlElement("Connection")]
        public Connection[] Connection
        {
            get { return this.connectionField; }
            set { this.connectionField = value; }
        }

        /// <remarks />
        [XmlElement("Folder", Form = XmlSchemaForm.Unqualified)]
        public vRDConfigurationFileConnectionsFolderFolder[] Folder
        {
            get { return this.folderField; }
            set { this.folderField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Credentials
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