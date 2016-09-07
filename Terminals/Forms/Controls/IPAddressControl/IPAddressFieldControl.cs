using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Terminals.Forms.Controls.IPAddressControl
{
    public class IPAddressFieldControl : TextBox
    {
        #region Fields

        private int _fieldIndex = -1;
        private byte _rangeLower;
        private byte _rangeUpper = MaximumValue;

        private const TextFormatFlags _textFormatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;

        #endregion

        #region Constructors

        public IPAddressFieldControl()
        {
            this.BorderStyle = BorderStyle.None;
            this.MaxLength = 3;
            this.Size = this.MinimumSize;
            this.TabStop = false;
            this.TextAlign = HorizontalAlignment.Center;
        }

        #endregion

        #region Public Constants

        private const byte MinimumValue = 0;
        private const byte MaximumValue = 255;

        #endregion // Public Constants

        #region Public Events

        public event EventHandler<CedeFocusEventArgs> CedeFocusEvent;
        public event EventHandler<TextChangedEventArgs> TextChangedEvent;

        #endregion // Public Events

        #region Public Properties

        public bool Blank
        {
            get { return (this.TextLength == 0); }
        }

        public int FieldIndex
        {
            private get { return this._fieldIndex; }
            set { this._fieldIndex = value; }
        }

        public override Size MinimumSize
        {
            get
            {
                Graphics g = Graphics.FromHwnd(this.Handle);

                Size minimumSize = TextRenderer.MeasureText(g, IPAddressControl.FieldMeasureText, this.Font, this.Size,
                                                            _textFormatFlags);

                g.Dispose();

                return minimumSize;
            }
        }

        public byte RangeLower
        {
            private get { return this._rangeLower; }
            set
            {
                if (value < MinimumValue)
                    this._rangeLower = MinimumValue;
                else if (value > this._rangeUpper)
                    this._rangeLower = this._rangeUpper;
                else
                    this._rangeLower = value;

                if (this.Value < this._rangeLower)
                    this.Text = this._rangeLower.ToString(CultureInfo.InvariantCulture);
            }
        }

        public byte RangeUpper
        {
            private get { return this._rangeUpper; }
            set
            {
                if (value < this._rangeLower)
                    this._rangeUpper = this._rangeLower;
                else if (value > MaximumValue)
                    this._rangeUpper = MaximumValue;
                else
                    this._rangeUpper = value;

                if (this.Value > this._rangeUpper)
                    this.Text = this._rangeUpper.ToString(CultureInfo.InvariantCulture);
            }
        }

        public byte Value
        {
            get
            {
                byte result;

                if (!Byte.TryParse(this.Text, out result))
                    result = this.RangeLower;

                return result;
            }
        }

        #endregion

        #region Public Methods

        public void TakeFocus(IPAddressControlAction ipAddressControlAction)
        {
            this.Focus();

            switch (ipAddressControlAction)
            {
                case IPAddressControlAction.Trim:

                    if (this.TextLength > 0)
                    {
                        int newLength = this.TextLength - 1;
                        base.Text = this.Text.Substring(0, newLength);
                    }

                    this.SelectionStart = this.TextLength;

                    return;

                case IPAddressControlAction.Home:

                    this.SelectionStart = 0;
                    this.SelectionLength = 0;

                    return;

                case IPAddressControlAction.End:

                    this.SelectionStart = this.TextLength;

                    return;
            }
        }

        public void TakeFocus(IPAddressControlDirection ipAddressControlDirection,
                              IPAddressControlSelection ipAddressControlSelection)
        {
            this.Focus();

            if (ipAddressControlSelection == IPAddressControlSelection.All)
            {
                this.SelectionStart = 0;
                this.SelectionLength = this.TextLength;
            }
            else
                this.SelectionStart = (ipAddressControlDirection == IPAddressControlDirection.Forward)
                                          ? 0
                                          : this.TextLength;
        }

        public override string ToString()
        {
            return this.Value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Protected Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Home:
                    this.SendCedeFocusEvent(IPAddressControlAction.Home);
                    return;

                case Keys.End:
                    this.SendCedeFocusEvent(IPAddressControlAction.End);
                    return;
            }

            if (this.IsCedeFocusKey(e))
            {
                this.SendCedeFocusEvent(IPAddressControlDirection.Forward, IPAddressControlSelection.All);
                e.SuppressKeyPress = true;
                return;
            }
            if (IsForwardKey(e))
            {
                if (e.Control)
                {
                    this.SendCedeFocusEvent(IPAddressControlDirection.Forward, IPAddressControlSelection.All);
                    return;
                }
                if (this.SelectionLength == 0 && this.SelectionStart == this.TextLength)
                {
                    this.SendCedeFocusEvent(IPAddressControlDirection.Forward, IPAddressControlSelection.None);
                    return;
                }
            }
            else if (IsReverseKey(e))
            {
                if (e.Control)
                {
                    this.SendCedeFocusEvent(IPAddressControlDirection.Reverse, IPAddressControlSelection.All);
                    return;
                }
                if (this.SelectionLength == 0 && this.SelectionStart == 0)
                {
                    this.SendCedeFocusEvent(IPAddressControlDirection.Reverse, IPAddressControlSelection.None);
                    return;
                }
            }
            else if (IsBackspaceKey(e))
                this.HandleBackspaceKey(e);
            else if (!IsNumericKey(e) &&
                     !IsEditKey(e) &&
                     !IsEnterKey(e))
                e.SuppressKeyPress = true;
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            this.BackColor = this.Parent.BackColor;
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

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (!this.Blank)
            {
                int value;
                if (!Int32.TryParse(this.Text, out value))
                    base.Text = String.Empty;
                else
                {
                    if (value > this.RangeUpper)
                    {
                        base.Text = this.RangeUpper.ToString(CultureInfo.InvariantCulture);
                        this.SelectionStart = 0;
                    }
                    else if ((this.TextLength == this.MaxLength) && (value < this.RangeLower))
                    {
                        base.Text = this.RangeLower.ToString(CultureInfo.InvariantCulture);
                        this.SelectionStart = 0;
                    }
                    else
                    {
                        int originalLength = this.TextLength;
                        int newSelectionStart = this.SelectionStart;

                        base.Text = value.ToString(CultureInfo.InvariantCulture);

                        if (this.TextLength < originalLength)
                        {
                            newSelectionStart -= (originalLength - this.TextLength);
                            this.SelectionStart = Math.Max(0, newSelectionStart);
                        }
                    }
                }
            }

            if (null != this.TextChangedEvent)
            {
                TextChangedEventArgs args = new TextChangedEventArgs {FieldIndex = this.FieldIndex, Text = this.Text};
                this.TextChangedEvent(this, args);
            }

            if (this.TextLength == this.MaxLength && this.Focused && this.SelectionStart == this.TextLength)
                this.SendCedeFocusEvent(IPAddressControlDirection.Forward, IPAddressControlSelection.All);
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            if (!this.Blank)
            {
                if (this.Value < this.RangeLower)
                    this.Text = this.RangeLower.ToString(CultureInfo.InvariantCulture);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x007b: // WM_CONTEXTMENU
                    return;
            }

            base.WndProc(ref m);
        }

        #endregion

        #region Private Methods

        private void HandleBackspaceKey(KeyEventArgs e)
        {
            if (!this.ReadOnly && (this.TextLength == 0 || (this.SelectionStart == 0 && this.SelectionLength == 0)))
            {
                this.SendCedeFocusEvent(IPAddressControlAction.Trim);
                e.SuppressKeyPress = true;
            }
        }

        private static bool IsBackspaceKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                return true;

            return false;
        }

        private bool IsCedeFocusKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.OemPeriod ||
                e.KeyCode == Keys.Decimal ||
                e.KeyCode == Keys.Space)
            {
                if (this.TextLength != 0 && this.SelectionLength == 0 && this.SelectionStart != 0)
                    return true;
            }

            return false;
        }

        private static bool IsEditKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Delete)
                return true;
            if (e.Modifiers == Keys.Control &&
                (e.KeyCode == Keys.C ||
                 e.KeyCode == Keys.V ||
                 e.KeyCode == Keys.X))
                return true;

            return false;
        }

        private static bool IsEnterKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Return)
                return true;

            return false;
        }

        private static bool IsForwardKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Down)
                return true;

            return false;
        }

        private static bool IsNumericKey(KeyEventArgs e)
        {
            if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
            {
                if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
                    return false;
            }

            return true;
        }

        private static bool IsReverseKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Up)
                return true;

            return false;
        }

        private void SendCedeFocusEvent(IPAddressControlAction ipAddressControlAction)
        {
            if (null != this.CedeFocusEvent)
            {
                CedeFocusEventArgs args = new CedeFocusEventArgs
                                              {
                                                  FieldIndex = this.FieldIndex,
                                                  IPAddressControlAction = ipAddressControlAction
                                              };
                this.CedeFocusEvent(this, args);
            }
        }

        private void SendCedeFocusEvent(IPAddressControlDirection ipAddressControlDirection,
                                        IPAddressControlSelection ipAddressControlSelection)
        {
            if (null != this.CedeFocusEvent)
            {
                CedeFocusEventArgs args = new CedeFocusEventArgs
                                              {
                                                  FieldIndex = this.FieldIndex,
                                                  IPAddressControlAction = IPAddressControlAction.None,
                                                  IPAddressControlDirection = ipAddressControlDirection,
                                                  IPAddressControlSelection = ipAddressControlSelection
                                              };
                this.CedeFocusEvent(this, args);
            }
        }

        #endregion
    }
}