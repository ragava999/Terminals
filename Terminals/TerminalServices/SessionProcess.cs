namespace Terminals.TerminalServices
{
    public class SessionProcess
    {
        public int SessionID { get; set; }

        public int ProcessID { private get; set; }

        public string ProcessName { private get; set; }

        public string User { private get; set; }

        public string UserType { private get; set; }
    }
}