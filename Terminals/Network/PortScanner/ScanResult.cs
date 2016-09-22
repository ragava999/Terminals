/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 12:37
 */
using System.Net;
using Metro.Scanning;

namespace Terminals.Network.PortScanner
{
    /// <summary>
    ///     Description of ScanResult.
    /// </summary>
    public class ScanResult
    {
        public IPEndPoint RemoteEndPoint { get; set; }

        public TcpPortState State { get; set; }
    }
}