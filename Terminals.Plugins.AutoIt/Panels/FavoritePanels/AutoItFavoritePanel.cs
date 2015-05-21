namespace Terminals.Plugins.AutoIt.Panels.FavoritePanels
{
    using Terminals.Configuration.Files.Main.Favorites;
    using Terminals.Connection.Manager;
    using Terminals.Connection.Panels.FavoritePanels;
    using Terminals.Plugins.AutoIt.Connection;

    public partial class AutoItFavoritePanel : FavoritePanel
    {
        #region Public Properties (4)
        public override string DefaultProtocolName
        {
            get
            {
                return typeof(AutoItConnection).GetProtocolName();
            }
        }

        public new string Text
        {
            get
            {
                if (edit1 == null)
                    return null;

                return edit1.Text;
            }
            set
            {
                if (edit1 == null)
                    return;

                edit1.Text = value;
            }
        }

        public string Language
        {
            get
            {
                if (edit1 == null)
                    return null;

                return edit1.Language;
            }
            set
            {
                if (edit1 == null)
                    return;

                edit1.Language = value;
            }
        }

        public bool Modified
        {
            get
            {
                if (edit1 == null)
                    return false;

                return edit1.Modified;
            }
        }
        #endregion

        #region Constructors (1)
        public AutoItFavoritePanel()
        {
            try
            {
                InitializeComponent();
                this.Load += AutoItFavoritePanel_Load;
            }
            catch (System.Exception ex)
            {
                Kohl.Framework.Logging.Log.Fatal("Error loading AutoIt panel.", ex);
            }
        }
        #endregion

        #region Public Methods (2)
        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            edit1.Text = favorite.AutoItScript();
            edit1.Language = "au3";
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.AutoItScript(edit1.Text);
        }
        #endregion

        #region Private Methods (1)
        private void AutoItFavoritePanel_Load(object sender, System.EventArgs e)
        {
            edit1.Language = "au3";
        }
        #endregion
    }
}