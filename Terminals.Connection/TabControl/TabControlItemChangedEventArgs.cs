namespace Terminals.Connection.TabControl
{
    // .NET namespaces
    using System;

    public class TabControlItemChangedEventArgs : EventArgs
    {
        readonly TabControlItem item;

        public TabControlItemChangedEventArgs(TabControlItem item)
        {
            this.item = item;
        }

        public TabControlItem Item
        {
            get { return this.item; }
        }
    }
}