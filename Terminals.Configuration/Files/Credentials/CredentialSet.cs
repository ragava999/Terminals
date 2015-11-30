using System;
using System.Xml.Serialization;
using Terminals.Configuration.Security;

namespace Terminals.Configuration.Files.Credentials
{
    /// <summary>
    ///     Container of stored user authentication.
    /// </summary>
    [Serializable]
    public class CredentialSet
    {
        private string name;

        private string userName;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                this.name = value;
            }
        }

        public string Username
        {
            get { return this.userName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                this.userName = value;
            }
        }

        public string Domain { get; set; }

        /// <summary>
        ///     Gets or sets the encrypted password hash.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the password in not encrypted form.
        /// </summary>
        [XmlIgnore]
        public string SecretKey
        {
            get
            {
            	if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
            		return this.Password;
            		
                if (!string.IsNullOrEmpty(this.Password))
                    return PasswordFunctions.DecryptPassword(this.Password, this.Name);

                return String.Empty;
            }
            set
            {
            	if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
            		this.Password = value;
            	else
	                if (string.IsNullOrEmpty(value))
	                    this.Password = String.Empty;
	                else
	                    this.Password = PasswordFunctions.EncryptPassword(value);
            }
        }

        public void UpdatePasswordByNewKeyMaterial(string newKeymaterial)
        {
            if (!string.IsNullOrEmpty(this.SecretKey))
                this.Password = PasswordFunctions.EncryptPassword(this.SecretKey, newKeymaterial);
        }

        public override string ToString()
        {
            return String.Format(@"{0}:{1}\{2}", this.Name, this.Domain, this.Username);
        }
    }
}