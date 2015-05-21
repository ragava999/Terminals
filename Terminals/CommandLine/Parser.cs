using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Kohl.Framework.Info;

using Kohl.Framework.Logging;

namespace Terminals.CommandLine
{
    /// <summary>
    ///     Parser for command line arguments.
    /// 
    ///     The parser specification is infered from the instance fields of the object
    ///     specified as the destination of the parse.
    ///     Valid argument types are: int, uint, string, bool, enums
    ///     Also argument types of Array of the above types are also valid.
    ///  
    ///     Error checking options can be controlled by adding a ArgumentAttribute
    ///     to the instance fields of the destination object.
    /// 
    ///     At most one field may be marked with the DefaultArgumentAttribute
    ///     indicating that arguments without a '-' or '/' prefix will be parsed as that argument.
    /// 
    ///     If not specified then the parser will infer default options for parsing each
    ///     instance field. The default long name of the argument is the field name. The
    ///     default short name is the first character of the long name. Long names and explicitly
    ///     specified short names must be unique. Default short names will be used provided that
    ///     the default short name does not conflict with a long name or an explicitly
    ///     specified short name.
    /// 
    ///     Arguments which are array types are collection arguments. Collection
    ///     arguments can be specified multiple times.
    /// </summary>
    public sealed class Parser
    {
        /// <summary>
        ///     The System Defined new line string.
        /// </summary>
        public const string NewLine = "\r\n";

        private const int StdOutputHandle = -11;
        private const int SpaceBeforeParam = 2;
        private readonly Hashtable argumentMap;
        private readonly ArrayList arguments;
        private readonly Argument defaultArgument;
        private readonly ErrorReporter reporter;

        /// <summary>
        ///     Don't ever call this.
        /// </summary>
        private Parser()
        {
        }

        /// <summary>
        ///     Creates a new command line argument parser.
        /// </summary>
        /// <param name="argumentSpecification"> The type of object to parse. </param>
        /// <param name="reporter"> The destination for parse errors. </param>
        private Parser(Type argumentSpecification, ErrorReporter reporter)
        {
            this.reporter = reporter;
            this.reporter += Log.Error;
            this.arguments = new ArrayList();
            this.argumentMap = new Hashtable();

            foreach (FieldInfo field in argumentSpecification.GetFields())
            {
                if (!field.IsStatic && !field.IsInitOnly && !field.IsLiteral)
                {
                    ArgumentAttribute attribute = GetAttribute(field);
                    if (attribute is DefaultArgumentAttribute)
                    {
                        Debug.Assert(this.defaultArgument == null);
                        this.defaultArgument = new Argument(attribute, field, reporter);
                    }
                    else
                    {
                        this.arguments.Add(new Argument(attribute, field, reporter));
                    }
                }
            }

            // add explicit names to map
            foreach (Argument argument in this.arguments)
            {
                Debug.Assert(!this.argumentMap.ContainsKey(argument.LongName));
                this.argumentMap[argument.LongName] = argument;
                if (argument.ExplicitShortName)
                {
                    if (!string.IsNullOrEmpty(argument.ShortName))
                    {
                        Debug.Assert(!this.argumentMap.ContainsKey(argument.ShortName));
                        this.argumentMap[argument.ShortName] = argument;
                    }
                    else
                    {
                        argument.ClearShortName();
                    }
                }
            }

            // add implicit names which don't collide to map
            foreach (Argument argument in this.arguments)
            {
                if (!argument.ExplicitShortName)
                {
                    if (!string.IsNullOrEmpty(argument.ShortName) &&
                        !this.argumentMap.ContainsKey(argument.ShortName))
                        this.argumentMap[argument.ShortName] = argument;
                    else
                        argument.ClearShortName();
                }
            }
        }

        /// <summary>
        ///     Does this parser have a default argument.
        /// </summary>
        /// <value> Does this parser have a default argument. </value>
        private bool HasDefaultArgument
        {
            get { return this.defaultArgument != null; }
        }

