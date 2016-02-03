using System.Collections.Generic;

namespace Terminals.TerminalServices
{
    public class Session
    {
        public Session()
        {
            this.Processes = new List<SessionProcess>();
        }

        public string ServerName { get; set; }

        public Client Client { internal get; set; }

        public int SessionId { get; set; }

        public string WindowsStationName { get; set; }

        public ConnectionStates State { internal get; set; }

        public List<SessionProcess> Processes { get; set; }
    }
}