using System.Collections.Generic;

namespace Terminals.TerminalServices
{
    public class Session
    {
        public Session()
        {
            this.Processes = new List<SessionProcess>();
        }

        public bool IsTheActiveSession { private get; set; }

        public string ServerName { get; set; }

        public Client Client { get; set; }

        public int SessionId { get; set; }

        public string WindowsStationName { private get; set; }

        public ConnectionStates State { private get; set; }

        public List<SessionProcess> Processes { get; set; }
    }
}