namespace Terminals.Network.DNS
{
    /// <summary>
    ///     A Name Server Resource Record (RR) (RFC1035 3.3.11)
    /// </summary>
    public class NSRecord : RecordBase
    {
        // the fields exposed outside the assembly
        private readonly string _domainName;

        // expose this domain name address r/o to the world

        /// <summary>
        ///     Constructs a NS record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer"> A logical pointer to the bytes holding the record </param>
        public NSRecord(Pointer pointer)
        {
            this._domainName = pointer.ReadDomain();
        }

        public string DomainName
        {
            get { return this._domainName; }
        }

        public override string ToString()
        {
            return this._domainName;
        }
    }
}