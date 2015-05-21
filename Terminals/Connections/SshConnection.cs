namespace Terminals.Connections
{
    using System.Drawing;
    using Properties;

    /// <summary>
    ///     Description of SshProtocol.
    /// </summary>
    public class SshConnection : TerminalConnection
    {
        protected override Image[] images
        {
            get { return new Image[] {Resources.SSH}; }
        }

        public new int Port
        {
            get { return 22; }
        }
    }
}