namespace Terminals.Connections
{
    // .NET namespaces
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    // Terminals and framework namespaces
    using Kohl.Framework.Localization;
    using Kohl.Framework.Logging;
    using Configuration.Files.Main.Favorites;
    using Properties;

    public class HTTPConnection : Connection.Connection
    {
        private MiniBrowser browser;

        protected override int image
        {
            get
            {
                if (this.Favorite != null)
                    if (this.Favorite.HttpBrowser == BrowserType.InternetExplorer)
                        return 0;
                   
                return 1;
            }
        }

        protected override Image[] images
        {
            get
            {
                return new Image[]
                           {
                               Resources.IE,
                               Resources.FIREFOX
                           };
            }
        }

        public override ushort Port
        {
            get { return 80; }
        }

        private bool connected = false;

        public override bool Connected
        {
            get { return connected; }
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
        }

        public override bool Connect()
        {
            this.connected = false;

            try
            {
                try
                {
                    this.browser = new MiniBrowser();
                }
                catch (Exception ex)
                {
                    string browserType = null;

                    if (this.browser != null)
                        browserType = this.browser.BrowserType.ToString() + " ";

                    Log.Error(string.Format(Localization.Text("Connections.HTTPConnection.Connect_Error1"), browserType), ex);

                    return this.connected = false;
                }

                this.Embed(this.browser);

                this.browser.HtmlFormFields = this.Favorite.HtmlFormFields;
                this.browser.Home = this.Favorite.Url;
                this.browser.BrowserCredential.DomainName = this.Favorite.Credential.DomainName;
                this.browser.BrowserCredential.UserName = this.Favorite.Credential.UserName;
                this.browser.BrowserCredential.Password = this.Favorite.Credential.Password;
                this.browser.BrowserCredential.Authentication = this.Favorite.BrowserAuthentication;

                browser.InvokeIfNecessary(() =>
                {
                    this.browser.BrowserType = this.Favorite.HttpBrowser;
                    this.browser.Navigate(this.Favorite.Url);
                });
            }
            catch (Exception ex)
            {
                Log.Fatal(string.Format(Localization.Text("Connections.HTTPConnection.Connect_Error2"), this.Favorite.Protocol), ex);
                return this.connected = false;
            }

            return this.connected = true;
        }

        public override void Disconnect()
        {
            this.connected = false;

            this.browser.Dispose();

            this.CloseTabPage();
        }
    }
}