        /// <summary>
        ///     Parses Command Line Arguments. Displays usage message to Console.Out
        ///     if /?, /help or invalid arguments are encounterd.
        ///     Errors are output on Console.Error.
        ///     Use ArgumentAttributes to control parsing behaviour.
        /// </summary>
        /// <param name="arguments"> The actual arguments. </param>
        /// <param name="destination"> The resulting parsed arguments. </param>
        /// <returns> true if no errors were detected. </returns>
        public static bool ParseArgumentsWithUsage(string[] arguments, object destination)
        {
            if (ParseHelp(arguments) || !ParseArguments(arguments, destination))
            {
                // error encountered in arguments. Display usage message
                Console.Write(ArgumentsUsage(destination.GetType()));
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Parses Command Line Arguments. 
        ///     Errors are output on Console.Error.
        ///     Use ArgumentAttributes to control parsing behaviour.
        /// </summary>
        /// <param name="arguments"> The actual arguments. </param>
        /// <param name="destination"> The resulting parsed arguments. </param>
        /// <returns> true if no errors were detected. </returns>
        public static bool ParseArguments(string[] arguments, object destination)
        {
            return ParseArguments(arguments, destination, Console.Error.WriteLine);
        }

        /// <summary>
        ///     Parses Command Line Arguments. 
        ///     Use ArgumentAttributes to control parsing behaviour.
        /// </summary>
        /// <param name="arguments"> The actual arguments. </param>
        /// <param name="destination"> The resulting parsed arguments. </param>
        /// <param name="reporter"> The destination for parse errors. </param>
        /// <returns> true if no errors were detected. </returns>
        private static bool ParseArguments(string[] arguments, object destination, ErrorReporter reporter)
        {
            Parser parser = new Parser(destination.GetType(), reporter);
            return parser.Parse(arguments, destination);
        }

        private static void NullErrorReporter(string message)
        {
        }

        /// <summary>
        ///     Checks if a set of arguments asks for help.
        /// </summary>
        /// <param name="args"> Args to check for help. </param>
        /// <returns> Returns true if args contains /? or /help. </returns>
        public static bool ParseHelp(string[] args)
        {
            Parser helpParser = new Parser(typeof (HelpArgument), NullErrorReporter);
            HelpArgument helpArgument = new HelpArgument();
            helpParser.Parse(args, helpArgument);
            return HelpArgument.Help;
        }

        /// <summary>
        ///     Returns a Usage string for command line argument parsing.
        ///     Use ArgumentAttributes to control parsing behaviour.
        ///     Formats the output to the width of the current console window.
        /// </summary>
        /// <param name="argumentType"> The type of the arguments to display usage for. </param>
        /// <returns> Printable string containing a user friendly description of command line arguments. </returns>
        public static string ArgumentsUsage(Type argumentType)
        {
            int screenWidth = GetConsoleWindowWidth();
            if (screenWidth == 0)
                screenWidth = 80;
            return ArgumentsUsage(argumentType, screenWidth);
        }

        /// <summary>
        ///     Returns a Usage string for command line argument parsing.
        ///     Use ArgumentAttributes to control parsing behaviour.
        /// </summary>
        /// <param name="argumentType"> The type of the arguments to display usage for. </param>
        /// <param name="columns"> The number of columns to format the output to. </param>
        /// <returns> Printable string containing a user friendly description of command line arguments. </returns>
        private static string ArgumentsUsage(Type argumentType, int columns)
        {
            return (new Parser(argumentType, null)).GetUsageString(columns);
        }

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleScreenBufferInfo", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput,
                                                             ref ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

        /// <summary>
        ///     Returns the number of columns in the current console window
        /// </summary>
        /// <returns> Returns the number of columns in the current console window </returns>
        private static int GetConsoleWindowWidth()
        {
            ConsoleScreenBufferInfo csbi = new ConsoleScreenBufferInfo();

            GetConsoleScreenBufferInfo(GetStdHandle(StdOutputHandle), ref csbi);
            int screenWidth = csbi.dwSize.x;
            return screenWidth;
        }

        /// <summary>
        ///     Searches a StringBuilder for a character
        /// </summary>
        /// <param name="text"> The text to search. </param>
        /// <param name="value"> The character value to search for. </param>
        /// <param name="startIndex"> The index to stat searching at. </param>
        /// <returns> The index of the first occurence of value or -1 if it is not found. </returns>
        public static int IndexOf(StringBuilder text, char value, int startIndex)
        {
            for (int index = startIndex; index < text.Length; index++)
            {
                if (text[index] == value)
                    return index;
            }

            return -1;
        }

        /// <summary>
        ///     Searches a StringBuilder for a character in reverse
        /// </summary>
        /// <param name="text"> The text to search. </param>
        /// <param name="value"> The character to search for. </param>
        /// <param name="startIndex"> The index to start the search at. </param>
        /// <returns> The index of the last occurence of value in text or -1 if it is not found. </returns>
        public static int LastIndexOf(StringBuilder text, char value, int startIndex)
        {
            for (int index = Math.Min(startIndex, text.Length - 1); index >= 0; index--)
            {
                if (text[index] == value)
                    return index;
            }

            return -1;
        }

        private static ArgumentAttribute GetAttribute(FieldInfo field)
        {
            object[] attributes = field.GetCustomAttributes(typeof (ArgumentAttribute), false);
            if (attributes.Length == 1)
                return (ArgumentAttribute) attributes[0];

            Debug.Assert(attributes.Length == 0);
            return null;
        }

        private void ReportUnrecognizedArgument(string argument)
        {
            this.reporter(string.Format("Unrecognized command line argument '{0}'", argument));
        }

        /// <summary>
        ///     Parses an argument list into an object
        /// </summary>
        /// <param name="args"> </param>
        /// <param name="destination"> </param>
        /// <returns> true if an error occurred or the help screen has been requested. </returns>
        private bool ParseArgumentList(string[] args, object destination)
        {
            bool hadError = false;
            if (args != null)
            {
                foreach (string argument in args)
                {
                    // Skip the program name if we use the Parser.HelpArgument
                    // "CommandLine.Parser.ReportUnrecognizedArgument")
                    // e.g. Error: Das Kommandozeilenargument 'terminals.exe' konnte nicht erkannt werden.
                    // e.g. Error: Unrecognized command line argument 'terminals.exe'
                    if (argument.ToLower().Contains(AssemblyInfo.Title().ToLower() + ".exe") | argument.ToLower().Contains(AssemblyInfo.Title().ToLower() + ".vshost.exe"))
                        continue;

                    // Skip the arguments if we don't parse the HelpArguments, but any program specific arguments
                    if (destination.GetType() != typeof (HelpArgument) &&
                        (argument.ToLower() == "help" || argument == "?" || argument == "/?" || argument == "-?" || argument.ToLower() == "/help" || argument.ToLower() == "-help" || argument.ToLower() == "--help"))
                        continue;

                    if (destination.GetType() == typeof(HelpArgument) && (argument.ToLower() == "help" || argument == "?" || argument == "/?" || argument == "-?" || argument.ToLower() == "/help" || argument.ToLower() == "-help" || argument.ToLower() == "--help"))
                    {
                        HelpArgument.Help = true;
                        continue;
                        //return false;
                    }

                    if (argument.Length > 0)
                    {
                        switch (argument[0])
                        {
                            case '-':
                            case '/':
                                int endIndex = argument.IndexOfAny(new[] {':', '+', '-'}, 1);
                                string option = argument.Substring(1,
                                                                   endIndex == -1 ? argument.Length - 1 : endIndex - 1);
                                string optionArgument;
                                if (option.Length + 1 == argument.Length)
                                {
                                    optionArgument = null;
                                }
                                else if (argument.Length > 1 + option.Length && argument[1 + option.Length] == ':')
                                {
                                    optionArgument = argument.Substring(option.Length + 2);
                                }
                                else
                                {
                                    optionArgument = argument.Substring(option.Length + 1);
                                }

                                Argument arg = (Argument) this.argumentMap[option];

                                // Skip (continue;) any option except the help option if we want to parse the HelpArguments
                                // If we don't want to parse the HelpArguments check if arg==null and either set the value or report an unrecognized argument
                                if (arg == null && destination.GetType() == typeof (HelpArgument))
                                {
                                    continue;
                                }

                                if (arg == null)
                                {
                                    this.ReportUnrecognizedArgument(argument);
                                    hadError = true;
                                }
                                else
                                {
                                    hadError |= !arg.SetValue(optionArgument, destination);
                                }
                                break;
                            case '@':
                                string[] nestedArguments;
                                hadError |= this.LexFileArguments(argument.Substring(1), out nestedArguments);
                                hadError |= this.ParseArgumentList(nestedArguments, destination);
                                break;
                            default:
                                if (this.defaultArgument != null)
                                {
                                    hadError |= !this.defaultArgument.SetValue(argument, destination);
                                }
                                else
                                {
                                    this.ReportUnrecognizedArgument(argument);
                                    hadError = true;
                                }
                                break;
                        }
                    }
                }
            }

            return hadError;
        }

        /// <summary>
        ///     Parses an argument list.
        /// </summary>
        /// <param name="args"> The arguments to parse. </param>
        /// <param name="destination"> The destination of the parsed arguments. </param>
        /// <returns> true if no parse errors were encountered. </returns>
        private bool Parse(string[] args, object destination)
        {
            bool hadError = this.ParseArgumentList(args, destination);

            // check for missing required arguments
            hadError = this.arguments.Cast<Argument>().Aggregate(hadError, (current, arg) => current | arg.Finish(destination));
            if (this.defaultArgument != null)
            {
                hadError |= this.defaultArgument.Finish(destination);
            }

            return !hadError;
        }

        /// <summary>
        ///     A user firendly usage string describing the command line argument syntax.
        /// </summary>
        private string GetUsageString(int screenWidth)
        {
            ArgumentHelpStrings[] strings = this.GetAllHelpStrings();

            int maxParamLen = strings.Select(helpString => helpString.Syntax.Length).Concat(new[] {0}).Max();

            const int minimumNumberOfCharsForHelpText = 10;
            const int minimumHelpTextColumn = 5;
            const int minimumScreenWidth = minimumHelpTextColumn + minimumNumberOfCharsForHelpText;

            int helpTextColumn;
            int idealMinimumHelpTextColumn = maxParamLen + SpaceBeforeParam;
            screenWidth = Math.Max(screenWidth, minimumScreenWidth);
            helpTextColumn = screenWidth < (idealMinimumHelpTextColumn + minimumNumberOfCharsForHelpText) ? minimumHelpTextColumn : idealMinimumHelpTextColumn;

            const string newLine = "\n";
            StringBuilder builder = new StringBuilder();
            foreach (ArgumentHelpStrings helpStrings in strings)
            {
                // add syntax string
                int syntaxLength = helpStrings.Syntax.Length;
                builder.Append(helpStrings.Syntax);

                // start help text on new line if syntax string is too long
                int currentColumn = syntaxLength;
                if (syntaxLength >= helpTextColumn)
                {
                    builder.Append(newLine);
                    currentColumn = 0;
                }

                // add help text broken on spaces
                int charsPerLine = screenWidth - helpTextColumn;
                int index = 0;
                while (index < helpStrings.Help.Length)
                {
                    // tab to start column
                    builder.Append(' ', helpTextColumn - currentColumn);
                    currentColumn = helpTextColumn;

                    // find number of chars to display on this line
                    int endIndex = index + charsPerLine;
                    if (endIndex >= helpStrings.Help.Length)
                    {
                        // rest of text fits on this line
                        endIndex = helpStrings.Help.Length;
                    }
                    else
                    {
                        endIndex = helpStrings.Help.LastIndexOf(' ', endIndex - 1,
                                                                Math.Min(endIndex - index, charsPerLine));
                        if (endIndex <= index)
                        {
                            // no spaces on this line, append full set of chars
                            endIndex = index + charsPerLine;
                        }
                    }

                    // add chars
                    builder.Append(helpStrings.Help, index, endIndex - index);
                    index = endIndex;

                    // do new line
                    AddNewLine(newLine, builder, ref currentColumn);

                    // don't start a new line with spaces
                    while (index < helpStrings.Help.Length && helpStrings.Help[index] == ' ')
                        index++;
                }

                // add newline if there's no help text                
                if (helpStrings.Help.Length == 0)
                {
                    builder.Append(newLine);
                }
            }

            return builder.ToString();
        }

        private static void AddNewLine(string newLine, StringBuilder builder, ref int currentColumn)
        {
            builder.Append(newLine);
            currentColumn = 0;
        }

        private ArgumentHelpStrings[] GetAllHelpStrings()
        {
            ArgumentHelpStrings[] strings = new ArgumentHelpStrings[this.NumberOfParametersToDisplay()];

            int index = 0;
            foreach (Argument arg in this.arguments)
            {
                strings[index] = GetHelpStrings(arg);
                index++;
            }
            strings[index++] = new ArgumentHelpStrings("@<file>","Read response file for more options");
            if (this.defaultArgument != null)
                strings[index++] = GetHelpStrings(this.defaultArgument);

            return strings;
        }

        private static ArgumentHelpStrings GetHelpStrings(Argument arg)
        {
            return new ArgumentHelpStrings(arg.SyntaxHelp, arg.FullHelpText);
        }

        private int NumberOfParametersToDisplay()
        {
            int numberOfParameters = this.arguments.Count + 1;
            if (this.HasDefaultArgument)
                numberOfParameters += 1;
            return numberOfParameters;
        }

        private bool LexFileArguments(string fileName, out string[] arguments)
        {
            string args = null;

            try
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    args = (new StreamReader(file)).ReadToEnd();
                }
            }
            catch (Exception e)
            {
                string msg = string.Format("Error: Can't open command line argument file '{0}' : '{1}'", fileName, e.Message);
                Log.Info(msg, e);
                this.reporter(msg);
                arguments = null;
                return false;
            }

            bool hadError = false;
            ArrayList argArray = new ArrayList();
            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;
            int index = 0;

            // while (index < args.Length)
            try
            {
                while (true)
                {
                    // skip whitespace
                    while (char.IsWhiteSpace(args[index]))
                    {
                        index += 1;
                    }

                    // # - comment to end of line
                    if (args[index] == '#')
                    {
                        index += 1;
                        while (args[index] != '\n')
                        {
                            index += 1;
                        }
                        continue;
                    }

                    // do one argument
                    do
                    {
                        if (args[index] == '\\')
                        {
                            int cSlashes = 1;
                            index += 1;
                            while (index == args.Length && args[index] == '\\')
                            {
                                cSlashes += 1;
                            }

                            if (index == args.Length || args[index] != '"')
                            {
                                currentArg.Append('\\', cSlashes);
                            }
                            else
                            {
                                currentArg.Append('\\', (cSlashes >> 1));
                                if (0 != (cSlashes & 1))
                                {
                                    currentArg.Append('"');
                                }
                                else
                                {
                                    inQuotes = !inQuotes;
                                }
                            }
                        }
                        else if (args[index] == '"')
                        {
                            inQuotes = !inQuotes;
                            index += 1;
                        }
                        else
                        {
                            currentArg.Append(args[index]);
                            index += 1;
                        }
                    } while (!char.IsWhiteSpace(args[index]) || inQuotes);
                    argArray.Add(currentArg.ToString());
                    currentArg.Length = 0;
                }
            }
            catch (IndexOutOfRangeException exc)
            {
                // got EOF 
                if (inQuotes)
                {
                    string msg = string.Format("Error: Unbalanced '\"' in command line argument file '{0}'", fileName);
                    Log.Info(msg, exc);
                    this.reporter(msg);
                    hadError = true;
                }
                else if (currentArg.Length > 0)
                {
                    // valid argument can be terminated by EOF
                    argArray.Add(currentArg.ToString());
                }
            }

            arguments = (string[]) argArray.ToArray(typeof (string));
            return hadError;
        }

