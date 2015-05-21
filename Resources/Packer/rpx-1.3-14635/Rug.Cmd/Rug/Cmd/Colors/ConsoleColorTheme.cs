namespace Rug.Cmd.Colors
{
    using Rug.Cmd;
    using System;
    using System.IO;
    using System.Reflection;

    public sealed class ConsoleColorTheme
    {
        internal ConsoleColorExt[] Mappings;

        public ConsoleColorTheme()
        {
            this.Mappings = new ConsoleColorExt[0x20];
        }

        private ConsoleColorTheme(ConsoleColor foregroundColour, ConsoleColor backgroundColour, ConsoleColorDefaultThemes theme)
        {
            this.Mappings = new ConsoleColorExt[0x20];
            switch (theme)
            {
                case ConsoleColorDefaultThemes.None:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    return;

                case ConsoleColorDefaultThemes.HighContrast:
                    this.SetHighContrastTheme(foregroundColour, backgroundColour);
                    return;
            }
        }

        private ConsoleColorExt GetColor(ConsoleThemeColor color)
        {
            return this.Mappings[(int) color];
        }

        private ConsoleColor GetConsoleColor(ConsoleColor current, ConsoleThemeColor color)
        {
            ConsoleColorExt ext = this.GetColor(color);
            if (ext != ConsoleColorExt.Inhreit)
            {
                return (ConsoleColor) ext;
            }
            return current;
        }

        public static ConsoleColorTheme Load(ConsoleColorDefaultThemes theme)
        {
            return Load((ConsoleColor) RugConsole.ForegroundColor, (ConsoleColor) RugConsole.BackgroundColor, theme);
        }

        public static ConsoleColorTheme Load(ConsoleColor backgroundColour, Stream stream)
        {
            return ConsoleColorThemeDirectory.Read(stream)[backgroundColour];
        }

        public static ConsoleColorTheme Load(ConsoleColor backgroundColour, string path)
        {
            return ConsoleColorThemeDirectory.Read(path)[backgroundColour];
        }

        public static ConsoleColorTheme Load(ConsoleColor foregroundColour, ConsoleColor backgroundColour, ConsoleColorDefaultThemes theme)
        {
            Stream manifestResourceStream;
            Assembly assembly = typeof(ConsoleColorTheme).Assembly;
            switch (theme)
            {
                case ConsoleColorDefaultThemes.None:
                case ConsoleColorDefaultThemes.HighContrast:
                    return new ConsoleColorTheme(foregroundColour, backgroundColour, theme);

                case ConsoleColorDefaultThemes.Simple:
                    manifestResourceStream = assembly.GetManifestResourceStream(typeof(ConsoleColorTheme), "Simple.ctheme");
                    return Load(backgroundColour, manifestResourceStream);

                case ConsoleColorDefaultThemes.Colorful:
                    manifestResourceStream = assembly.GetManifestResourceStream(typeof(ConsoleColorTheme), "Colorful.ctheme");
                    return Load(backgroundColour, manifestResourceStream);
            }
            return new ConsoleColorTheme(foregroundColour, backgroundColour, ConsoleColorDefaultThemes.None);
        }

        public static ConsoleColorTheme Load(ConsoleColor backgroundColour, Type type, string path)
        {
            return ConsoleColorThemeDirectory.Read(type.Assembly.GetManifestResourceStream(type, path))[backgroundColour];
        }

        private void SetHighContrastTheme(ConsoleColor foregroundColour, ConsoleColor backgroundColour)
        {
            switch (backgroundColour)
            {
                case ConsoleColor.Black:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.DarkBlue:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.DarkGreen:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.DarkCyan:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.DarkRed:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.DarkMagenta:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.DarkYellow:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.Gray:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.DarkGray:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.Blue:
                    this.SetNoTheme(ConsoleColor.Yellow, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.White);
                    this.SetIndicatorColors(ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.Yellow);
                    return;

                case ConsoleColor.Green:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.Cyan:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.Red:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.Magenta:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.Yellow:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;

                case ConsoleColor.White:
                    this.SetNoTheme(foregroundColour, backgroundColour);
                    this.SetMessageColors(ConsoleColor.Red, ConsoleColor.DarkYellow, ConsoleColor.Blue);
                    this.SetIndicatorColors(foregroundColour, foregroundColour, foregroundColour);
                    return;
            }
        }

        private void SetIndicatorColors(ConsoleColor Good, ConsoleColor Nutral, ConsoleColor Bad)
        {
            this.SetIndicatorTextColors(Good, Nutral, Bad);
            this.SetIndicatorSubTextColors(Good, Nutral, Bad);
        }

        private void SetIndicatorSubTextColors(ConsoleColor Good, ConsoleColor Nutral, ConsoleColor Bad)
        {
            this[ConsoleThemeColor.SubTextGood] = (ConsoleColorExt) Good;
            this[ConsoleThemeColor.SubTextNutral] = (ConsoleColorExt) Nutral;
            this[ConsoleThemeColor.SubTextBad] = (ConsoleColorExt) Bad;
        }

        private void SetIndicatorTextColors(ConsoleColor Good, ConsoleColor Nutral, ConsoleColor Bad)
        {
            this[ConsoleThemeColor.TextGood] = (ConsoleColorExt) Good;
            this[ConsoleThemeColor.TextNutral] = (ConsoleColorExt) Nutral;
            this[ConsoleThemeColor.TextBad] = (ConsoleColorExt) Bad;
        }

        private void SetMessageColors(ConsoleColor ErrorColor, ConsoleColor WarningColor, ConsoleColor PromptColor)
        {
            this.SetMessageColors1(ErrorColor, WarningColor, PromptColor);
            this.SetMessageColors2(ErrorColor, WarningColor, PromptColor);
        }

        private void SetMessageColors1(ConsoleColor ErrorColor1, ConsoleColor WarningColor1, ConsoleColor PromptColor1)
        {
            this[ConsoleThemeColor.ErrorColor1] = (ConsoleColorExt) ErrorColor1;
            this[ConsoleThemeColor.WarningColor1] = (ConsoleColorExt) WarningColor1;
            this[ConsoleThemeColor.PromptColor1] = (ConsoleColorExt) PromptColor1;
        }

        private void SetMessageColors2(ConsoleColor ErrorColor2, ConsoleColor WarningColor2, ConsoleColor PromptColor2)
        {
            this[ConsoleThemeColor.ErrorColor2] = (ConsoleColorExt) ErrorColor2;
            this[ConsoleThemeColor.WarningColor2] = (ConsoleColorExt) WarningColor2;
            this[ConsoleThemeColor.PromptColor2] = (ConsoleColorExt) PromptColor2;
        }

        private void SetNoTheme(ConsoleColor foregroundColour, ConsoleColor backgroundColour)
        {
            this[ConsoleThemeColor.AppBackground] = (ConsoleColorExt) backgroundColour;
            this[ConsoleThemeColor.PanelBackground] = (ConsoleColorExt) backgroundColour;
            this[ConsoleThemeColor.TitleText] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.TitleText1] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.TitleText2] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.TitleText3] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.Text] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.Text1] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.Text2] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.Text3] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.SubText] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.SubText1] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.SubText2] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.SubText3] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.ErrorColor1] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.ErrorColor2] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.WarningColor1] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.WarningColor2] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.PromptColor1] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.PromptColor2] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.TextGood] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.SubTextGood] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.TextNutral] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.SubTextNutral] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.TextBad] = (ConsoleColorExt) foregroundColour;
            this[ConsoleThemeColor.SubTextBad] = (ConsoleColorExt) foregroundColour;
        }

        public ConsoleColorExt this[ConsoleThemeColor color]
        {
            get
            {
                return this.Mappings[(int) color];
            }
            set
            {
                this.Mappings[(int) color] = value;
            }
        }
    }
}

