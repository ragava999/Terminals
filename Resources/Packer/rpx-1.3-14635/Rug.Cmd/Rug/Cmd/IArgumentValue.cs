namespace Rug.Cmd
{
    using System;

    public interface IArgumentValue
    {
        string ArgumentString();
        bool Parse(ArgumentParser parser, string key, string[] arguments, ref int index);
        void Reset();
        bool SetValue(string value);

        bool Defined { get; set; }

        string Help { get; }

        object ObjectValue { get; }

        string ShortHelp { get; }
    }
}

