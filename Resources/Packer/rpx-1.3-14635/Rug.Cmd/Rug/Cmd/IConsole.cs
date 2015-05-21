namespace Rug.Cmd
{
    using Rug.Cmd.Colors;
    using System;

    public interface IConsole
    {
        ConsoleKeyInfo PromptForKey(string message, bool intercept, bool throwException);
        string PromptForLine(string message, bool throwException);
        int Read();
        ConsoleKeyInfo ReadKey();
        ConsoleKeyInfo ReadKey(bool intercept);
        string ReadLine();
        void ResetColor();
        void SetCursorPosition(int left, int top);
        bool ShouldWrite(ConsoleVerbosity verbosity);
        void Write(string str);
        void Write(ConsoleColorExt colour, string str);
        void Write(ConsoleThemeColor colour, string str);
        void Write(ConsoleVerbosity level, string str);
        void Write(ConsoleVerbosity level, ConsoleColorExt colour, string str);
        void Write(ConsoleVerbosity level, ConsoleThemeColor colour, string str);
        void WriteError(int id, string str);
        void WriteError(ConsoleColorExt colour, int id, string str);
        void WriteError(ConsoleThemeColor colour, int id, string str);
        void WriteError(int id, string sourceFile, int line, int character, string str);
        void WriteException(int id, Exception ex);
        void WriteException(int id, string title, Exception ex);
        void WriteException(int id, string sourceFile, int line, int character, Exception ex);
        void WriteException(int id, string sourceFile, int line, int character, string title, Exception ex);
        void WriteInterpreted(string buffer);
        void WriteInterpreted(string buffer, int paddingLeft, int paddingRight);
        void WriteInterpreted(ConsoleColorExt colour, string buffer, int paddingLeft, int paddingRight);
        void WriteInterpreted(ConsoleThemeColor colour, string buffer, int paddingLeft, int paddingRight);
        void WriteInterpretedLine(string buffer);
        void WriteLine();
        void WriteLine(ConsoleVerbosity level);
        void WriteLine(string str);
        void WriteLine(ConsoleColorExt colour, string str);
        void WriteLine(ConsoleThemeColor colour, string str);
        void WriteLine(ConsoleVerbosity level, string str);
        void WriteLine(ConsoleVerbosity level, ConsoleColorExt colour, string str);
        void WriteLine(ConsoleVerbosity level, ConsoleThemeColor colour, string str);
        void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, string str);
        void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, string str);
        void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string str);
        void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string str);
        void WriteMessage(ConsoleMessage type, ConsoleColorExt colour, int errorId, string sourceFile, int line, int character, string str);
        void WriteMessage(ConsoleMessage type, ConsoleThemeColor colour, int errorId, string sourceFile, int line, int character, string str);
        void WritePrompt(string str);
        void WritePrompt(ConsoleColorExt colour, string str);
        void WritePrompt(ConsoleThemeColor colour, string str);
        void WriteStackTrace(string trace);
        void WriteWarning(int id, string str);
        void WriteWarning(ConsoleColorExt colour, int id, string str);
        void WriteWarning(ConsoleThemeColor colour, int id, string str);
        void WriteWarning(ConsoleColorExt colour, int id, string sourceFile, int line, int character, string str);
        void WriteWarning(ConsoleThemeColor colour, int id, string sourceFile, int line, int character, string str);
        void WriteWrapped(ConsoleColorExt colour, string message, int paddingLeft, int paddingRight);
        void WriteWrapped(ConsoleThemeColor colour, string message, int paddingLeft, int paddingRight);

        string ApplicationBuildReportPrefix { get; set; }

        ConsoleColorExt BackgroundColor { get; set; }

        ConsoleThemeColor BackgroundThemeColor { set; }

        int BufferHeight { get; set; }

        int BufferWidth { get; set; }

        bool CanManipulateBuffer { get; }

        ConsoleColorState ColorState { get; set; }

        int CursorLeft { get; set; }

        int CursorSize { get; set; }

        int CursorTop { get; set; }

        bool DefaultPromptAnswer { get; set; }

        ConsoleColorExt ForegroundColor { get; set; }

        ConsoleThemeColor ForegroundThemeColor { set; }

        bool IsBuildMode { get; set; }

        bool ReportWarnings { get; set; }

        ConsoleColorTheme Theme { get; set; }

        bool UseDefaultPromptAnswer { get; set; }

        ConsoleVerbosity Verbosity { get; set; }

        bool WarningsAsErrors { get; set; }
    }
}

