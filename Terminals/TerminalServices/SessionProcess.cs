namespace Terminals.TerminalServices
{
    using System;
    using System.Runtime.InteropServices;

    public class SessionProcess
    {
        /// <summary>
        /// The Remote Desktop Services session identifier for the session associated with the process.
        /// </summary>
        public int SessionID { get; set; }

        /// <summary>
        /// The process identifier that uniquely identifies the process on the RD Session Host server.
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// A pointer to a null-terminated string that contains the name of the executable file associated with the process.
        /// </summary>
        public string ProcessName { get; set; }

        public string User { get; set; }

        /// <summary>
        /// A pointer to the user security identifiers (SIDs) in the primary access token of the process.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// The number of threads in the process.
        /// </summary>
        public int NumberOfThreads { get; set; }

        /// <summary>
        /// The number of handles in the process.
        /// </summary>
        public int HandleCount { get; set; }

        /// <summary>
        /// The page file usage of the process, in bytes.
        /// </summary>
        public string PagefileUsage { get; set; }

        /// <summary>
        /// The peak page file usage of the process, in bytes.
        /// </summary>
        public string PeakPagefileUsage { get; set; }

        /// <summary>
        /// The working set size of the process, in bytes.
        /// </summary>
        public string WorkingSetSize { get; set; }

        /// <summary>
        /// The peak working set size of the process, in bytes.
        /// </summary>
        public string PeakWorkingSetSize { get; set; }

        /// <summary>
        /// The amount of time, in milliseconds, the process has been running in user mode.
        /// </summary>
        public LARGE_INTEGER UserTime { get; set; }

        /// <summary>
        /// The amount of time, in milliseconds, the process has been running in kernel mode.
        /// </summary>
        public LARGE_INTEGER KernelTime { get; set; }
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct LARGE_INTEGER
    {
        [FieldOffset(0)]
        public Int64 QuadPart;
        [FieldOffset(0)]
        public UInt32 LowPart;
        [FieldOffset(4)]
        public Int32 HighPart;
    }
}