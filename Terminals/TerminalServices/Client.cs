using System;
using System.Net;
using Kohl.Framework.Logging;

namespace Terminals.TerminalServices
{
    public class Client
    {
        public bool Status { private get; set; }

        public string UserName { private get; set; }

        public string StationName { private get; set; }

        public string DomianName { private get; set; }

        public string ClientName { private get; set; }

        public int AddressFamily { private get; set; }

        public byte[] Address { private get; set; }

        private IPAddress IPAddress
        {
            get
            {
                try
                {
                    return new IPAddress(this.Address);
                }
                catch (Exception exc)
                {
                    Log.Error("IP Address", exc);
                }
                return new IPAddress(0);
            }
        }

        public override string ToString()
        {
            return string.Format("Domain:{0}, Client:{1}, Station:{2}, Address:{3}, Username:{4}, Status:{5}",
                                 this.DomianName, this.ClientName, this.StationName, this.IPAddress, this.UserName,
                                 this.Status);
        }
    }
}