namespace Kohl.PInvoke
{
    using System.Runtime.InteropServices;

    internal static class UnixNativeMethods
    {
        [DllImport("libc")]
        public static extern uint getuid();
    }
}