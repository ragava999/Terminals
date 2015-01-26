namespace Terminals.Configuration.Files.Credentials
{
    using Main.Favorites;
    using Main.Settings;

    public class Credential : Kohl.Framework.Security.Credential
    {
        public bool IsSetEncryptedPassword { get; set; }


        /// <summary>
        /// Returns the tab page caption plus the user name.
        /// </summary>
        /// <remarks>
        /// If the domain name is empty than only the user name will be appended to the tab page caption.
        /// If the domain name and the user name are set than the FQ user name will be appended.
        /// If the user name hasn't been set -> return only the caption.
        /// </remarks>
        /// <param name="title"></param>
        /// <returns>Returns the tab page caption plus the user name.</returns>
        public string AppendTabPageTitleWithUserName(string title)
        {
            AppendTabPageTitleWithUserName(ref title);

            return title;
        }

        /// <summary>
        /// Returns the tab page caption plus the user name.
        /// </summary>
        /// <remarks>
        /// If the domain name is empty than only the user name will be appended to the tab page caption.
        /// If the domain name and the user name are set than the FQ user name will be appended.
        /// If the user name hasn't been set -> return only the caption.
        /// </remarks>
        /// <param name="title">Returns the tab page caption plus the user name.</param>
        public void AppendTabPageTitleWithUserName(ref string title)
        {
            // If the user name has been set
            // then return either the user name or the user name plus domain name (depending on the existance of the domain name)
            // If the user name hasn't been set don't check for the domain (the domain is worthless without the user name)
            // ... so return NULL. -> just use the favorite's name as the tab page title.
            title += (this.IsSetUserName ? " (" + this.UserNameWithDomain + ")" : null);
        }
        
        /// <summary>
        /// Returns user name, password and domain for the current favorite in the form of an object.
        /// </summary>
        /// <remarks>If neither a credential has been specified in the connection manual or
        /// no credential set from the XML file (credentials.xml) has been choosen, the defaults
        /// specified in the settings will be returned i.e. the default password, user name and domain.
        /// If the user has choosen not to use any of the options:
        ///    * credential set
        ///    * manual credential entry per favorite
        ///    * default credentials in settings
        /// <c>NULL</c> values will be returned for each property in the credential object.
        /// </remarks>
        public static Credential GetCredentials(FavoriteConfigurationElement favorite)
        {
            Credential credential = new Credential
            {
                DomainName = string.IsNullOrEmpty(favorite.DomainName) ? string.IsNullOrEmpty(Settings.DefaultDomain) ? null : Settings.DefaultDomain : favorite.DomainName,
                UserName = string.IsNullOrEmpty(favorite.UserName) ? string.IsNullOrEmpty(Settings.DefaultUsername) ? null : Settings.DefaultUsername : favorite.UserName,
                Password = string.IsNullOrEmpty(favorite.Password) ? string.IsNullOrEmpty(Settings.DefaultPassword) ? null : Settings.DefaultPassword : favorite.Password,
                IsSetEncryptedPassword = !string.IsNullOrEmpty(favorite.EncryptedPassword)
            };

            return credential;
        }
    }
}
