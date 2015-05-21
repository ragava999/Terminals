using System;
using System.Windows.Forms;

namespace Terminals.Connections
{
    public partial class NetworkingToolsLayout : UserControl
    {
        public delegate void TabChanged(object sender, TabControlEventArgs e);

        public NetworkingToolsLayout()
        {
            this.InitializeComponent();
        }

        public event TabChanged OnTabChanged;

        private void tabbedTools1_Load(object sender, EventArgs e)
        {
            this.tabbedTools1.OnTabChanged += this.tabbedTools1_OnTabChanged;
        }

        public void Execute(string Action, string Host)
        {
            this.tabbedTools1.Execute(Action, Host);
        }

        private void tabbedTools1_OnTabChanged(object sender, TabControlEventArgs e)
        {
            if (this.OnTabChanged != null) this.OnTabChanged(sender, e);
        }
    }
}