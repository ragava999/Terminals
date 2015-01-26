namespace Terminals.TerminalServices
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Kohl.Framework.Logging;

    public class TSManager
    {
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
            WTSClientProtocolType
        }

        [DllImport("wtsapi32.dll")]
        private static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] String pServerName);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll")]
        private static extern Int32 WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] Int32 Reserved,
            [MarshalAs(UnmanagedType.U4)] Int32 Version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref Int32 pCount);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(
            IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out uint pBytesReturned);

        private static IntPtr OpenServer(String name)
        {
            IntPtr server = WTSOpenServer(name);
            return server;
        }

        private static void CloseServer(IntPtr serverHandle)
        {
            WTSCloseServer(serverHandle);
        }

        private static string QuerySessionInfo(IntPtr server, int sessionId, WTS_INFO_CLASS infoClass)
        {
            IntPtr buffer = IntPtr.Zero;
            try
            {
                uint bytesReturned;
                WTSQuerySessionInformation(server, sessionId, infoClass, out buffer, out bytesReturned);
                return Marshal.PtrToStringAnsi(buffer);
            }
            catch (Exception exc)
            {
                Log.Info("", exc);
                return String.Empty;
            }
            finally
            {
                WTSFreeMemory(buffer);
                buffer = IntPtr.Zero;
            }
        }
        
        public static List<SessionInfo> ListSessions(string serverName, string userName, string domainName, string clientName, WTS_CONNECTSTATE_CLASS? state)
        {
            IntPtr server = IntPtr.Zero;
            List<SessionInfo> sessions = new List<SessionInfo>();
            server = OpenServer(serverName);
            try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof (WTS_SESSION_INFO));
                Int32 current = (int) ppSessionInfo;
                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        SessionInfo sessionInfo = new SessionInfo();
                        WTS_SESSION_INFO si =
                            (WTS_SESSION_INFO) Marshal.PtrToStructure((IntPtr) current, typeof (WTS_SESSION_INFO));
                        current += dataSize;

                        sessionInfo.Id = si.SessionID;
                        sessionInfo.UserName = QuerySessionInfo(server, sessionInfo.Id, WTS_INFO_CLASS.WTSUserName);
                        sessionInfo.DomainName = QuerySessionInfo(server, sessionInfo.Id, WTS_INFO_CLASS.WTSDomainName);
                        sessionInfo.ClientName = QuerySessionInfo(server, sessionInfo.Id, WTS_INFO_CLASS.WTSClientName);
                        sessionInfo.State = si.State;

                        if (userName != null || domainName != null || clientName != null || state != null)
                            //In this case, the caller is asking to return only matching sessions
                        {
                            if (userName != null &&
                                !String.Equals(userName, sessionInfo.UserName, StringComparison.CurrentCultureIgnoreCase))
                                continue; //Not matching
                            if (clientName != null &&
                                !String.Equals(clientName, sessionInfo.ClientName,
                                               StringComparison.CurrentCultureIgnoreCase))
                                continue; //Not matching
                            if (domainName != null &&
                                !String.Equals(domainName, sessionInfo.DomainName,
                                               StringComparison.CurrentCultureIgnoreCase))
                                continue; //Not matching
                            if (state != null && sessionInfo.State != state.Value)
                                continue;
                        }

                        sessions.Add(sessionInfo);
                    }
                    WTSFreeMemory(ppSessionInfo);
                }
            }
            finally
            {
                CloseServer(server);
            }
            return sessions;
        }

        public static SessionInfo GetCurrentSession(string serverName, string userName, string domainName, string clientName)
        {
            List<SessionInfo> sessions = ListSessions(serverName, userName, domainName, clientName, WTS_CONNECTSTATE_CLASS.WTSActive);

            if (sessions.Count == 0)
                return null;

            if (sessions.Count > 1)
                throw new Exception("Duplicate sessions found for user");

            return sessions[0];
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_SESSION_INFO
        {
            public readonly Int32 SessionID;

            [MarshalAs(UnmanagedType.LPStr)] private readonly String pWinStationName;

            public readonly WTS_CONNECTSTATE_CLASS State;
        }
    }
}