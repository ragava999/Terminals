namespace Terminals.Network.NTP
{
    public enum Stratum
    {
        Unspecified, // 0 - unspecified or unavailable
        PrimaryReference, // 1 - primary reference (e.g. radio-clock)
        SecondaryReference, // 2-15 - secondary reference (via NTP or SNTP)
        Reserved // 16-255 - reserved
    }
}