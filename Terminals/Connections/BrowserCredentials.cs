namespace Terminals.Connections
{
    using Configuration.Files.Credentials;   
    using Configuration.Files.Main.Favorites;

    public class BrowserCredentials : Credential
    {
        public BrowserCredentials()
        {
            this.Authentication = BrowserAuthentication.None;
        }

        public BrowserAuthentication Authentication { get; set; }
    }
}