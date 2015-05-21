namespace Rug.Cmd
{
    using System;

    public abstract class BaseArgument : IArgumentValue
    {
        private bool m_Defined;
        private string m_Help;
        private string m_ShortHelp;

        public BaseArgument(string shortHelp, string help)
        {
            this.m_ShortHelp = shortHelp;
            this.m_Help = help;
        }

        public abstract string ArgumentString();
        public abstract bool Parse(ArgumentParser parser, string key, string[] arguments, ref int index);
        public virtual void Reset()
        {
            this.m_Defined = false;
        }

        public abstract bool SetValue(string value);

        public bool Defined
        {
            get
            {
                return this.m_Defined;
            }
            set
            {
                this.m_Defined = value;
            }
        }

        public virtual string Help
        {
            get
            {
                return this.m_Help;
            }
        }

        public abstract object ObjectValue { get; }

        public virtual string ShortHelp
        {
            get
            {
                return this.m_ShortHelp;
            }
        }
    }
}

