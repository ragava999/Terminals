using System.Windows.Forms;

namespace Terminals.Wizard
{
    public partial class MasterPassword : UserControl
    {
        public MasterPassword()
        {
            this.InitializeComponent();
        }

        public bool StorePassword
        {
            get { return this.enterPassword1.StorePassword; }
        }

        public string Password
        {
            get { return this.enterPassword1.Password; }
        }
    }
}