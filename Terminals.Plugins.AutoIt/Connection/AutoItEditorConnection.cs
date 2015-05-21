namespace Terminals.Plugins.AutoIt.Connection
{
    // .NET namespaces
    using System.Drawing;
    using System.Windows.Forms;

    // Terminals namespaces
    using Configuration.Files.Main.Favorites;
    using MainSettings = Configuration.Files.Main.Settings;
    
    using Kohl.Framework.Info;
    using Panels.FavoritePanels;

    public partial class AutoItEditorConnection : Terminals.Connection.Connection
    {
        private bool connected = false;

        public override bool IsPortRequired
        {
            get
            {
                return false;
            }
        }

        protected override Image[] images
        {
            get { return new Image[] { Properties.Resources.AutoIt }; }
        }

        public AutoItEditorConnection()
        {
            InitializeComponent();
        }

        public override bool Connected
        {
            get { return connected; }
        }

        public override bool Connect()
        {
            InvokeIfNecessary(() => panel = new AutoItFavoritePanel());
            panel.Load += panel_Load;
            InvokeIfNecessary(() => Embed(panel));
            InvokeIfNecessary(() => panel.Text = Favorite.AutoItScript());

            return connected = true;
        }

        void panel_Load(object sender, System.EventArgs e)
        {
            panel.Language = "au3";
        }

        public override void Disconnect()
        {
            if (panel != null)
            {
                if (panel.Modified && MessageBox.Show("Would you like to save your changes?", "Save modifications?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    InvokeIfNecessary(() => Favorite.AutoItScript(panel.Text));
            }

            this.CloseTabPage();
            connected = false;
        }

        private AutoItFavoritePanel panel = null;

        protected override void ChangeDesktopSize(DesktopSize size, System.Drawing.Size siz)
        {
            
        }
    }
}