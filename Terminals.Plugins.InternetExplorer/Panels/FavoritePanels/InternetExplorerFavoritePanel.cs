namespace Terminals.Plugins.InternetExplorer.Panels.FavoritePanels
{
    using Terminals.Configuration.Files.Main.Favorites;
    using Terminals.Connection.Manager;
    using Terminals.Connection.Panels.FavoritePanels;
    using Terminals.Plugins.InternetExplorer.Connection;

    public partial class InternetExplorerFavoritePanel : FavoritePanel
    {
        #region Public Properties (4)
        public override string DefaultProtocolName
        {
            get
            {
                return typeof(InternetExplorerConnection).GetProtocolName();
            }
        }
        #endregion

        #region Constructors (1)
        public InternetExplorerFavoritePanel()
        {
            try
            {
                InitializeComponent();
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

        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {

        }
        #endregion

        #region Private Methods (1)
        private void AutoItFavoritePanel_Load(object sender, System.EventArgs e)
        {
            
        }
        #endregion
    }
}