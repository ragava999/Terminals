namespace Terminals.Connection.TabControl
{
    // .NET namespaces
    using System;
    using System.ComponentModel;

    public sealed class TabControlItemCollection : CollectionWithEvents
    {
        #region Private Fields (1)
        private int lockUpdate;
        #endregion

        #region Public Events (1)
        [Browsable(false)]
        public event CollectionChangeEventHandler CollectionChanged;
        #endregion

        #region Constructors (1)
        public TabControlItemCollection()
        {
            this.lockUpdate = 0;
        }
        #endregion

        #region Public Properties (5)
        public TabControlItem this[int index]
        {
            get
            {
                if (index < 0 || this.List.Count - 1 < index)
                    return null;

                return (TabControlItem)this.List[index];
            }
        }

        [Browsable(false)]
        public int VisibleCount
        {
            get
            {
                int count = this.Count, res = 0;
                if (count == 0) return 0;
                for (int n = 0; n < count; n++)
                {
                    if (this[n].Visible) res++;
                }
                return res;
            }
        }

        [Browsable(false)]
        public int DrawnCount
        {
            get
            {
                int count = this.Count, res = 0;
                if (count == 0) return 0;
                for (int n = 0; n < count; n++)
                {
                    if (this[n].IsDrawn) res++;
                }
                return res;
            }
        }

        public TabControlItem LastVisible
        {
            get
            {
                for (int n = this.Count - 1; n > 0; n--)
                {
                    if (this[n].Visible)
                        return this[n];
                }

                return null;
            }
        }

        public TabControlItem FirstVisible
        {
            get
            {
                for (int n = 0; n < this.Count; n++)
                {
                    if (this[n].Visible)
                        return this[n];
                }

                return null;
            }
        }
        #endregion

        #region Public Methods (5)
        public TabControlItem MoveTo(int newIndex, TabControlItem item)
        {
            int currentIndex = this.List.IndexOf(item);
            if (currentIndex >= 0)
            {
                this.RemoveAt(currentIndex);
                this.Insert(newIndex, item);

                return item;
            }

            return null;
        }

        public void Add(TabControlItem item)
        {
            if (!this.List.Contains(item))
                this.List.Add(item);
        }

        public void Remove(TabControlItem item)
        {
            if (this.List.Contains(item))
                this.List.Remove(item);
        }

        public int IndexOf(TabControlItem item)
        {
            return this.List.IndexOf(item);
        }

        public bool Contains(TabControlItem item)
        {
            return this.List.Contains(item);
        }

        public void Insert(int index, TabControlItem item)
        {
            if (this.Contains(item)) return;
            this.List.Insert(index, item);
        }
        #endregion

        #region Protected Methods (3)
        protected override void OnInsertComplete(int index, object item)
        {
            TabControlItem itm = item as TabControlItem;
            itm.Changed += new EventHandler(this.OnItem_Changed);
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
        }

        protected override void OnRemove(int index, object item)
        {
            base.OnRemove(index, item);
            TabControlItem itm = item as TabControlItem;
            itm.Changed -= new EventHandler(this.OnItem_Changed);
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
        }

        protected override void OnClear()
        {
            if (this.Count == 0) return;
            this.BeginUpdate();
            try
            {
                for (int n = this.Count - 1; n >= 0; n--)
                {
                    this.RemoveAt(n);
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }
        #endregion

        #region Private Methods (4)
        private void OnItem_Changed(object sender, EventArgs e)
        {
            this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
        }

        private void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }

        private void BeginUpdate()
        {
            this.lockUpdate++;
        }

        private void EndUpdate()
        {
            if (--this.lockUpdate == 0)
                this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }
        #endregion
    }
}
