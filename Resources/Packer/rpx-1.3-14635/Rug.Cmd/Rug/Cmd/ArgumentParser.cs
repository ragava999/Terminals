namespace Rug.Cmd
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    public class ArgumentParser
    {
        private string m_AboutText;
        private string m_AboutTextLong;
        private string m_AboutTitleText;
        private string m_AppName;
        private string m_CreditsText;
        private StringListArgument m_DefaultArgument;
        private string m_Description;
        private IArgumentValue m_FirstArgument;
        private bool m_HasApplicationDocument;
        private bool m_HelpMode;
        private string m_LegalText;
        private Dictionary<ArgumentKey, IArgumentValue> m_Switches = new Dictionary<ArgumentKey, IArgumentValue>();
        private string m_UsageText;

        public ArgumentParser(string appName, string description)
        {
            this.m_AppName = appName;
            this.m_Description = description;
        }

        public void Add(string prefix, string key, IArgumentValue value)
        {
            if (this.ContainsName(key))
            {
                throw new Exception(string.Format(Strings.ArgumentParser_Add_AllreadyContainsKey, key));
            }
            int length = 1;
            int num2 = key.Length;
            while (length < num2)
            {
                if (!this.ContainsSymbol(key.Substring(0, length)))
                {
                    this.m_Switches.Add(new ArgumentKey(prefix, key, key.Substring(0, length)), value);
                    return;
                }
                length++;
            }
            throw new Exception(string.Format(Strings.ArgumentParser_Add_CannotMatchSymbolForKey, key));
        }

        public void Add(string prefix, string symbol, string key, IArgumentValue value)
        {
            if (this.ContainsSymbol(symbol))
            {
                throw new Exception(string.Format(Strings.ArgumentParser_Add_SymbolInUse, symbol));
            }
            this.m_Switches.Add(new ArgumentKey(prefix, key, symbol), value);
        }

        public bool ContainsKey(string key)
        {
            foreach (ArgumentKey key2 in this.m_Switches.Keys)
            {
                if (key2.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsName(string name)
        {
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                if (key.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsSymbol(string symbol)
        {
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                if (key.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public IArgumentValue GetForKey(string keyIn, out ArgumentKey outKey)
        {
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                if (key.Equals(keyIn))
                {
                    outKey = key;
                    return this.m_Switches[key];
                }
            }
            outKey = null;
            return null;
        }

        public IArgumentValue GetForName(string name)
        {
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                if (key.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.m_Switches[key];
                }
            }
            return null;
        }

        public string GetIndividualHelpString(ArgumentKey k)
        {
            StringBuilder builder = new StringBuilder();
            IArgumentValue value2 = this.m_Switches[k];
            builder.Append("[" + k.Prefix + k.Symbol.ToUpper() + value2.ArgumentString() + "] ");
            string str = k.Symbol.ToUpper();
            if (k.Name.ToUpper().StartsWith(k.Symbol.ToUpper()))
            {
                str = str + k.Name.Substring(k.Symbol.Length);
            }
            else
            {
                str = str + " (" + k.Name + ")";
            }
            builder.AppendLine("  " + k.Prefix + str);
            builder.AppendLine("  " + value2.ShortHelp);
            builder.AppendLine();
            if (Helper.IsNotNullOrEmpty(value2.Help))
            {
                builder.AppendLine(ConsoleFormatter.StripFormat(value2.Help));
                builder.AppendLine();
            }
            return builder.ToString();
        }

        public string GetLongArgumentsUsage()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                IArgumentValue value2 = this.m_Switches[key];
                string str = key.Symbol.ToUpper();
                if (key.Name.ToUpper().StartsWith(key.Symbol.ToUpper()))
                {
                    str = str + key.Name.Substring(key.Symbol.Length);
                }
                else
                {
                    str = str + " (" + key.Name + ")";
                }
                builder.AppendLine("  " + key.Prefix + str.PadRight(15) + " " + value2.ShortHelp);
            }
            if (this.HasApplicationAbout)
            {
                builder.AppendLine("  " + "/??".PadRight(15) + " " + Strings.ArgumentParser_AboutScreen_Title);
            }
            if (this.HasApplicationDocument)
            {
                builder.AppendLine("  " + "/?D".PadRight(15) + " " + Strings.ArgumentParser_DocumentGenerator_Title);
            }
            return (this.GetShortArgumentsUsage() + builder.ToString());
        }

        public string GetShortArgumentsUsage()
        {
            StringBuilder builder = new StringBuilder();
            new StringBuilder();
            builder.AppendLine(this.m_Description);
            builder.AppendLine("");
            builder.Append(this.m_AppName.ToUpper() + " ");
            if (this.m_FirstArgument != null)
            {
                builder.Append("<" + this.m_FirstArgument.ArgumentString().Trim() + "> ");
            }
            if (this.m_DefaultArgument != null)
            {
                builder.Append("<" + this.m_DefaultArgument.ArgumentString().Trim() + "> ");
            }
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                IArgumentValue value2 = this.m_Switches[key];
                builder.Append("[" + key.Prefix + key.Symbol.ToUpper() + value2.ArgumentString() + "] ");
            }
            if (this.HasApplicationAbout)
            {
                builder.Append("/?? ");
            }
            if (this.HasApplicationDocument)
            {
                builder.Append("/?D <" + Strings.ArgumentParser_Documentation_NameForPath + "> ");
            }
            builder.AppendLine("");
            builder.AppendLine("");
            return builder.ToString();
        }

        public void Parse(string[] args)
        {
            ConsoleColorState colorState = RugConsole.ColorState;
            try
            {
                if (args.Length > 0)
                {
                    int num = 0;
                    StringListArgument firstArgument = null;
                    if (args[0] == "/?")
                    {
                        if (args.Length == 1)
                        {
                            this.WriteLongArgumentsUsage();
                        }
                        else
                        {
                            this.WriteShortArgumentsUsage();
                        }
                        this.m_HelpMode = true;
                        num++;
                    }
                    else if (args[0] == "/??")
                    {
                        this.WriteApplicationAboutToConsole(true);
                        this.m_HelpMode = true;
                        num++;
                    }
                    else
                    {
                        if (args[0].Equals("/?D", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string str;
                            if (args.Length > 1)
                            {
                                str = args[1];
                            }
                            else
                            {
                                str = Path.Combine(Environment.CurrentDirectory, this.m_AppName + " Documentation.txt");
                            }
                            this.WriteApplicationDocumentationToPath(str);
                            this.m_HelpMode = true;
                            return;
                        }
                        if (this.m_FirstArgument != null)
                        {
                            this.m_FirstArgument.SetValue(args[0]);
                            this.m_FirstArgument.Defined = true;
                            if (this.m_FirstArgument is StringListArgument)
                            {
                                firstArgument = this.m_FirstArgument as StringListArgument;
                            }
                            num++;
                        }
                    }
                    int index = num;
                    int length = args.Length;
                    while (index < length)
                    {
                        string str2 = args[index];
                        string str3 = str2;
                        if (str2.StartsWith("-") || str2.StartsWith("+"))
                        {
                            str3 = ConsoleChars.GetMathsChar(ConsoleMathsChars.PlusMinus) + str2.Substring(1);
                        }
                        ArgumentKey outKey = null;
                        IArgumentValue forKey = null;
                        if (this.ContainsKey(str2))
                        {
                            forKey = this.GetForKey(str2, out outKey);
                        }
                        else if (!str3.Equals(str2) && this.ContainsKey(str3))
                        {
                            forKey = this.GetForKey(str3, out outKey);
                        }
                        if ((outKey != null) && (forKey != null))
                        {
                            if (this.m_HelpMode)
                            {
                                this.WriteIndividualHelpToConsole(outKey);
                            }
                            else
                            {
                                forKey.Defined = true;
                                if (forKey.Parse(this, str2, args, ref index))
                                {
                                    firstArgument = forKey as StringListArgument;
                                }
                                else
                                {
                                    firstArgument = null;
                                }
                            }
                        }
                        else if ((firstArgument != null) && !this.m_HelpMode)
                        {
                            firstArgument.SetValue(str2);
                        }
                        else
                        {
                            if ((this.m_DefaultArgument == null) || this.m_HelpMode)
                            {
                                throw new Exception(string.Format(Strings.ArgumentParser_InvalidSwitch, str2));
                            }
                            this.m_DefaultArgument.SetValue(str2);
                        }
                        index++;
                    }
                }
            }
            finally
            {
                RugConsole.ColorState = colorState;
            }
        }

        public void Reset()
        {
            this.m_HelpMode = false;
            if (this.m_FirstArgument != null)
            {
                this.m_FirstArgument.Reset();
            }
            if (this.m_DefaultArgument != null)
            {
                this.m_DefaultArgument.Reset();
            }
            foreach (IArgumentValue value2 in this.m_Switches.Values)
            {
                value2.Reset();
            }
        }

        public bool WasDefined(string name)
        {
            IArgumentValue forName = this.GetForName(name);
            return ((forName != null) && forName.Defined);
        }

        private void WriteApplicationAboutToConsole(bool colourise)
        {
            if (this.m_AboutTitleText != null)
            {
                CmdHelper.WriteSimpleBanner(this.m_AboutTitleText, ' ', RugConsole.Theme[ConsoleThemeColor.AppBackground], RugConsole.Theme[ConsoleThemeColor.TitleText]);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
            }
            if (this.m_AboutText != null)
            {
                RugConsole.WriteInterpreted(ConsoleThemeColor.TitleText, this.m_AboutText, 3, 3);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
            }
            if (this.m_AboutTextLong != null)
            {
                RugConsole.WriteInterpreted(ConsoleThemeColor.Text, this.m_AboutTextLong, 3, 3);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
            }
            if (this.m_CreditsText != null)
            {
                RugConsole.WriteInterpreted(ConsoleThemeColor.Text, this.m_CreditsText, 3, 3);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
            }
            if (this.m_LegalText != null)
            {
                RugConsole.WriteInterpreted(ConsoleThemeColor.SubText2, this.m_LegalText, 3, 3);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
            }
            if (this.m_UsageText != null)
            {
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.TitleText, string.Format(" {0}:", Strings.ArgumentParser_AboutScreen_Usage));
                RugConsole.WriteInterpreted(ConsoleThemeColor.Text1, this.m_UsageText, 3, 3);
                RugConsole.WriteLine(ConsoleVerbosity.Silent);
            }
        }

        public void WriteApplicationDocumentationToPath(string path)
        {
            StringBuilder builder = new StringBuilder();
            if (this.m_AboutTitleText != null)
            {
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_AboutTitleText));
                builder.AppendLine();
            }
            if (this.m_CreditsText != null)
            {
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_CreditsText));
                builder.AppendLine();
            }
            if (this.m_AboutText != null)
            {
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_AboutText));
                builder.AppendLine();
            }
            if (this.m_AboutTextLong != null)
            {
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_AboutTextLong));
                builder.AppendLine();
            }
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine(new string('=', RugConsole.BufferWidth));
            builder.AppendLine();
            builder.AppendLine(ConsoleFormatter.StripFormat(string.Format("{0}:", Strings.ArgumentParser_AboutScreen_Usage)));
            if (this.m_UsageText != null)
            {
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_UsageText));
                builder.AppendLine();
                builder.AppendLine();
            }
            builder.AppendLine(this.GetLongArgumentsUsage());
            builder.AppendLine();
            if ((this.m_FirstArgument != null) && Helper.IsNotNullOrEmpty(this.m_FirstArgument.Help))
            {
                builder.AppendLine(new string('-', RugConsole.BufferWidth));
                builder.AppendLine("<" + this.m_FirstArgument.ArgumentString().Trim() + ">");
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_FirstArgument.Help));
                builder.AppendLine();
            }
            if ((this.m_DefaultArgument != null) && Helper.IsNotNullOrEmpty(this.m_DefaultArgument.Help))
            {
                builder.AppendLine(new string('-', RugConsole.BufferWidth));
                builder.AppendLine("<" + this.m_DefaultArgument.ArgumentString().Trim() + ">");
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_DefaultArgument.Help));
                builder.AppendLine();
            }
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                builder.AppendLine(new string('-', RugConsole.BufferWidth));
                builder.Append(this.GetIndividualHelpString(key));
            }
            if (this.m_LegalText != null)
            {
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine(new string('=', RugConsole.BufferWidth));
                builder.AppendLine(ConsoleFormatter.StripFormat(this.m_LegalText));
                builder.AppendLine();
            }
            try
            {
                File.WriteAllText(path, builder.ToString());
                RugConsole.Write(ConsoleVerbosity.Normal, ConsoleThemeColor.Text1, string.Format(Strings.ArgumentParser_DocumentationWrittenToPath, path));
            }
            catch (Exception exception)
            {
                RugConsole.WriteException(10, string.Format(Strings.Error_0010, path), exception);
            }
        }

        public void WriteIndividualHelpToConsole(ArgumentKey k)
        {
            ConsoleColorState colorState = RugConsole.ColorState;
            IArgumentValue value2 = this.m_Switches[k];
            RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText, new string('-', RugConsole.BufferWidth) + "[");
            RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.TitleText, k.Prefix + k.Symbol.ToUpper());
            RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.Text, value2.ArgumentString());
            RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText, "] ");
            RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.TitleText, "  " + k.Prefix + k.Symbol.ToUpper());
            if (k.Name.ToUpper().StartsWith(k.Symbol.ToUpper()))
            {
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.Text, k.Name.Substring(k.Symbol.Length).PadRight(15 - k.Symbol.Length));
            }
            else
            {
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.Text, (" (" + k.Name + ")").PadRight(15 - k.Symbol.Length));
            }
            RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.Text1, "  " + value2.ShortHelp);
            RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.TitleText, "");
            if (Helper.IsNotNullOrEmpty(value2.Help))
            {
                RugConsole.WriteInterpreted(ConsoleThemeColor.TitleText, value2.Help, 2, 2);
                RugConsole.WriteLine(ConsoleVerbosity.Silent, "");
            }
            RugConsole.ColorState = colorState;
        }

        public void WriteLongArgumentsUsage()
        {
            ConsoleColorState colorState = RugConsole.ColorState;
            this.WriteShortArgumentsUsage();
            if ((this.m_FirstArgument != null) && Helper.IsNotNullOrEmpty(this.m_FirstArgument.Help))
            {
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText2, "  <" + this.m_FirstArgument.ArgumentString().Trim() + ">");
                RugConsole.WriteWrapped(ConsoleThemeColor.Text1, this.m_FirstArgument.Help, 4, 4);
                RugConsole.WriteLine(ConsoleVerbosity.Silent, "");
            }
            if ((this.m_DefaultArgument != null) && Helper.IsNotNullOrEmpty(this.m_DefaultArgument.Help))
            {
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText3, "  <" + this.m_DefaultArgument.ArgumentString().Trim() + ">");
                RugConsole.WriteWrapped(ConsoleThemeColor.Text1, this.m_DefaultArgument.Help, 4, 2);
                RugConsole.WriteLine(ConsoleVerbosity.Silent, "");
            }
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                IArgumentValue value2 = this.m_Switches[key];
                string str = key.Prefix + key.Symbol.ToUpper();
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.TitleText, "  " + str);
                if (key.Name.ToUpper().StartsWith(key.Symbol.ToUpper()))
                {
                    RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.Text, key.Name.Substring(key.Symbol.Length).PadRight(0x12 - str.Length));
                }
                else
                {
                    RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.Text, (" (" + key.Name + ")").PadRight(0x12 - str.Length));
                }
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.Text1, " " + value2.ShortHelp);
            }
            if (this.HasApplicationAbout)
            {
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.WarningColor2, "  " + "/?? ".PadRight(0x12));
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.Text1, " " + Strings.ArgumentParser_AboutScreen_Title);
            }
            if (this.HasApplicationDocument)
            {
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.WarningColor2, "  " + "/?D ".PadRight(0x12));
                RugConsole.WriteLine(ConsoleVerbosity.Silent, ConsoleThemeColor.Text1, " " + Strings.ArgumentParser_DocumentGenerator_Title);
            }
            RugConsole.ColorState = colorState;
        }

        public void WriteShortArgumentsUsage()
        {
            ConsoleColorState colorState = RugConsole.ColorState;
            RugConsole.ForegroundThemeColor = ConsoleThemeColor.TitleText;
            RugConsole.WriteLine(ConsoleVerbosity.Silent, this.m_Description);
            RugConsole.WriteLine(ConsoleVerbosity.Silent);
            int count = this.m_AppName.Length + 1;
            int num2 = count;
            RugConsole.Write(ConsoleVerbosity.Silent, this.m_AppName.ToUpper() + " ");
            if (this.m_FirstArgument != null)
            {
                string str = "<" + this.m_FirstArgument.ArgumentString().Trim() + "> ";
                num2 += str.Length + str.Length;
                if (num2 > RugConsole.BufferWidth)
                {
                    num2 = count;
                    RugConsole.WriteLine();
                    RugConsole.Write(new string(' ', count));
                }
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText2, str);
            }
            if (this.m_DefaultArgument != null)
            {
                string str2 = "<" + this.m_DefaultArgument.ArgumentString().Trim() + "> ";
                num2 += str2.Length;
                if (num2 > RugConsole.BufferWidth)
                {
                    num2 = count + str2.Length;
                    RugConsole.WriteLine();
                    RugConsole.Write(new string(' ', count));
                }
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText3, str2);
            }
            foreach (ArgumentKey key in this.m_Switches.Keys)
            {
                IArgumentValue value2 = this.m_Switches[key];
                int num3 = ((key.Prefix.Length + key.Symbol.Length) + value2.ArgumentString().Length) + 3;
                num2 += num3;
                if (num2 > RugConsole.BufferWidth)
                {
                    num2 = count + num3;
                    RugConsole.WriteLine();
                    RugConsole.Write(new string(' ', count));
                }
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText, "[");
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.TitleText, key.Prefix + key.Symbol.ToUpper());
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.Text, value2.ArgumentString());
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.SubText, "] ");
            }
            if (this.HasApplicationAbout)
            {
                int num4 = 4;
                num2 += num4;
                if (num2 > RugConsole.BufferWidth)
                {
                    num2 = count + num4;
                    RugConsole.WriteLine();
                    RugConsole.Write(new string(' ', count));
                }
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.WarningColor2, "/?? ");
            }
            if (this.HasApplicationDocument)
            {
                int num5 = 11;
                num2 += num5;
                if (num2 > RugConsole.BufferWidth)
                {
                    num2 = count + num5;
                    RugConsole.WriteLine();
                    RugConsole.Write(new string(' ', count));
                }
                RugConsole.Write(ConsoleVerbosity.Silent, ConsoleThemeColor.WarningColor2, "/?D <path> ");
            }
            RugConsole.ColorState = colorState;
            RugConsole.WriteLine(ConsoleVerbosity.Silent);
            RugConsole.WriteLine(ConsoleVerbosity.Silent);
        }

        public string AboutText
        {
            get
            {
                return this.m_AboutText;
            }
            set
            {
                this.m_AboutText = value;
            }
        }

        public string AboutTextLong
        {
            get
            {
                return this.m_AboutTextLong;
            }
            set
            {
                this.m_AboutTextLong = value;
            }
        }

        public string AboutTitleText
        {
            get
            {
                return this.m_AboutTitleText;
            }
            set
            {
                this.m_AboutTitleText = value;
            }
        }

        public string CreditsText
        {
            get
            {
                return this.m_CreditsText;
            }
            set
            {
                this.m_CreditsText = value;
            }
        }

        public StringListArgument DefaultArgument
        {
            get
            {
                return this.m_DefaultArgument;
            }
            set
            {
                this.m_DefaultArgument = value;
            }
        }

        public IArgumentValue FirstArgument
        {
            get
            {
                return this.m_FirstArgument;
            }
            set
            {
                this.m_FirstArgument = value;
            }
        }

        public bool HasApplicationAbout
        {
            get
            {
                if ((((this.m_AboutTitleText == null) && (this.m_AboutText == null)) && ((this.m_AboutTextLong == null) && (this.m_CreditsText == null))) && (this.m_UsageText == null))
                {
                    return (this.m_LegalText != null);
                }
                return true;
            }
        }

        public bool HasApplicationDocument
        {
            get
            {
                return this.m_HasApplicationDocument;
            }
            set
            {
                this.m_HasApplicationDocument = value;
            }
        }

        public bool HelpMode
        {
            get
            {
                return this.m_HelpMode;
            }
        }

        public IArgumentValue this[string key]
        {
            get
            {
                foreach (ArgumentKey key2 in this.m_Switches.Keys)
                {
                    if (key2.Equals(key))
                    {
                        return this.m_Switches[key2];
                    }
                }
                return null;
            }
        }

        public IEnumerable<ArgumentKey> Keys
        {
            get
            {
                return this.m_Switches.Keys;
            }
        }

        public string LegalText
        {
            get
            {
                return this.m_LegalText;
            }
            set
            {
                this.m_LegalText = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_AppName;
            }
        }

        public string UsageText
        {
            get
            {
                return this.m_UsageText;
            }
            set
            {
                this.m_UsageText = value;
            }
        }
    }
}

