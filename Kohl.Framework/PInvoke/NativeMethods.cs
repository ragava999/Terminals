namespace Kohl.PInvoke
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr process, out bool wow64Process);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        public static extern bool GetVolumeInformation(string rootPathName, StringBuilder volumeNameBuffer, int volumeNameSize, out int volumeSerialNumber, out uint maximumComponentLength, out uint fileSystemFlags, StringBuilder fileSystemNameBuffer, int nFileSystemNameSize);

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
    }
}
