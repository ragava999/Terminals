namespace Terminals.Connection.TabControl
{
    // .NET namespaces
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    internal class TabControlMenuGlyph
    {
        #region Fields (1)
        private readonly ToolStripProfessionalRenderer renderer;
        #endregion

        #region Properties (2)
        public bool IsMouseOver { get; set; }
        public Rectangle Rect { get; set; }
        #endregion

        #region Constructor (1)
        internal TabControlMenuGlyph(ToolStripProfessionalRenderer renderer)
        {
            IsMouseOver = false;
            Rect = Rectangle.Empty;
            this.renderer = renderer;
        }
        #endregion

        #region Methods (1)
        public void DrawGlyph(Graphics g)
        {
            if (this.IsMouseOver)
            {
                Color fill = this.renderer.ColorTable.ButtonSelectedHighlight; //Color.FromArgb(35, SystemColors.Highlight);
                g.FillRectangle(new SolidBrush(fill), this.Rect);
                Rectangle borderRect = this.Rect;

                borderRect.Width--;
                borderRect.Height--;

                g.DrawRectangle(SystemPens.Highlight, borderRect);
            }

            SmoothingMode bak = g.SmoothingMode;

            g.SmoothingMode = SmoothingMode.Default;

            using (Pen pen = new Pen(Color.Black))
            {
                pen.Width = 2;

                g.DrawLine(pen, new Point(this.Rect.Left + (this.Rect.Width / 3) - 2, this.Rect.Height / 2 - 1),
                    new Point(this.Rect.Right - (this.Rect.Width / 3), this.Rect.Height / 2 - 1));
            }

            g.FillPolygon(Brushes.Black, new Point[]{
                new Point(this.Rect.Left + (this.Rect.Width / 3)-2, this.Rect.Height / 2+2),
                new Point(this.Rect.Right - (this.Rect.Width / 3), this.Rect.Height / 2+2),
                new Point(this.Rect.Left + this.Rect.Width / 2-1,this.Rect.Bottom-4)});

            g.SmoothingMode = bak;
        }
        #endregion
    }
}