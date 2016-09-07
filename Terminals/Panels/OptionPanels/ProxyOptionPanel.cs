using System;
using System.Windows.Forms;

using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Panels.OptionPanels;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.Panels.OptionPanels
{
    public partial class ProxyOptionPanel : IOptionPanel
    {
        public ProxyOptionPanel()
        {
            this.InitializeComponent();
			this.ProxyCredentials.FillCredentials(null);
        }

        public override void LoadSettings()
        {
            this.rdoAutoProxy.Checked = Settings.ProxyAutoDetect;
            
            if (!this.rdoAutoProxy.Checked)
	            if (!Settings.ProxyUse)
	            	this.rdoDontUseProxy.Checked = true;
	            else
				if (Settings.ProxyAutoDetect)
	            		this.rdoAutoProxy.Checked = true;
	            	else
	            		this.rdoProxy.Checked = true;
			
			if (Settings.ProxyUse)
            {
            	rdoNoAuth.Checked = false;
            	rdoDefaultCredentials.Checked = true;
            }
        	else
        	{
				if (Settings.ProxyUseAuthCustom)
        		{
        			rdoCustomCredentials.Checked = true;

					if (Settings.ProxyXmlCredentialSetName == Terminals.Forms.Controls.CredentialPanel.Custom) {
						this.ProxyCredentials.FillControls (new FavoriteConfigurationElement () {
							DomainName = Settings.ProxyDomainName,
							UserName = Settings.ProxyUserName,
							Password = Settings.ProxyPassword,
							XmlCredentialSetName = Settings.ProxyXmlCredentialSetName
						});
					}
					this.ProxyCredentials.FillCredentials(Settings.ProxyXmlCredentialSetName);
        		}
        		else
        		{
        			rdoNoAuth.Checked = true;
        			rdoDefaultCredentials.Checked = false;
        		}
        	}
			
            this.txtProxyAddress.Text = Settings.ProxyAddress;
            this.txtProxyPort.Text = (Settings.ProxyPort.ToString().Equals("0"))
                                             ? "80"
                                             : Settings.ProxyPort.ToString();
            
            this.txtProxyAddress.Enabled = rdoProxy.Checked;
            this.txtProxyPort.Enabled = rdoProxy.Checked;
        }

        public override void SaveSettings()
        {
			var credentials = new FavoriteConfigurationElement () {
				DomainName = Settings.ProxyDomainName,
				UserName = Settings.ProxyUserName,
				Password = Settings.ProxyPassword
			};
			
			this.ProxyCredentials.FillFavorite(credentials);

			Settings.ProxyUserName = credentials.Credential.UserName;
			Settings.ProxyDomainName = credentials.Credential.DomainName;
			Settings.ProxyPassword = credentials.Credential.Password;

			Settings.ProxyXmlCredentialSetName = this.ProxyCredentials.SelectedCredentialSet == null || string.IsNullOrWhiteSpace(this.ProxyCredentials.SelectedCredentialSet.Name) ? Terminals.Forms.Controls.CredentialPanel.Custom : this.ProxyCredentials.SelectedCredentialSet.Name;

        	if (this.rdoDontUseProxy.Checked)
        	{
        		Settings.ProxyUse = false;
				Settings.ProxyAutoDetect = false;
        	}
        	else
        	{
        		Settings.ProxyUse = true;
        		
				Settings.ProxyAutoDetect = this.rdoAutoProxy.Checked;
        	}
        	
			Settings.ProxyUseAuth = false;
			Settings.ProxyUseAuthCustom = false;
            
			Settings.ProxyUseAuth |= rdoDefaultCredentials.Checked;
            
            if (rdoNoAuth.Checked)
				Settings.ProxyUseAuth = false;
            
			Settings.ProxyUseAuthCustom |= rdoCustomCredentials.Checked;                   	

            Settings.ProxyAddress = this.txtProxyAddress.Text;
            Settings.ProxyPort = Convert.ToInt32(this.txtProxyPort.Text);
        }

        public new IHostingForm IHostingForm { get; set; }
        
		private void ProxyCheckedChanged(object sender, EventArgs e)
		{
			panCredentials.Enabled = !rdoDontUseProxy.Checked;
			
			if (!rdoDefaultCredentials.Checked)
        		if (rdoCustomCredentials.Checked)
        			ProxyCredentials.Enabled = true;
        		else
        			ProxyCredentials.Enabled = false;
        		
    		if (rdoDontUseProxy.Checked)
    			ProxyCredentials.Enabled =false;
		}
		
		private void CredentialsCheckedChanged(object sender, EventArgs e)
		{
			ProxyCredentials.Enabled = rdoCustomCredentials.Checked;
		}
    }
}