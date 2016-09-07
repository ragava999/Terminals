using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Kohl.Framework.Logging;
using System.Linq;

namespace Terminals.TerminalServices
{
    public static class TerminalServicesApi
    {
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

        public static TerminalServer GetSessions(string serverName, Kohl.Framework.Security.Credential credential)
        {        	
            TerminalServer terminalServer = new TerminalServer {ServerName = serverName};

            Kohl.Framework.Security.Impersonator impersonator = null;
        	
            // Start impersonation if we requested to do so ...
            try
            {
	            if (credential != null)
	            	impersonator = new Kohl.Framework.Security.Impersonator(credential);
            }
            catch (Exception ex)
            {
                Log.Warn("Error impersonating RDP session enumerator. Trying to query RDP session details without impersonation ...", ex);
            }
            
            // Open a WTS connection to the server to be able to get details about the sessions and processes running on it
            IntPtr ptrOpenedServer = IntPtr.Zero;
            try
            {
	            ptrOpenedServer = WTSOpenServer(terminalServer.ServerName);
            }
            catch (Exception ex)
            {
        		Log.Error("Error opening WTS server connection. Aborting to query RDP sessions.", ex);
        		return terminalServer;
            }
            
            if (ptrOpenedServer == IntPtr.Zero)
            {
                terminalServer.IsTerminalServer = false;
                return terminalServer;
            }
            
            terminalServer.ServerPointer = ptrOpenedServer;
            terminalServer.IsTerminalServer = true;
            
            // Try to get information about the sessions and the clients connected to it.
            GetClientInfos(terminalServer);
            
            // Try to get information about the server's processes.
            GetProcessInfos(terminalServer);

            if (ptrOpenedServer != IntPtr.Zero)
            	WTSCloseServer(ptrOpenedServer);
            
            if (impersonator != null)
            	impersonator.Dispose();
            
            return terminalServer;
        }
	
        private static void GetProcessInfos(TerminalServer terminalServer)
        {
			try
            {        	
	           	IntPtr pProcessInfo = IntPtr.Zero;
	            int processCount = 0;
	            IntPtr useProcessesExStructure = new IntPtr(1);
	            
	            if (WTSEnumerateProcessesExW(terminalServer.ServerPointer, ref useProcessesExStructure, WTS_ANY_SESSION, ref pProcessInfo, ref processCount))
	            {
	            	const int NO_ERROR = 0;
	            	const int ERROR_INSUFFICIENT_BUFFER = 122;
	
		            WTS_PROCESS_INFO_EX[] processInfos = new WTS_PROCESS_INFO_EX[processCount];
		
		            for (int i = 0; i < processCount; i++)
		            {
		                processInfos[i] = (WTS_PROCESS_INFO_EX) Marshal.PtrToStructure(pProcessInfo, typeof (WTS_PROCESS_INFO_EX));
		                
		                SessionProcess p = new SessionProcess
						{
		                   SessionID = processInfos[i].SessionID,
						   ProcessID = processInfos[i].ProcessID,
						   ProcessName = processInfos[i].ProcessName,
						   NumberOfThreads = processInfos[i].NumberOfThreads,
						   HandleCount = processInfos[i].HandleCount,
						   PagefileUsage = (processInfos[i].PagefileUsage / 1024.0 / 1024.0).ToString("##0.## MB"),
						   PeakPagefileUsage = (processInfos[i].PeakPagefileUsage / 1024.0 / 1024.0).ToString("##0.## MB"),
						   WorkingSetSize = (processInfos[i].WorkingSetSize / 1024.0 / 1024.0).ToString("##0.## MB"),
						   PeakWorkingSetSize = (processInfos[i].PeakWorkingSetSize / 1024.0 / 1024.0).ToString("##0.## MB"),
						   KernelTime = processInfos[i].KernelTime,
						   UserTime = processInfos[i].UserTime 
						};
		                
		                if (processInfos[i].UserSid != IntPtr.Zero)
		                {
		                    byte[] Sid = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
		                    Marshal.Copy(processInfos[i].UserSid, Sid, 0, 14);
		                    StringBuilder name = new StringBuilder();
		
		                    uint cchName = (uint) name.Capacity;
		                    SID_NAME_USE sidUse;
		                    StringBuilder referencedDomainName = new StringBuilder();
		
		                    uint cchReferencedDomainName = (uint) referencedDomainName.Capacity;
		
		                    if (LookupAccountSid(terminalServer.ServerName, Sid, name, ref cchName, referencedDomainName, ref cchReferencedDomainName, out sidUse))
		                    {
		                        int err = Marshal.GetLastWin32Error();
		
		                        if (err == ERROR_INSUFFICIENT_BUFFER)
		                        {
		                            name.EnsureCapacity((int) cchName);
		                            referencedDomainName.EnsureCapacity((int) cchReferencedDomainName);
		                            
		                            err = NO_ERROR;
		
		                            if (!LookupAccountSid(null, Sid, name, ref cchName, referencedDomainName, ref cchReferencedDomainName, out sidUse))
		                                err = Marshal.GetLastWin32Error();
		                        }
		
		                        p.Sid = sidUse.ToString();
		                        p.User = name.ToString();
		                    }
		                }
	
		                terminalServer.Sessions.FirstOrDefault(s => s.SessionId == p.SessionID).Processes.Add(p);
		                pProcessInfo = (IntPtr) ((int) pProcessInfo + Marshal.SizeOf(processInfos[i]));
		            }
	            }
	            
	            if (pProcessInfo != IntPtr.Zero)
	            	WTSFreeMemory(pProcessInfo);
            }
            catch (Exception ex)
            {
                Log.Info("Error enumerating remote processes for RDP sessions.", ex);
                terminalServer.Errors.Add(ex.Message + "\r\n" + Marshal.GetLastWin32Error());
            }
        }
        
