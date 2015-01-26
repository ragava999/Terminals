namespace Rug.Cmd
{
    using System;

    public static class CmdHelper
    {
        private static string[] RuglandLogo = new string[] { "{0}{0}{1}{0}{2}{1}{0}{0}{2}", "{0}{2}{2}{0}{2}{1}{0}{2}{0}", "{0}{2}{2}{0}{0}{1}{0}{0}{0}" };

        public static string GetMemStringFromBytes(long bytes, bool space)
        {
            decimal num = bytes / 1024M;
            string str = "KB";
            while (num >= 1000M)
            {
                if (str == "KB")
                {
                    num /= 1024M;
                    str = "MB";
                }
                else
                {
                    if (str == "MB")
                    {
                        num /= 1024M;
                        str = "GB";
                        continue;
                    }
                    if (str == "GB")
                    {
                        num /= 1024M;
                        str = "TB";
                        continue;
                    }
                    if (str == "TB")
                    {
                        num /= 1024M;
                        str = "PB";
                        continue;
                    }
                    if (str == "PB")
                    {
                        num /= 1024M;
                        str = "XB";
                        continue;
                    }
                    if (str == "XB")
                    {
                        num /= 1024M;
                        str = "ZB";
                        continue;
                    }
                    if (str == "ZB")
                    {
                        num /= 1024M;
                        str = "YB";
                        continue;
                    }
                    if (str == "YB")
                    {
                        num /= 1024M;
                        str = "??";
                        continue;
                    }
                    num /= 1024M;
                }
            }
            return (num.ToString("N2") + (space ? (" " + str) : str));
        }

        public static string GetMemStringFromBytes(long bytes, int maxLength)
        {
            return GetMemStringFromBytes(bytes, false).PadLeft(maxLength, ' ');
        }

        public static string GetMemStringFromBytes(long bytes, int maxLength, bool space)
        {
            return GetMemStringFromBytes(bytes, space).PadLeft(maxLength, ' ');
        }

        public static string MaxLengthLeftPadded(string str, int totalWidth, char paddingChar, string appendIfCut)
        {
            if (str.Length > totalWidth)
            {
                return (str.Substring(0, totalWidth - appendIfCut.Length) + appendIfCut);
            }
            return str.PadLeft(totalWidth, paddingChar);
        }

        public static string MaxLengthPadded(string str, int totalWidth, char paddingChar, string appendIfCut)
        {
            if (str.Length > totalWidth)
            {
                return (str.Substring(0, totalWidth - appendIfCut.Length) + appendIfCut);
            }
            return str.PadRight(totalWidth, paddingChar);
        }

        public static void WriteInfoToConsole(string label, string value, ConsoleColorExt valueColor)
        {
            RugConsole.Write(ConsoleThemeColor.TitleText, " " + label.PadRight(0x12) + ":");
            RugConsole.Write(ConsoleThemeColor.SubText, new string('.', 0x16 - value.Length));
            RugConsole.WriteLine(valueColor, value);
        }

        public static void WriteInfoToConsole(ConsoleVerbosity verbose, string label, string value, ConsoleColorExt valueColor)
        {
            if (RugConsole.Verbosity >= verbose)
            {
                WriteInfoToConsole(label, value, valueColor, false);
            }
        }

        public static void WriteInfoToConsole(string label, string value, ConsoleColorExt valueColor, bool extended)
        {
            RugConsole.Write(ConsoleThemeColor.TitleText, " " + label.PadRight(0x12) + ":");
            if (extended)
            {
                RugConsole.WriteLine(valueColor, value);
            }
            else if (value.Length < 0x16)
            {
                RugConsole.Write(ConsoleThemeColor.SubText, new string('.', 0x16 - value.Length));
                RugConsole.WriteLine(valueColor, value);
            }
            else
            {
                RugConsole.WriteLine(valueColor, value.Substring(value.Length - 0x16));
            }
        }

        public static void WriteInfoToConsole(ConsoleVerbosity verbose, string label, string value, ConsoleColorExt valueColor, bool extended)
        {
            if (RugConsole.Verbosity >= verbose)
            {
                WriteInfoToConsole(label, value, valueColor, extended);
            }
        }

        public static void WriteLogo(int x, int y, int pixWidth, int pixHeight, ConsoleShade fillShade, ConsoleShade endShade, ConsoleShade shadowShade, string[] lines)
        {
            if (RugConsole.CanManipulateBuffer)
            {
                ConsoleColorState colorState = RugConsole.ColorState;
                int cursorLeft = RugConsole.CursorLeft;
                int cursorTop = RugConsole.CursorTop;
                string str = new string(ConsoleChars.GetShade(fillShade), pixWidth);
                string str2 = "";
                if (pixWidth > 1)
                {
                    str2 = new string(ConsoleChars.GetShade(fillShade), pixWidth - 1) + ConsoleChars.GetShade(endShade);
                }
                else
                {
                    str2 = new string(ConsoleChars.GetShade(endShade), 1);
                }
                string str3 = new string(ConsoleChars.GetShade(ConsoleShade.Clear), pixWidth);
                string str4 = new string(ConsoleChars.GetShade(shadowShade), pixWidth);
                string str5 = "";
                if (pixWidth > 1)
                {
                    str5 = new string(ConsoleChars.GetShade(shadowShade), 1) + new string(ConsoleChars.GetShade(ConsoleShade.Clear), pixWidth - 1);
                }
                else
                {
                    str5 = new string(ConsoleChars.GetShade(shadowShade), 1);
                }
                int num3 = y;
                RugConsole.CursorLeft = x;
                RugConsole.CursorTop = num3++;
                RugConsole.ForegroundThemeColor = ConsoleThemeColor.TitleText;
                for (int i = 0; i < lines.Length; i++)
                {
                    for (int j = 0; j < pixHeight; j++)
                    {
                        RugConsole.Write(string.Format(lines[i], new object[] { str, str2, str3, str4, str5 }));
                        RugConsole.CursorLeft = x;
                        RugConsole.CursorTop = num3++;
                    }
                }
                RugConsole.ColorState = colorState;
                RugConsole.CursorTop = cursorTop;
                RugConsole.CursorLeft = cursorLeft;
            }
        }

        public static void WriteLogo(int x, int y, int pixWidth, int pixHeight, ConsoleShade fillShade, ConsoleShade endShade, ConsoleShade shadowShade, bool makeSpace, bool replaceCursor, string[] lines)
        {
            if (RugConsole.CanManipulateBuffer)
            {
                int cursorLeft = RugConsole.CursorLeft;
                int cursorTop = RugConsole.CursorTop;
                if (makeSpace)
                {
                    RugConsole.CursorLeft = x;
                    RugConsole.CursorTop = y;
                    int num3 = 0;
                    int num4 = pixHeight * lines.Length;
                    while (num3 < num4)
                    {
                        RugConsole.WriteLine();
                        num3++;
                    }
                    if (!replaceCursor)
                    {
                        cursorLeft = RugConsole.CursorLeft;
                        cursorTop = RugConsole.CursorTop;
                    }
                }
                WriteLogo(x, y, pixWidth, pixHeight, fillShade, endShade, shadowShade, lines);
                RugConsole.CursorTop = cursorTop;
                RugConsole.CursorLeft = cursorLeft;
            }
        }

        public static void WriteRuglandLogo(int x, int y, int pixWidth, int pixHeight, bool divideChars, ConsoleShade fillShade)
        {
            WriteLogo(x, y, pixWidth, pixHeight, fillShade, divideChars ? ConsoleShade.HalfLeft : ConsoleShade.Opaque, ConsoleShade.Dark, RuglandLogo);
        }

        public static void WriteRuglandLogo(int x, int y, int pixWidth, int pixHeight, bool divideChars, ConsoleShade fillShade, bool makeSpace, bool replaceCursor)
        {
            WriteLogo(x, y, pixWidth, pixHeight, fillShade, divideChars ? ConsoleShade.HalfLeft : ConsoleShade.Opaque, ConsoleShade.Dark, makeSpace, replaceCursor, RuglandLogo);
        }

        public static void WriteSimpleBanner(string str, char fill, ConsoleColorExt ForeColour, ConsoleColorExt BackColour)
        {
            WriteSimpleBanner(str, fill, 0, ForeColour, BackColour);
        }

        public static void WriteSimpleBanner(string str, char fill, int paddingRight, ConsoleColorExt ForeColour, ConsoleColorExt BackColour)
        {
            ConsoleColorState colorState = RugConsole.ColorState;
            int num = RugConsole.BufferWidth - paddingRight;
            int count = (num - str.Length) / 2;
            if (ForeColour != ConsoleColorExt.Inhreit)
            {
                RugConsole.ForegroundColor = ForeColour;
            }
            if (BackColour != ConsoleColorExt.Inhreit)
            {
                RugConsole.BackgroundColor = BackColour;
            }
            if (paddingRight > 0)
            {
                RugConsole.Write(ConsoleVerbosity.Silent, new string(fill, count) + str + new string(fill, num - (count + str.Length)));
                RugConsole.ColorState = colorState;
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
            }
            else
            {
                RugConsole.Write(ConsoleVerbosity.Silent, new string(fill, count) + str + new string(fill, num - (count + str.Length)));
                RugConsole.ColorState = colorState;
                RugConsole.Write(ConsoleVerbosity.Silent, new string(' ', RugConsole.BufferWidth));
                if (RugConsole.CanManipulateBuffer)
                {
                    RugConsole.CursorTop--;
                }
            }
        }
    }
}

