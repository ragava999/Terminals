using System.Windows.Forms;

namespace Terminals.SSHClient
{
    public partial class KeyChooser : UserControl
    {
        public KeyChooser()
        {
            this.InitializeComponent();
        }

        public override string Text
        {
            get { return this.box.Text; }
            set { this.box.Text = value; }
        }

        public int SelectedIndex
        {
            get { return this.box.SelectedIndex; }
            set { this.box.SelectedIndex = value; }
        }

        public ComboBox.ObjectCollection Items
        {
            get { return this.box.Items; }
        }
    }
}