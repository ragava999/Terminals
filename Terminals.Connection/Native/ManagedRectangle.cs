namespace Terminals.Connection.Native
{
    // .NET namespaces
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ManagedRectangle
    {
        #region User32.dll (1)
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(IntPtr hwnd, out ManagedRectangle managedRectangle);
        #endregion

        #region Private Fields (4)
        private readonly int left;
        private readonly int top;
        private readonly int right;
        private readonly int bottom;
        #endregion

        #region Constructors (1)
        public ManagedRectangle(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        #endregion

        #region Private Methods (1)
        private Rectangle Rectangle
        {
            get { return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top); }
        }
        #endregion

        #region Public Methods (3)
        public static Rectangle GetRectangle(IntPtr hWnd)
        {
            ManagedRectangle managedRectangle = new ManagedRectangle();
            GetWindowRect(hWnd, out managedRectangle);
            return managedRectangle.Rectangle;
        }

        public static ManagedRectangle FromXYWH(int x, int y, int width, int height)
        {
            return new ManagedRectangle(x, y, x + width, y + height);
        }

        public static ManagedRectangle FromRectangle(Rectangle rect)
        {
            return new ManagedRectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
        #endregion
    }
}