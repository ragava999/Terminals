/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 13:06
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Terminals.Network.Servers
{
    /// <summary>
    ///     Class that encapsulates the Win32 API call of NetServerEnum
    /// </summary>
    public class Servers : IEnumerable
    {
        /// <summary>
        /// </summary>
        public Servers()
        {
            this.Type = ServerType.None;
        }

        /// <summary>
        ///     Specifies a value that filters the server entries to return from the enumeration
        /// </summary>
        /// <param name="aServerType"> </param>
        public Servers(ServerType aServerType)
        {
            this.Type = aServerType;
        }

        /// <summary>
        ///     Gets or sets the server type.
        /// </summary>
        public ServerType Type { get; set; }

        /// <summary>
        ///     Gets or sets the domain name.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// </summary>
        /// <returns> IEnumerator </returns>
        public IEnumerator GetEnumerator()
        {
            return new ServerEnumerator(this.Type, this.DomainName);
        }

        /// <summary>
        ///     Returns the server type of the named server.
        /// </summary>
        /// <param name="serverName"> </param>
        /// <returns> </returns>
        public static ServerType GetServerType(string serverName)
        {
            ServerType result = ServerType.None;

            IntPtr serverInfoPtr = IntPtr.Zero;
            uint rc = Win32API.NetServerGetInfo(serverName, 101, ref serverInfoPtr);

            if (rc != 0 && serverInfoPtr != IntPtr.Zero)
            {
                Win32API.SERVER_INFO_101 si =
                    (Win32API.SERVER_INFO_101) Marshal.PtrToStructure(serverInfoPtr, typeof (Win32API.SERVER_INFO_101));
                result = (ServerType) si.dwType;

                Win32API.NetApiBufferFree(serverInfoPtr);
                serverInfoPtr = IntPtr.Zero;
            }

            return result;
        }
    }
}