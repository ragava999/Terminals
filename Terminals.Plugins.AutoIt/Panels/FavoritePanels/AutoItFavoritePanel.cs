namespace Terminals.Plugins.AutoIt.Panels.FavoritePanels
{
	using System;
    using Terminals.Configuration.Files.Main.Favorites;
    using Terminals.Connection.Manager;
    using Terminals.Connection.Panels.FavoritePanels;
    using Terminals.Plugins.AutoIt.Connection;
    using System.Windows.Forms;

    public partial class AutoItFavoritePanel : FavoritePanel
    {
        #region Public Properties (2)
        public override string DefaultProtocolName
        {
            get
            {
            	try
            	{
                	return typeof(AutoItConnection).GetProtocolName();
            	}
            	catch (Exception ex)
            	{
            		Kohl.Framework.Logging.Log.Fatal("Error loading AutoIt panel. Unable to retrieve protocol name.", ex);
	        		return null;
            	}
            }
        }

        public new string Text
    	{
            get
            {
                return edit1.Text;
            }
            set
            { 
                edit1.Text = value;
            }
        }
        #endregion

        #region Constructors (1)
        public AutoItFavoritePanel()
        {
            try
            {
                InitializeComponent();

                edit1.Text = "; Your available predefined variables:\n; $Terminals_Protocol, $Terminals_ServerName, $Terminals_Port, $Terminals_ConnectionName,\n; $Terminals_CredentialName, $Terminals_User, $Terminals_Domain, $Terminals_Password\n; $Terminals_ConnectionHWND, $Terminals_ProcessId, $Terminals_Version, $Terminals_Directory\n; $Terminals_CurrentUser, $Terminals_CurrentUserDomain, $Terminals_CurrentUserSID, $Terminals_MachineDomain\n; Predefined functions: Func Embed($hWnd), Func Disconnect()\n\n#include <MsgBoxConstants.au3>\n\nLocal $iTimeout = 10\n\n; MsgBox($MB_SYSTEMMODAL, \"Title\", \"This message box will timeout after \" & $iTimeout & \" seconds or select the OK button.\", $iTimeout)\"";

                this.Load += AutoItFavoritePanel_Load;
            }
            catch (Exception ex)
            {
            	this.DontLoadMe = true;
                Kohl.Framework.Logging.Log.Fatal("Error loading AutoIt panel. Unable to complete control construction.", ex);
            }
        }
        #endregion

        #region Public Methods (2)
        public override void FillControls(FavoriteConfigurationElement favorite)
        {
        	try
        	{
	            edit1.Text = favorite.AutoItScript();
        	}
        	catch (Exception ex)
        	{
        		Kohl.Framework.Logging.Log.Fatal("Error loading AutoIt panel. Unable to fill controls.", ex);
        	}
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
        	try
        	{
            	favorite.AutoItScript(edit1.Text);
        	}
        	catch (Exception ex)
        	{
        		Kohl.Framework.Logging.Log.Fatal("Error loading AutoIt panel. Unable to fill favorite.", ex);
        	}
        }
        #endregion

        #region Private Methods (1)
        private void AutoItFavoritePanel_Load(object sender, System.EventArgs e)
        {
        	try
        	{
                
        	}
        	catch (Exception ex)
        	{
        		Kohl.Framework.Logging.Log.Fatal("Error loading AutoIt panel. Unable to set default script language in editor", ex);
        	}
        }
        #endregion
    }
}