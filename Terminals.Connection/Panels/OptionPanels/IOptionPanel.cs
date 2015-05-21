namespace Terminals.Connection.Panels.OptionPanels
{
    // Terminals and framework namespaces
    
    /// <summary>
    ///     All panels in Options dialog should implement both methods, without catching exceptions
    ///     or setting delay save. The save would be handled for all of them.
    /// </summary>
    public class IOptionPanel : System.Windows.Forms.UserControl
    {
        public virtual void LoadSettings()
        {

        }

        public virtual void SaveSettings()
        {

        }

        public IHostingForm IHostingForm { get; set; }
    }
}