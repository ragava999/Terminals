namespace Rug.Cmd
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleColorState
    {
        public ConsoleColorExt ForegroundColor;
        public ConsoleColorExt BackgroundColor;
        public ConsoleColorState(ConsoleColorExt ForegroundColor, ConsoleColorExt BackgroundColor)
        {
            this.ForegroundColor = ForegroundColor;
            this.BackgroundColor = BackgroundColor;
        }

        public ConsoleColorState Inverse
        {
            get
            {
                return new ConsoleColorState(this.BackgroundColor, this.ForegroundColor);
            }
        }
    }
}

