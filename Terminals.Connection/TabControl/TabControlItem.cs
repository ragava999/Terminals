namespace Terminals.Connection.TabControl
{
    // .NET namespace
    using System;
    using System.Windows.Forms;
    using System.Drawing;
    using System.ComponentModel;

    // Terminals and framework namespaces
    using Configuration.Files.Main.Favorites;

    [ToolboxItem(false)]
    [DefaultProperty("Title")]
    [DefaultEvent("Changed")]
    public class TabControlItem : Panel
    {
        #region Private Fields (2)
        private bool visible = true;
        private string title = string.Empty;
        #endregion

        #region Public Events (1)
        public event EventHandler Changed;
        #endregion

        #region Public Properties (8)
        public Color TabColor { get; set; }

        [DefaultValue(true)]
        public new bool Visible
        {
            get { return this.visible; }
            set
            {
                if (this.visible == value)
                    return;

                this.visible = value;
                this.OnChanged();
            }
        }

        [Browsable(false), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDrawn { get; set; }

        [DefaultValue(true)]
        public bool CanClose { get; private set; }

        public RectangleF StripRect { get; internal set; }

        [DefaultValue("Name")]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (this.title == value)
                    return;

                this.title = value;
                this.OnChanged();
            }
        }

        [DefaultValue("Name")]
        public string ToolTipText { get; set; }

        /// <summary>
        /// Gets and sets a value indicating if the page is selected.
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        public bool Selected { get; set; }
        #endregion

        #region Constructors (2)
        public TabControlItem() : this(string.Empty, string.Empty, null)
        {
        }

        protected TabControlItem(string caption, string name, Control displayControl) 
        {
            ToolTipText = string.Empty;
            StripRect = Rectangle.Empty;
            CanClose = true;
            IsDrawn = false;

            this.TabColor = FavoriteConfigurationElement.TranslateColor(FavoriteConfigurationElement.DefaultColor);

            this.Name = name;
            this.Selected = false;
            this.Visible = true;
            this.BorderStyle = BorderStyle.None;

            this.UpdateText(caption, displayControl);

            //Add to controls
            this.Controls.Add(displayControl);
        }
        #endregion

        #region Public Methods (1)
        /// <summary>
        /// Return a string representation of page.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("TabControlItem:{0}", this.Title);
        }
        #endregion

        #region Internal Methods (1)
        internal bool LocationIsInTitle(Point mouseLocation)
        {
            return (this.StripRect.X + this.StripRect.Width - 1) > mouseLocation.X && (this.StripRect.Y + this.StripRect.Height - 1) > mouseLocation.Y;
        }
        #endregion

        #region Private Properties (2)
        private void UpdateText(string caption, Control displayControl)
        {
            if (displayControl != null && displayControl is ICaptionSupport)
            {
                ICaptionSupport capControl = displayControl as ICaptionSupport;
                this.Title = capControl.Caption;
            }
            else if (caption != null && caption.Length <= 0 && displayControl != null)
            {
                this.Title = displayControl.Text;
            }
            else if (caption != null)
            {
                this.Title = caption;
            }
            else
            {
                this.Title = string.Empty;
            }
        }

        private void OnChanged()
        {
            if (this.Changed != null)
                this.Changed(this, EventArgs.Empty);
        }
        #endregion       
    }
}
