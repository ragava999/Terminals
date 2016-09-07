namespace Terminals.Forms.Controls
{
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.ComponentModel;
    using System;

    #region Delegates (3)
    public delegate void BNDroppedDownEventHandler(object sender, EventArgs e);
    public delegate void BNDrawItemEventHandler(object sender, DrawItemEventArgs e);
    public delegate void BNMeasureItemEventHandler(object sender, MeasureItemEventArgs e);
    #endregion

    public class CustomComboBox : ListControl
    {
        #region Variables (20)
        private bool _hovered = false;
        private bool _resize = false;

        private Color _backColor = Color.White;
        private Color _color1 = Color.LightSkyBlue;
        private Color _color2 = Color.DodgerBlue;
        private Color _color3 = Color.DeepSkyBlue;
        private Color _color4 = Color.DeepSkyBlue;
        private readonly Radius _radius = new Radius();

        private int _dropDownHeight = 200;
        private int _dropDownWidth = 0;
        private int _maxDropDownItems = 8;
        
        private int _selectedIndex = -1;

        private bool _isDroppedDown = false;

        private ComboBoxStyle _dropDownStyle = ComboBoxStyle.DropDownList;

        private Rectangle _rectBtn = new Rectangle(0, 0, 1, 1);
        private Rectangle _rectContent = new Rectangle(0, 0, 1, 1);

        private readonly ToolStripControlHost _controlHost;
        private readonly ListBox _listBox;
        private readonly ToolStripDropDown _popupControl;
        private readonly TextBox _textBox;
        #endregion

        #region Events (4)
        [Category("Behavior"), Description("Occurs when IsDroppedDown changed to True.")]
        public event BNDroppedDownEventHandler DroppedDown;

        [Category("Behavior"), Description("Occurs when the SelectedIndex property changes.")]
        public event EventHandler SelectedIndexChanged;

        [Category("Behavior"), Description("Occurs whenever a particular item/area needs to be painted.")]
        public event BNDrawItemEventHandler DrawItem;

        [Category("Behavior"), Description("Occurs whenever a particular item's height needs to be calculated.")]
        public event BNMeasureItemEventHandler MeasureItem;
        #endregion

        #region Shadowed methods (1)
        /// <summary>
        /// Setzt den Eingabefokus auf das Steuerelement.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// true, wenn die Anforderung des Eingabefokus erfolgreich war, andernfalls false.
        /// 
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new bool Focus()
        {
            return this._textBox.Focus();
        }
        #endregion

        #region Properties (17)
        public int SelectionLength
        {
            get { return _textBox.SelectionLength; }
            set { _textBox.SelectionLength = value; }
        }

        public int SelectionStart
        {
            get { return _textBox.SelectionStart; }
            set { _textBox.SelectionStart = value; }
        }

        public Color Color1
        {
            get { return _color1; }
            set { _color1 = value; Invalidate(true); }
        }

        public Color Color2
        {
            get { return _color2; }
            set { _color2 = value; Invalidate(true); }
        }
        
        public Color Color3
        {
            get { return _color3; }
            set { _color3 = value; Invalidate(true); }
        }

        public Color Color4
        {
            get { return _color4; }
            set { _color4 = value; Invalidate(true); }
        }

        public int DropDownHeight
        {
            get { return _dropDownHeight; }
            set { _dropDownHeight = value; }
        }

        public ListBox.ObjectCollection Items
        {
            get { return _listBox.Items; }
        }

        public int DropDownWidth
        {
            get { return _dropDownWidth; }
            set { _dropDownWidth = value; }
        }

        public int MaxDropDownItems
        {
            get { return _maxDropDownItems; }
            set { _maxDropDownItems = value; }
        }

        public new object DataSource
        {
            get { return base.DataSource; }
            set 
            { 
                _listBox.DataSource = value;
                base.DataSource = value;
                OnDataSourceChanged(EventArgs.Empty);
            }
        }

        public bool Sorted
        {
            get
            {
                return _listBox.Sorted;
            }
            set
            {
                _listBox.Sorted = value;
            }
        }

        [Category("Behavior"), Description("Indicates whether the code or the OS will handle the drawing of elements in the list.")]
        public DrawMode DrawMode
        {
            get { return _listBox.DrawMode; }
            set
            {
                _listBox.DrawMode = value;
            }
        }
        
        public ComboBoxStyle DropDownStyle
        {
            get { return _dropDownStyle; }
            set 
            { 
                _dropDownStyle = value; 
            
                if (_dropDownStyle == ComboBoxStyle.DropDownList)
                {
                    _textBox.Visible = false;
                }
                else
                {
                    _textBox.Visible = true;
                }
                Invalidate(true);
            }
        }

        public new Color BackColor
        {
            get { return _backColor; }
            set 
            { 
                this._backColor = value;
                _textBox.BackColor = value;
                Invalidate(true);
            }
        }

        public bool IsDroppedDown
        {
            get { return _isDroppedDown; }
            set 
            {
                if (_isDroppedDown && value == false )
                {
                    if (_popupControl.IsDropDown)
                    {
                        _popupControl.Close();
                    }
                }

                _isDroppedDown = value;

                if (_isDroppedDown)
                {
                    _controlHost.Control.Width = _dropDownWidth;

                    _listBox.Refresh();

                    if (_listBox.Items.Count > 0) 
                    {
                        int h = 0;
                        int i = 0;
                        int maxItemHeight = 0;
                        int highestItemHeight = 0;
                        foreach(object item in _listBox.Items)
                        {
                            int itHeight = _listBox.GetItemHeight(i);
                            if (highestItemHeight < itHeight) 
                            {
                                highestItemHeight = itHeight;
                            }
                            h = h + itHeight;
                            if (i <= (_maxDropDownItems - 1)) 
                            {
                                maxItemHeight = h;
                            }
                            i = i + 1;
                        }

                        if (maxItemHeight > _dropDownHeight)
                            _listBox.Height = _dropDownHeight + 3;
                        else
                        {
                            if (maxItemHeight > highestItemHeight )
                                _listBox.Height = maxItemHeight + 3;
                            else
                                _listBox.Height = highestItemHeight + 3;
                        }
                    }
                    else
                    {
                        _listBox.Height = 15;
                    }

                    _popupControl.AutoClose = false;
                    _popupControl.Show(this, CalculateDropPosition(), ToolStripDropDownDirection.BelowRight);
                    _popupControl.Invalidate();
                }

                Invalidate();
                if (_isDroppedDown)
                    OnDroppedDown(this, EventArgs.Empty);
            }
        }

        public Radius Radius
        {
            get { return _radius; }
        }
        #endregion

        #region Constructor (1)
        public CustomComboBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Selectable, true);

            base.BackColor = Color.Transparent;
            _radius.BottomLeft = 2;
            _radius.BottomRight = 2;
            _radius.TopLeft = 2;
            _radius.TopRight = 6;

            this.Height = 21;
            this.Width = 95;

            this.SuspendLayout();
            _textBox = new TextBox
                           {
                               BorderStyle = BorderStyle.None,
                               Location = new Point(3, 4),
                               Size = new Size(60, 13),
                               TabIndex = 0,
                               WordWrap = false,
                               Margin = new Padding(0),
                               Padding = new Padding(0),
                               TextAlign = HorizontalAlignment.Left
                           };

            this.Controls.Add(_textBox);
            this.ResumeLayout(false);

            AdjustControls();

            _listBox = new ListBox
                           {
                               IntegralHeight = true,
                               BorderStyle = BorderStyle.FixedSingle,
                               SelectionMode = SelectionMode.One,
                               BindingContext = new BindingContext()
                           };

            _controlHost = new ToolStripControlHost(_listBox)
                               {
                                   Padding = new Padding(0),
                                   Margin = new Padding(0),
                                   AutoSize = false
                               };

            _popupControl = new ToolStripDropDown
                                {
                                    Padding = new Padding(0),
                                    Margin = new Padding(0),
                                    AutoSize = true,
                                    DropShadowEnabled = false
                                };

            _popupControl.Items.Add(_controlHost);

            _dropDownWidth = this.Width;

            _listBox.MeasureItem += this._listBox_MeasureItem;
            _listBox.DrawItem += this._listBox_DrawItem;
            _listBox.MouseClick += this._listBox_MouseClick;
            _listBox.MouseMove += this._listBox_MouseMove;

            _popupControl.Closed += this._popupControl_Closed;

            _textBox.Resize += this._textBox_Resize;
            _textBox.TextChanged += this._textBox_TextChanged;

            _textBox.KeyPress += (sender, args) => this.OnKeyPress(args);
            _popupControl.KeyPress += (sender, args) => this.OnKeyPress(args);
        }
        #endregion

        #region Overrides (21)
        protected override void OnDataSourceChanged(EventArgs e)
        {
            this.SelectedIndex = 0;
            base.OnDataSourceChanged(e);
        }

        protected override void OnDisplayMemberChanged(EventArgs e)
        {
            _listBox.DisplayMember = this.DisplayMember;
            this.SelectedIndex = this.SelectedIndex;
            base.OnDisplayMemberChanged(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate(true);
            base.OnEnabledChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            _textBox.ForeColor = this.ForeColor;
            base.OnForeColorChanged(e);
        }

        protected override void OnFormatInfoChanged(EventArgs e)
        {
            _listBox.FormatInfo = this.FormatInfo;
            base.OnFormatInfoChanged(e);
        }

        protected override void OnFormatStringChanged(EventArgs e)
        {
            _listBox.FormatString = this.FormatString;
            base.OnFormatStringChanged(e);
        }

        protected override void OnFormattingEnabledChanged(EventArgs e)
        {
            _listBox.FormattingEnabled = this.FormattingEnabled;
            base.OnFormattingEnabledChanged(e);
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                this._resize = true;
                _textBox.Font = value;
                base.Font = value;
                Invalidate(true);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            e.Control.MouseDown += new MouseEventHandler(Control_MouseDown);
            e.Control.MouseEnter += new EventHandler(Control_MouseEnter);
            e.Control.MouseLeave += new EventHandler(Control_MouseLeave);
            e.Control.GotFocus += new EventHandler(Control_GotFocus);
            e.Control.LostFocus += new EventHandler(Control_LostFocus);
            base.OnControlAdded(e);
        }        

        protected override void OnMouseEnter(EventArgs e)
        {
            this._hovered = true;
            this.Invalidate(true);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.RectangleToScreen(this.ClientRectangle).Contains(MousePosition))
            {
                this._hovered = false;
                Invalidate(true);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _textBox.Focus();
            if ((this.RectangleToScreen(this._rectBtn).Contains(MousePosition) || (DropDownStyle == ComboBoxStyle.DropDownList)))
            {
                this.Invalidate(true);
                if (this.IsDroppedDown) 
                {
                    this.IsDroppedDown = false;
                }
                this.IsDroppedDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._hovered = this.RectangleToScreen(this.ClientRectangle).Contains(MousePosition);

            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta < 0)
                this.SelectedIndex = this.SelectedIndex + 1;
            else if (e.Delta > 0)
            {
                if (this.SelectedIndex > 0)
                    this.SelectedIndex = this.SelectedIndex - 1;
            }

            base.OnMouseWheel(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate(true);
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!this.ContainsFocus)
            {
                Invalidate();
            }

            base.OnLostFocus(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if(SelectedIndexChanged!=null)
                SelectedIndexChanged(this, e);

            base.OnSelectedIndexChanged(e);
        }

        protected override void OnValueMemberChanged(EventArgs e)
        {
            _listBox.ValueMember = this.ValueMember;
            this.SelectedIndex = this.SelectedIndex;
            base.OnValueMemberChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (this._resize)
            {
                this._resize = false;
                AdjustControls();

                Invalidate(true);
            }
            else
                Invalidate(true);

            if (DesignMode)
                _dropDownWidth = this.Width;
        }

        public override string Text
        {
            get
            {
                return _textBox.Text;
            }
            set
            {
                _textBox.Text = value;
                base.Text = _textBox.Text;
                OnTextChanged(EventArgs.Empty);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //content border
            Rectangle rectCont = this._rectContent;
            rectCont.X += 1;
            rectCont.Y += 1;
            rectCont.Width -= 3;
            rectCont.Height -= 3;
            GraphicsPath pathContentBorder = CreateRoundRectangle(rectCont, Radius.TopLeft, Radius.TopRight, Radius.BottomRight,
                Radius.BottomLeft);

            //button border
            Rectangle rectButton = this._rectBtn;
            rectButton.X += 1;
            rectButton.Y += 1;
            rectButton.Width -= 3;
            rectButton.Height -= 3;
            GraphicsPath pathBtnBorder = CreateRoundRectangle(rectButton, 0, Radius.TopRight, Radius.BottomRight, 0);

            //outer border
            Rectangle rectOuter = this._rectContent;
            rectOuter.Width -= 1;
            rectOuter.Height -= 1;
            GraphicsPath pathOuterBorder = CreateRoundRectangle(rectOuter, Radius.TopLeft, Radius.TopRight, Radius.BottomRight,
                Radius.BottomLeft);

            //inner border
            Rectangle rectInner = this._rectContent;
            rectInner.X += 1;
            rectInner.Y += 1;
            rectInner.Width -= 3;
            rectInner.Height -= 3;
            GraphicsPath pathInnerBorder = CreateRoundRectangle(rectInner, Radius.TopLeft, Radius.TopRight, Radius.BottomRight,
                Radius.BottomLeft);

            //brushes and pens
            Brush brInnerBrush = new LinearGradientBrush(
                new Rectangle(rectInner.X,rectInner.Y,rectInner.Width,rectInner.Height+1), 
                (this._hovered || IsDroppedDown || ContainsFocus)?Color4:Color2, Color.Transparent,
                LinearGradientMode.Vertical);
            Brush brBackground;
            if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                brBackground = new LinearGradientBrush(pathInnerBorder.GetBounds(), 
                    Color.FromArgb(IsDroppedDown ? 100 : 255, Color.White), 
                    Color.FromArgb(IsDroppedDown?255:100, BackColor),
                    LinearGradientMode.Vertical);
            }
            else
            {
                brBackground = new SolidBrush(BackColor);
            }
            Pen penOuterBorder = new Pen(Color1, 0);
            Pen penInnerBorder = new Pen(brInnerBrush, 0);
            LinearGradientBrush brButtonLeft = new LinearGradientBrush(this._rectBtn, Color1, Color2, LinearGradientMode.Vertical);
            ColorBlend blend = new ColorBlend();
            blend.Colors = new Color[] { Color.Transparent, Color2, Color.Transparent };
            blend.Positions = new float[] { 0.0f, 0.5f, 1.0f};
            brButtonLeft.InterpolationColors = blend;
            Pen penLeftButton = new Pen(brButtonLeft, 0);
            Brush brButton = new LinearGradientBrush(pathBtnBorder.GetBounds(),
                Color.FromArgb(100, IsDroppedDown? Color2:Color.White),
                    Color.FromArgb(100, IsDroppedDown ? Color.White : Color2),
                    LinearGradientMode.Vertical);

            //draw
            e.Graphics.FillPath(brBackground, pathContentBorder);
            if (DropDownStyle != ComboBoxStyle.DropDownList)
            {
                e.Graphics.FillPath(brButton, pathBtnBorder);
            }
            e.Graphics.DrawPath(penOuterBorder, pathOuterBorder);
            e.Graphics.DrawPath(penInnerBorder, pathInnerBorder);

            e.Graphics.DrawLine(penLeftButton, this._rectBtn.Left + 1, rectInner.Top+1, this._rectBtn.Left + 1, rectInner.Bottom-1);
            
            //Glimph
            Rectangle rectGlimph = rectButton;
            rectButton.Width -= 4;
            e.Graphics.TranslateTransform(rectGlimph.Left + rectGlimph.Width / 2.0f, rectGlimph.Top + rectGlimph.Height / 2.0f);
            GraphicsPath path = new GraphicsPath();
            PointF[] points = new PointF[3];
            points[0] = new PointF(-6 / 2.0f, -3 / 2.0f);
            points[1] = new PointF(6 / 2.0f, -3 / 2.0f);
            points[2] = new PointF(0, 6 / 2.0f);
            path.AddLine(points[0], points[1]);
            path.AddLine(points[1], points[2]);
            path.CloseFigure();
            e.Graphics.RotateTransform(0);

            SolidBrush br = new SolidBrush(Enabled?Color.Gray:Color.Gainsboro);
            e.Graphics.FillPath(br, path);
            e.Graphics.ResetTransform();
            br.Dispose();
            path.Dispose();
            
            //text
            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                StringFormat sf  = new StringFormat(StringFormatFlags.NoWrap) {Alignment = StringAlignment.Near};

                Rectangle rectText = _textBox.Bounds;
                rectText.Offset(-3, 0);

                SolidBrush foreBrush = new SolidBrush(ForeColor);
                if (Enabled)
                {
                    e.Graphics.DrawString(_textBox.Text, this.Font, foreBrush, rectText.Location);
                }
                else
                {
                    ControlPaint.DrawStringDisabled(e.Graphics, _textBox.Text, Font, BackColor, rectText, sf);
                }
            }

            pathContentBorder.Dispose();
            pathOuterBorder.Dispose();
            pathInnerBorder.Dispose();
            pathBtnBorder.Dispose();

            penOuterBorder.Dispose();
            penInnerBorder.Dispose();
            penLeftButton.Dispose();

            brBackground.Dispose();
            brInnerBrush.Dispose();
            brButtonLeft.Dispose();
            brButton.Dispose();
        }
        #endregion

        #region ListControl overrides (7)
        public override int SelectedIndex
        {
            get { return _selectedIndex; }
            set 
            { 
                if(_listBox != null)
                {
                    if (_listBox.Items.Count == 0)
                        return;

                    if ((this.DataSource != null) && value == -1)
                        return;

                    if (value <= (_listBox.Items.Count - 1) && value >= -1)
                    {
                        _listBox.SelectedIndex = value;
                        _selectedIndex = value;
                        _textBox.Text = _listBox.GetItemText(_listBox.SelectedItem);
                        OnSelectedIndexChanged(EventArgs.Empty);
                    }
                }
            }
        }

        public object SelectedItem
        {
            get { return _listBox.SelectedItem;  }
            set 
            { 
                _listBox.SelectedItem = value;
                this.SelectedIndex = _listBox.SelectedIndex;
            }
        }

        public new object SelectedValue
        {
            get { return base.SelectedValue; }
            set
            {
                base.SelectedValue = value;
            }
        }

        protected override void RefreshItem(int index)
        {
            
        }

        protected override void RefreshItems()
        {
            
        }

        protected override void SetItemCore(int index, object value)
        {
            
        }

        protected override void SetItemsCore(System.Collections.IList items)
        {
            
        }
        #endregion
        
        #region Nested controls events (12)
        void Control_LostFocus(object sender, EventArgs e)
        {
            OnLostFocus(e);
        }

        void Control_GotFocus(object sender, EventArgs e)
        {
            OnGotFocus(e);
        }

        void Control_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        void Control_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        void Control_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        void _listBox_MouseMove(object sender, MouseEventArgs e)
        {
            int i;
            for (i = 0; i < (_listBox.Items.Count); i++)
            {
                if (_listBox.GetItemRectangle(i).Contains(_listBox.PointToClient(MousePosition)))
                {
                    _listBox.SelectedIndex = i;
                    return;
                }
            }
        }

        void _listBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (_listBox.Items.Count == 0)
            {
                return;
            }

            if (_listBox.SelectedItems.Count != 1)
            {
                return;
            }

            this.SelectedIndex = _listBox.SelectedIndex;

            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                this.Invalidate(true);
            }

            IsDroppedDown = false;
        }

        void _listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                if (DrawItem != null)
                {
                    DrawItem(this, e);
                }
            }
        }

        void _listBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (MeasureItem != null)
            {
                MeasureItem(this, e);
            }
        }

        void _popupControl_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _isDroppedDown = false;
            if (!this.RectangleToScreen(this.ClientRectangle).Contains(MousePosition))
            {
                this._hovered = false;
            }
            Invalidate(true);
        }

        void _textBox_Resize(object sender, EventArgs e)
        {
            this.AdjustControls();
        }

        void _textBox_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(e);
        }
        #endregion
        
        #region Private methods (2)
        private void AdjustControls()
        {
            this.SuspendLayout();

            this._resize = true;
            _textBox.Top = 4;
            _textBox.Left = 5;
            this.Height = _textBox.Top + _textBox.Height + _textBox.Top;

            this._rectBtn =
                    new Rectangle(this.ClientRectangle.Width - 18,
                    this.ClientRectangle.Top, 18, _textBox.Height + 2 * _textBox.Top);


            _textBox.Width = this._rectBtn.Left - 1 - _textBox.Left;

            this._rectContent = new Rectangle(ClientRectangle.Left, ClientRectangle.Top,
                ClientRectangle.Width, _textBox.Height + 2 * _textBox.Top);

            this.ResumeLayout();

            Invalidate(true);
        }

        private Point CalculateDropPosition()
        {
            Point point = new Point(0, this.Height);
            if ((this.PointToScreen(new Point(0, 0)).Y + this.Height + _controlHost.Height) > Screen.PrimaryScreen.WorkingArea.Height)
            {
                point.Y = -this._controlHost.Height - 7;
            }
            return point;
        }
        #endregion      

        #region Virtual methods (1)
        public virtual void OnDroppedDown(object sender, EventArgs e)
        {
            if (DroppedDown != null)
            {
                DroppedDown(this, e);
            }
        }
        #endregion

        #region Render (1)
        public static GraphicsPath CreateRoundRectangle(Rectangle rectangle, int topLeftRadius, int topRightRadius, int bottomRightRadius, int bottomLeftRadius)
        {
            GraphicsPath path = new GraphicsPath();
            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;

            if(topLeftRadius > 0)
            {
                path.AddArc(l, t, topLeftRadius * 2, topLeftRadius * 2, 180, 90);
            }
            path.AddLine(l + topLeftRadius, t, l + w - topRightRadius, t);
            if (topRightRadius > 0)
            {
                path.AddArc(l + w - topRightRadius * 2, t, topRightRadius * 2, topRightRadius * 2, 270, 90);
            }
            path.AddLine(l + w, t + topRightRadius, l + w, t + h - bottomRightRadius);
            if (bottomRightRadius > 0)
            {
                path.AddArc(l + w - bottomRightRadius * 2, t + h - bottomRightRadius * 2,
                    bottomRightRadius * 2, bottomRightRadius * 2, 0, 90);
            }
            path.AddLine(l + w - bottomRightRadius, t + h, l + bottomLeftRadius, t + h);
            if(bottomLeftRadius >0)
            {
                path.AddArc(l, t + h - bottomLeftRadius * 2, bottomLeftRadius * 2, bottomLeftRadius * 2, 90, 90);
            }
            path.AddLine(l, t + h - bottomLeftRadius, l, t + topLeftRadius);
            path.CloseFigure();
            return path;
        }
        #endregion
    } 
}