        private static string LongName(ArgumentAttribute attribute, FieldInfo field)
        {
            return (attribute == null || attribute.DefaultLongName) ? field.Name : attribute.LongName;
        }

        private static string ShortName(ArgumentAttribute attribute, FieldInfo field)
        {
            if (attribute is DefaultArgumentAttribute)
                return null;
            if (!ExplicitShortName(attribute))
                return LongName(attribute, field).Substring(0, 1);
            return attribute.ShortName;
        }

        private static string HelpText(ArgumentAttribute attribute, FieldInfo field)
        {
            if (attribute == null)
                return null;
            return attribute.HelpText;
        }

        private static bool HasHelpText(ArgumentAttribute attribute)
        {
            return (attribute != null && attribute.HasHelpText);
        }

        private static bool ExplicitShortName(ArgumentAttribute attribute)
        {
            return (attribute != null && !attribute.DefaultShortName);
        }

        private static object DefaultValue(ArgumentAttribute attribute, FieldInfo field)
        {
            return (attribute == null || !attribute.HasDefaultValue) ? null : attribute.DefaultValue;
        }

        private static Type ElementType(FieldInfo field)
        {
            if (IsCollectionType(field.FieldType))
                return field.FieldType.GetElementType();
            return null;
        }

        private static ArgumentType Flags(ArgumentAttribute attribute, FieldInfo field)
        {
            if (attribute != null)
                return attribute.Type;
            if (IsCollectionType(field.FieldType))
                return ArgumentType.MultipleUnique;
            return ArgumentType.AtMostOnce;
        }

