namespace Terminals.Network.NTP
{
    public enum LeapIndicator
    {
        NoWarning, // 0 - No warning
        LastMinute61, // 1 - Last minute has 61 seconds
        LastMinute59, // 2 - Last minute has 59 seconds
        Alarm // 3 - Alarm condition (clock not synchronized)
    }
}