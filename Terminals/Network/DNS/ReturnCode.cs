namespace Terminals.Network.DNS
{
    /// <summary>
    ///     (RFC1035 4.1.1) These are the return codes the server can send back
    /// </summary>
    public enum ReturnCode
    {
        Success = 0,
        FormatError = 1,
        ServerFailure = 2,
        NameError = 3,
        NotImplemented = 4,
        Refused = 5,
        Other = 6
    }
}