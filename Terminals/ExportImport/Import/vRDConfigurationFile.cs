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
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class vRDConfigurationFile
    {
        private object[] itemsField;

        /// <remarks />
        [XmlElement("Connection", typeof (Connection))]
        [XmlElement("ConnectionsFolder", typeof (vRDConfigurationFileConnectionsFolder),
            Form = XmlSchemaForm.Unqualified)]
        [XmlElement("CredentialsFolder", typeof (vRDConfigurationFileCredentialsFolder),
            Form = XmlSchemaForm.Unqualified)]
        public object[] Items
        {
            get { return this.itemsField; }
            set { this.itemsField = value; }
        }
    }
}