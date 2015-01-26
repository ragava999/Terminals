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
    public class vRDConfigurationFileCredentialsFolderCredentials
    {
        private string descriptionField;

        private string domainField;

        private string guidField;
        private string nameField;
        private string passwordField;
        private string passwordPromptField;
        private string userNameField;

        private string versionField;

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string UserName
        {
            get { return this.userNameField; }
            set { this.userNameField = value; }
        }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Password
        {
            get { return this.passwordField; }
            set { this.passwordField = value; }
        }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Domain
        {
            get { return this.domainField; }
            set { this.domainField = value; }
        }

        /// <remarks />
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string PasswordPrompt
        {
            get { return this.passwordPromptField; }
            set { this.passwordPromptField = value; }
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
        public string Guid
        {
            get { return this.guidField; }
            set { this.guidField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public string Version
        {
            get { return this.versionField; }
            set { this.versionField = value; }
        }
    }
}