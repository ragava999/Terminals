namespace Terminals.Connections
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Connection.Native;

    public class ShellFileOperation
    {
        [Flags]
        private enum ShellFileOperationFlags
        {
            /// <summary>
            /// The pTo member specifies multiple destination files (one for 
            /// each source file) rather than one directory where all source 
            /// files are to be deposited. 
            /// </summary>
            Multidestfiles = 0x0001,

            /// <summary>
            /// Not currently used. 
            /// </summary>
            Confirmmouse = 0x0002, 

            /// <summary>
            /// Do not display a progress dialog box. 
            /// </summary>
            Silent = 0x0004,

            /// <summary>
            /// Give the file being operated on a new name in a move, copy, or 
            /// rename operation if a file with the target name already exists. 
            /// </summary>
            Renameoncollision = 0x0008,

            /// <summary>
            /// Respond with "Yes to All" for any dialog box that is displayed. 
            /// </summary>
            Noconfirmation = 0x0010,

            /// <summary>
            /// If RENAMEONCOLLISION is specified and any files were renamed,
            /// assign a name mapping object containing their old and new names
            /// to the hNameMappings member.
            /// </summary>
            Wantmappinghandle = 0x0020,

            /// <summary>
            /// Perform the operation on files only if a wildcard file 
            /// name (*.*) is specified.
            /// </summary>
            Allowundo = 0x0040,

            /// <summary>
            /// Display a progress dialog box but do not show the file names. 
            /// </summary>
            Filesonly = 0x0080,

            /// <summary>
            /// Do not confirm the creation of a new directory if the operation
            /// </summary>
            Simpleprogress = 0x0100,

            /// <summary>
            /// Requires one to be created. 
            /// </summary>
            Noconfirmmkdir = 0x0200,

            /// <summary>
            /// Do not display a user interface if an error occurs.
            /// </summary>
            Noerrorui = 0x0400,

            /// <summary>
            /// Do not copy the security attributes of the file.
            /// </summary>
            Nocopysecurityattribs = 0x0800,

            /// <summary>
            /// Only operate in the local directory. Don't operate recursively
            /// into subdirectories.
            /// </summary>
            Norecursion = 0x1000,

            /// <summary>
            /// Do not move connected files as a group. Only move the 
            /// specified files. 
            /// </summary>
            NoConnectedElements = 0x2000,

            /// <summary>
            /// Send a warning if a file is being destroyed during a delete 
            /// operation rather than recycled. This flag partially 
            /// overrides NOCONFIRMATION.
            /// </summary>
            Wantnukewarning = 0x4000,

            /// <summary>
            /// Treat reparse points as objects, not containers.
            /// </summary>
            Norecursereparse = 0x8000
        }

        private readonly ShellFileOperationFlags operationFlags;
        private readonly String progressTitle;

        public ShellFileOperation()
        {
            // set default properties
            this.operationFlags = ShellFileOperationFlags.Allowundo
                                  | ShellFileOperationFlags.Multidestfiles
                                  | ShellFileOperationFlags.NoConnectedElements
                                  | ShellFileOperationFlags.Wantnukewarning;

            this.progressTitle = "";
        }

        public bool InvokeOperation(IntPtr? ownerWindow = null, FileOperations operation = FileOperations.Copy, String[] sourceFiles = null, String[] destinationFiles = null)
        {
            if (!ownerWindow.HasValue || ownerWindow.Value == null)
                ownerWindow = IntPtr.Zero;

            Connection.Native.ShellFileOperation shellFileOperation = new Connection.Native.ShellFileOperation { hwnd = ownerWindow.Value, wFunc = (uint)operation };

            string multiSource = this.StringArrayToMultiString(sourceFiles);
            string multiDest = this.StringArrayToMultiString(destinationFiles);
            shellFileOperation.pFrom = Marshal.StringToHGlobalUni(multiSource);
            shellFileOperation.pTo = Marshal.StringToHGlobalUni(multiDest);

            shellFileOperation.fFlags = (ushort) this.operationFlags;
            shellFileOperation.lpszProgressTitle = this.progressTitle;
            shellFileOperation.fAnyOperationsAborted = 0;
            shellFileOperation.hNameMappings = IntPtr.Zero;

            int retVal = WindowsApi.SHFileOperation(ref shellFileOperation);

            WindowsApi.SHChangeNotify(/* ShellChangeNotificationEvents.SHCNE_ALLEVENTS = */ 0x7FFFFFFF /* All events have occurred. */,/* ShellChangeNotificationFlags.SHCNF_DWORD = */ 0x0003 /*The dwItem1 and dwItem2 parameters are DWORD values.  */,IntPtr.Zero,IntPtr.Zero);

            if (retVal != 0)
                return false;

            if (shellFileOperation.fAnyOperationsAborted != 0)
                return false;

            return true;
        }

        private string StringArrayToMultiString(string[] stringArray)
        {
            if (stringArray == null)
                return "";

            string multiString = stringArray.Aggregate("", (current, t) => current + (t + '\0'));

            multiString += '\0';

            return multiString;
        }
    }
}