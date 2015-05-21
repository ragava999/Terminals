namespace Rug.Cmd
{
    using System;

    public class OptionalStringArgument : BaseArgument
    {
        private string m_Name;
        public string Value;

        public OptionalStringArgument(string name, string shortHelp, string help) : base(shortHelp, help)
        {
            this.m_Name = name;
        }

        public override string ArgumentString()
        {
            return (" <" + this.m_Name + ">");
        }

        public override bool Parse(ArgumentParser parser, string key, string[] values, ref int index)
        {
            if ((index + 1) < values.Length)
            {
                string str = values[index + 1];
                string str2 = str;
                if (str.StartsWith("-") || str.StartsWith("+"))
                {
                    str2 = ConsoleChars.GetMathsChar(ConsoleMathsChars.PlusMinus) + key.Substring(1);
                }
                ArgumentKey outKey = null;
                IArgumentValue forKey = null;
                if (parser.ContainsKey(str))
                {
                    forKey = parser.GetForKey(str, out outKey);
                }
                else if (!str2.Equals(str) && parser.ContainsKey(str2))
                {
                    forKey = parser.GetForKey(str2, out outKey);
                }
                if ((outKey == null) || (forKey == null))
                {
                    this.SetValue(values[index + 1]);
                    index++;
                }
            }
            return false;
        }

        public override void Reset()
        {
            this.Value = null;
            base.Reset();
        }

        public override bool SetValue(string value)
        {
            this.Value = value;
            return true;
        }

        public override object ObjectValue
        {
            get
            {
                return this.Value;
            }
        }
    }
}