        private static void GetClientInfos(TerminalServer terminalServer)
        {
        	try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 Count = 0;
                            
                Int32 FRetVal = WTSEnumerateSessions(terminalServer.ServerPointer, 0, 1, ref ppSessionInfo, ref Count);

                if (FRetVal != 0)
                {
                    terminalServer.Sessions = new List<Session>();
                    WTS_SESSION_INFO[] sessionInfo = new WTS_SESSION_INFO[Count + 1];

                    for (int i = 0; i <= Count - 1; i++)
                    {
                        IntPtr session_ptr = new IntPtr(ppSessionInfo.ToInt32() + (i*Marshal.SizeOf(sessionInfo[i])));
                        sessionInfo[i] = (WTS_SESSION_INFO) Marshal.PtrToStructure(session_ptr, typeof (WTS_SESSION_INFO));
                        
                        Session session = new Session
                                        {
                                            SessionId = sessionInfo[i].SessionID,
                                            State = (ConnectionStates) (int) sessionInfo[i].State,
                                            WindowsStationName = string.IsNullOrWhiteSpace(sessionInfo[i].pWinStationName) ? "RPD-Tcp#?" : sessionInfo[i].pWinStationName,
                                            ServerName = terminalServer.ServerName
                                        };
                        
                        session.Client = GetClientInfoForSession(terminalServer.ServerPointer, session.SessionId);
                    	session.Client.Status = Enum.GetName(typeof(ConnectionStates), session.State).Replace("WTS", "");
                        
                        terminalServer.Sessions.Add(session);
                    }

                    WTSFreeMemory(ppSessionInfo);
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error enumerating RDP sessions.", ex);
                terminalServer.Errors.Add(ex.Message + "\r\n" + Marshal.GetLastWin32Error());
            }
        }
        
