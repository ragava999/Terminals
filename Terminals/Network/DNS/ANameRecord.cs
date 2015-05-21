using System.Net;

namespace Terminals.Network.DNS
{
    /// <summary>
    ///     ANAME Resource Record (RR) (RFC1035 3.4.1)
    /// </summary>
    public class ANameRecord : RecordBase
    {
        // An ANAME records consists simply of an IP address
        private readonly IPAddress ipAddress;

        // expose this IP address r/o to the world

        /// <summary>
        ///     Constructs an ANAME record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer"> A logical pointer to the bytes holding the record </param>
        public ANameRecord(Pointer pointer)
        {
            byte b1 = pointer.ReadByte();
            byte b2 = pointer.ReadByte();
            byte b3 = pointer.ReadByte();
            byte b4 = pointer.ReadByte();

            // this next line's not brilliant - couldn't find a better way though
            this.ipAddress = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}", b1, b2, b3, b4));
        }

        public IPAddress IPAddress
        {
            get { return this.ipAddress; }
        }

        public override string ToString()
        {
            return this.ipAddress.ToString();
        }
    }
}