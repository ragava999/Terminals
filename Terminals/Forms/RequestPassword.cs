using System;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Forms
{
    public partial class RequestPassword : Form
    {
        public RequestPassword()
        {
            this.InitializeComponent();
            this.PasswordTextBox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
        }

        private const int COUNTERMAX = 3;

        private int counter = 0;

        private void OkButton_Click(object sender, EventArgs e)
        {
            String newPass = this.PasswordTextBox.Text;
            if (!Settings.IsMasterPasswordValid(newPass))
            {
                this.PasswordTextBox.Focus();
                this.PasswordTextBox.Text = "";
                this.errorProvider1.SetError(this.PasswordTextBox, "Invalid password");
                counter++;

                if (counter == COUNTERMAX)
                    CancelPasswordButton_Click(sender, e);
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
        }

        private void CancelPasswordButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            this.errorProvider1.Clear();
        }
    }
}