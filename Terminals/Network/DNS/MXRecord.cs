using System;

namespace Terminals.Network.DNS
{
    /// <summary>
    ///     An MX (Mail Exchanger) Resource Record (RR) (RFC1035 3.3.9)
    /// </summary>
    [Serializable]
    public class MXRecord : RecordBase, IComparable
    {
        // an MX record is a domain name and an integer preference
        private readonly string _domainName;
        private readonly int _preference;

        /// <summary>
        ///     Constructs an MX record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer"> A logical pointer to the bytes holding the record </param>
        public MXRecord(Pointer pointer)
        {
            this._preference = pointer.ReadShort();
            this._domainName = pointer.ReadDomain();
        }

        // expose these fields public read/only
        public string DomainName
        {
            get { return this._domainName; }
        }

        public int Preference
        {
            get { return this._preference; }
        }

        /// <summary>
        ///     Implements the IComparable interface so that we can sort the MX records by their
        ///     lowest preference
        /// </summary>
        /// <param name="obj"> the other MxRecord to compare against </param>
        /// <returns> 1, 0, -1 </returns>
        public int CompareTo(object obj)
        {
            MXRecord mxOther = (MXRecord) obj;

            // we want to be able to sort them by preference
            if (mxOther._preference < this._preference) return 1;
            if (mxOther._preference > this._preference) return -1;

            // order mail servers of same preference by name
            return -mxOther._domainName.CompareTo(this._domainName);
        }

        public override string ToString()
        {
            return string.Format("Mail Server = {0}, Preference = {1}", this._domainName, this._preference.ToString());
        }

        public static bool operator ==(MXRecord record1, MXRecord record2)
        {
            if (record1 == null) throw new ArgumentNullException("record1");

            return record1.Equals(record2);
        }

        public static bool operator !=(MXRecord record1, MXRecord record2)
        {
            return !(record1 == record2);
        }

        public override bool Equals(object obj)
        {
            // this object isn't null
            if (obj == null) return false;

            // must be of same type
            if (this.GetType() != obj.GetType()) return false;

            MXRecord mxOther = (MXRecord) obj;

            // preference must match
            if (mxOther._preference != this._preference) return false;

            // and so must the domain name
            if (mxOther._domainName != this._domainName) return false;

            // its a match
            return true;
        }

        public override int GetHashCode()
        {
            return this._preference;
        }
    }
}