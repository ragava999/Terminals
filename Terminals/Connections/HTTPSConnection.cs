namespace Terminals.Connections
{
    /// <summary>
    ///     Description of HTTPSConnection.
    /// </summary>
    public class HTTPSConnection : HTTPConnection
    {
        public override ushort Port
        {
            get { return 443; }
        }
    }
}