        private static Client GetClientInfoForSession(IntPtr ptrOpenedServer, int sessionId)
        {
        	Client client = new Client();

        	client.SessionId = (uint)sessionId;
            client.ClientName = GetString(ptrOpenedServer, sessionId, WTS_INFO_CLASS.WTSClientName);
            client.StationName = GetString(ptrOpenedServer, sessionId, WTS_INFO_CLASS.WTSWinStationName);
            client.DomianName = GetString(ptrOpenedServer, sessionId, WTS_INFO_CLASS.WTSDomainName);
            client.UserName = GetString(ptrOpenedServer, sessionId, WTS_INFO_CLASS.WTSUserName);
            
            //Get connection state
            IntPtr ppBuffer = IntPtr.Zero;
	        int iReturned = 0;

	        /*
            if (WTSQuerySessionInformation(ptrOpenedServer, sessionId, WTS_INFO_CLASS.WTSConnectState, ref ppBuffer, ref iReturned))
            {
	        	var iDataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
	        	var current = new IntPtr(ppBuffer.ToInt32());
	        	//shift pointer value by the size of struct because of array
                current = current + iDataSize;
                
             	var sessionInfo = (WTS_SESSION_INFO)Marshal.PtrToStructure(current, typeof(WTS_SESSION_INFO));
                client.Status = Enum.GetName(typeof(WTS_CONNECTSTATE_CLASS), sessionInfo.State);
            }
                        
            ppBuffer = IntPtr.Zero;
	        iReturned = 0;
	        */

			//Get client IP address	        
            if (WTSQuerySessionInformation(ptrOpenedServer,
               sessionId,
                WTS_INFO_CLASS.WTSClientAddress,
                ref ppBuffer,
                ref iReturned))
            {
                var clientAddress = (_WTS_CLIENT_ADDRESS)Marshal.PtrToStructure(ppBuffer, typeof(_WTS_CLIENT_ADDRESS));
                client.AddressFamily = Enum.GetName(typeof(AddressFamilyType), clientAddress.AddressFamily).Replace("AF_","");

                if (clientAddress.AddressFamily == AddressFamilyType.AF_INET)
                {
                	if (client.StationName.ToUpper() != "RDP-TCP" && client.StationName.ToLower() != "services" && client.StationName.ToLower() != "console")
                	{
                		// IPv4 address
                    	client.Address = string.Join(".", clientAddress.Address.Skip(2).Take(4));
                	}
                }
            }
			else
				Log.Warn("Unable to get the IP address of the client for session " + sessionId.ToString());
            
            return client;
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

        #region Windows Native Api
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        [System.Runtime.InteropServices.BestFitMapping(true)]
        private struct WTS_PROCESS_INFO_EX
        {
        	public Int32 SessionID;			// The Remote Desktop Services session identifier for the session associated with the process.
            public Int32 ProcessID;			// The process identifier that uniquely identifies the process on the RD Session Host server.
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ProcessName;		// A pointer to a null-terminated string that contains the name of the executable file associated with the process.
            public IntPtr UserSid;			// A pointer to the user security identifiers (SIDs) in the primary access token of the process. 
            public Int32 NumberOfThreads;	// The number of threads in the process.
            public Int32 HandleCount;		// The number of handles in the process.
            public Int32 PagefileUsage;		// The page file usage of the process, in bytes.
            public Int32 PeakPagefileUsage;	// The peak page file usage of the process, in bytes.
            public Int32 WorkingSetSize;	// The working set size of the process, in bytes.
            public Int32 PeakWorkingSetSize;// The peak working set size of the process, in bytes.
            public LARGE_INTEGER UserTime;	// The amount of time, in milliseconds, the process has been running in user mode.
            public LARGE_INTEGER KernelTime;// The amount of time, in milliseconds, the process has been running in kernel mode.
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

        [StructLayout(LayoutKind.Sequential)]
        private struct _WTS_CLIENT_ADDRESS
        {
            public readonly AddressFamilyType AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public readonly byte[] Address;
        }

        private enum AddressFamilyType
        {
            AF_INET,
            AF_INET6, 
            AF_IPX, 
            AF_NETBIOS, 
            AF_UNSPEC
        }
        
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

        private enum WTS_CONNECTSTATE_CLASS
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

        private enum WTS_INFO_CLASS
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
        private const Int32 WTS_ANY_SESSION	= -2;
        
        [DllImport("WtsApi32.dll", EntryPoint = "WTSQuerySessionInformationW", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, ref IntPtr ppBuffer, ref int pBytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSEnumerateProcessesExW(
            IntPtr hServer, // A handle to an RD Session Host server.. 
            ref IntPtr pLevel, // must be 1 - To return an array of WTS_PROCESS_INFO_EX structures, specify one.
            Int32 SessionID, // The session for which to enumerate processes. To enumerate processes for all sessions on the server, specify WTS_ANY_SESSION.
            ref IntPtr ppProcessInfo, // A pointer to a variable that receives a pointer to an array of WTS_PROCESS_INFO or WTS_PROCESS_INFO_EX structures. The type of structure is determined by the value passed to the pLevel parameter. Each structure in the array contains information about an active process. When you have finished using the array, free it by calling the WTSFreeMemoryEx function. You should also set the pointer to NULL.
            ref Int32 pCount); // pointer to number of processes -> A pointer to a variable that receives the number of structures returned in the buffer referenced by the ppProcessInfo parameter.

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSEnumerateProcesses(
            IntPtr serverHandle, // Handle to a terminal server. 
            Int32 reserved, // must be 0
            Int32 version, // must be 1
            ref IntPtr ppProcessInfo, // pointer to array of WTS_PROCESS_INFO
            ref Int32 pCount); // pointer to number of processes

        [DllImport("WtsApi32.dll", EntryPoint = "WTSQuerySessionInformationW", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern bool WTSQuerySessionInformation2(IntPtr hServer, int SessionId, WTS_INFO_CLASS WTSInfoClass, ref IntPtr ppBuffer, ref Int32 pCount);

        [DllImport("Kernel32.dll", EntryPoint = "GetCurrentProcessId", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 GetCurrentProcessId();

        [DllImport("Kernel32.dll", EntryPoint = "ProcessIdToSessionId", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern bool ProcessIdToSessionId(Int32 processID, ref Int32 sessionID);

        [DllImport("Kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
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

        [DllImport("wtsapi32.dll", BestFitMapping = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "WTSEnumerateSessions", SetLastError = true, ThrowOnUnmappableChar = true)]
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
    
        #endregion
    }
}