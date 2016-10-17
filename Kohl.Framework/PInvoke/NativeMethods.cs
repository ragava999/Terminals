namespace Kohl.PInvoke
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    public static class NativeMethods
    {
        #region Process and Module Details for Developer Tools
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);
        #endregion

        #region Machine Information
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        public static extern bool GetVolumeInformation(string rootPathName, StringBuilder volumeNameBuffer, int volumeNameSize, out int volumeSerialNumber, out uint maximumComponentLength, out uint fileSystemFlags, StringBuilder fileSystemNameBuffer, int nFileSystemNameSize);
        #endregion

        #region Impersonation
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int LogonUser(string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CloseHandle(IntPtr handle);
        #endregion

        #region Icon Handling, Drawing, Extracting
        [DllImport("Shell32", CharSet = CharSet.Unicode)]
        public extern static int ExtractIconEx(
        [MarshalAs(UnmanagedType.LPWStr)]
                string lpszFile,                //path to the binary
        int nIconIndex,                 //index of the icon (in case we have more then 1 icon in the file
        IntPtr[] phIconLarge,           //32x32 icon
        IntPtr[] phIconSmall,           //16x16 icon
        int nIcons);                    //how many to get

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            [MarshalAs(UnmanagedType.LPWStr)]
                string pszPath,             //path
            uint dwFileAttributes,          //attributes
            ref SHFILEINFO psfi,            //struct pointer
            uint cbSizeFileInfo,            //size
            uint uFlags);                   //flags

        // We need this function to release the unmanaged resources of the icon
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public extern static bool DestroyIcon(IntPtr handle);

        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        #endregion

        #region Drag & Drop File Copy Support
        [DllImport("Mpr.dll", EntryPoint = "WNetAddConnection2", CallingConvention = CallingConvention.Winapi)]
        public static extern int WNetAddConnection2(NETRESOURCE lpNetResource, string lpPassword, string lpUsername, System.UInt32 dwFlags);

        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CallingConvention = CallingConvention.Winapi)]
        public static extern int WNetCancelConnection2(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        public class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        };

        public enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        };

        public enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        };

        public enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        };

        public enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        };
        #endregion
    }
}
