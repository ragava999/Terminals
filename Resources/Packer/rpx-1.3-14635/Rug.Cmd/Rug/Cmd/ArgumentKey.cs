namespace Rug.Cmd
{
    using System;

    public class ArgumentKey
    {
        public readonly string Name;
        public readonly string Prefix;
        public readonly string Symbol;

        public ArgumentKey(string prefix, string name, string symbol)
        {
            this.Prefix = prefix;
            this.Name = name;
            this.Symbol = symbol;
        }

        public override bool Equals(object obj)
        {
            if (obj is ArgumentKey)
            {
                return this.Symbol.Equals((obj as ArgumentKey).Symbol, StringComparison.InvariantCultureIgnoreCase);
            }
            if (obj is string)
            {
                string str = obj as string;
                if (str.StartsWith(this.Prefix))
                {
                    string str2 = str.Substring(this.Prefix.Length);
                    if (str2.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase) || str2.Equals(this.Symbol, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}/{1}/{2}", this.Prefix, this.Name, this.Symbol).GetHashCode();
        }
    }
}