        private static bool IsCollectionType(Type type)
        {
            return type.IsArray;
        }

        private static bool IsValidElementType(Type type)
        {
            return type != null && (
                                       type == typeof (int) ||
                                       type == typeof (uint) ||
                                       type == typeof (string) ||
                                       type == typeof (bool) ||
                                       type.IsEnum);
        }

        private class Argument
        {
            private readonly ArrayList collectionValues;
            private readonly object defaultValue;
            private readonly Type elementType;
            private readonly bool explicitShortName;
            private readonly FieldInfo field;
            private readonly ArgumentType flags;
            private readonly bool hasHelpText;
            private readonly string helpText;
            private readonly bool isDefault;
            private readonly string longName;
            private readonly ErrorReporter reporter;
            private bool seenValue;
            private string shortName;

            public Argument(ArgumentAttribute attribute, FieldInfo field, ErrorReporter reporter)
            {
                this.longName = Parser.LongName(attribute, field);
                this.explicitShortName = Parser.ExplicitShortName(attribute);
                this.shortName = Parser.ShortName(attribute, field);
                this.hasHelpText = Parser.HasHelpText(attribute);
                this.helpText = Parser.HelpText(attribute, field);
                this.defaultValue = Parser.DefaultValue(attribute, field);
                this.elementType = ElementType(field);
                this.flags = Flags(attribute, field);
                this.field = field;
                this.seenValue = false;
                this.reporter = reporter;
                this.isDefault = attribute != null && attribute is DefaultArgumentAttribute;

                if (this.IsCollection)
                {
                    this.collectionValues = new ArrayList();
                }
            }

