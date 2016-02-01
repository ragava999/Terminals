using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Kohl.Framework.Logging;
using System.Threading.Tasks;

namespace Terminals.TerminalServices
{
    public class TerminalServicesAPI
    {
        private enum SID_NAME_USE
        {
            User = 1,
            Group,
            Domain,
            Alias,
            WellKnownGroup,
            DeletedAccount,
            Invalid,
            Unknown,
            Computer
        }

        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        public enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames
        }

        private const long WTS_WSD_REBOOT = 0x00000004;
        private const long WTS_WSD_SHUTDOWN = 0x00000002;

        [DllImport("WtsApi32.dll", EntryPoint = "WTSQuerySessionInformationW", CharSet = CharSet.Ansi,
            SetLastError = true, ExactSpelling = true)]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass,
                                                             ref IntPtr ppBuffer, ref int pBytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSEnumerateProcesses(
            IntPtr serverHandle, // Handle to a terminal server. 
            Int32 reserved, // must be 0
            Int32 version, // must be 1
            ref IntPtr ppProcessInfo, // pointer to array of WTS_PROCESS_INFO
            ref Int32 pCount);

        // pointer to number of processes


        [DllImport("WtsApi32.dll", EntryPoint = "WTSQuerySessionInformationW", CharSet = CharSet.Ansi,
            SetLastError = true, ExactSpelling = true)]
        private static extern bool WTSQuerySessionInformation2(IntPtr hServer, int SessionId,
                                                               WTS_INFO_CLASS WTSInfoClass, ref IntPtr ppBuffer,
                                                               ref Int32 pCount);

        [DllImport("Kernel32.dll", EntryPoint = "GetCurrentProcessId", CharSet = CharSet.Ansi, SetLastError = true,
            ExactSpelling = true)]
        private static extern Int32 GetCurrentProcessId();

        [DllImport("Kernel32.dll", EntryPoint = "ProcessIdToSessionId", CharSet = CharSet.Ansi, SetLastError = true,
            ExactSpelling = true)]
        private static extern bool ProcessIdToSessionId(Int32 processID, ref Int32 sessionID);

