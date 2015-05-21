namespace Rug.Cmd
{
    using System;
    using System.Text;

    public class EnumSwitch : BaseArgument
    {
        private Type m_Enumeration;
        public object Value;

        public EnumSwitch(string shortHelp, string help, Type enumeration) : base(shortHelp, help)
        {
            this.m_Enumeration = enumeration;
        }

        public override string ArgumentString()
        {
            return (" " + this.m_Enumeration.Name);
        }

        public override bool Parse(ArgumentParser parser, string key, string[] values, ref int index)
        {
            if ((index + 1) >= values.Length)
            {
                throw new Exception(string.Format(Strings.ArgumentParser_InvalidArgument, key));
            }
            if (!this.SetValue(values[index + 1]))
            {
                throw new Exception(string.Format(Strings.EnumSwitch_Unknown, values[index + 1]));
            }
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
            foreach (string str in Enum.GetNames(this.m_Enumeration))
            {
                if (str.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Value = Enum.Parse(this.m_Enumeration, str);
                    return true;
                }
            }
            return false;
        }

        public Type Enumeration
        {
            get
            {
                return this.m_Enumeration;
            }
        }

        public override string Help
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(string.Format("{0} : ", Strings.EnumSwitch_Posible));
                bool flag = true;
                foreach (string str in Enum.GetNames(this.m_Enumeration))
                {
                    if (flag)
                    {
                        builder.Append(str);
                        flag = false;
                    }
                    else
                    {
                        builder.Append(", " + str);
                    }
                }
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine(base.Help);
                return builder.ToString();
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

