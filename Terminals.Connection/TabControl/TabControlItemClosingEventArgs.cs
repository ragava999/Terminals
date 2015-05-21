namespace Terminals.Connection.TabControl
{
    // .NET namespaces
    using System;

    public class TabControlItemClosingEventArgs : EventArgs
    {
        public TabControlItemClosingEventArgs()
        {
            Cancel = false;
        }

        public bool Cancel { get; set; }
    }
}
