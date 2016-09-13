namespace Terminals.Plugins.AutoIt.Panels.OptionPanels
{
    using Terminals.Connection.Panels.OptionPanels;
    using Terminals.Plugins.AutoIt.Connection;
    using Terminals.Connection.Manager;

    public class AutoItEnableForProtocolsOptionPanel : EnableProtocolOptionPanel
    {
        #region Constructors (1)
        public AutoItEnableForProtocolsOptionPanel()
        {
            // Don't load the auto it connection, if the user has selected
            // the auto it editor in the favorites editor combobox.
            //ExcludeProtocols.Add(typeof(AutoItEditorConnection).GetProtocolName());
        }
        #endregion

        #region Public Properties (1)
        public override string DefaultProtocolName
        {
            get
            {
                return typeof(AutoItConnection).GetProtocolName();
            }
        }
        #endregion
    }
}