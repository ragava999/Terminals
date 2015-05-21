/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 12:59
 */

using System;
using System.Runtime.InteropServices;

namespace Terminals.Network.Servers
{
    /// <summary>
    ///     Wrapper class for all Win32 API calls and structures
    /// </summary>
    internal class Win32API
    {
        [DllImport("netapi32.dll", EntryPoint = "NetApiBufferFree")]
        public static extern void NetApiBufferFree(IntPtr bufptr);

        [DllImport("netapi32.dll", EntryPoint = "NetServerEnum")]
        public static extern uint NetServerEnum(
            IntPtr ServerName,
            uint level,
            ref IntPtr siPtr,
            uint prefmaxlen,
            ref uint entriesread,
            ref uint totalentries,
            uint servertype,
            [MarshalAs(UnmanagedType.LPWStr)] string domain,
            IntPtr resumeHandle);

        /// <summary>
        ///     Windows NT/2000/XP Only
        /// </summary>
        [DllImport("netapi32.dll", EntryPoint = "NetServerGetInfo")]
        public static extern uint NetServerGetInfo(
            [MarshalAs(UnmanagedType.LPWStr)] string ServerName,
            int level,
            ref IntPtr buffPtr);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SERVER_INFO_101
        {
            public int dwPlatformID;
            public IntPtr lpszServerName;
            public int dwVersionMajor;
            public int dwVersionMinor;
            public int dwType;
            public IntPtr lpszComment;
        }
    }
}