            private Type ValueType
            {
                get { return this.IsCollection ? this.elementType : this.Type; }
            }

            public string LongName
            {
                get { return this.longName; }
            }

            public bool ExplicitShortName
            {
                get { return this.explicitShortName; }
            }

            public string ShortName
            {
                get { return this.shortName; }
            }

            public bool HasShortName
            {
                get { return this.shortName != null; }
            }

            public bool HasHelpText
            {
                get { return this.hasHelpText; }
            }

            public string HelpText
            {
                get { return this.helpText; }
            }

            public object DefaultValue
            {
                get { return this.defaultValue; }
            }

            public bool HasDefaultValue
            {
                get { return null != this.defaultValue; }
            }

            public string FullHelpText
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    if (this.HasHelpText)
                    {
                        builder.Append(this.HelpText);
                    }
                    if (this.HasDefaultValue)
                    {
                        if (builder.Length > 0)
                            builder.Append(" ");
                        builder.Append("Default value:'");
                        this.AppendValue(builder, this.DefaultValue);
                        builder.Append('\'');
                    }
                    if (this.HasShortName)
                    {
                        if (builder.Length > 0)
                            builder.Append(" ");
                        builder.Append("(Short Form /");
                        builder.Append(this.ShortName);
                        builder.Append(")");
                    }
                    return builder.ToString();
                }
            }

