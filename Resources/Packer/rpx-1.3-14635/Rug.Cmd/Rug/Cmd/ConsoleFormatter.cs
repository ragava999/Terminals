namespace Rug.Cmd
{
    using Rug.Cmd.Colors;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ConsoleFormatter
    {
        public static readonly Regex FormatRegex = new Regex(@"(?<Tag>\x3c(?<Inner>c:\s*(\d+))\x3e)|(?<EndTag>\x3c\x2fc:\s*(\d+)\x3e)|(?<EndTag>\x3c\x2fc\x3e)|(?<Tag>\x3c(?<Inner>t:\s*(\d+))\x3e)|(?<EndTag>\x3c\x2ft:\s*(\d+)\x3e)|(?<EndTag>\x3c\x2ft\x3e)|(?<Text>[^\x3c\x3e]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        public static readonly Regex FormatStripperRegex = new Regex(@"(?<Tag>\x3cc:\s*(\d+)\x3e)|(?<EndTag>\x3c\x2fc:\s*(\d+)\x3e)|(?<EndTag>\x3c\x2fc\x3e)|(?<Tag>\x3ct:\s*(\d+)\x3e)|(?<EndTag>\x3c\x2ft:\s*(\d+)\x3e)|(?<EndTag>\x3c\x2ft\x3e)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private StringBuilder m_Result = new StringBuilder();

        private static string BuildTagCloseString(Stack<string> stack)
        {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            int count = stack.Count;
            while (num < count)
            {
                builder.Append("</c>");
                num++;
            }
            return builder.ToString();
        }

        private static string BuildTagOpenString(Stack<string> stack)
        {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            int count = stack.Count;
            while (num < count)
            {
                builder.Append("<{" + num + "}>");
                num++;
            }
            return string.Format(builder.ToString(), (object[]) stack.ToArray());
        }

        public void Clear()
        {
            this.m_Result = this.m_Result.Remove(0, this.m_Result.Length);
        }

        public static string EscapeString(string str)
        {
            return str.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string Format(ConsoleColorExt color, string message)
        {
            if (color != ConsoleColorExt.Inhreit)
            {
                return string.Format("<c:{0}>{1}</c>", (int) color, EscapeString(message));
            }
            return string.Format("{0}", EscapeString(message));
        }

        public static string Format(ConsoleColorExt color, string format, params object[] args)
        {
            if (color != ConsoleColorExt.Inhreit)
            {
                return string.Format("<c:{0}>{1}</c>{2}", (int) color, EscapeString(string.Format(format, args)), Environment.NewLine);
            }
            return EscapeString(string.Format(format, args));
        }

        public static string FormatAsRtf(string buffer, ConsoleColorTheme theme)
        {
            StringBuilder builder = new StringBuilder();
            Stack<ConsoleColorExt> stack = new Stack<ConsoleColorExt>();
            foreach (Match match in FormatRegex.Matches(buffer))
            {
                if (match.Groups["Tag"].Success)
                {
                    ConsoleColorExt item = ParseColour(match.Groups["Inner"].Value, theme);
                    stack.Push(item);
                    builder.Append(@"\cf" + ((((int) item) + 1)).ToString() + @"\ulnone ");
                }
                else if (match.Groups["EndTag"].Success)
                {
                    if (stack.Count < 0)
                    {
                        throw new Exception(string.Format(Strings.ConsoleInterpreter_UnexpectedEndTag, match.Index));
                    }
                    builder.Append(@"\cf" + ((int) (((ConsoleColorExt) stack.Pop()) + (int)ConsoleColorExt.DarkBlue)).ToString() + @"\ulnone ");
                }
                else if (match.Groups["Text"].Success)
                {
                    string str = UnescapeString(match.Value);
                    builder.Append(ReplaceLineEnds(str));
                }
            }
            return builder.ToString();
        }

        public static string FormatLine(ConsoleColorExt color, string message)
        {
            if (color != ConsoleColorExt.Inhreit)
            {
                return string.Format("<c:{0}>{1}</c>{2}", (int) color, EscapeString(message), Environment.NewLine);
            }
            return string.Format("{0}{1}", EscapeString(message), Environment.NewLine);
        }

        public static ConsoleColorExt ParseColour(string tagInner, ConsoleColorTheme theme)
        {
            string str = tagInner.Trim();
            int index = str.IndexOf(':');
            if ((index == 1) && (str[0] == 'c'))
            {
                string s = str.Substring(2);
                int result = 0;
                if (int.TryParse(s, out result))
                {
                    return (ConsoleColorExt) result;
                }
                try
                {
                    return (ConsoleColorExt) Enum.Parse(typeof(ConsoleColorExt), s, true);
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format(Strings.ConsoleInterpreter_ParseColour_Error, str), exception);
                }
            }
            if ((index == 1) && (str[0] == 't'))
            {
                string str3 = str.Substring(2);
                int num3 = 0;
                if (int.TryParse(str3, out num3))
                {
                    return theme[(ConsoleThemeColor) num3];
                }
                try
                {
                    return theme[(ConsoleThemeColor) Enum.Parse(typeof(ConsoleThemeColor), str3, true)];
                }
                catch (Exception exception2)
                {
                    throw new Exception(string.Format(Strings.ConsoleInterpreter_ParseColour_Error, str), exception2);
                }
            }
            throw new Exception(string.Format(Strings.ConsoleInterpreter_ParseColour_Error, str));
        }

        private static string ReplaceLineEnds(string value)
        {
            return value.Replace("\n", "\\par\r\n").Replace("\n\n", "\n").Replace("\r\r", "\r").Replace("{", @"\{").Replace("}", @"\}");
        }

        private static List<string> SplitLinebreaks(string buffer)
        {
            string item = buffer;
            List<string> list = new List<string>();
            while (item.Length > 0)
            {
                int length = item.IndexOfAny(new char[] { '\r', '\n' });
                if (length >= 0)
                {
                    if (((item[length] == '\r') && (item.Length > (length + 1))) && (item[length + 1] == '\n'))
                    {
                        list.Add(item.Substring(0, length));
                        list.Add(item.Substring(length, 2));
                        item = item.Substring(length + 2);
                    }
                    else
                    {
                        list.Add(item.Substring(0, length));
                        list.Add(item.Substring(length, 1));
                        item = item.Substring(length + 1);
                    }
                }
                else
                {
                    list.Add(item);
                    item = "";
                }
            }
            return list;
        }

        public static string[] SplitLines(string buffer, int maxWidth)
        {
            int num = 0;
            Stack<string> stack = new Stack<string>();
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            foreach (Match match in FormatRegex.Matches(buffer))
            {
                if (match.Groups["Tag"].Success)
                {
                    stack.Push(match.Groups["Inner"].Value);
                    builder.Append(match.Value);
                }
                else if (match.Groups["EndTag"].Success)
                {
                    if (stack.Count < 0)
                    {
                        throw new Exception(string.Format(Strings.ConsoleInterpreter_UnexpectedEndTag, match.Index));
                    }
                    stack.Pop();
                    builder.Append(match.Value);
                }
                else if (match.Groups["Text"].Success)
                {
                    string str = UnescapeString(match.Value);
                    foreach (string str2 in SplitLinebreaks(str))
                    {
                        string str3 = str2;
                        if (!(str2 == "\n") && !(str2 == Environment.NewLine))
                        {
                            goto Label_0295;
                        }
                        builder.Append(BuildTagCloseString(stack));
                        list.Add(builder.ToString());
                        builder = new StringBuilder();
                        builder.Append(BuildTagOpenString(stack));
                        num = 0;
                        continue;
                    Label_0178:
                        if ((num + str3.Length) > maxWidth)
                        {
                            if (num > 0)
                            {
                                builder.Append(EscapeString(str3.Substring(0, maxWidth - num)));
                                builder.Append(BuildTagCloseString(stack));
                                list.Add(builder.ToString());
                                builder = new StringBuilder();
                                builder.Append(BuildTagOpenString(stack));
                            }
                            else
                            {
                                builder.Append(EscapeString(str3.Substring(0, maxWidth)));
                                builder.Append(BuildTagCloseString(stack));
                                list.Add(builder.ToString());
                                builder = new StringBuilder();
                                builder.Append(BuildTagOpenString(stack));
                            }
                            str3 = str3.Substring(maxWidth - num);
                            num = 0;
                        }
                        else
                        {
                            if (str.EndsWith("\n") || str.EndsWith(Environment.NewLine))
                            {
                                builder.Append(EscapeString(str3));
                                builder.Append(BuildTagCloseString(stack));
                                list.Add(builder.ToString());
                                builder = new StringBuilder();
                                builder.Append(BuildTagOpenString(stack));
                                num = 0;
                            }
                            else
                            {
                                builder.Append(str3);
                                num += str3.Length;
                            }
                            str3 = "";
                        }
                    Label_0295:
                        if (str3.Length > 0)
                        {
                            goto Label_0178;
                        }
                    }
                }
            }
            if (builder.Length > 0)
            {
                builder.Append(BuildTagCloseString(stack));
                list.Add(builder.ToString());
            }
            return list.ToArray();
        }

        public static string StripFormat(string buffer)
        {
            return FormatStripperRegex.Replace(buffer, "");
        }

        public static string Substring(string buffer, int index, int length)
        {
            Stack<string> stack = new Stack<string>();
            StringBuilder builder = new StringBuilder();
            int num = 0;
            int num2 = index + length;
            bool flag = false;
            foreach (Match match in FormatRegex.Matches(buffer))
            {
                if (match.Groups["Tag"].Success)
                {
                    stack.Push(match.Groups["Inner"].Value);
                    if (flag)
                    {
                        builder.Append(match.Value);
                    }
                }
                else if (match.Groups["EndTag"].Success)
                {
                    if (stack.Count < 0)
                    {
                        throw new Exception(string.Format(Strings.ConsoleInterpreter_UnexpectedEndTag, match.Index));
                    }
                    stack.Pop();
                    if (flag)
                    {
                        builder.Append(match.Value);
                    }
                }
                else if (match.Groups["Text"].Success)
                {
                    foreach (string str2 in SplitLinebreaks(UnescapeString(match.Value)))
                    {
                        string str = str2;
                        if (num < num2)
                        {
                            if (num > index)
                            {
                                int num3 = num2 - num;
                                if (num3 <= str.Length)
                                {
                                    builder.Append(EscapeString(str.Substring(0, num3)));
                                    builder.Append(BuildTagCloseString(stack));
                                    return builder.ToString();
                                }
                                builder.Append(EscapeString(str));
                                num += str.Length;
                            }
                            else if ((num + str.Length) >= index)
                            {
                                int startIndex = index - num;
                                num += startIndex;
                                str = str.Substring(startIndex);
                                int num5 = num2 - num;
                                if (num5 <= str.Length)
                                {
                                    builder.Append(BuildTagOpenString(stack));
                                    builder.Append(EscapeString(str.Substring(0, num5)));
                                    builder.Append(BuildTagCloseString(stack));
                                    return builder.ToString();
                                }
                                builder.Append(BuildTagOpenString(stack));
                                builder.Append(EscapeString(str));
                                num += str.Length;
                                flag = true;
                            }
                            else
                            {
                                num += str.Length;
                            }
                        }
                    }
                }
            }
            builder.Append(BuildTagCloseString(stack));
            return builder.ToString();
        }

        public override string ToString()
        {
            return this.m_Result.ToString();
        }

        public static string UnescapeString(string str)
        {
            return str.Replace("&lt;", "<").Replace("&gt;", ">");
        }

        public void Write(ConsoleColorExt color, string message)
        {
            if (color != ConsoleColorExt.Inhreit)
            {
                this.m_Result.AppendFormat("<c:{0}>{1}</c>", (int) color, EscapeString(message));
            }
            else
            {
                this.m_Result.AppendFormat("{0}", EscapeString(message));
            }
        }

        public void Write(ConsoleThemeColor color, string message)
        {
            this.m_Result.AppendFormat("<t:{0}>{1}</t>", (int) color, EscapeString(message));
        }

        public void Write(ConsoleColorExt color, string format, params object[] args)
        {
            if (color != ConsoleColorExt.Inhreit)
            {
                this.m_Result.AppendFormat("<c:{0}>{1}</c>{2}", (int) color, EscapeString(string.Format(format, args)), Environment.NewLine);
            }
            else
            {
                this.m_Result.Append(EscapeString(string.Format(format, args)));
            }
        }

        public void Write(ConsoleThemeColor color, string format, params object[] args)
        {
            this.m_Result.AppendFormat("<t:{0}>{1}</t>{2}", (int) color, EscapeString(string.Format(format, args)), Environment.NewLine);
        }

        public static void WriteInterpreted(IConsole console, string buffer)
        {
            WriteInterpreted(console, buffer, 0, 0);
        }

        public static void WriteInterpreted(IConsole console, string buffer, int paddingLeft, int paddingRight)
        {
            int num = console.BufferWidth - (paddingLeft + paddingRight);
            string str = new string(' ', paddingLeft);
            int num2 = 0;
            Stack<ConsoleColorExt> stack = new Stack<ConsoleColorExt>();
            foreach (Match match in FormatRegex.Matches(buffer))
            {
                if (match.Groups["Tag"].Success)
                {
                    ConsoleColorExt ext = ParseColour(match.Groups["Inner"].Value, console.Theme);
                    stack.Push(console.ForegroundColor);
                    console.ForegroundColor = ext;
                }
                else if (match.Groups["EndTag"].Success)
                {
                    if (stack.Count < 0)
                    {
                        throw new Exception(string.Format(Strings.ConsoleInterpreter_UnexpectedEndTag, match.Index));
                    }
                    console.ForegroundColor = stack.Pop();
                }
                else if (match.Groups["Text"].Success)
                {
                    foreach (string str3 in SplitLinebreaks(UnescapeString(match.Value)))
                    {
                        string str4 = str3;
                        if (!(str4 == "\n") && !(str4 == Environment.NewLine))
                        {
                            goto Label_020A;
                        }
                        console.WriteLine(ConsoleVerbosity.Silent);
                        num2 = 0;
                        continue;
                    Label_015D:
                        if ((num2 + str4.Length) > num)
                        {
                            string str5;
                            int length = str4.LastIndexOf(' ', num - num2, num - num2);
                            if (length <= 0)
                            {
                                length = num - num2;
                            }
                            if (num2 > 0)
                            {
                                str5 = str4.Substring(0, length);
                            }
                            else
                            {
                                str5 = str + str4.Substring(0, length);
                            }
                            if (console.BufferWidth == (num2 + str5.Length))
                            {
                                console.Write(ConsoleVerbosity.Silent, str5);
                            }
                            else
                            {
                                console.WriteLine(ConsoleVerbosity.Silent, str5);
                            }
                            str4 = str4.Substring(length + 1);
                            num2 = 0;
                        }
                        else
                        {
                            string str6 = str4;
                            if (num2 <= 0)
                            {
                                str6 = str + str4;
                            }
                            console.Write(ConsoleVerbosity.Silent, str6);
                            num2 += str4.Length;
                            str4 = "";
                        }
                    Label_020A:
                        if (str4.Length > 0)
                        {
                            goto Label_015D;
                        }
                    }
                }
            }
        }

        public static void WriteInterpreted(IConsole console, ConsoleColorExt colour, string buffer, int paddingLeft, int paddingRight)
        {
            ConsoleColorExt foregroundColor = console.ForegroundColor;
            if (colour != ConsoleColorExt.Inhreit)
            {
                console.ForegroundColor = colour;
            }
            WriteInterpreted(console, buffer, paddingLeft, paddingRight);
            console.ForegroundColor = foregroundColor;
        }

        public static void WriteInterpretedLine(IConsole console, string buffer)
        {
            WriteInterpreted(console, buffer + Environment.NewLine);
        }

        public void WriteLine()
        {
            this.m_Result.AppendFormat(Environment.NewLine, new object[0]);
        }

        public void WriteLine(ConsoleColorExt color, string message)
        {
            if (color != ConsoleColorExt.Inhreit)
            {
                this.m_Result.AppendFormat("<c:{0}>{1}</c>{2}", (int) color, EscapeString(message), Environment.NewLine);
            }
            else
            {
                this.m_Result.AppendFormat("{0}{1}", EscapeString(message), Environment.NewLine);
            }
        }

        public void WriteLine(ConsoleThemeColor color, string message)
        {
            this.m_Result.AppendFormat("<t:{0}>{1}</t>{2}", (int) color, EscapeString(message), Environment.NewLine);
        }
    }
}

