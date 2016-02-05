namespace Terminals.Panels.OptionPanels
{
	using System;
	using System.Windows.Forms;
	
	using Terminals.Configuration.Files.Main.Settings;
	using Terminals.Connection.Panels.OptionPanels;
	using Terminals.Forms.Controls;

    public partial class CredentialStoreOptionPanel : IOptionPanel
    {
        public CredentialStoreOptionPanel()
        {
            this.InitializeComponent();

            this.txtKeePassPassword.PasswordChar = CredentialPanel.HIDDEN_PASSWORD_CHAR;
            rdoUseCredentialsXml.Checked = true;
        }

        public override void LoadSettings()
        {
            this.txtKeePassPassword.Text = Settings.KeePassPassword;
            this.txtKeePassPath.Text = Settings.KeePassPath;
            
            if (Settings.CredentialStore == Terminals.Configuration.Files.Main.CredentialStoreType.DB)
            	rdoUseDatabase.Checked = true;
            else if (Settings.CredentialStore == Terminals.Configuration.Files.Main.CredentialStoreType.KeePass)
            	rdoUseKeePass.Checked = true;
            else
            	rdoUseCredentialsXml.Checked = true;
        }

        public override void SaveSettings()
        {
        	var old = Settings.CredentialStore;
        	
        	Settings.CredentialStore = rdoUseCredentialsXml.Checked ? Terminals.Configuration.Files.Main.CredentialStoreType.Xml :rdoUseDatabase.Checked ? Terminals.Configuration.Files.Main.CredentialStoreType.DB :  Terminals.Configuration.Files.Main.CredentialStoreType.KeePass;
            Settings.KeePassPassword = txtKeePassPassword.Text;
            Settings.KeePassPath = txtKeePassPath.Text;
            
            if (old != Settings.CredentialStore)
            {
            	MessageBox.Show("It seems that your credential configuration has changed. Please close your existing connections and restart Terminals.");
            }
        }

        public new IHostingForm IHostingForm { get; set; }
       
		void ButtonBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select your KeePass2 database";
                dlg.Filter = "*.kdbx (KeePass2 Database)|*.kdbx";
                
                if (!string.IsNullOrEmpty(txtKeePassPath.Text) && System.IO.File.Exists(txtKeePassPath.Text))
                	dlg.FileName = txtKeePassPath.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                	txtKeePassPath.Text = dlg.FileName.NormalizePath();
                }
            }
		}
		
		void CredentialStoreChanged(object sender, EventArgs e)
		{
			if (!rdoUseKeePass.Checked)
				grpKeePass.Enabled = false;
			else
				grpKeePass.Enabled = true;
		}
    }
}