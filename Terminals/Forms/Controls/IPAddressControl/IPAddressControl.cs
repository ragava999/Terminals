using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Terminals.Connection.Native;

namespace Terminals.Forms.Controls.IPAddressControl
{
    [Designer(typeof (IPAddressControlDesigner))]
    public class IPAddressControl : ContainerControl
    {
        #region Fields

        private readonly TextBox _referenceTextBox = new TextBox();
        private readonly IPAddressDotControl[] ipAddressDotControls = new IPAddressDotControl[FieldCount - 1];
        private readonly IPAddressFieldControl[] ipAddressFieldControls = new IPAddressFieldControl[FieldCount];
        private Size Fixed3DOffset = new Size(3, 3);
        private Size FixedSingleOffset = new Size(2, 2);
        private bool _autoHeight = true;
        private bool _backColorChanged;
        private BorderStyle _borderStyle = BorderStyle.Fixed3D;
        private bool _focused;
        private bool _hasMouse;
        private bool _readOnly;

        #endregion

        #region Constructors

        public IPAddressControl()
        {
            this.BackColor = SystemColors.Window;

            this.ResetBackColorChanged();

            for (int index = 0; index < this.ipAddressFieldControls.Length; ++index)
            {
                this.ipAddressFieldControls[index] = new IPAddressFieldControl();

                this.ipAddressFieldControls[index].CreateControl();

                this.ipAddressFieldControls[index].FieldIndex = index;
                this.ipAddressFieldControls[index].Name = "FieldControl" + index.ToString(CultureInfo.InvariantCulture);
                this.ipAddressFieldControls[index].Parent = this;

                this.ipAddressFieldControls[index].CedeFocusEvent += this.OnCedeFocus;
                this.ipAddressFieldControls[index].Click += this.OnSubControlClicked;
                this.ipAddressFieldControls[index].DoubleClick += this.OnSubControlDoubleClicked;
                this.ipAddressFieldControls[index].GotFocus += this.OnFieldGotFocus;
                this.ipAddressFieldControls[index].KeyDown += this.OnFieldKeyDown;
                this.ipAddressFieldControls[index].KeyPress += this.OnFieldKeyPressed;
                this.ipAddressFieldControls[index].KeyUp += this.OnFieldKeyUp;
                this.ipAddressFieldControls[index].LostFocus += this.OnFieldLostFocus;
                this.ipAddressFieldControls[index].MouseClick += this.OnSubControlMouseClicked;
                this.ipAddressFieldControls[index].MouseDoubleClick += this.OnSubControlMouseDoubleClicked;
                this.ipAddressFieldControls[index].MouseEnter += this.OnSubControlMouseEntered;
                this.ipAddressFieldControls[index].MouseHover += this.OnSubControlMouseHovered;
                this.ipAddressFieldControls[index].MouseLeave += this.OnSubControlMouseLeft;
                this.ipAddressFieldControls[index].MouseMove += this.OnSubControlMouseMoved;
                this.ipAddressFieldControls[index].PreviewKeyDown += this.OnFieldPreviewKeyDown;
                this.ipAddressFieldControls[index].TextChangedEvent += this.OnFieldTextChanged;

                this.Controls.Add(this.ipAddressFieldControls[index]);

                if (index < (FieldCount - 1))
                {
                    this.ipAddressDotControls[index] = new IPAddressDotControl();

                    this.ipAddressDotControls[index].CreateControl();

                    this.ipAddressDotControls[index].Name = "DotControl" + index.ToString(CultureInfo.InvariantCulture);
                    this.ipAddressDotControls[index].Parent = this;

                    this.ipAddressDotControls[index].Click += this.OnSubControlClicked;
                    this.ipAddressDotControls[index].DoubleClick += this.OnSubControlDoubleClicked;
                    this.ipAddressDotControls[index].MouseClick += this.OnSubControlMouseClicked;
                    this.ipAddressDotControls[index].MouseDoubleClick += this.OnSubControlMouseDoubleClicked;
                    this.ipAddressDotControls[index].MouseEnter += this.OnSubControlMouseEntered;
                    this.ipAddressDotControls[index].MouseHover += this.OnSubControlMouseHovered;
                    this.ipAddressDotControls[index].MouseLeave += this.OnSubControlMouseLeft;
                    this.ipAddressDotControls[index].MouseMove += this.OnSubControlMouseMoved;

                    this.Controls.Add(this.ipAddressDotControls[index]);
                }
            }

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ContainerControl, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.FixedWidth, true);
            this.SetStyle(ControlStyles.FixedHeight, true);

            this._referenceTextBox.AutoSize = true;

            this.Cursor = Cursors.IBeam;

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;

            this.Size = this.MinimumSize;

