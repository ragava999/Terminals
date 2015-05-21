namespace Terminals.Plugins.Text.Panels.OptionPanels
{
    using Terminals.Connection.Panels.OptionPanels;
    using Terminals.Plugins.Text.Connection;
    using Terminals.Connection.Manager;

    public class TextEnableForProtocolsOptionPanel : EnableProtocolOptionPanel
    {
        #region Public Methods (1)
        public override string DefaultProtocolName
        {
            get
            {
                return typeof(TextConnection).GetProtocolName();
            }
        }
        #endregion
    }
}