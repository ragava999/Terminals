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

        public bool IsTerminalServer { get; set; }

        public string ServerName { get; set; }

        public IntPtr ServerPointer { get; set; }

        public static TerminalServer LoadServer(string serverName, Kohl.Framework.Security.Credential credentials)
        {
            TerminalServer server = null;

            System.Threading.Thread t = new System.Threading.Thread(() => server = TerminalServicesApi.GetSessions(serverName, credentials));

            t.Start();

            while (t.ThreadState == System.Threading.ThreadState.Running)
            {
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }

            return server;
        }
    }
}