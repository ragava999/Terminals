namespace Rug.Cmd
{
    using Rug.Cmd.Colors;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    public static class ConsoleExt
    {
        private static string m_ApplicationBuildReportPrefix = "RUG";
        private static int m_BufferHeight = -1;
        private static int m_BufferWidth = -1;
        private static bool m_CanManipulateBuffer = false;
        private static bool m_DefaultPromptAnswer = false;
        private static bool m_FeaturesLoaded = false;
        private static bool m_IsBuildMode = false;
        private static bool m_ReportWarnings = true;
        private static IConsole m_SystemConsole = new SystemConsoleImpementation();
        private static ConsoleColorTheme m_Theme;
        private static bool m_UseDefaultPromptAnswer = false;
        private static ConsoleVerbosity m_Verbosity = ConsoleVerbosity.Normal;
        private static bool m_WarningsAsErrors = false;

        private static void EnsureConsoleFeatures()
        {
            if (!m_FeaturesLoaded)
            {
                GetConsoleFeatures();
            }
        }

        private static void GetConsoleFeatures()
        {
            try
            {
                m_BufferWidth = Console.BufferWidth;
                m_BufferHeight = Console.BufferHeight;
                m_CanManipulateBuffer = true;
            }
            catch
            {
                m_BufferWidth = 0x52;
                m_BufferHeight = 0x400;
                m_CanManipulateBuffer = false;
            }
            finally
            {
                m_FeaturesLoaded = true;
            }
        }

        public static ConsoleKeyInfo PromptForKey(string message, bool intercept, bool throwException)
        {
            WritePrompt(message);
            if (!IsBuildMode && CanManipulateBuffer)
            {
                return Console.ReadKey(intercept);
            }
            if (throwException)
            {
                throw new Exception(Strings.ConsoleExt_CannotAcceptInput);
            }
            return new ConsoleKeyInfo();
        }

        public static string PromptForLine(string message, bool throwException)
        {
            WritePrompt(message);
            if (!IsBuildMode && CanManipulateBuffer)
            {
                return Console.ReadLine();
            }
            if (throwException)
            {
                throw new Exception(Strings.ConsoleExt_CannotAcceptInput);
            }
            return null;
        }

        public static int Read()
        {
            ThrowInputDuringBuildModeException();
            return Console.Read();
        }

        public static ConsoleKeyInfo ReadKey()
        {
            ThrowInputDuringBuildModeException();
            return Console.ReadKey();
        }

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            ThrowInputDuringBuildModeException();
            return Console.ReadKey(intercept);
        }

        public static string ReadLine()
        {
            ThrowInputDuringBuildModeException();
            return Console.ReadLine();
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static void SetCursorPosition(int left, int top)
        {
            EnsureConsoleFeatures();
            if (m_IsBuildMode || !m_CanManipulateBuffer)
            {
                throw new Exception(Strings.ConsoleExt_CursorManipulate_GetSetError);
            }
            Console.SetCursorPosition(left, top);
        }

        public static bool ShouldWrite(ConsoleVerbosity verbosity)
        {
            return (verbosity <= m_Verbosity);
        }

        private static void ThrowInputDuringBuildModeException()
        {
            if (IsBuildMode || !CanManipulateBuffer)
            {
                throw new Exception(Strings.ConsoleExt_CannotAcceptInput);
            }
        }

        public static void Write(string str)
        {
            if (ConsoleVerbosity.Normal <= m_Verbosity)
            {
                Console.Write(str);
            }
        }

        public static void Write(ConsoleColorExt colour, string str)
        {
            if (ConsoleVerbosity.Normal <= m_Verbosity)
            {
                if (colour != ConsoleColorExt.Inhreit)
                {
                    ConsoleColor foregroundColor = Console.ForegroundColor;
                    if (colour != ConsoleColorExt.Inhreit)
                    {
                        Console.ForegroundColor = (ConsoleColor) colour;
                    }
                    Console.Write(str);
                    Console.ForegroundColor = foregroundColor;
                }
                else
                {
                    Console.Write(str);
                }
            }
        }

        public static void Write(ConsoleThemeColor colour, string str)
        {
            Write(Theme[colour], str);
        }

        public static void Write(ConsoleVerbosity level, string str)
        {
            if (level <= m_Verbosity)
            {
                Console.Write(str);
            }
        }

        public static void Write(ConsoleVerbosity level, ConsoleColorExt colour, string str)
        {
            if (level <= m_Verbosity)
            {
                if (colour != ConsoleColorExt.Inhreit)
                {
                    ConsoleColor foregroundColor = Console.ForegroundColor;
                    if (colour != ConsoleColorExt.Inhreit)
                    {
                        Console.ForegroundColor = (ConsoleColor) colour;
                    }
                    Console.Write(str);
                    Console.ForegroundColor = foregroundColor;
                }
                else
                {
                    Console.Write(str);
                }
            }
        }

        public static void Write(ConsoleVerbosity level, ConsoleThemeColor colour, string str)
        {
            Write(level, Theme[colour], str);
        }

        public static void WriteError(int id, string str)
        {
            WriteMessage(ConsoleMessage.Error, ConsoleThemeColor.ErrorColor1, id, str);
        }

        public static void WriteError(ConsoleColorExt colour, int id, string str)
        {
            WriteMessage(ConsoleMessage.Error, colour, id, str);
        }

        public static void WriteError(ConsoleThemeColor colour, int id, string str)
        {
            WriteMessage(ConsoleMessage.Error, Theme[colour], id, str);
        }

        public static void WriteError(int id, string sourceFile, int line, int character, string str)
        {
            WriteMessage(ConsoleMessage.Error, ConsoleThemeColor.ErrorColor1, id, sourceFile, line, character, str);
        }

        public static void WriteException(int id, Exception ex)
        {
            WriteMessage(ConsoleMessage.Error, ConsoleThemeColor.ErrorColor1, id, ex.Message);
            if (Verbosity == ConsoleVerbosity.Debug)
            {
                WriteStackTrace(ex.StackTrace);
            }
        }

        public static void WriteException(int id, string title, Exception ex)
        {
            WriteMessage(ConsoleMessage.Error, ConsoleThemeColor.ErrorColor1, id, title + Environment.NewLine + ex.Message);
            if (Verbosity == ConsoleVerbosity.Debug)
            {
                WriteStackTrace(ex.StackTrace);
            }
        }

        public static void WriteException(int id, string sourceFile, int line, int character, Exception ex)
        {
            WriteMessage(ConsoleMessage.Error, ConsoleThemeColor.ErrorColor1, id, sourceFile, line, character, ex.Message);
            if (Verbosity == ConsoleVerbosity.Debug)
            {
                WriteStackTrace(ex.StackTrace);
            }
        }

        public static void WriteException(int id, string sourceFile, int line, int character, string title, Exception ex)
        {
            WriteMessage(ConsoleMessage.Error, ConsoleThemeColor.ErrorColor1, id, sourceFile, line, character, title + Environment.NewLine + ex.Message);
            if (Verbosity == ConsoleVerbosity.Debug)
            {
                WriteStackTrace(ex.StackTrace);
            }
        }

        public static void WriteInterpreted(string buffer)
        {
            ConsoleFormatter.WriteInterpreted(m_SystemConsole, buffer);
        }

        public static void WriteInterpreted(string buffer, int paddingLeft, int paddingRight)
        {
            ConsoleFormatter.WriteInterpreted(m_SystemConsole, buffer, paddingLeft, paddingRight);
        }

        public static void WriteInterpreted(ConsoleColorExt colour, string buffer, int paddingLeft, int paddingRight)
        {
            ConsoleFormatter.WriteInterpreted(m_SystemConsole, colour, buffer, paddingLeft, paddingRight);
        }

        public static void WriteInterpreted(ConsoleThemeColor colour, string buffer, int paddingLeft, int paddingRight)
        {
            ConsoleFormatter.WriteInterpreted(m_SystemConsole, Theme[colour], buffer, paddingLeft, paddingRight);
        }

        public static void WriteInterpretedLine(string buffer)
        {
            ConsoleFormatter.WriteInterpretedLine(m_SystemConsole, buffer);
        }

        public static void WriteLine()
        {
            if (ConsoleVerbosity.Normal <= m_Verbosity)
            {
                Console.WriteLine();
            }
        }

        public static void WriteLine(ConsoleVerbosity level)
        {
            if (level <= m_Verbosity)
            {
                Console.WriteLine();
            }
        }

        public static void WriteLine(string str)
        {
            if (ConsoleVerbosity.Normal <= m_Verbosity)
            {
                Console.WriteLine(str);
            }
        }

        public static void WriteLine(ConsoleColorExt colour, string str)
        {
            if (ConsoleVerbosity.Normal <= m_Verbosity)
            {
                if (colour != ConsoleColorExt.Inhreit)
                {
                    ConsoleColor foregroundColor = Console.ForegroundColor;
                    if (colour != ConsoleColorExt.Inhreit)
                    {
                        Console.ForegroundColor = (ConsoleColor) colour;
                    }
                    Console.WriteLine(str);
                    Console.ForegroundColor = foregroundColor;
                }
                else
                {
                    Console.WriteLine(str);
                }
            }
        }

        public static void WriteLine(ConsoleThemeColor colour, string str)
        {
            WriteLine(Theme[colour], str);
        }

        public static void WriteLine(ConsoleVerbosity level, string str)
        {
            if (level <= m_Verbosity)
            {
                Console.WriteLine(str);
            }
        }

        public static void WriteLine(ConsoleVerbosity level, ConsoleColorExt colour, string str)
        {
            if (level <= m_Verbosity)
            {
                if (colour != ConsoleColorExt.Inhreit)
                {
                    ConsoleColor foregroundColor = Console.ForegroundColor;
                    if (colour != ConsoleColorExt.Inhreit)
                    {
                        Console.ForegroundColor = (ConsoleColor) colour;
                    }
                    Console.WriteLine(str);
                    Console.ForegroundColor = foregroundColor;
                }
                else
                {
                    Console.WriteLine(str);
                }
            }
        }

        public static void WriteLine(ConsoleVerbosity level, ConsoleThemeColor colour, string str)
        {
            WriteLine(level, Theme[colour], str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, string str)
        {
            WriteMessage(type, colour, 0, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, string str)
        {
            WriteMessage(type, Theme[colour], 0, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string str)
        {
            WriteMessage(type, colour, errorId, null, 0, 0, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string str)
        {
            WriteMessage(type, Theme[colour], errorId, null, 0, 0, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string sourceFile, int line, int character, string str)
        {
            string str2 = str;
            if (IsBuildMode)
            {
                string str3 = "";
                if (type == ConsoleMessage.Warning)
                {
                    if (WarningsAsErrors)
                    {
                        str3 = "error";
                    }
                    else
                    {
                        str3 = "warning";
                    }
                }
                else if (type == ConsoleMessage.Prompt)
                {
                    if (WarningsAsErrors)
                    {
                        str3 = "error";
                    }
                    else
                    {
                        str3 = "warning";
                    }
                }
                else
                {
                    str3 = "error";
                }
                string executablePath = null;
                if (sourceFile == null)
                {
                    executablePath = Application.ExecutablePath;
                }
                else
                {
                    executablePath = string.Concat(new object[] { sourceFile, "(", line, ",", character, ")" });
                }
                if ((str3 != "warning") || ((str3 == "warning") && ReportWarnings))
                {
                    str2 = executablePath + ": " + str3 + " " + ApplicationBuildReportPrefix + errorId.ToString().PadLeft(4, '0') + ": " + str.Replace("\n", " ");
                }
            }
            ConsoleColor foregroundColor = Console.ForegroundColor;
            if (colour != ConsoleColorExt.Inhreit)
            {
                Console.ForegroundColor = (ConsoleColor) colour;
            }
            if (CanManipulateBuffer)
            {
                if (Console.CursorLeft > 0)
                {
                    Console.WriteLine();
                }
                Console.WriteLine(str2);
            }
            else
            {
                Console.WriteLine(Environment.NewLine + str2);
            }
            Console.ForegroundColor = foregroundColor;
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string sourceFile, int line, int character, string str)
        {
            WriteMessage(type, Theme[colour], errorId, sourceFile, line, character, str);
        }

        public static void WritePrompt(string str)
        {
            WriteMessage(ConsoleMessage.Prompt, ConsoleThemeColor.PromptColor1, str);
        }

        public static void WritePrompt(ConsoleColorExt colour, string str)
        {
            WriteMessage(ConsoleMessage.Prompt, colour, str);
        }

        public static void WritePrompt(ConsoleThemeColor colour, string str)
        {
            WriteMessage(ConsoleMessage.Prompt, Theme[colour], str);
        }

        public static void WriteStackTrace(string trace)
        {
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = (ConsoleColor) Theme[ConsoleThemeColor.ErrorColor2];
            Console.WriteLine(new string(ConsoleChars.GetShade(ConsoleShade.Dim), BufferWidth));
            Console.ForegroundColor = (ConsoleColor) Theme[ConsoleThemeColor.TitleText];
            Console.WriteLine("  " + trace.Replace("\n", "\n  "));
            Console.WriteLine();
            Console.ForegroundColor = (ConsoleColor) Theme[ConsoleThemeColor.ErrorColor2];
            Console.WriteLine(new string(ConsoleChars.GetShade(ConsoleShade.Dim), BufferWidth));
            Console.WriteLine();
            Console.ForegroundColor = foregroundColor;
        }

        public static void WriteWarning(int id, string str)
        {
            WriteMessage(ConsoleMessage.Warning, ConsoleThemeColor.WarningColor2, id, str);
        }

        public static void WriteWarning(ConsoleColorExt colour, int id, string str)
        {
            WriteMessage(ConsoleMessage.Warning, colour, id, str);
        }

        public static void WriteWarning(ConsoleThemeColor colour, int id, string str)
        {
            WriteMessage(ConsoleMessage.Warning, Theme[colour], id, str);
        }

        public static void WriteWarning(ConsoleColorExt colour, int id, string sourceFile, int line, int character, string str)
        {
            WriteMessage(ConsoleMessage.Warning, colour, id, sourceFile, line, character, str);
        }

        public static void WriteWarning(ConsoleThemeColor colour, int id, string sourceFile, int line, int character, string str)
        {
            WriteMessage(ConsoleMessage.Warning, Theme[colour], id, sourceFile, line, character, str);
        }

        public static void WriteWrapped(ConsoleColorExt colour, string message, int paddingLeft, int paddingRight)
        {
            ConsoleColor foregroundColor = Console.ForegroundColor;
            if (colour != ConsoleColorExt.Inhreit)
            {
                Console.ForegroundColor = (ConsoleColor) colour;
            }
            int length = BufferWidth - (paddingLeft + paddingRight);
            List<string> list = new List<string>(message.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None));
            string str = new string(' ', paddingLeft);
            foreach (string str2 in list)
            {
                string str3 = str2;
                if (str3.Length != 0)
                {
                    goto Label_00B2;
                }
                Console.WriteLine();
                continue;
            Label_0074:
                if (str3.Length > length)
                {
                    Console.WriteLine(str + str3.Substring(0, length));
                    str3 = str3.Substring(length);
                }
                else
                {
                    Console.WriteLine(str + str3);
                    str3 = "";
                }
            Label_00B2:
                if (str3.Length > 0)
                {
                    goto Label_0074;
                }
            }
            Console.ForegroundColor = foregroundColor;
        }

        public static void WriteWrapped(ConsoleThemeColor colour, string message, int paddingLeft, int paddingRight)
        {
            WriteWrapped(Theme[colour], message, paddingLeft, paddingRight);
        }

        public static string ApplicationBuildReportPrefix
        {
            get
            {
                return m_ApplicationBuildReportPrefix;
            }
            set
            {
                string str = value;
                if ((str == null) || (str.Trim().Length == 0))
                {
                    str = "RUG";
                }
                m_ApplicationBuildReportPrefix = str.Trim();
            }
        }

        public static ConsoleColorExt BackgroundColor
        {
            get
            {
                return (ConsoleColorExt) Console.BackgroundColor;
            }
            set
            {
                if (value != ConsoleColorExt.Inhreit)
                {
                    Console.BackgroundColor = (ConsoleColor) value;
                }
            }
        }

        public static ConsoleThemeColor BackgroundThemeColor
        {
            set
            {
                BackgroundColor = Theme[value];
            }
        }

        public static int BufferHeight
        {
            get
            {
                EnsureConsoleFeatures();
                if (!m_IsBuildMode && m_CanManipulateBuffer)
                {
                    return Console.BufferHeight;
                }
                return m_BufferHeight;
            }
            set
            {
                if (m_IsBuildMode || !CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_BufferHeight_Error);
                }
                Console.BufferHeight = value;
            }
        }

        public static int BufferWidth
        {
            get
            {
                EnsureConsoleFeatures();
                if (!m_IsBuildMode && m_CanManipulateBuffer)
                {
                    return Console.BufferWidth;
                }
                return m_BufferWidth;
            }
            set
            {
                if (m_IsBuildMode || !CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_BufferWidth_Error);
                }
                Console.BufferWidth = value;
            }
        }

        public static bool CanManipulateBuffer
        {
            get
            {
                EnsureConsoleFeatures();
                return (m_CanManipulateBuffer && !m_IsBuildMode);
            }
        }

        public static ConsoleColorState ColorState
        {
            get
            {
                return new ConsoleColorState(ForegroundColor, BackgroundColor);
            }
            set
            {
                ForegroundColor = value.ForegroundColor;
                BackgroundColor = value.BackgroundColor;
            }
        }

        public static int CursorLeft
        {
            get
            {
                EnsureConsoleFeatures();
                if (m_IsBuildMode || !m_CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_CursorManipulate_GetSetError);
                }
                return Console.CursorLeft;
            }
            set
            {
                if (m_IsBuildMode || !CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_CursorManipulate_GetSetError);
                }
                Console.CursorLeft = value;
            }
        }

        public static int CursorSize
        {
            get
            {
                EnsureConsoleFeatures();
                if (m_IsBuildMode || !m_CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_CursorManipulate_GetSetError);
                }
                return Console.CursorSize;
            }
            set
            {
                if (m_IsBuildMode || !CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_CursorManipulate_GetSetError);
                }
                Console.CursorSize = value;
            }
        }

        public static int CursorTop
        {
            get
            {
                EnsureConsoleFeatures();
                if (m_IsBuildMode || !m_CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_CursorManipulate_GetSetError);
                }
                return Console.CursorTop;
            }
            set
            {
                if (m_IsBuildMode || !CanManipulateBuffer)
                {
                    throw new Exception(Strings.ConsoleExt_CursorManipulate_GetSetError);
                }
                Console.CursorTop = value;
            }
        }

        public static bool DefaultPromptAnswer
        {
            get
            {
                return m_DefaultPromptAnswer;
            }
            set
            {
                m_DefaultPromptAnswer = value;
            }
        }

        public static ConsoleColorExt ForegroundColor
        {
            get
            {
                return (ConsoleColorExt) Console.ForegroundColor;
            }
            set
            {
                if (value != ConsoleColorExt.Inhreit)
                {
                    Console.ForegroundColor = (ConsoleColor) value;
                }
            }
        }

        public static ConsoleThemeColor ForegroundThemeColor
        {
            set
            {
                ForegroundColor = Theme[value];
            }
        }

        public static bool IsBuildMode
        {
            get
            {
                return m_IsBuildMode;
            }
            set
            {
                m_IsBuildMode = value;
            }
        }

        public static bool ReportWarnings
        {
            get
            {
                return m_ReportWarnings;
            }
            set
            {
                m_ReportWarnings = value;
            }
        }

        public static IConsole SystemConsole
        {
            get
            {
                return m_SystemConsole;
            }
        }

        public static ConsoleColorTheme Theme
        {
            get
            {
                if (m_Theme == null)
                {
                    m_Theme = ConsoleColorTheme.Load(Console.ForegroundColor, Console.BackgroundColor, ConsoleColorDefaultThemes.None);
                }
                return m_Theme;
            }
            set
            {
                m_Theme = value;
            }
        }

        public static bool UseDefaultPromptAnswer
        {
            get
            {
                return m_UseDefaultPromptAnswer;
            }
            set
            {
                m_UseDefaultPromptAnswer = value;
            }
        }

        public static ConsoleVerbosity Verbosity
        {
            get
            {
                return m_Verbosity;
            }
            set
            {
                m_Verbosity = value;
            }
        }

        public static bool WarningsAsErrors
        {
            get
            {
                return m_WarningsAsErrors;
            }
            set
            {
                m_WarningsAsErrors = value;
            }
        }

        internal class SystemConsoleImpementation : IConsole
        {
            public ConsoleKeyInfo PromptForKey(string message, bool intercept, bool throwException)
            {
                return ConsoleExt.PromptForKey(message, intercept, throwException);
            }

            public string PromptForLine(string message, bool throwException)
            {
                return ConsoleExt.PromptForLine(message, throwException);
            }

            public int Read()
            {
                return ConsoleExt.Read();
            }

            public ConsoleKeyInfo ReadKey()
            {
                return ConsoleExt.ReadKey();
            }

            public ConsoleKeyInfo ReadKey(bool intercept)
            {
                return ConsoleExt.ReadKey(intercept);
            }

            public string ReadLine()
            {
                return ConsoleExt.ReadLine();
            }

            public void ResetColor()
            {
                ConsoleExt.ResetColor();
            }

            public void SetCursorPosition(int left, int top)
            {
                ConsoleExt.SetCursorPosition(left, top);
            }

            public bool ShouldWrite(ConsoleVerbosity verbosity)
            {
                return ConsoleExt.ShouldWrite(verbosity);
            }

            public void Write(string str)
            {
                ConsoleExt.Write(str);
            }

            public void Write(ConsoleColorExt colour, string str)
            {
                ConsoleExt.Write(colour, str);
            }

            public void Write(ConsoleThemeColor colour, string str)
            {
                ConsoleExt.Write(colour, str);
            }

            public void Write(ConsoleVerbosity level, string str)
            {
                ConsoleExt.Write(level, str);
            }

            public void Write(ConsoleVerbosity level, ConsoleColorExt colour, string str)
            {
                ConsoleExt.Write(level, colour, str);
            }

            public void Write(ConsoleVerbosity level, ConsoleThemeColor colour, string str)
            {
                ConsoleExt.Write(level, colour, str);
            }

            public void WriteError(int id, string str)
            {
                ConsoleExt.WriteError(id, str);
            }

            public void WriteError(ConsoleColorExt colour, int id, string str)
            {
                ConsoleExt.WriteError(colour, id, str);
            }

            public void WriteError(ConsoleThemeColor colour, int id, string str)
            {
                ConsoleExt.WriteError(colour, id, str);
            }

            public void WriteError(int id, string sourceFile, int line, int character, string str)
            {
                ConsoleExt.WriteError(id, sourceFile, line, character, str);
            }

            public void WriteException(int id, Exception ex)
            {
                ConsoleExt.WriteException(id, ex);
            }

            public void WriteException(int id, string title, Exception ex)
            {
                ConsoleExt.WriteException(id, title, ex);
            }

            public void WriteException(int id, string sourceFile, int line, int character, Exception ex)
            {
                ConsoleExt.WriteException(id, sourceFile, line, character, ex);
            }

            public void WriteException(int id, string sourceFile, int line, int character, string title, Exception ex)
            {
                ConsoleExt.WriteException(id, sourceFile, line, character, title, ex);
            }

            public void WriteInterpreted(string buffer)
            {
                ConsoleExt.WriteInterpreted(buffer);
            }

            public void WriteInterpreted(string buffer, int paddingLeft, int paddingRight)
            {
                ConsoleExt.WriteInterpreted(buffer, paddingLeft, paddingRight);
            }

            public void WriteInterpreted(ConsoleColorExt colour, string buffer, int paddingLeft, int paddingRight)
            {
                ConsoleExt.WriteInterpreted(colour, buffer, paddingLeft, paddingRight);
            }

            public void WriteInterpreted(ConsoleThemeColor colour, string buffer, int paddingLeft, int paddingRight)
            {
                ConsoleExt.WriteInterpreted(colour, buffer, paddingLeft, paddingRight);
            }

            public void WriteInterpretedLine(string buffer)
            {
                ConsoleExt.WriteInterpretedLine(buffer);
            }

            public void WriteLine()
            {
                ConsoleExt.WriteLine();
            }

            public void WriteLine(ConsoleVerbosity level)
            {
                ConsoleExt.WriteLine(level);
            }

            public void WriteLine(string str)
            {
                ConsoleExt.WriteLine(str);
            }

            public void WriteLine(ConsoleColorExt colour, string str)
            {
                ConsoleExt.WriteLine(colour, str);
            }

            public void WriteLine(ConsoleThemeColor colour, string str)
            {
                ConsoleExt.WriteLine(colour, str);
            }

            public void WriteLine(ConsoleVerbosity level, string str)
            {
                ConsoleExt.WriteLine(level, str);
            }

            public void WriteLine(ConsoleVerbosity level, ConsoleColorExt colour, string str)
            {
                ConsoleExt.WriteLine(level, colour, str);
            }

            public void WriteLine(ConsoleVerbosity level, ConsoleThemeColor colour, string str)
            {
                ConsoleExt.WriteLine(level, colour, str);
            }

            public void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, string str)
            {
                ConsoleExt.WriteMessage(type, colour, str);
            }

            public void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, string str)
            {
                ConsoleExt.WriteMessage(type, colour, str);
            }

            public void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string str)
            {
                ConsoleExt.WriteMessage(type, colour, errorId, str);
            }

            public void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string str)
            {
                ConsoleExt.WriteMessage(type, colour, errorId, str);
            }

            public void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string sourceFile, int line, int character, string str)
            {
                ConsoleExt.WriteMessage(type, colour, errorId, sourceFile, line, character, str);
            }

            public void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string sourceFile, int line, int character, string str)
            {
                ConsoleExt.WriteMessage(type, colour, errorId, sourceFile, line, character, str);
            }

            public void WritePrompt(string str)
            {
                ConsoleExt.WritePrompt(str);
            }

            public void WritePrompt(ConsoleColorExt colour, string str)
            {
                ConsoleExt.WritePrompt(colour, str);
            }

            public void WritePrompt(ConsoleThemeColor colour, string str)
            {
                ConsoleExt.WritePrompt(colour, str);
            }

            public void WriteStackTrace(string trace)
            {
                ConsoleExt.WriteStackTrace(trace);
            }

            public void WriteWarning(int id, string str)
            {
                ConsoleExt.WriteWarning(id, str);
            }

            public void WriteWarning(ConsoleColorExt colour, int id, string str)
            {
                ConsoleExt.WriteWarning(colour, id, str);
            }

            public void WriteWarning(ConsoleThemeColor colour, int id, string str)
            {
                ConsoleExt.WriteWarning(colour, id, str);
            }

            public void WriteWarning(ConsoleColorExt colour, int id, string sourceFile, int line, int character, string str)
            {
                ConsoleExt.WriteWarning(colour, id, sourceFile, line, character, str);
            }

            public void WriteWarning(ConsoleThemeColor colour, int id, string sourceFile, int line, int character, string str)
            {
                ConsoleExt.WriteWarning(colour, id, sourceFile, line, character, str);
            }

            public void WriteWrapped(ConsoleColorExt colour, string message, int paddingLeft, int paddingRight)
            {
                ConsoleExt.WriteWrapped(colour, message, paddingLeft, paddingRight);
            }

            public void WriteWrapped(ConsoleThemeColor colour, string message, int paddingLeft, int paddingRight)
            {
                ConsoleExt.WriteWrapped(colour, message, paddingLeft, paddingRight);
            }

            public string ApplicationBuildReportPrefix
            {
                get
                {
                    return ConsoleExt.ApplicationBuildReportPrefix;
                }
                set
                {
                    ConsoleExt.ApplicationBuildReportPrefix = value;
                }
            }

            public ConsoleColorExt BackgroundColor
            {
                get
                {
                    return ConsoleExt.BackgroundColor;
                }
                set
                {
                    ConsoleExt.BackgroundColor = value;
                }
            }

            public ConsoleThemeColor BackgroundThemeColor
            {
                set
                {
                    ConsoleExt.BackgroundThemeColor = value;
                }
            }

            public int BufferHeight
            {
                get
                {
                    return ConsoleExt.BufferHeight;
                }
                set
                {
                    ConsoleExt.BufferHeight = value;
                }
            }

            public int BufferWidth
            {
                get
                {
                    return ConsoleExt.BufferWidth;
                }
                set
                {
                    ConsoleExt.BufferWidth = value;
                }
            }

            public bool CanManipulateBuffer
            {
                get
                {
                    return ConsoleExt.CanManipulateBuffer;
                }
            }

            public ConsoleColorState ColorState
            {
                get
                {
                    return ConsoleExt.ColorState;
                }
                set
                {
                    ConsoleExt.ColorState = value;
                }
            }

            public int CursorLeft
            {
                get
                {
                    return ConsoleExt.CursorLeft;
                }
                set
                {
                    ConsoleExt.CursorLeft = value;
                }
            }

            public int CursorSize
            {
                get
                {
                    return ConsoleExt.CursorSize;
                }
                set
                {
                    ConsoleExt.CursorSize = value;
                }
            }

            public int CursorTop
            {
                get
                {
                    return ConsoleExt.CursorTop;
                }
                set
                {
                    ConsoleExt.CursorTop = value;
                }
            }

            public bool DefaultPromptAnswer
            {
                get
                {
                    return ConsoleExt.DefaultPromptAnswer;
                }
                set
                {
                    ConsoleExt.DefaultPromptAnswer = value;
                }
            }

            public ConsoleColorExt ForegroundColor
            {
                get
                {
                    return ConsoleExt.ForegroundColor;
                }
                set
                {
                    ConsoleExt.ForegroundColor = value;
                }
            }

            public ConsoleThemeColor ForegroundThemeColor
            {
                set
                {
                    ConsoleExt.ForegroundThemeColor = value;
                }
            }

            public bool IsBuildMode
            {
                get
                {
                    return ConsoleExt.IsBuildMode;
                }
                set
                {
                    ConsoleExt.IsBuildMode = value;
                }
            }

            public bool ReportWarnings
            {
                get
                {
                    return ConsoleExt.ReportWarnings;
                }
                set
                {
                    ConsoleExt.ReportWarnings = value;
                }
            }

            public ConsoleColorTheme Theme
            {
                get
                {
                    return ConsoleExt.Theme;
                }
                set
                {
                    ConsoleExt.Theme = value;
                }
            }

            public bool UseDefaultPromptAnswer
            {
                get
                {
                    return ConsoleExt.UseDefaultPromptAnswer;
                }
                set
                {
                    ConsoleExt.UseDefaultPromptAnswer = value;
                }
            }

            public ConsoleVerbosity Verbosity
            {
                get
                {
                    return ConsoleExt.Verbosity;
                }
                set
                {
                    ConsoleExt.Verbosity = value;
                }
            }

            public bool WarningsAsErrors
            {
                get
                {
                    return ConsoleExt.WarningsAsErrors;
                }
                set
                {
                    ConsoleExt.WarningsAsErrors = value;
                }
            }
        }
    }
}

