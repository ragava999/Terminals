namespace Rug.Cmd.Gui
{
    using Rug.Cmd;
    using System;
    using System.Drawing;

    public class ConsoleProgressBar
    {
        public ConsoleColorExt BackColor = ConsoleColorExt.Inhreit;
        public ConsoleColorExt BarDimBackColor = ConsoleColorExt.Inhreit;
        public ConsoleColorExt BarDimForeColor = ConsoleColorExt.Inhreit;
        public ConsoleShade BarDimShade = ConsoleShade.Dim;
        public ConsoleColorExt BarLitBackColor = ConsoleColorExt.Inhreit;
        public ConsoleColorExt BarLitForeColor = ConsoleColorExt.Inhreit;
        public ConsoleShade BarLitShade;
        public ConsoleProgressBarCaps Caps = ConsoleProgressBarCaps.BlocksJoined;
        public string CustomFormat = "{0}";
        public ConsoleColorExt ForeColor = ConsoleColorExt.Inhreit;
        private Point m_Location = new Point(0, 0);
        public int Maximum = 1;
        public string Message = "";
        public ConsoleProgressBarTextAlignment TextAlignment = ConsoleProgressBarTextAlignment.Right;
        public ConsoleProgressBarTextFormat TextFormat = ConsoleProgressBarTextFormat.Percent;
        public int TextPadding;
        public int Value;
        public int Width = 80;

        public void Render()
        {
            if (this.Maximum >= 1)
            {
                string str = null;
                if (this.TextAlignment != ConsoleProgressBarTextAlignment.None)
                {
                    string str2 = "";
                    if (this.TextPadding > 0)
                    {
                        str2 = new string(' ', this.TextPadding);
                    }
                    switch (this.TextFormat)
                    {
                        case ConsoleProgressBarTextFormat.Value:
                            str2 = str2 + this.Value.ToString().PadLeft(this.Maximum.ToString().Length, ' ');
                            break;

                        case ConsoleProgressBarTextFormat.ValueOfMax:
                        {
                            string introduced18 = this.Value.ToString().PadLeft(this.Maximum.ToString().Length, ' ');
                            str2 = str2 + string.Format(Strings.ConsoleProgressBar_ValueOfMax, introduced18, this.Maximum.ToString());
                            break;
                        }
                        case ConsoleProgressBarTextFormat.Percent:
                        {
                            double num = (100.0 / ((double) this.Maximum)) * this.Value;
                            str2 = str2 + num.ToString("N1").PadLeft(5, ' ') + "%";
                            break;
                        }
                        case ConsoleProgressBarTextFormat.Decimal:
                        {
                            double num2 = (1.0 / ((double) this.Maximum)) * this.Value;
                            str2 = str2 + num2.ToString("N3").PadLeft(this.Maximum.ToString("N3").Length, ' ');
                            break;
                        }
                    }
                    str = string.Format(this.CustomFormat, str2);
                }
                int num3 = (str != null) ? str.Length : 0;
                int length = this.Width - (2 + num3);
                double num5 = length;
                double num6 = (num5 / ((double) this.Maximum)) * this.Value;
                int num7 = (int) num6;
                if (num7 > length)
                {
                    num7 = length;
                }
                ConsoleColorState colorState = RugConsole.ColorState;
                RugConsole.SetCursorPosition(this.Location.X, this.Location.Y);
                RugConsole.ForegroundColor = this.ForeColor;
                RugConsole.BackgroundColor = this.BackColor;
                this.WriteEndCap(true, this.Caps);
                string str3 = (this.Message.Length > length) ? this.Message.Substring(0, length) : this.Message;
                if (num7 > 0)
                {
                    RugConsole.ForegroundColor = this.BarLitForeColor;
                    RugConsole.BackgroundColor = this.BarLitBackColor;
                    if (str3.Length < num7)
                    {
                        RugConsole.Write(str3);
                        RugConsole.Write(new string(ConsoleChars.GetShade(this.BarLitShade), num7 - str3.Length));
                    }
                    else if (str3.Length == num7)
                    {
                        RugConsole.Write(str3);
                    }
                    else if (str3.Length > num7)
                    {
                        RugConsole.Write(str3.Substring(0, num7));
                    }
                    if (num7 < length)
                    {
                        RugConsole.ForegroundColor = this.BarDimForeColor;
                        RugConsole.BackgroundColor = this.BarDimBackColor;
                        int count = length - num7;
                        if (str3.Length <= num7)
                        {
                            RugConsole.Write(new string(ConsoleChars.GetShade(this.BarDimShade), count));
                        }
                        else if (str3.Length > num7)
                        {
                            string str4 = str3.Substring(num7);
                            RugConsole.Write(str4);
                            RugConsole.Write(new string(ConsoleChars.GetShade(this.BarDimShade), count - str4.Length));
                        }
                    }
                }
                else
                {
                    RugConsole.ForegroundColor = this.BarDimForeColor;
                    RugConsole.BackgroundColor = this.BarDimBackColor;
                    RugConsole.Write(str3);
                    RugConsole.Write(new string(ConsoleChars.GetShade(this.BarDimShade), length - str3.Length));
                }
                RugConsole.ForegroundColor = this.ForeColor;
                RugConsole.BackgroundColor = this.BackColor;
                this.WriteEndCap(false, this.Caps);
                if (this.TextAlignment != ConsoleProgressBarTextAlignment.None)
                {
                    RugConsole.SetCursorPosition(this.Location.X + (this.Width - num3), this.Location.Y);
                    RugConsole.ForegroundColor = this.ForeColor;
                    RugConsole.BackgroundColor = this.BackColor;
                    RugConsole.Write(str);
                }
                RugConsole.ColorState = colorState;
            }
        }

        private void WriteEndCap(bool leftSide, ConsoleProgressBarCaps caps)
        {
            if (leftSide)
            {
                switch (this.Caps)
                {
                    case ConsoleProgressBarCaps.Brackets:
                        RugConsole.Write("(");
                        return;

                    case ConsoleProgressBarCaps.GreaterThanLessThan:
                        RugConsole.Write("<");
                        return;

                    case ConsoleProgressBarCaps.SqrBrackets:
                        RugConsole.Write("[");
                        return;

                    case ConsoleProgressBarCaps.Arrows:
                        RugConsole.Write(ConsoleChars.GetArrow(ConsoleArrows.Left).ToString());
                        return;

                    case ConsoleProgressBarCaps.BlocksSeperated:
                        RugConsole.Write(ConsoleChars.GetShade(ConsoleShade.HalfLeft).ToString());
                        return;

                    case ConsoleProgressBarCaps.BlocksJoined:
                        RugConsole.Write(ConsoleChars.GetShade(ConsoleShade.HalfRight).ToString());
                        return;

                    case ConsoleProgressBarCaps.Blocks:
                        RugConsole.Write(ConsoleChars.GetShade(ConsoleShade.Opaque).ToString());
                        return;

                    case ConsoleProgressBarCaps.Clear:
                        RugConsole.Write(" ");
                        return;
                }
            }
            else
            {
                switch (this.Caps)
                {
                    case ConsoleProgressBarCaps.Brackets:
                        RugConsole.Write(")");
                        return;

                    case ConsoleProgressBarCaps.GreaterThanLessThan:
                        RugConsole.Write(">");
                        return;

                    case ConsoleProgressBarCaps.SqrBrackets:
                        RugConsole.Write("]");
                        return;

                    case ConsoleProgressBarCaps.Arrows:
                        RugConsole.Write(ConsoleChars.GetArrow(ConsoleArrows.Right).ToString());
                        return;

                    case ConsoleProgressBarCaps.BlocksSeperated:
                        RugConsole.Write(ConsoleChars.GetShade(ConsoleShade.HalfRight).ToString());
                        return;

                    case ConsoleProgressBarCaps.BlocksJoined:
                        RugConsole.Write(ConsoleChars.GetShade(ConsoleShade.HalfLeft).ToString());
                        return;

                    case ConsoleProgressBarCaps.Blocks:
                        RugConsole.Write(ConsoleChars.GetShade(ConsoleShade.Opaque).ToString());
                        return;

                    case ConsoleProgressBarCaps.Clear:
                        RugConsole.Write(" ");
                        return;
                }
            }
        }

        public Point Location
        {
            get
            {
                return this.m_Location;
            }
            set
            {
                this.m_Location = value;
                if ((this.m_Location.X < 0) || (this.m_Location.Y < 0))
                {
                    throw new Exception(Strings.ConsoleControl_CtrlOutOfBounds);
                }
            }
        }
    }
}

