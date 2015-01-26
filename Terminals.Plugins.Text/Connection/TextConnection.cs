using System.Drawing;
using System.Windows.Forms;
using Kohl.TinyMce;
using Terminals.Configuration.Files.Main.Favorites;
using System.Threading;

namespace Terminals.Plugins.Text.Connection
{
    public class TextConnection : Terminals.Connection.Connection
    {
        private bool connected = false;

        public TextConnection()
        {

        }

        public override bool IsPortRequired
        {
            get
            {
                return false;
            }
        }

        public new string Text
        {
            get
            {
                // If we are coming from TextFavoritePanel
                if (Favorite == null)
                    return null;

                return Favorite.TinyMceText();
            }
            set
            {
                // If we are coming from TextFavoritePanel
                if (Favorite == null)
                {
                    if (value != tinyMce.Text)
                        tinyMce.Text = value;
                }
                // If we are in this class and want to update the value
                else
                {
                    if (!Favorite.ShowTinyMceInConnectionMode())
                        return;

                    Favorite.TinyMceText(value);

                    if (tinyMce.Text != Favorite.TinyMceText())
                        tinyMce.Text = Favorite.TinyMceText();

                    this.SaveFavorites();
                }
            }
        }

        public override bool Connected
        {
            get { return connected; }
        }

        private TinyMce tinyMce = null;

        public override bool Connect()
        {
            // Creates a new tiny Mce instance
            CreateInstance().Render();

            // Embed this instance in our terminal window.
            this.Embed(tinyMce);

            return connected = true;
        }

        public TinyMce CreateInstance()
        {
            if (tinyMce == null)
            {
                // If we are coming from TextFavoritePanel
                if (ParentForm == null)
                    tinyMce = new TinyMce();
                // If we are in this class
                else
                    InvokeIfNecessary(() => tinyMce = new TinyMce());

                // Set the text automatically in our public "Text" property when it has been changed.
                tinyMce.TextChanged += (sender, args) => this.Text = tinyMce.Text;
            }

            // Assume by default that tiny mce is now shown -> only the resulting html
            tinyMce.IsTextEditable = false;

            // If we are coming from TextFavoritePanel
            if (Favorite != null)
                tinyMce.IsTextEditable = Favorite.ShowTinyMceInConnectionMode();

            tinyMce.Text = this.Text;

            return tinyMce;
        }

        public override void Disconnect()
        {
            // Dispose Tiny Mce
            if (tinyMce != null)
                tinyMce.Dispose();

            connected = false;

			// If we are coming from TextFavoritePanel
            if (Favorite != null)
	            // Close the tab page
	            this.CloseTabPage();
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {

        }

        protected override Image[] images
        {
            get { return new Image[] { Properties.Resources.Text }; }
        }
    }
}