namespace Terminals.Connection.TabControl
{
    using Configuration.Files.Main.Favorites;

    public class TerminalTabControlItem : TabControlItem
    {
        #region Constructors (1)
        public TerminalTabControlItem(string caption, string name) : base(caption, name, null)
        {
        }
        #endregion

        #region Public Properties (2)
        public ConnectionBase Connection { get; set; }

        public FavoriteConfigurationElement Favorite { get; set; }
        #endregion

        #region Private Methods (1)
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TerminalTabControlItem
            // 
            this.AllowDrop = true;
            this.ResumeLayout(false);
        }
        #endregion
    }
}