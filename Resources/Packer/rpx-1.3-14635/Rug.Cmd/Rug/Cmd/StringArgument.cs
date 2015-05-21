namespace Rug.Cmd
{
    using System;

    public class StringArgument : BaseArgument
    {
        private string m_Name;
        public string Value;

        public StringArgument(string name, string shortHelp, string help) : base(shortHelp, help)
        {
            this.m_Name = name;
        }

        public override string ArgumentString()
        {
            return (" " + this.m_Name);
        }

        public override bool Parse(ArgumentParser parser, string key, string[] values, ref int index)
        {
            if ((index + 1) >= values.Length)
            {
                throw new Exception(string.Format(Strings.ArgumentParser_InvalidArgument, key));
            }
            this.SetValue(values[index + 1]);
            index++;
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

