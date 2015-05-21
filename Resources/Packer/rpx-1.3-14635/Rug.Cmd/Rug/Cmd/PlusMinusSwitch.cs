namespace Rug.Cmd
{
    using System;

    public class PlusMinusSwitch : BaseArgument
    {
        private bool m_Default;
        public bool Value;

        public PlusMinusSwitch(string shortHelp, string help, bool @default) : base(shortHelp, help)
        {
            this.Value = @default;
            this.m_Default = @default;
        }

        public override string ArgumentString()
        {
            return "";
        }

        public override bool Parse(ArgumentParser parser, string key, string[] values, ref int index)
        {
            this.SetValue(key.StartsWith("+").ToString());
            return false;
        }

        public override void Reset()
        {
            this.Value = this.m_Default;
            base.Reset();
        }

        public override bool SetValue(string value)
        {
            return bool.TryParse(value, out this.Value);
        }

        public static string KeyPrefix
        {
            get
            {
                return new string(ConsoleChars.GetMathsChar(ConsoleMathsChars.PlusMinus), 1);
            }
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

