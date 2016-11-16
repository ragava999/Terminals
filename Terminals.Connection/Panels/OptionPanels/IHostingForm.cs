using Terminals.Connection.TabControl;

namespace Terminals.Connection.Panels.OptionPanels
{
    // Terminals and framework namespaces
    
    public interface IHostingForm
    {
        #region Properties (5)
        bool Disposing { get; }
        bool FullScreen { get; set; }
        bool InvokeRequired { get; }
        bool IsDisposed { get; }
		int Width { get; }
		int Height { get; }
        #endregion

        #region Methods (6)
        void SetGrabInput(bool grab);
        void DetachTabToNewWindow(TerminalTabControlItem tabControlToOpen);
        object Invoke(System.Delegate method);
        void RemoveTabPage(TabControlItem tabControlToRemove);
        void RemoveAndUnSelect(TerminalTabControlItem toRemove);
        void TerminalTabPage_DoubleClick(object sender, System.EventArgs e);
        void UpdateControls();
        #endregion
    }
}