            public string SyntaxHelp
            {
                get
                {
                    StringBuilder builder = new StringBuilder();

                    if (this.IsDefault)
                    {
                        builder.Append("<");
                        builder.Append(this.LongName);
                        builder.Append(">");
                    }
                    else
                    {
                        builder.Append("/");
                        builder.Append(this.LongName);
                        Type valueType = this.ValueType;
                        if (valueType == typeof (int))
                        {
                            builder.Append(":<int>");
                        }
                        else if (valueType == typeof (uint))
                        {
                            builder.Append(":<uint>");
                        }
                        else if (valueType == typeof (bool))
                        {
                            builder.Append("[+|-]");
                        }
                        else if (valueType == typeof (string))
                        {
                            builder.Append(":<string>");
                        }
                        else
                        {
                            Debug.Assert(valueType.IsEnum);

                            builder.Append(":{");
                            bool first = true;
                            foreach (FieldInfo field in valueType.GetFields())
                            {
                                if (field.IsStatic)
                                {
                                    if (first)
                                        first = false;
                                    else
                                        builder.Append('|');
                                    builder.Append(field.Name);
                                }
                            }
                            builder.Append('}');
                        }
                    }

                    return builder.ToString();
                }
            }

            public bool IsRequired
            {
                get { return 0 != (this.flags & ArgumentType.Required); }
            }

