using System;
using System.Windows.Forms;

namespace Terminals.Forms
{
    public partial class SaveActiveConnectionsForm : Form
    {
        public SaveActiveConnectionsForm()
        {
            this.InitializeComponent();
            this.Height = 160;
        }

        public bool PromptNextTime
        {
            get { return !this.chkDontShowDialog.Checked; }
        }

        public bool OpenConnectionsNextTime
        {
            get { return this.chkOpenOnNextTime.Checked; }
        }

        private void MoreButton_Click(object sender, EventArgs e)
        {
            if (this.Height == 160)
            {
                this.MoreButton.Text = "Less...";
                this.Height = 230;
            }
            else
            {
                this.MoreButton.Text = "More...";
                this.Height = 160;
            }
        }
    }
}