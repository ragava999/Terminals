namespace Terminals.Connection.TabControl
{
    // .NET namespaces
    using System;

    public class TabControlMouseOnTitleEventArgs : EventArgs
    {
        #region Private Fields (1)
        private readonly TabControlItem item;
        #endregion

        #region Constructor (1)
        public TabControlMouseOnTitleEventArgs(TabControlItem item)
        {
            this.item = item;
        }
        #endregion

        #region Public Properties (1)
        public TabControlItem Item
        {
            get { return this.item; }
        }
        #endregion
    }
}