using System;
using System.Drawing;
using System.Windows.Forms;

namespace Terminals.Forms.Controls.IPAddressControl
{
    public class IPAddressDotControl : Control
    {
        #region Fields

        private readonly StringFormat _stringFormat;
        private bool _backColorChanged;
        private bool _readOnly;

        private SizeF _sizeText;

        #endregion

        #region Constructors

        public IPAddressDotControl()
        {
            this.Text = IPAddressControl.FieldSeparator;

            this._stringFormat = StringFormat.GenericTypographic;
            this._stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

            this.BackColor = SystemColors.Window;
            this.Size = this.MinimumSize;
            this.TabStop = false;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            this.SetStyle(ControlStyles.FixedHeight, true);
            this.SetStyle(ControlStyles.FixedWidth, true);
        }

        #endregion

        #region Public Properties

        public override Size MinimumSize
        {
            get
            {
                using (Graphics g = Graphics.FromHwnd(this.Handle))
                {
                    this._sizeText = g.MeasureString(this.Text, this.Font, -1, this._stringFormat);
                }

                // MeasureString() cuts off the bottom pixel for descenders no matter
                // which StringFormatFlags are chosen.  This doesn't matter for '.' but
                // it's here in case someone wants to modify the text.
                //
                this._sizeText.Height += 1F;

                return this._sizeText.ToSize();
            }
        }

        public bool ReadOnly
        {
            private get { return this._readOnly; }
            set
            {
                this._readOnly = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return this.Text;
        }

        #endregion

        #region Protected Methods

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.Size = this.MinimumSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Color backColor = this.BackColor;

            if (!this._backColorChanged)
            {
                if (!this.Enabled || this.ReadOnly)
                    backColor = SystemColors.Control;
            }

            Color textColor = this.ForeColor;

            if (!this.Enabled)
                textColor = SystemColors.GrayText;
            else if (this.ReadOnly)
            {
                if (!this._backColorChanged)
                    textColor = SystemColors.WindowText;
            }

            using (SolidBrush backgroundBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);
            }

            using (SolidBrush foreBrush = new SolidBrush(textColor))
            {
                float x = this.ClientRectangle.Width/2F - this._sizeText.Width/2F;
                e.Graphics.DrawString(this.Text, this.Font, foreBrush,
                                      new RectangleF(x, 0F, this._sizeText.Width, this._sizeText.Height),
                                      this._stringFormat);
            }
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            this.BackColor = this.Parent.BackColor;
            this._backColorChanged = true;
        }

        protected override void OnParentForeColorChanged(EventArgs e)
        {
            base.OnParentForeColorChanged(e);
            this.ForeColor = this.Parent.ForeColor;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Size = this.MinimumSize;
        }

        #endregion
    }
}