            this.DragEnter += this.IPAddressControl_DragEnter;
            this.DragDrop += this.IPAddressControl_DragDrop;
        }

        #endregion

        #region Public Constants

        private const int FieldCount = 4;

        public const string FieldMeasureText = "333";

        public const string FieldSeparator = ".";

        #endregion

        #region Public Events

        public event EventHandler<FieldChangedEventArgs> FieldChangedEvent;

        #endregion

        #region Public Properties

        [Browsable(true)]
        public bool AllowInternalTab
        {
            get
            { return this.ipAddressFieldControls.Select(fc => fc.TabStop).FirstOrDefault(); }
            set
            {
                foreach (IPAddressFieldControl fc in this.ipAddressFieldControls)
                {
                    fc.TabStop = value;
                }
            }
        }

        [Browsable(true)]
        public bool AnyBlank
        {
            get
            { return this.ipAddressFieldControls.Any(fc => fc.Blank); }
        }

        [Browsable(true)]
        public bool AutoHeight
        {
            get { return this._autoHeight; }
            set
            {
                this._autoHeight = value;

                if (this._autoHeight)
                    this.AdjustSize();
            }
        }

        [Browsable(false)]
        public int Baseline
        {
            get
            {
                TextMetric textMetric = GetTextMetrics(this.Handle, this.Font);

                int offset = textMetric.tmAscent + 1;

                switch (this.BorderStyle)
                {
                    case BorderStyle.Fixed3D:
                        offset += this.Fixed3DOffset.Height;
                        break;
                    case BorderStyle.FixedSingle:
                        offset += this.FixedSingleOffset.Height;
                        break;
                }

                return offset;
            }
        }

        [Browsable(true)]
        public bool Blank
        {
            get
            { return this.ipAddressFieldControls.All(fc => fc.Blank); }
        }

        [Browsable(true)]
        private BorderStyle BorderStyle
        {
            get { return this._borderStyle; }
            set
            {
                this._borderStyle = value;
                this.AdjustSize();
                this.Invalidate();
            }
        }

        [Browsable(false)]
        public override bool Focused
        {
            get
            { return this.ipAddressFieldControls.Any(fc => fc.Focused); }
        }

        [Browsable(true)]
        public override Size MinimumSize
        {
            get { return this.CalculateMinimumSize(); }
        }

        [Browsable(true)]
        private bool ReadOnly
        {
            get { return this._readOnly; }
            set
            {
                this._readOnly = value;

                foreach (IPAddressFieldControl fc in this.ipAddressFieldControls)
                {
                    fc.ReadOnly = this._readOnly;
                }

                foreach (IPAddressDotControl dc in this.ipAddressDotControls)
                {
                    dc.ReadOnly = this._readOnly;
                }

                this.Invalidate();
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                
                for (int index = 0; index < this.ipAddressFieldControls.Length; ++index)
                {
                    sb.Append(this.ipAddressFieldControls[index].Text);

                    if (index < this.ipAddressDotControls.Length)
                        sb.Append(this.ipAddressDotControls[index].Text);
                }

                return sb.ToString();
            }
            set { this.Parse(value); }
        }

        #endregion

        #region Public Methods

        private void Clear()
        {
            foreach (IPAddressFieldControl fc in this.ipAddressFieldControls)
            {
                fc.Clear();
            }
        }

        public byte[] GetAddressBytes()
        {
            byte[] bytes = new byte[FieldCount];

            for (int index = 0; index < FieldCount; ++index)
            {
                bytes[index] = this.ipAddressFieldControls[index].Value;
            }

            return bytes;
        }

        [SuppressMessage("Microsoft.Naming", "CA1720", Justification = "Prefer to use bytes as a variable name.")]
        public void SetAddressBytes(byte[] bytes)
        {
            this.Clear();

            if (bytes == null)
                return;

            int length = Math.Min(FieldCount, bytes.Length);

            for (int i = 0; i < length; ++i)
            {
                this.ipAddressFieldControls[i].Text = bytes[i].ToString(CultureInfo.InvariantCulture);
            }
        }

        public void SetFieldFocus(int fieldIndex)
        {
            if ((fieldIndex >= 0) && (fieldIndex < FieldCount))
                this.ipAddressFieldControls[fieldIndex].TakeFocus(IPAddressControlDirection.Forward,
                                                                  IPAddressControlSelection.All);
        }

        public void SetFieldRange(int fieldIndex, byte rangeLower, byte rangeUpper)
        {
            if ((fieldIndex >= 0) && (fieldIndex < FieldCount))
            {
                this.ipAddressFieldControls[fieldIndex].RangeLower = rangeLower;
                this.ipAddressFieldControls[fieldIndex].RangeUpper = rangeUpper;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < FieldCount; ++index)
            {
                sb.Append(this.ipAddressFieldControls[index]);

                if (index < this.ipAddressDotControls.Length)
                    sb.Append(this.ipAddressDotControls[index]);
            }

            return sb.ToString();
        }

