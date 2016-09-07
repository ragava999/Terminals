namespace Rug.Cmd
{
    using Rug.Cmd.Colors;
    using System;

    public class RugConsole
    {
        public static IConsole App = ConsoleExt.SystemConsole;
        public static IConsole Sys = ConsoleExt.SystemConsole;

        public static ConsoleKeyInfo PromptForKey(string message, bool intercept, bool throwException)
        {
            return App.PromptForKey(message, intercept, throwException);
        }

        public static string PromptForLine(string message, bool throwException)
        {
            return App.PromptForLine(message, throwException);
        }

        public static int Read()
        {
            return App.Read();
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return App.ReadKey();
        }

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            return App.ReadKey(intercept);
        }

        public static string ReadLine()
        {
            return App.ReadLine();
        }

        public static void ResetColor()
        {
            App.ResetColor();
        }

        public static void SetCursorPosition(int left, int top)
        {
            App.SetCursorPosition(left, top);
        }

        public static bool ShouldWrite(ConsoleVerbosity verbosity)
        {
            return App.ShouldWrite(verbosity);
        }

        public static void Write(string str)
        {
            App.Write(str);
        }

        public static void Write(ConsoleColorExt colour, string str)
        {
            App.Write(colour, str);
        }

        public static void Write(ConsoleThemeColor colour, string str)
        {
            App.Write(colour, str);
        }

        public static void Write(ConsoleVerbosity level, string str)
        {
            App.Write(level, str);
        }

        public static void Write(ConsoleVerbosity level, ConsoleColorExt colour, string str)
        {
            App.Write(level, colour, str);
        }

        public static void Write(ConsoleVerbosity level, ConsoleThemeColor colour, string str)
        {
            App.Write(level, colour, str);
        }

        public static void WriteError(int id, string str)
        {
            App.WriteError(id, str);
        }

        public static void WriteError(ConsoleColorExt colour, int id, string str)
        {
            App.WriteError(colour, id, str);
        }

        public static void WriteError(ConsoleThemeColor colour, int id, string str)
        {
            App.WriteError(colour, id, str);
        }

        public static void WriteError(int id, string sourceFile, int line, int character, string str)
        {
            App.WriteError(id, sourceFile, line, character, str);
        }

        public static void WriteException(int id, Exception ex)
        {
            App.WriteException(id, ex);
        }

        public static void WriteException(int id, string title, Exception ex)
        {
            App.WriteException(id, title, ex);
        }

        public static void WriteException(int id, string sourceFile, int line, int character, Exception ex)
        {
            App.WriteException(id, sourceFile, line, character, ex);
        }

        public static void WriteException(int id, string sourceFile, int line, int character, string title, Exception ex)
        {
            App.WriteException(id, sourceFile, line, character, title, ex);
        }

        public static void WriteInterpreted(string buffer)
        {
            App.WriteInterpreted(buffer);
        }

        public static void WriteInterpreted(string buffer, int paddingLeft, int paddingRight)
        {
            App.WriteInterpreted(buffer, paddingLeft, paddingRight);
        }

        public static void WriteInterpreted(ConsoleColorExt colour, string buffer, int paddingLeft, int paddingRight)
        {
            App.WriteInterpreted(colour, buffer, paddingLeft, paddingRight);
        }

        public static void WriteInterpreted(ConsoleThemeColor colour, string buffer, int paddingLeft, int paddingRight)
        {
            App.WriteInterpreted(colour, buffer, paddingLeft, paddingRight);
        }

        public static void WriteInterpretedLine(string buffer)
        {
            App.WriteInterpretedLine(buffer);
        }

        public static void WriteLine()
        {
            App.WriteLine();
        }

        public static void WriteLine(ConsoleVerbosity level)
        {
            App.WriteLine(level);
        }

        public static void WriteLine(string str)
        {
            App.WriteLine(str);
        }

        public static void WriteLine(ConsoleColorExt colour, string str)
        {
            App.WriteLine(colour, str);
        }

        public static void WriteLine(ConsoleThemeColor colour, string str)
        {
            App.WriteLine(colour, str);
        }

        public static void WriteLine(ConsoleVerbosity level, string str)
        {
            App.WriteLine(level, str);
        }

        public static void WriteLine(ConsoleVerbosity level, ConsoleColorExt colour, string str)
        {
            App.WriteLine(level, colour, str);
        }

        public static void WriteLine(ConsoleVerbosity level, ConsoleThemeColor colour, string str)
        {
            App.WriteLine(level, colour, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, string str)
        {
            App.WriteMessage(type, colour, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, string str)
        {
            App.WriteMessage(type, colour, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string str)
        {
            App.WriteMessage(type, colour, errorId, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string str)
        {
            App.WriteMessage(type, colour, errorId, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string sourceFile, int line, int character, string str)
        {
            App.WriteMessage(type, colour, errorId, sourceFile, line, character, str);
        }

        public static void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string sourceFile, int line, int character, string str)
        {
            App.WriteMessage(type, colour, errorId, sourceFile, line, character, str);
        }

        public static void WritePrompt(string str)
        {
            App.WritePrompt(str);
        }

        public static void WritePrompt(ConsoleColorExt colour, string str)
        {
            App.WritePrompt(colour, str);
        }

        public static void WritePrompt(ConsoleThemeColor colour, string str)
        {
            App.WritePrompt(colour, str);
        }

        public static void WriteStackTrace(string trace)
        {
            App.WriteStackTrace(trace);
        }

        public static void WriteWarning(int id, string str)
        {
            App.WriteWarning(id, str);
        }

        public static void WriteWarning(ConsoleColorExt colour, int id, string str)
        {
            App.WriteWarning(colour, id, str);
        }

        public static void WriteWarning(ConsoleThemeColor colour, int id, string str)
        {
            App.WriteWarning(colour, id, str);
        }

        public static void WriteWarning(ConsoleColorExt colour, int id, string sourceFile, int line, int character, string str)
        {
            App.WriteWarning(colour, id, sourceFile, line, character, str);
        }

        public static void WriteWarning(ConsoleThemeColor colour, int id, string sourceFile, int line, int character, string str)
        {
            App.WriteWarning(colour, id, sourceFile, line, character, str);
        }

        public static void WriteWrapped(ConsoleColorExt colour, string message, int paddingLeft, int paddingRight)
        {
            App.WriteWrapped(colour, message, paddingLeft, paddingRight);
        }

        public static void WriteWrapped(ConsoleThemeColor colour, string message, int paddingLeft, int paddingRight)
        {
            App.WriteWrapped(colour, message, paddingLeft, paddingRight);
        }

        public static string ApplicationBuildReportPrefix
        {
            get
            {
                return App.ApplicationBuildReportPrefix;
            }
            set
            {
                App.ApplicationBuildReportPrefix = value;
            }
        }

        public static ConsoleColorExt BackgroundColor
        {
            get
            {
                return App.BackgroundColor;
            }
            set
            {
                if (value != ConsoleColorExt.Inhreit)
                {
                    App.BackgroundColor = value;
                }
            }
        }

        public static ConsoleThemeColor BackgroundThemeColor
        {
            set
            {
                App.BackgroundThemeColor = value;
            }
        }

        public static int BufferHeight
        {
            get
            {
                return App.BufferHeight;
            }
            set
            {
                App.BufferHeight = value;
            }
        }

        public static int BufferWidth
        {
            get
            {
                return App.BufferWidth;
            }
            set
            {
                App.BufferWidth = value;
            }
        }

        public static bool CanManipulateBuffer
        {
            get
            {
                return App.CanManipulateBuffer;
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
                return App.CursorLeft;
            }
            set
            {
                App.CursorLeft = value;
            }
        }

        public static int CursorSize
        {
            get
            {
                return App.CursorSize;
            }
            set
            {
                App.CursorSize = value;
            }
        }

        public static int CursorTop
        {
            get
            {
                return App.CursorTop;
            }
            set
            {
                App.CursorTop = value;
            }
        }

        public static bool DefaultPromptAnswer
        {
            get
            {
                return App.DefaultPromptAnswer;
            }
            set
            {
                App.DefaultPromptAnswer = value;
            }
        }

        public static ConsoleColorExt ForegroundColor
        {
            get
            {
                return App.ForegroundColor;
            }
            set
            {
                if (value != ConsoleColorExt.Inhreit)
                {
                    App.ForegroundColor = value;
                }
            }
        }

        public static ConsoleThemeColor ForegroundThemeColor
        {
            set
            {
                App.ForegroundThemeColor = value;
            }
        }

        public static bool IsBuildMode
        {
            get
            {
                return App.IsBuildMode;
            }
            set
            {
                App.IsBuildMode = value;
            }
        }

        public static bool ReportWarnings
        {
            get
            {
                return App.ReportWarnings;
            }
            set
            {
                App.ReportWarnings = value;
            }
        }

        public static ConsoleColorTheme Theme
        {
            get
            {
                return App.Theme;
            }
            set
            {
                App.Theme = value;
            }
        }

        public static bool UseDefaultPromptAnswer
        {
            get
            {
                return App.UseDefaultPromptAnswer;
            }
            set
            {
                App.UseDefaultPromptAnswer = value;
            }
        }

        public static ConsoleVerbosity Verbosity
        {
            get
            {
                return App.Verbosity;
            }
            set
            {
                App.Verbosity = value;
            }
        }

        public static bool WarningsAsErrors
        {
            get
            {
                return App.WarningsAsErrors;
            }
            set
            {
                App.WarningsAsErrors = value;
            }
        }
    }
}

