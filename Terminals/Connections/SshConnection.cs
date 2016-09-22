namespace Terminals.Connections
{
    using Properties;
    using System.Drawing;

    public class SshConnection : TerminalConnection
    {
        protected override Image[] images
        {
            get { return new Image[] { Resources.SSH }; }
        }

        public new int Port
        {
            get { return 22; }
        }
    }
}