using Kohl.Framework.Logging;
using System;
using System.Management;
using System.Windows.Forms;

namespace Terminals.Network.WMI
{
    /// <summary>
    ///     Summary description for LoginForm.
    /// </summary>
    public partial class LoginForm : Form
    {
        #region Constructors (1)

        public LoginForm()
        {
            this.InitializeComponent();

            this.PasswordTextBox.PasswordChar = Terminals.Forms.Controls.CredentialPanel.HIDDEN_PASSWORD_CHAR;
        }

        #endregion

        #region Public Properties (4)

        public bool Cancelled { get; private set; }

        public string MachineName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        #endregion

        #region Private Methods (3)

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            bool success = false;

            if (this.MachineNameTextBox.Text == string.Empty)
            {
                this.MachineNameTextBox.Text = @"\\localhost\root\cimv2";
            }
            else
            {
                if (!this.MachineNameTextBox.Text.StartsWith(@"\\"))
                    this.MachineNameTextBox.Text = @"\\" + this.MachineNameTextBox.Text;
            }

            if (!this.MachineNameTextBox.Text.StartsWith(@"\\localhost"))
            {
                if (this.UsernameTextBox.Text != string.Empty && this.PasswordTextBox.Text != string.Empty &&
                    this.MachineNameTextBox.Text != string.Empty)
                {
                    try
                    {
                        ConnectionOptions oConn = new ConnectionOptions
                        {
                            Username = this.UsernameTextBox.Text,
                            Password = this.PasswordTextBox.Text,
                            Impersonation = ImpersonationLevel.Impersonate,
                            Authentication = AuthenticationLevel.Connect
                        };
                        //oConn.Authority = this.MachineNameTextBox.Text;
                        ManagementScope oMs = new ManagementScope(this.MachineNameTextBox.Text, oConn)
                        {
                            Path = new ManagementPath(this.MachineNameTextBox.Text)
                        };
                        oMs.Connect();
                        success = oMs.IsConnected;
                    }
                    catch (Exception exc)
                    {
                        Log.Info("The login failed.", exc);
                        MessageBox.Show("The login failed.");
                    }

                    if (success)
                    {
                        this.Cancelled = false;
                        this.Visible = false;
                    }
                }
            }
            else
            {
                this.Cancelled = false;
                this.Visible = false;
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (this.MachineNameTextBox.Text == string.Empty)
            {
                this.MachineNameTextBox.Text = @"\\localhost\root\cimv2";
            }
            else
            {
                if (!this.MachineNameTextBox.Text.StartsWith(@"\\"))
                    this.MachineNameTextBox.Text = @"\\" + this.MachineNameTextBox.Text;
            }
        }

        #endregion
    }
}