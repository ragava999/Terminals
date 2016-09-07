using System;
using System.Windows.Forms;


namespace Terminals.Network.WMI
{
    public partial class WMIServerCredentials : UserControl
    {
        public WMIServerCredentials()
        {
            this.InitializeComponent();
            
            this.PasswordTextbox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
        }

        public string SelectedServer
        {
            get { return this.comboBox1.Text; }
            set { this.comboBox1.Text = value; }
        }

        public string Username
        {
            get { return this.UsernameTextbox.Text; }
            set { this.UsernameTextbox.Text = value; }
        }

        public string Password
        {
            get { return this.PasswordTextbox.Text; }
            set { this.PasswordTextbox.Text = value; }
        }

        private void WMIServerCredentials_Load(object sender, EventArgs e)
        {
            if (Environment.UserDomainName != null && Environment.UserDomainName != "")
            {
                this.UsernameTextbox.Text = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
            }
            else
            {
                this.UsernameTextbox.Text = Environment.UserName;
            }
        }
    }
}