/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 13:02
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Terminals.Network.Servers
{
    /// <summary>
    ///     Enumerates over a set of servers returning the server's name.
    /// </summary>
    public class ServerEnumerator : IEnumerator
    {
        /// <summary>
        ///     Save the size of the SERVER_INFO_101 structure.
        ///     This allows us to only have a single time we need
        ///     to use 'unsafe' code.
        /// </summary>
        private static readonly int SERVER_INFO_101_SIZE;

        /// <summary>
        ///     The current item number
        /// </summary>
        private int currentItem;

        /// <summary>
        ///     The name of the machine returned by Current
        /// </summary>
        private string currentServerName;

        /// <summary>
        ///     Number of items in the collection
        /// </summary>
        private readonly uint itemCount;

        /// <summary>
        ///     Memory buffer pointer returned by NetServerEnum
        /// </summary>
        private IntPtr serverInfoPtr;

        static ServerEnumerator()
        {
            SERVER_INFO_101_SIZE = Marshal.SizeOf(typeof (Win32API.SERVER_INFO_101));
        }

        /// <summary>
        /// </summary>
        /// <param name="serverType"> </param>
        /// <param name="domainName"> </param>
        public ServerEnumerator(ServerType serverType, string domainName = null)
        {
            const uint level = 101;
            const uint prefmaxlen = 0xFFFFFFFF;
            uint entriesread = 0, totalentries = 0;

            this.Reset();
            this.serverInfoPtr = IntPtr.Zero;

            uint nRes = Win32API.NetServerEnum(
                IntPtr.Zero, // Server Name: Reserved; must be NULL. 
                level,
                // Return server names, types, and associated software. The bufptr parameter points to an array of SERVER_INFO_101 structures.
                ref this.serverInfoPtr, // Pointer to the buffer that receives the data.
                prefmaxlen, // Specifies the preferred maximum length of returned data, in bytes.
                ref entriesread, // count of elements actually enumerated.
                ref totalentries, // total number of visible servers and workstations on the network
                (uint) serverType, // value that filters the server entries to return from the enumeration
                domainName,
                // Pointer to a constant string that specifies the name of the domain for which a list of servers is to be returned.
                IntPtr.Zero); // Reserved; must be set to zero. 

            this.itemCount = entriesread;
        }

        /// <summary>
        ///     Returns the current server/machine/domain name
        /// </summary>
        public object Current
        {
            get { return this.currentServerName; }
        }

        /// <summary>
        ///     Moves to the next server/machine/domain
        /// </summary>
        /// <returns> </returns>
        public bool MoveNext()
        {
            bool result = false;

            if (++this.currentItem < this.itemCount)
            {
                int newOffset = this.serverInfoPtr.ToInt32() + SERVER_INFO_101_SIZE*this.currentItem;
                Win32API.SERVER_INFO_101 si =
                    (Win32API.SERVER_INFO_101)
                    Marshal.PtrToStructure(new IntPtr(newOffset), typeof (Win32API.SERVER_INFO_101));
                this.currentServerName = Marshal.PtrToStringAuto(si.lpszServerName);
                result = true;
            }
            return result;
        }

        /// <summary>
        ///     Resets the enumeration back to the beginning.
        /// </summary>
        public void Reset()
        {
            this.currentItem = -1;
            this.currentServerName = null;
        }

        /// <summary>
        /// </summary>
        ~ServerEnumerator()
        {
            if (! this.serverInfoPtr.Equals(IntPtr.Zero))
            {
                Win32API.NetApiBufferFree(this.serverInfoPtr);
                this.serverInfoPtr = IntPtr.Zero;
            }
        }
    }
}