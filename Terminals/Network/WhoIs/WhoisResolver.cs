using Kohl.Framework.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Terminals.Network.WhoIs
{
    /// <summary>
    ///     Queries the appropriate whois server for a given domain name and returns the results.
    /// </summary>
    public static class WhoisResolver
    {
        public static String Whois(String domain, String host)
        {
            if (domain == null)
                return "No domain specified.";

            String ret = String.Empty;
            Socket s = null;

            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //s.Connect(new IPEndPoint(Dns.Resolve(host).AddressList[0], 43));
                s.Connect(new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], 43));
                s.Send(Encoding.ASCII.GetBytes(domain + Environment.NewLine));

                Byte[] buffer = new Byte[1024];
                Int32 recv = s.Receive(buffer);
                while (recv > 0)
                {
                    ret += Encoding.ASCII.GetString(buffer, 0, recv);
                    recv = s.Receive(buffer);
                }

                s.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Log.Error("Could not connect to the WhoIs server. Check if you are allowed to use port 43 or please try again later.", e);
            }
            finally
            {
                if (s != null)
                    s.Close();
            }

            return ret;
        }

        /// <summary>
        ///     Queries an appropriate whois server for the given domain name.
        /// </summary>
        /// <param name="domain"> The domain name to retrieve the information of. </param>
        /// <returns> A string that contains the whois information of the specified domain name. </returns>
        /// <exception cref="ArgumentNullException"><c>domain</c> is null.</exception>
        /// <exception cref="ArgumentException"><c>domain</c> is invalid.</exception>
        /// <exception cref="SocketException">A network error occured.</exception>
        public static String Whois(String domain)
        {
            Int32 ccStart = domain.LastIndexOf(".");
            if (ccStart < 0 || ccStart == domain.Length)
                throw new ArgumentException();

            String cc = domain.Substring(ccStart + 1);
            //String host = (cc + ".whois-servers.net");
            const string host = ("whois-servers.net");
            //String host = ("whois.crsnic.net");

            return Whois(domain, host);
        }
    }
}