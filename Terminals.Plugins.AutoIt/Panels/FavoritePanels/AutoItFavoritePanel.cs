namespace Terminals.Plugins.AutoIt.Panels.FavoritePanels
{
	using System;
    using Terminals.Configuration.Files.Main.Favorites;
    using Terminals.Connection.Manager;
    using Terminals.Connection.Panels.FavoritePanels;
    using Terminals.Plugins.AutoIt.Connection;
    using System.Windows.Forms.Integration;

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
            	try
            	{
	                if (edit1 == null)
	                    return null;

                    return edit1.AvalonTextEditor.Text;
                }
            	catch (Exception ex)
	        	{
	        		Kohl.Framework.Logging.Log.Error("Error loading AutoIt panel. Unable to retrieve text property.", ex);
	        		return null;
	        	}
            }
            set
            {
            	try
            	{
	                if (edit1 == null)
	                    return;

                    edit1.AvalonTextEditor.Text = value;
            	}
	        	catch (Exception ex)
	        	{
	        		Kohl.Framework.Logging.Log.Error("Error loading AutoIt panel. Unable to set text property.", ex);
	        	}
            }
        }
        #endregion

        private TextEditor edit1 = null;

        #region Constructors (1)
        public AutoItFavoritePanel()
        {
            try
            {
                edit1 = new TextEditor();
                //edit1.AvalonTextEditor.Text = "; Your available predefined variables:\n; $Terminals_Protocol, $Terminals_ServerName, $Terminals_Port, $Terminals_ConnectionName,\n; $Terminals_CredentialName, $Terminals_User, $Terminals_Domain, $Terminals_Password\n; $Terminals_ConnectionHWND, $Terminals_ProcessId, $Terminals_Version, $Terminals_Directory\n; $Terminals_CurrentUser, $Terminals_CurrentUserDomain, $Terminals_CurrentUserSID, $Terminals_MachineDomain\n; Predefined functions: Func Embed($hWnd), Func Disconnect()\n\n#include <MsgBoxConstants.au3>\n\nLocal $iTimeout = 10\n\nMsgBox($MB_SYSTEMMODAL, \"Title\", \"This message box will timeout after \" & $iTimeout & \" seconds or select the OK button.\", $iTimeout)";
                InitializeComponent();
                ElementHost controlHost = new ElementHost();
                controlHost.Dock = System.Windows.Forms.DockStyle.Fill;
                controlHost.Child = edit1;
                this.Controls.Add(controlHost);

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
	            edit1.AvalonTextEditor.Text = favorite.AutoItScript();
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
            	favorite.AutoItScript(edit1.AvalonTextEditor.Text);
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