namespace Terminals.CommandLine
{
    using Kohl.Framework.Localization;

    /// <summary>
    ///     Allows control of command line parsing.
    ///     Attach this attribute to instance fields of types used
    ///     as the destination of command line argument parsing.
    ///     This class supports multilanguage help texts.
    /// </summary>
    public class LocalizedArgumentAttribute : ArgumentAttribute
    {
        /// <summary>
        ///     Allows control of command line parsing.
        /// </summary>
        /// <param name="type"> Specifies the error checking to be done on the argument. </param>
        public LocalizedArgumentAttribute(ArgumentType type) : base(type)
        {
        }

        public new string HelpText
        {
            get { return Localization.Text(base.HelpText); }
            set { base.HelpText = Localization.Text(value); }
        }
    }
}