namespace Terminals.TerminalServices
{
    public class SessionInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string DomainName { get; set; }

        public string ClientName { get; set; }

        public TSManager.WTS_CONNECTSTATE_CLASS State { get; set; }
    }
}