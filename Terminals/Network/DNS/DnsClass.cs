namespace Terminals.Network.DNS
{
    /// <summary>
    ///     The DNS CLASS (RFC1035 3.2.4/5)
    ///     Internet will be the one we'll be using (IN), the others are for completeness
    /// </summary>
    public enum DnsClass
    {
        None = 0,
        IN = 1,
        CS = 2,
        CH = 3,
        HS = 4
    }
}