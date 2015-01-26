namespace Terminals.Connection.TabControl
{
    // .NET namespace
    using System.Drawing;
    using System.Windows.Forms;

    internal class TabControlCloseButton
    {
        #region Fields (1)
        private readonly ToolStripProfessionalRenderer renderer;
        #endregion

        #region Properties (2)
        public bool IsMouseOver { get; set; }
        public Rectangle Rect { get; set; }
        #endregion

        #region Constructor (1)
        internal TabControlCloseButton(ToolStripProfessionalRenderer renderer)
        {
            Rect = Rectangle.Empty;
            IsMouseOver = false;
            this.renderer = renderer;
        }
        #endregion

        #region Methods (1)
        public void DrawCross(Graphics g)
        {
            if (this.IsMouseOver)
            {
                Color fill = this.renderer.ColorTable.ButtonSelectedHighlight;

                g.FillRectangle(new SolidBrush(fill), this.Rect);

                Rectangle borderRect = this.Rect;

                borderRect.Width--;
                borderRect.Height--;

                g.DrawRectangle(SystemPens.Highlight, borderRect);
            }

            using (Pen pen = new Pen(Color.Black, 1f))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawLine(pen, this.Rect.Left + 3, this.Rect.Top + 4,
                    this.Rect.Right - 6, this.Rect.Bottom - 4);
                g.DrawLine(pen, this.Rect.Left + 4, this.Rect.Top + 4,
                    this.Rect.Right - 5, this.Rect.Bottom - 4);

                g.DrawLine(pen, this.Rect.Right - 6, this.Rect.Top + 4,
                    this.Rect.Left + 3, this.Rect.Bottom - 4);
                g.DrawLine(pen, this.Rect.Right - 5, this.Rect.Top + 4,
                    this.Rect.Left + 4, this.Rect.Bottom - 4);
            }
        }
        #endregion
    }
}
