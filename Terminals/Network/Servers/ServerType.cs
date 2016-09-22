/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 13:01
 */
using System;

namespace Terminals.Network.Servers
{
    /// <summary>
    ///     The possible flag values for Server Type (see lmserver.h).
    /// </summary>
    [Flags]
    public enum ServerType : long
    {
        /// <summary>
        ///     Opposite of All.  No servers will be returned.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        ///     All workstations
        /// </summary>
        Workstation = 0x00000001,

        /// <summary>
        ///     All servers
        /// </summary>
        Server = 0x00000002,

        /// <summary>
        ///     Any server running with Microsoft SQL Server
        /// </summary>
        SQLServer = 0x00000004,

        /// <summary>
        ///     Primary domain controller
        /// </summary>
        DomainController = 0x00000008,

        /// <summary>
        ///     Backup domain controller
        /// </summary>
        DomainBackupController = 0x00000010,

        /// <summary>
        ///     Server running the Timesource service
        /// </summary>
        TimeSource = 0x00000020,

        /// <summary>
        ///     Apple File Protocol servers
        /// </summary>
        AFP = 0x00000040,

        /// <summary>
        ///     Novell servers
        /// </summary>
        Novell = 0x00000080,

        /// <summary>
        ///     LAN Manager 2.x domain member
        /// </summary>
        DomainMember = 0x00000100,

        /// <summary>
        ///     Server sharing print queue
        /// </summary>
        PrintQueue = 0x00000200,

        /// <summary>
        ///     Server running dial-in service
        /// </summary>
        Dialin = 0x00000400,

        /// <summary>
        ///     Xenix server
        /// </summary>
        Xenix = 0x00000800,

        /// <summary>
        ///     Unix servers?
        /// </summary>
        Unix = Xenix,

        /// <summary>
        ///     Windows NT workstation or server
        /// </summary>
        NT = 0x00001000,

        /// <summary>
        ///     Server running Windows for Workgroups
        /// </summary>
        WFW = 0x00002000,

        /// <summary>
        ///     Microsoft File and Print for NetWare
        /// </summary>
        MFPN = 0x00004000,

        /// <summary>
        ///     Server that is not a domain controller
        /// </summary>
        NTServer = 0x00008000,

        /// <summary>
        ///     Server that can run the browser service
        /// </summary>
        PotentialBrowser = 0x00010000,

        /// <summary>
        ///     Server running a browser service as backup
        /// </summary>
        BackupBrowser = 0x00020000,

        /// <summary>
        ///     Server running the master browser service
        /// </summary>
        MasterBrowser = 0x00040000,

        /// <summary>
        ///     Server running the domain master browser
        /// </summary>
        DomainMaster = 0x00080000,

        /// <summary>
        ///     Not documented on MSDN? Help Microsoft!
        /// </summary>
        OSF = 0x00100000,

        /// <summary>
        ///     Running VMS
        /// </summary>
        VMS = 0x00200000,

        /// <summary>
        ///     Windows 95 or later
        /// </summary>
        Windows = 0x00400000,

        /// <summary>
        ///     Distributed File System??
        /// </summary>
        DFS = 0x00800000,

        /// <summary>
        ///     Not documented on MSDN? Help Microsoft!
        /// </summary>
        ClusterNT = 0x01000000,

        /// <summary>
        ///     Terminal Server
        /// </summary>
        TerminalServer = 0x02000000,

        /// <summary>
        ///     Not documented on MSDN? Help Microsoft!
        /// </summary>
        DCE = 0x10000000,

        /// <summary>
        ///     Not documented on MSDN? Help Microsoft!
        /// </summary>
        AlternateXPort = 0x20000000,

        /// <summary>
        ///     Servers maintained by the browser
        /// </summary>
        ListOnly = 0x40000000,

        /// <summary>
        ///     List Domains
        /// </summary>
        DomainEnum = 0x80000000,

        /// <summary>
        ///     All servers
        /// </summary>
        All = 0xFFFFFFFF
    }
}