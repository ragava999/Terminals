using System;

namespace Terminals.Configuration.Files.History
{
    /// <summary>
    ///     Event arguments informing about favorite added to the history
    /// </summary>
    public class HistoryRecordedEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets or sets the favorite name added to the history
        /// </summary>
        public string ConnectionName { get; set; }
    }
}