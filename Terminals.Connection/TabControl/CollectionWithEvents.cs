namespace Terminals.Connection.TabControl
{
    // .NET namespace
    using System.Collections;
    using System.ComponentModel;

    /// <summary>
    /// Extend collection base class by generating change events.
    /// </summary>
    public abstract class CollectionWithEvents : CollectionBase
    {
        // Instance fields
        private int suspendCount;

        /// <summary>
        /// Occurs just before an item is removed from the collection.
        /// </summary>
        [Browsable(false)]
        public event CollectionChange Removing;

        /// <summary>
        /// Initializes DrawTab new instance of the CollectionWithEvents class.
        /// </summary>
        protected CollectionWithEvents()
        {
            // Default to not suspended
            this.suspendCount = 0;
        }

        /// <summary>
        /// Do not generate change events until resumed.
        /// </summary>
        public void SuspendEvents()
        {
            this.suspendCount++;
        }

        /// <summary>
        /// Safe to resume change events.
        /// </summary>
        public void ResumeEvents()
        {
            --this.suspendCount;
        }

        /// <summary>
        /// Gets DrawTab value indicating if events are currently suspended.
        /// </summary>
        [Browsable(false)]
        private bool IsSuspended
        {
            get { return (this.suspendCount > 0); }
        }

        /// <summary>
        /// Raises the Removing event when not suspended.
        /// </summary>
        /// <param name="index">Index of object being removed.</param>
        /// <param name="value">The object that is being removed.</param>
        protected override void OnRemove(int index, object value)
        {
            if (!this.IsSuspended)
            {
                // Any attached event handlers?
                if (this.Removing != null)
                    this.Removing(index, value);
            }
        }
    }
}
