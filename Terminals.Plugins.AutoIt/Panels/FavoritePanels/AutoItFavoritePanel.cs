namespace Terminals.Plugins.AutoIt.Panels.FavoritePanels
{
	using System;
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
	
	                return edit1.Text;
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
	
	                edit1.Text = value;
            	}
	        	catch (Exception ex)
	        	{
	        		Kohl.Framework.Logging.Log.Error("Error loading AutoIt panel. Unable to set text property.", ex);
	        	}
            }
        }
        
        public string Language
        {
            get
            {
            	try
            	{
	                if (edit1 == null)
	                    return null;
	
	                return edit1.Language;
            	}
	        	catch (Exception ex)
	        	{
	        		Kohl.Framework.Logging.Log.Error("Error loading AutoIt panel. Unable to retrieve language.", ex);
	        		return null;
	        	}
            }
            set
            {
            	try
            	{
	                if (edit1 == null)
	                    return;
	
	                edit1.Language = value;
            	}
	        	catch (Exception ex)
	        	{
	        		Kohl.Framework.Logging.Log.Error("Error loading AutoIt panel. Unable to set language.", ex);
	        	}
            }
        }

        public bool Modified
        {
            get
            {
            	try
            	{
	                if (edit1 == null)
	                    return false;
	
	                return edit1.Modified;
            	}
	        	catch (Exception ex)
	        	{
	        		Kohl.Framework.Logging.Log.Error("Error loading AutoIt panel. Unable to retrieve modication state.", ex);
	        		return false;
	        	}
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
	            edit1.Language = "au3";
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
            	edit1.Language = "au3";
        	}
        	catch (Exception ex)
        	{
        		Kohl.Framework.Logging.Log.Fatal("Error loading AutoIt panel. Unable to set default script language in editor", ex);
        	}
        }
        #endregion
    }
}