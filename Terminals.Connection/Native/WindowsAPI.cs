namespace Terminals.Connection.Native
{
    // .NET namespaces
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Contains unmanged Windows API method calls.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    [ComVisible(false)]
    public static class WindowsApi
    {
        #region User32.dll
        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            return ManagedRectangle.GetRectangle(hWnd);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ShowWindow(HandleRef hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(HandleRef hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        #endregion

        #region Gdi32.dll
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTextMetrics(IntPtr hdc, out TextMetric lptm);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hdc);
        #endregion

        #region UxTheme.dll
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int SetWindowTheme(IntPtr hWnd, String appName, String partList);
        #endregion

        #region Shell32.dll
        // Copies, moves, renames, or deletes a file system object. 
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        // Address of an SHFILEOPSTRUCT structure that contains information 
        // this function needs to carry out the specified operation. This 
        // parameter must contain a valid value that is not NULL. You are 
        // responsibile for validating the value. If you do not validate it, 
        // you will experience unexpected results. 
        public static extern Int32 SHFileOperation(ref ShellFileOperation lpFileOp);

        // Notifies the system of an event that an application has performed. An application should use this function
        // if it performs an action that may affect the Shell. 
        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(
            UInt32 wEventId, // Describes the event that has occurred.
            UInt32 uFlags, // Flags that indicate the meaning of the dwItem1 and dwItem2 parameters.
            IntPtr dwItem1, // First event-dependent value. 
            IntPtr dwItem2); // Second event-dependent value. 
        #endregion
    }
}