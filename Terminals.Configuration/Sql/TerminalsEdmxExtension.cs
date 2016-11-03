using System.Configuration;
namespace Terminals.Configuration.Sql
{
    public partial class TerminalsObjectContext
    {
    	public static string GetConfigValue(string key)
    	{
    		//Open the configuration file using the dll location
    		System.Configuration.Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(typeof(TerminalsObjectContext).Assembly.Location);
			
    		// Get the appSettings section
    		AppSettingsSection myDllConfigAppSettings = (AppSettingsSection)myDllConfig.GetSection("appSettings");

            if (myDllConfigAppSettings.Settings[key] == null)
                return null;

    		// return the desired field
			return myDllConfigAppSettings.Settings[key].Value; 
    	}
    	
    	public static TerminalsObjectContext Create()
    	{
    		string connectionString = "metadata=res://*/;";

            try
    		{
                string connectionStringInConfigFile = GetConfigValue("TerminalsConnection");

                if (string.IsNullOrWhiteSpace(connectionStringInConfigFile))
                {
                    // Configuration file has been ommitted, the config file entry is missing or the
                    // value is null -> return null
                    Kohl.Framework.Logging.Log.Warn("The connection to the terminals database will be ignored.");
                    return null;
                }

                connectionString += "provider=System.Data.SqlClient;provider connection string=\"" + connectionStringInConfigFile + "\"";
    		}
    		catch
    		{
    			// Configuration file has been ommitted, the config file entry is missing or the
    			// value is null -> return null
    			Kohl.Framework.Logging.Log.Warn("The connection to the terminals database will be ignored.");
    			return null;
    		}
    		
    		return new TerminalsObjectContext(connectionString);
    	}
    }
}