        [DllImport("Kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId", CharSet = CharSet.Ansi,
            SetLastError = true, ExactSpelling = true)]
        private static extern Int32 WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSSendMessage(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.I4)] int SessionId,
            String pTitle,
            [MarshalAs(UnmanagedType.U4)] int TitleLength,
            String pMessage,
            [MarshalAs(UnmanagedType.U4)] int MessageLength,
            [MarshalAs(UnmanagedType.U4)] int Style,
            [MarshalAs(UnmanagedType.U4)] int Timeout,
            [MarshalAs(UnmanagedType.U4)] out int pResponse,
            bool bWait);


        //Function for TS Client IP Address 
        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSLogoffSession(IntPtr hServer, int SessionId, bool bWait);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool LookupAccountSid(
            string lpSystemName,
            [MarshalAs(UnmanagedType.LPArray)] byte[] Sid,
            StringBuilder lpName,
            ref uint cchName,
            StringBuilder ReferencedDomainName,
            ref uint cchReferencedDomainName,
            out SID_NAME_USE peUse);

        [DllImport("wtsapi32.dll", BestFitMapping = true, CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Auto, EntryPoint = "WTSEnumerateSessions", SetLastError = true,
            ThrowOnUnmappableChar = true)]
        private static extern Int32 WTSEnumerateSessions(
            [MarshalAs(UnmanagedType.SysInt)] IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] int Reserved,
            [MarshalAs(UnmanagedType.U4)] int Vesrion,
            [MarshalAs(UnmanagedType.SysInt)] ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref int pCount);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr WTSOpenServer(string pServerName);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern int WTSShutdownSystem(IntPtr ServerHandle, long ShutdownFlags);

        public static void ShutdownSystem(TerminalServer Server, bool Reboot)
        {
            long action = WTS_WSD_REBOOT;

            if (!Reboot) action = WTS_WSD_SHUTDOWN;

            IntPtr server = WTSOpenServer(Server.ServerName);

            if (server != IntPtr.Zero) WTSShutdownSystem(server, action);
        }

        public static bool SendMessage(Session Session, string Title, string Message, int Style, int Timeout, bool Wait)
        {
            IntPtr server = WTSOpenServer(Session.ServerName);

            if (server != IntPtr.Zero)
            {
                int respose = 0;
                return WTSSendMessage(server, Session.SessionId, Title, Title.Length, Message, Message.Length, Style,
                                      Timeout, out respose, Wait);
            }
            return false;
        }


        public static bool LogOffSession(Session Session, bool Wait)
        {
            IntPtr server = WTSOpenServer(Session.ServerName);

            if (server != IntPtr.Zero)
            {
                return WTSLogoffSession(server, Session.SessionId, Wait);
            }
            return false;
        }

        public static TerminalServer GetSessions(string ServerName, Kohl.Framework.Security.Credential credential)
        {
            TerminalServer Data = new TerminalServer {ServerName = ServerName};

            IntPtr ptrOpenedServer = IntPtr.Zero;
            try
            {
            	ptrOpenedServer = WTSOpenServer(ServerName);
            	
                if (ptrOpenedServer == IntPtr.Zero)
                {
                    Data.IsATerminalServer = false;
                    return Data;
                }

                Data.ServerPointer = ptrOpenedServer;
                Data.IsATerminalServer = true;

                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 Count = 0;

                Kohl.Framework.Security.Impersonator impersonator = null;
                
                if (credential != null)
                	impersonator = new Kohl.Framework.Security.Impersonator(credential);
                
                try
                {
                    Int32 FRetVal = WTSEnumerateSessions(ptrOpenedServer, 0, 1, ref ppSessionInfo, ref Count);

                    if (FRetVal != 0)
                    {
                        Data.Sessions = new List<Session>();
                        WTS_SESSION_INFO[] sessionInfo = new WTS_SESSION_INFO[Count + 1];
                        int i;

                        for (i = 0; i <= Count - 1; i++)
                        {
                            IntPtr session_ptr = new IntPtr(ppSessionInfo.ToInt32() + (i*Marshal.SizeOf(sessionInfo[i])));
                            sessionInfo[i] =
                                (WTS_SESSION_INFO) Marshal.PtrToStructure(session_ptr, typeof (WTS_SESSION_INFO));
                            Data.Sessions.Add(new Session
                                            {
                                                SessionId = sessionInfo[i].SessionID,
                                                State = (ConnectionStates) (int) sessionInfo[i].State,
                                                WindowsStationName = sessionInfo[i].pWinStationName,
                                                ServerName = ServerName
                                            });
                        }

                        WTSFreeMemory(ppSessionInfo);
                        strSessionsInfo[] tmpArr = new strSessionsInfo[sessionInfo.GetUpperBound(0) + 1];

                        for (i = 0; i <= tmpArr.GetUpperBound(0); i++)
                        {
                            tmpArr[i].SessionID = sessionInfo[i].SessionID;
                            tmpArr[i].StationName = sessionInfo[i].pWinStationName;
                            tmpArr[i].ConnectionState = GetConnectionState(sessionInfo[i].State);
                        }
                        // ERROR: Not supported in C#: ReDimStatement 
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Get Sessions Inner", ex);
                    Data.Errors.Add(ex.Message + "\r\n" + Marshal.GetLastWin32Error());
                }
                
                if (impersonator != null)
                	impersonator.Dispose();
            }
            catch (Exception ex)
            {
                Log.Info("Get Sessions Outer", ex);
                Data.Errors.Add(ex.Message + "\r\n" + Marshal.GetLastWin32Error());
            }

            WTS_PROCESS_INFO[] plist = WTSEnumerateProcesses(ptrOpenedServer, Data);

            //Get ProcessID of TS Session that executed this TS Session 
            Int32 active_process = GetCurrentProcessId();
            Int32 active_session = 0;

            bool success1 = ProcessIdToSessionId(active_process, ref active_session);

            if (active_session <= 0) success1 = false;

            if (Data != null && Data.Sessions != null)
            {
                foreach (Session s in Data.Sessions)
                {
                    if (s.Client == null) s.Client = new Client();

                    WTS_CLIENT_INFO ClientInfo = LoadClientInfoForSession(Data.ServerPointer, s.SessionId);
                    s.Client.Address = ClientInfo.Address;
                    s.Client.AddressFamily = ClientInfo.AddressFamily;
                    s.Client.ClientName = ClientInfo.WTSClientName;
                    s.Client.DomianName = ClientInfo.WTSDomainName;
                    s.Client.StationName = ClientInfo.WTSStationName;
                    s.Client.Status = ClientInfo.WTSStatus;
                    s.Client.UserName = ClientInfo.WTSUserName;
                    s.IsTheActiveSession = false;

                    if (success1 && s.SessionId == active_session) s.IsTheActiveSession = true;
                }
            }

            WTSCloseServer(ptrOpenedServer);
            return Data;
        }


        private static WTS_PROCESS_INFO[] WTSEnumerateProcesses(IntPtr WTS_CURRENT_SERVER_HANDLE, TerminalServer Data)
        {
            IntPtr pProcessInfo = IntPtr.Zero;
            int processCount = 0;

            if (!WTSEnumerateProcesses(WTS_CURRENT_SERVER_HANDLE, 0, 1, ref pProcessInfo, ref processCount))
                return null;

            const int NO_ERROR = 0;
            const int ERROR_INSUFFICIENT_BUFFER = 122;

            IntPtr pMemory = pProcessInfo;
            WTS_PROCESS_INFO[] processInfos = new WTS_PROCESS_INFO[processCount];

            for (int i = 0; i < processCount; i++)
            {
                processInfos[i] = (WTS_PROCESS_INFO) Marshal.PtrToStructure(pProcessInfo, typeof (WTS_PROCESS_INFO));
                pProcessInfo = (IntPtr) ((int) pProcessInfo + Marshal.SizeOf(processInfos[i]));

                SessionProcess p = new SessionProcess
                                       {
                                           ProcessID = processInfos[i].ProcessID,
                                           ProcessName =
                                               Marshal.PtrToStringAnsi(processInfos[i].ProcessName)
                                       };

                if (processInfos[i].UserSid != IntPtr.Zero)
                {
                    byte[] Sid = new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                    Marshal.Copy(processInfos[i].UserSid, Sid, 0, 14);
                    StringBuilder name = new StringBuilder();

                    uint cchName = (uint) name.Capacity;
                    SID_NAME_USE sidUse;
                    StringBuilder referencedDomainName = new StringBuilder();

                    uint cchReferencedDomainName = (uint) referencedDomainName.Capacity;

                    if (LookupAccountSid(Data.ServerName, Sid, name, ref cchName, referencedDomainName,
                                         ref cchReferencedDomainName, out sidUse))
                    {
                        int err = Marshal.GetLastWin32Error();

                        if (err == ERROR_INSUFFICIENT_BUFFER)
                        {
                            name.EnsureCapacity((int) cchName);
                            referencedDomainName.EnsureCapacity((int) cchReferencedDomainName);
                            err = NO_ERROR;

                            if (
                                !LookupAccountSid(null, Sid, name, ref cchName, referencedDomainName,
                                                  ref cchReferencedDomainName, out sidUse))
                                err = Marshal.GetLastWin32Error();
                        }

                        p.UserType = sidUse.ToString();
                        p.User = name.ToString();
                    }
                }

                p.SessionID = processInfos[i].SessionID;

                foreach (Session s in Data.Sessions)
                {
                    if (s.SessionId == p.SessionID)
                    {
                        if (s.Processes == null) s.Processes = new List<SessionProcess>();
                        s.Processes.Add(p);
                        break;
                    }
                }
            }

            WTSFreeMemory(pMemory);
            return processInfos;
        }

        private static WTS_CLIENT_INFO LoadClientInfoForSession(IntPtr ptrOpenedServer, int active_session)
        {
            int returned = 0;
            IntPtr str = IntPtr.Zero;

            WTS_CLIENT_INFO ClientInfo = new WTS_CLIENT_INFO
                                             {
                                                 WTSStationName = "",
                                                 WTSClientName = "",
                                                 Address = new byte[6]
                                             };

            ClientInfo.Address[2] = 0;
            ClientInfo.Address[3] = 0;
            ClientInfo.Address[4] = 0;
            ClientInfo.Address[5] = 0;

            ClientInfo.WTSClientName = GetString(ptrOpenedServer, active_session, WTS_INFO_CLASS.WTSClientName);
            ClientInfo.WTSStationName = GetString(ptrOpenedServer, active_session, WTS_INFO_CLASS.WTSWinStationName);
            ClientInfo.WTSDomainName = GetString(ptrOpenedServer, active_session, WTS_INFO_CLASS.WTSDomainName);

            //Get client IP address 
            IntPtr addr = IntPtr.Zero;

            if (WTSQuerySessionInformation2(ptrOpenedServer, active_session, WTS_INFO_CLASS.WTSClientAddress, ref addr,
                                            ref returned))
            {
                _WTS_CLIENT_ADDRESS obj = new _WTS_CLIENT_ADDRESS();
                obj = (_WTS_CLIENT_ADDRESS) Marshal.PtrToStructure(addr, obj.GetType());
                ClientInfo.Address[2] = obj.Address[2];
                ClientInfo.Address[3] = obj.Address[3];
                ClientInfo.Address[4] = obj.Address[4];
                ClientInfo.Address[5] = obj.Address[5];
            }

            return ClientInfo;
        }

        private static string GetString(IntPtr ptrOpenedServer, int active_session, WTS_INFO_CLASS whichOne)
        {
            IntPtr str = IntPtr.Zero;
            int returned = 0;

            if (WTSQuerySessionInformation(ptrOpenedServer, active_session, whichOne, ref str, ref returned))
            {
                return Marshal.PtrToStringAuto(str);
            }

            return "";
        }

        private static string GetConnectionState(WTS_CONNECTSTATE_CLASS State)
        {
            string RetVal;

            switch (State)
            {
                case WTS_CONNECTSTATE_CLASS.WTSActive:
                    RetVal = "Active";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSConnected:
                    RetVal = "Connected";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSConnectQuery:
                    RetVal = "Query";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSDisconnected:
                    RetVal = "Disconnected";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSDown:
                    RetVal = "Down";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSIdle:
                    RetVal = "Idle";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSInit:
                    RetVal = "Initializing.";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSListen:
                    RetVal = "Listen";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSReset:
                    RetVal = "reset";
                    break;
                case WTS_CONNECTSTATE_CLASS.WTSShadow:
                    RetVal = "Shadowing";
                    break;
                default:
                    RetVal = "Unknown connect state";
                    break;
            }

            return RetVal;
        }

        private struct WTS_CLIENT_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray)] public byte[] Address;
            [MarshalAs(UnmanagedType.LPWStr)] public int AddressFamily;
            [MarshalAs(UnmanagedType.LPWStr)] public string WTSClientName;
            [MarshalAs(UnmanagedType.LPWStr)] public string WTSDomainName;
            [MarshalAs(UnmanagedType.LPWStr)] public string WTSStationName;
            [MarshalAs(UnmanagedType.Bool)] public bool WTSStatus;
            [MarshalAs(UnmanagedType.LPWStr)] public string WTSUserName;
        }

        public struct WTS_PROCESS_INFO
        {
            public int ProcessID;
            public IntPtr ProcessName;
            public int SessionID;
            public IntPtr UserSid;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WTS_SESSION_INFO
        {
            //DWORD integer 
            public readonly Int32 SessionID;
            // integer LPTSTR - Pointer to a null-terminated string containing the name of the WinStation for this session 
            public readonly string pWinStationName;
            public readonly WTS_CONNECTSTATE_CLASS State;
        }

        //Structure for TS Client IP Address 
        [StructLayout(LayoutKind.Sequential)]
        private struct _WTS_CLIENT_ADDRESS
        {
            private readonly int AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public readonly byte[] Address;
        }

        private struct strSessionsInfo
        {
            public string ConnectionState;
            public int SessionID;
            public string StationName;
        }

        //Structure for TS Client Information 
    }
}