            public bool SeenValue
            {
                get { return this.seenValue; }
            }

            public bool AllowMultiple
            {
                get { return 0 != (this.flags & ArgumentType.Multiple); }
            }

            public bool Unique
            {
                get { return 0 != (this.flags & ArgumentType.Unique); }
            }

            public Type Type
            {
                get { return this.field.FieldType; }
            }

            public bool IsCollection
            {
                get { return IsCollectionType(this.Type); }
            }

            public bool IsDefault
            {
                get { return this.isDefault; }
            }

            public bool Finish(object destination)
            {
                if (!this.SeenValue && this.HasDefaultValue)
                {
                    this.field.SetValue(destination, this.DefaultValue);
                }
                if (this.IsCollection)
                {
                    this.field.SetValue(destination, this.collectionValues.ToArray(this.elementType));
                }

                return this.ReportMissingRequiredArgument();
            }

            private bool ReportMissingRequiredArgument()
            {
                if (this.IsRequired && !this.SeenValue)
                {
                    if (this.IsDefault)
                        this.reporter(
                            string.Format("Missing required argument '&lt;{0}&gt;'.",
                                          this.LongName));
                    else
                        this.reporter(
                            string.Format("Missing required argument '/{0}'.",
                                          this.LongName));
                    return true;
                }
                return false;
            }

