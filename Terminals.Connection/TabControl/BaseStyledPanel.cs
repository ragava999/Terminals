namespace Terminals.Connection.TabControl
{
    // .NET namespace
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    [ToolboxItem(false)]
    public class BaseStyledPanel : UserControl
    {
        #region Fields (1)
        private static readonly ToolStripProfessionalRenderer Renderer;
        #endregion

        #region Constructors (2)
        static BaseStyledPanel()
        {
            Renderer = new ToolStripProfessionalRenderer();
        }

        protected BaseStyledPanel()
        {
            // Set painting style for better performance.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }
        #endregion

        #region Methods (1)
        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            Renderer.ColorTable.UseSystemColors = !this.UseThemes;
            this.Invalidate();
        }
        #endregion

        #region Properties (2)
        [Browsable(false)]
        protected ToolStripProfessionalRenderer ToolStripRenderer
        {
            get { return Renderer; }
        }

        [DefaultValue(true)]
        [Browsable(false)]
        private bool UseThemes
        {
            get
            {
                return VisualStyleRenderer.IsSupported && VisualStyleInformation.IsSupportedByOS && Application.RenderWithVisualStyles;
            }
        }
        #endregion
    }
}
