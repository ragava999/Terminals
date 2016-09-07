namespace Rug.Cmd
{
    using System;

    public class BoolSwitch : BaseArgument
    {
        public bool Value;

        public BoolSwitch(string shortHelp, string help) : base(shortHelp, help)
        {
        }

        public override string ArgumentString()
        {
            return "";
        }

        public override bool Parse(ArgumentParser parser, string key, string[] values, ref int index)
        {
            this.SetValue(bool.TrueString);
            return false;
        }

        public override void Reset()
        {
            this.Value = false;
            base.Reset();
        }

        public override bool SetValue(string value)
        {
            return bool.TryParse(value, out this.Value);
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

