using System;
using System.Windows.Forms;

namespace Terminals.Forms
{
    public partial class NewGroupForm : Form
    {
        public NewGroupForm()
        {
            this.InitializeComponent();
        }

        private void txtGroupName_TextChanged(object sender, EventArgs e)
        {
            this.btnOk.Enabled = this.txtGroupName.Text != String.Empty;
        }
    }
}