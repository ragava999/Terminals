namespace Terminals.Network.DNS
{
    /// <summary>
    ///     An SOA Resource Record (RR) (RFC1035 3.3.13)
    /// </summary>
    public class SoaRecord : RecordBase
    {
        // these fields constitute an SOA RR
        private readonly int _defaultTtl;
        private readonly int _expire;
        private readonly string _primaryNameServer;
        private readonly int _refresh;
        private readonly string _responsibleMailAddress;
        private readonly int _retry;
        private readonly int _serial;

        /// <summary>
        ///     Constructs an SOA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer"> A logical pointer to the bytes holding the record </param>
        public SoaRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            this._primaryNameServer = pointer.ReadDomain();
            this._responsibleMailAddress = pointer.ReadDomain();
            this._serial = pointer.ReadInt();
            this._refresh = pointer.ReadInt();
            this._retry = pointer.ReadInt();
            this._expire = pointer.ReadInt();
            this._defaultTtl = pointer.ReadInt();
        }

        // expose these fields public read/only
        public string PrimaryNameServer
        {
            get { return this._primaryNameServer; }
        }

        public string ResponsibleMailAddress
        {
            get { return this._responsibleMailAddress; }
        }

        public int Serial
        {
            get { return this._serial; }
        }

        public int Refresh
        {
            get { return this._refresh; }
        }

        public int Retry
        {
            get { return this._retry; }
        }

        public int Expire
        {
            get { return this._expire; }
        }

        public int DefaultTtl
        {
            get { return this._defaultTtl; }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "primary name server = {0}, responsible mail addr = {1}, serial  = {2}, refresh = {3}, retry   = {4}, expire  = {5}, default TTL = {6}",
                    this._primaryNameServer,
                    this._responsibleMailAddress,
                    this._serial.ToString(),
                    this._refresh.ToString(),
                    this._retry.ToString(),
                    this._expire.ToString(),
                    this._defaultTtl.ToString());
        }
    }
}