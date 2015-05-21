namespace Terminals.Connection.Native
{
    // .NET namespaces
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information that the SHFileOperation function uses to perform file operations.
    /// Note  As of Windows Vista, the use of the IFileOperation interface is recommended over this function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ShellFileOperation
    {
        /// <summary>
        /// Window handle to the dialog box to display information about the status of the file operation.
        /// </summary>
        public IntPtr hwnd;

        /// <summary>
        /// Value that indicates which operation to perform.
        /// </summary>
        public UInt32 wFunc;

        /// <summary>
        /// Address of a buffer to specify one or more source file names. 
        /// These names must be fully qualified paths. Standard Microsoft® 
        /// MS-DOS® wild cards, such as "*", are permitted in the file-name 
        /// position. Although this member is declared as a null-terminated 
        /// string, it is used as a buffer to hold multiple file names. Each 
        /// file name must be terminated by a single NULL character. An	
        /// additional NULL character must be appended to the end of the 
        /// final name to indicate the end of pFrom. 
        /// </summary>
        public IntPtr pFrom;

        /// <summary>
        /// Address of a buffer to contain the name of the destination file or
        /// directory. This parameter must be set to <C>NULL</C> if it is not used.
        /// Like <C>pFrom</C>, the pTo member is also a double-null terminated
        /// string and is handled in much the same way. 
        /// </summary>
        public IntPtr pTo;

        /// <summary>
        /// Flags that control the file operation.
        /// </summary>
        public UInt16 fFlags;

        /// <summary>
        /// Value that receives <c>TRUE</c> if the user aborted any file operations
        /// before they were completed, or FALSE otherwise. 
        /// </summary>
        public Int32 fAnyOperationsAborted;

        /// <summary>
        /// A handle to a name mapping object containing the old and new 
        /// names of the renamed files. This member is used only if the 
        /// <see cref="fFlags"/> member includes the <c>FOF_WANTMAPPINGHANDLE</c> flag.
        /// </summary>
        public IntPtr hNameMappings;

        /// <summary>
        /// Address of a string to use as the title of a progress dialog box.
        /// </summary>
        /// <remarks>
        /// This member is used only if fFlags includes the
        /// <c>FOF_SIMPLEPROGRESS</c> flag.
        /// </remarks>
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszProgressTitle;
    }
}