            private void ReportDuplicateArgumentValue(string value)
            {
                this.reporter(string.Format("Duplicate '{0}' argument '{1}'",
                                            this.LongName, value));
            }

            public bool SetValue(string value, object destination)
            {
                if (this.SeenValue && !this.AllowMultiple)
                {
                    this.reporter(string.Format("Duplicate '{0}' argument.", this.LongName));
                    return false;
                }
                this.seenValue = true;

                object newValue;
                if (!this.ParseValue(this.ValueType, value, out newValue))
                    return false;
                if (this.IsCollection)
                {
                    if (this.Unique && this.collectionValues.Contains(newValue))
                    {
                        this.ReportDuplicateArgumentValue(value);
                        return false;
                    }
                    this.collectionValues.Add(newValue);
                }
                else
                {
                    this.field.SetValue(destination, newValue);
                }

                return true;
            }

            private void ReportBadArgumentValue(string value)
            {
                this.reporter(string.Format("'{0}' is not a valid value for the '{1}' command line option.", value,
                                            this.LongName));
            }

            private bool ParseValue(Type type, string stringData, out object value)
            {
                // null is only valid for bool variables
                // empty string is never valid
                if ((stringData != null || type == typeof (bool)) && (stringData == null || stringData.Length > 0))
                {
                    try
                    {
                        if (type == typeof (string))
                        {
                            value = stringData;
                            return true;
                        }
                        if (type == typeof (bool))
                        {
                            if (stringData == null || stringData == "+")
                            {
                                value = true;
                                return true;
                            }
                            if (stringData == "-")
                            {
                                value = false;
                                return true;
                            }
                        }
                        else if (type == typeof (int))
                        {
                            value = int.Parse(stringData);
                            return true;
                        }
                        else if (type == typeof (uint))
                        {
                            value = int.Parse(stringData);
                            return true;
                        }
                        else
                        {
                            Debug.Assert(type.IsEnum);
                            value = Enum.Parse(type, stringData, true);
                            return true;
                        }
                    }
                    catch (Exception exc)
                    {
                        this.reporter("Failed parsing commandline arguments. " + exc.Message);
                        //Terminals.Configuration.Logging.Log.Error("CommandLine.Parser.ParseValue"), exc);
                    }
                }

                this.ReportBadArgumentValue(stringData);
                value = null;
                return false;
            }

            private void AppendValue(StringBuilder builder, object value)
            {
                if (value is string || value is int || value is uint || value.GetType().IsEnum)
                {
                    builder.Append(value);
                }
                else if (value is bool)
                {
                    builder.Append((bool) value ? "+" : "-");
                }
                else
                {
                    bool first = true;
                    foreach (object o in (Array) value)
                    {
                        if (!first)
                        {
                            builder.Append(", ");
                        }
                        this.AppendValue(builder, o);
                        first = false;
                    }
                }
            }

            public void ClearShortName()
            {
                this.shortName = null;
            }
        }

        private struct ArgumentHelpStrings
        {
            public readonly string Help;
            public readonly string Syntax;

            public ArgumentHelpStrings(string syntax, string help)
            {
                this.Syntax = syntax;
                this.Help = help;
            }
        }

        private struct ConsoleScreenBufferInfo
        {
            public Coord DwCursorPosition;
            public Coord DwMaximumWindowSize;
            public Coord dwSize;
            public SmallRect SrWindow;
            public Int16 WAttributes;
        }

        private struct Coord
        {
            public Int16 x;
            public Int16 y;
        }

        private class HelpArgument
        {
            [Argument(ArgumentType.AtMostOnce, ShortName = "?", LongName = "help")] public static bool Help = false;
        }

        private struct SmallRect
        {
            public Int16 Bottom;
            public Int16 Left;
            public Int16 Right;
            public Int16 Top;
        }
    }
}