        #endregion

        #region Protected Methods

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            this._backColorChanged = true;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.AdjustSize();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this._focused = true;
            this.ipAddressFieldControls[0].TakeFocus(IPAddressControlDirection.Forward, IPAddressControlSelection.All);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!this.Focused)
            {
                this._focused = false;
                base.OnLostFocus(e);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this._hasMouse)
            {
                this._hasMouse = true;
                base.OnMouseEnter(e);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.HasMouse)
            {
                base.OnMouseLeave(e);
                this._hasMouse = false;
            }
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

            using (SolidBrush backgroundBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);
            }

            Rectangle rectBorder = new Rectangle(this.ClientRectangle.Left, this.ClientRectangle.Top,
                                                 this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);

            switch (this.BorderStyle)
            {
                case BorderStyle.Fixed3D:

                    if (Application.RenderWithVisualStyles)
                        ControlPaint.DrawVisualStyleBorder(e.Graphics, rectBorder);
                    else
                        ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, Border3DStyle.Sunken);
                    break;

                case BorderStyle.FixedSingle:

                    ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                                            SystemColors.WindowFrame, ButtonBorderStyle.Solid);
                    break;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.AdjustSize();
        }

        #endregion

        #region Private Properties

        private bool HasMouse
        {
            get { return this.DisplayRectangle.Contains(this.PointToClient(MousePosition)); }
        }

        #endregion

        #region Private Methods

        private void AdjustSize()
        {
            Size newSize = this.MinimumSize;

            if (this.Width > newSize.Width)
                newSize.Width = this.Width;

            if (this.Height > newSize.Height)
                newSize.Height = this.Height;

            this.Size = this.AutoHeight ? new Size(newSize.Width, this.MinimumSize.Height) : newSize;

            this.LayoutControls();
        }

        private Size CalculateMinimumSize()
        {
            Size minimumSize = new Size(0, 0);

            foreach (IPAddressFieldControl fc in this.ipAddressFieldControls)
            {
                minimumSize.Width += fc.Width;
                minimumSize.Height = Math.Max(minimumSize.Height, fc.Height);
            }

            foreach (IPAddressDotControl dc in this.ipAddressDotControls)
            {
                minimumSize.Width += dc.Width;
                minimumSize.Height = Math.Max(minimumSize.Height, dc.Height);
            }

            switch (this.BorderStyle)
            {
                case BorderStyle.Fixed3D:
                    minimumSize.Width += 6;
                    minimumSize.Height += (this.GetSuggestedHeight() - minimumSize.Height);
                    break;
                case BorderStyle.FixedSingle:
                    minimumSize.Width += 4;
                    minimumSize.Height += (this.GetSuggestedHeight() - minimumSize.Height);
                    break;
            }

            return minimumSize;
        }

        private int GetSuggestedHeight()
        {
            this._referenceTextBox.BorderStyle = this.BorderStyle;
            this._referenceTextBox.Font = this.Font;
            return this._referenceTextBox.Height;
        }

        [SuppressMessage("Microsoft.Usage", "CA1806", Justification = "What should be done if ReleaseDC() doesn't work?"
            )]
        private static TextMetric GetTextMetrics(IntPtr hwnd, Font font)
        {
            IntPtr hdc = WindowsApi.GetWindowDC(hwnd);

            TextMetric textMetric;
            IntPtr hFont = font.ToHfont();

            try
            {
                IntPtr hFontPrevious = WindowsApi.SelectObject(hdc, hFont);
                WindowsApi.GetTextMetrics(hdc, out textMetric);
                WindowsApi.SelectObject(hdc, hFontPrevious);
            }
            finally
            {
                WindowsApi.ReleaseDC(hwnd, hdc);
                WindowsApi.DeleteObject(hFont);
            }

            return textMetric;
        }

        private void IPAddressControl_DragDrop(object sender, DragEventArgs e)
        {
            this.Text = e.Data.GetData(DataFormats.Text).ToString();
        }

        private void IPAddressControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.Text) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void LayoutControls()
        {
            this.SuspendLayout();

            int difference = this.Width - this.MinimumSize.Width;

            Debug.Assert(difference >= 0);

            int numOffsets = this.ipAddressFieldControls.Length + this.ipAddressDotControls.Length + 1;

            int div = difference/(numOffsets);
            int mod = difference%(numOffsets);

            int[] offsets = new int[numOffsets];

            for (int index = 0; index < numOffsets; ++index)
            {
                offsets[index] = div;

                if (index < mod)
                    ++offsets[index];
            }

            int x = 0;
            int y = 0;

            switch (this.BorderStyle)
            {
                case BorderStyle.Fixed3D:
                    x = this.Fixed3DOffset.Width;
                    y = this.Fixed3DOffset.Height;
                    break;
                case BorderStyle.FixedSingle:
                    x = this.FixedSingleOffset.Width;
                    y = this.FixedSingleOffset.Height;
                    break;
            }

            int offsetIndex = 0;

            x += offsets[offsetIndex++];

            for (int i = 0; i < this.ipAddressFieldControls.Length; ++i)
            {
                this.ipAddressFieldControls[i].Location = new Point(x, y);

                x += this.ipAddressFieldControls[i].Width;

                if (i < this.ipAddressDotControls.Length)
                {
                    x += offsets[offsetIndex++];
                    this.ipAddressDotControls[i].Location = new Point(x, y);
                    x += this.ipAddressDotControls[i].Width;
                    x += offsets[offsetIndex++];
                }
            }

            this.ResumeLayout(false);
        }

        private void OnCedeFocus(Object sender, CedeFocusEventArgs e)
        {
            switch (e.IPAddressControlAction)
            {
                case IPAddressControlAction.Home:

                    this.ipAddressFieldControls[0].TakeFocus(IPAddressControlAction.Home);
                    return;

                case IPAddressControlAction.End:

                    this.ipAddressFieldControls[FieldCount - 1].TakeFocus(IPAddressControlAction.End);
                    return;

                case IPAddressControlAction.Trim:

                    if (e.FieldIndex == 0)
                        return;

                    this.ipAddressFieldControls[e.FieldIndex - 1].TakeFocus(IPAddressControlAction.Trim);
                    return;
            }

            if ((e.IPAddressControlDirection == IPAddressControlDirection.Reverse && e.FieldIndex == 0) ||
                (e.IPAddressControlDirection == IPAddressControlDirection.Forward && e.FieldIndex == (FieldCount - 1)))
                return;

            int fieldIndex = e.FieldIndex;

            if (e.IPAddressControlDirection == IPAddressControlDirection.Forward)
                ++fieldIndex;
            else
                --fieldIndex;

            this.ipAddressFieldControls[fieldIndex].TakeFocus(e.IPAddressControlDirection, e.IPAddressControlSelection);
        }

        private void OnFieldGotFocus(Object sender, EventArgs e)
        {
            if (!this._focused)
            {
                this._focused = true;
                base.OnGotFocus(EventArgs.Empty);
            }
        }

        private void OnFieldKeyDown(Object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void OnFieldKeyPressed(Object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        private void OnFieldPreviewKeyDown(Object sender, PreviewKeyDownEventArgs e)
        {
            this.OnPreviewKeyDown(e);
        }

        private void OnFieldKeyUp(Object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        private void OnFieldLostFocus(Object sender, EventArgs e)
        {
            if (!this.Focused)
            {
                this._focused = false;
                base.OnLostFocus(EventArgs.Empty);
            }
        }

        private void OnFieldTextChanged(Object sender, TextChangedEventArgs e)
        {
            if (null != this.FieldChangedEvent)
            {
                FieldChangedEventArgs args = new FieldChangedEventArgs {FieldIndex = e.FieldIndex, Text = e.Text};
                this.FieldChangedEvent(this, args);
            }

            this.OnTextChanged(EventArgs.Empty);
        }

        private void OnSubControlClicked(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void OnSubControlDoubleClicked(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }

        private void OnSubControlMouseClicked(object sender, MouseEventArgs e)
        {
            this.OnMouseClick(e);
        }

        private void OnSubControlMouseDoubleClicked(object sender, MouseEventArgs e)
        {
            this.OnMouseDoubleClick(e);
        }

        private void OnSubControlMouseEntered(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void OnSubControlMouseHovered(object sender, EventArgs e)
        {
            this.OnMouseHover(e);
        }

        private void OnSubControlMouseLeft(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        private void OnSubControlMouseMoved(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void Parse(String text)
        {
            this.Clear();

            if (null == text)
                return;

            int textIndex = 0;

            int index = 0;

            for (index = 0; index < this.ipAddressDotControls.Length; ++index)
            {
                int findIndex = text.IndexOf(this.ipAddressDotControls[index].Text, textIndex, StringComparison.Ordinal);

                if (findIndex >= 0)
                {
                    this.ipAddressFieldControls[index].Text = text.Substring(textIndex, findIndex - textIndex);
                    textIndex = findIndex + this.ipAddressDotControls[index].Text.Length;
                }
                else
                    break;
            }

            this.ipAddressFieldControls[index].Text = text.Substring(textIndex);
        }

        private void ResetBackColorChanged()
        {
            this._backColorChanged = false;
        }

        #endregion
    }
}