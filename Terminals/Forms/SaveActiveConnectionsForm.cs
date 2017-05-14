using System;
using System.Windows.Forms;

namespace Terminals.Forms
{
    public partial class SaveActiveConnectionsForm : Form
    {
        int ExpandSize { get; }

        public SaveActiveConnectionsForm()
        {
            this.InitializeComponent();
            ExpandSize = pnlOptions.Height;
            ToggleView();
        }

        public bool PromptNextTime
        {
            get { return !this.chkDontShowDialog.Checked; }
        }

        public bool OpenConnectionsNextTime
        {
            get { return this.chkOpenOnNextTime.Checked; }
        }

        private void ToggleView(object sender = null, EventArgs e = null)
        {
            if (MoreButton.Text == "More...")
            {
                MoreButton.Text = "Less...";
                Height += ExpandSize;
            }
            else
            {
                MoreButton.Text = "More...";
                Height -= ExpandSize;
            }
        }
    }
}