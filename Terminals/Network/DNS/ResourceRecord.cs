using System;

namespace Terminals.Network.DNS
{
    /// <summary>
    ///     Represents a Resource Record as detailed in RFC1035 4.1.3
    /// </summary>
    [Serializable]
    public class ResourceRecord
    {
        // private, constructor initialised fields
        private readonly RecordBase _record;

        /// <summary>
        ///     Construct a resource record from a pointer to a byte array
        /// </summary>
        /// <param name="pointer"> the position in the byte array of the record </param>
        protected ResourceRecord(Pointer pointer)
        {
            // the next short is the record length, we only use it for unrecognised record types
            int recordLength = pointer.ReadShort();

            // and create the appropriate RDATA record based on the dnsType
            switch ((DnsType)pointer.ReadShort())
            {
                case DnsType.NS:
                    this._record = new NSRecord(pointer);
                    break;
                case DnsType.MX:
                    this._record = new MXRecord(pointer);
                    break;
                case DnsType.ANAME:
                    this._record = new ANameRecord(pointer);
                    break;
                case DnsType.SOA:
                    this._record = new SoaRecord(pointer);
                    break;
                default:
                    {
                        // move the pointer over this unrecognised record
                        pointer += recordLength;
                        break;
                    }
            }
        }

        public RecordBase Record
        {
            get { return this._record; }
        }
    }

    // Answers, Name Servers and Additional Records all share the same RR format

    [Serializable]
    public class NameServer : ResourceRecord
    {
        public NameServer(Pointer pointer) : base(pointer)
        {
        }
    }
}