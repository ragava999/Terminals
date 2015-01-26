using System;
using System.Collections.Generic;

namespace Terminals.TerminalServices
{
    public class TerminalServer
    {
        public TerminalServer()
        {
            this.Errors = new List<string>();
        }

        public List<string> Errors { get; private set; }

        public List<Session> Sessions { get; set; }

        public bool IsATerminalServer { get; set; }

        public string ServerName { get; set; }

        public IntPtr ServerPointer { get; set; }

        public static TerminalServer LoadServer(string serverName)
        {
            return TerminalServicesAPI.GetSessions(serverName);
        }
    }
}