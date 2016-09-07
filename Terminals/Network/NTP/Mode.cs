namespace Terminals.Network.NTP
{
    public enum Mode
    {
        SymmetricActive, // 1 - Symmetric active
        SymmetricPassive, // 2 - Symmetric pasive
        Client, // 3 - Client
        Server, // 4 - Server
        Broadcast, // 5 - Broadcast
        Unknown // 0, 6, 7 - Reserved
    }
}