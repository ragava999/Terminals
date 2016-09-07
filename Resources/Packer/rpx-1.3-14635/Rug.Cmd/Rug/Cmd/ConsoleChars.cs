namespace Rug.Cmd
{
    using System;

    public static class ConsoleChars
    {
        public static readonly char[] Arrows = new char[] { '▲', '▼', '◄', '►' };
        public static readonly char[] DoubleLines = new char[] { '╔', '═', '╗', '║', ' ', '║', '╚', '═', '╝' };
        public static readonly string[] LineEndSplits = new string[] { Environment.NewLine, "\n" };
        public static readonly char[] Shades = new char[] { ' ', '░', '▒', '▓', '█', '▀', '▄', '▌', '▐' };
        public static readonly char[] SingleLines = new char[] { '┌', '─', '┐', '│', ' ', '│', '└', '─', '┘' };

        public static char GetArrow(ConsoleArrows arrow)
        {
            return Arrows[(int) arrow];
        }

        public static char GetLineChar(ConsoleLineStyle style, LineChars edge)
        {
            switch (style)
            {
                case ConsoleLineStyle.None:
                    return ' ';

                case ConsoleLineStyle.Single:
                    return SingleLines[(int) edge];

                case ConsoleLineStyle.Double:
                    return DoubleLines[(int) edge];

                case ConsoleLineStyle.Block:
                    return Shades[4];
            }
            return ' ';
        }

        public static char GetMathsChar(ConsoleMathsChars @char)
        {
            if (@char == ConsoleMathsChars.PlusMinus)
            {
                return '\x00b1';
            }
            return '?';
        }

        public static char GetShade(ConsoleShade shade)
        {
            return Shades[(int) shade];
        }
    }
}

