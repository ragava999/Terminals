namespace Terminals.Forms.Credentials
{
    using System;
    using System.Windows.Forms;

    using Configuration.Files.Credentials;
    using Controls;

    public partial class UserSelectForm : Form
    {
        private CredentialSet set;

        public UserSelectForm()
        {
            this.InitializeComponent();
            
            this.passwordTextBox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.set = new CredentialSet
                           {
                               Name = "",
                               Username = this.userTextBox.Text,
                               SecretKey = this.passwordTextBox.Text,
                               Domain = this.domainTextBox.Text
                           };
            this.Close();
        }

        private void UserSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FavsList.CredSet = this.set;
        }
    }
}