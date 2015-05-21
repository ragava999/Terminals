namespace Terminals.Network.DNS
{
    /// <summary>
    ///     The DNS TYPE (RFC1035 3.2.2/3) - 4 types are currently supported. Also, I know that this
    ///     enumeration goes against naming guidelines, but I have done this as an ANAME is most
    ///     definetely an 'ANAME' and not an 'Aname'
    ///     3.2.2. TYPE values
    ///     TYPE fields are used in resource records.  Note that these types are a
    ///     subset of QTYPEs.
    ///     TYPE            value and meaning
    ///     A               1 a host address
    ///     NS              2 an authoritative name server
    ///     MD              3 a mail destination (Obsolete - use MX)
    ///     MF              4 a mail forwarder (Obsolete - use MX)
    ///     CNAME           5 the canonical name for an alias
    ///     SOA             6 marks the start of a zone of authority
    ///     MB              7 a mailbox domain name (EXPERIMENTAL)
    ///     MG              8 a mail group member (EXPERIMENTAL)
    ///     MR              9 a mail rename domain name (EXPERIMENTAL)
    ///     NULL            10 a null RR (EXPERIMENTAL)
    ///     WKS             11 a well known service description
    ///     PTR             12 a domain name pointer
    ///     HINFO           13 host information
    ///     MINFO           14 mailbox or mail list information
    ///     MX              15 mail exchange
    ///     TXT             16 text strings
    /// </summary>
    public enum DnsType
    {
        None = 0,
        ANAME = 1,
        NS = 2,
        MD = 3,
        MF = 4,
        CNAME = 5,
        SOA = 6,
        MB = 7,
        MG = 8,
        MR = 9,
        NULL = 10,
        WKS = 11,
        PTR = 12,
        HINFO = 13,
        MINFO = 14,
        MX = 15,
        TXT = 16
    }
}