namespace Terminals.Connection.TabControl
{
    // .NET namespace
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.ComponentModel;

    using Configuration.Files.Main.Settings;

    [DefaultEvent("TabControlItemSelectionChanged")]
    [DefaultProperty("Items")]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TabControl))]
    public sealed class TabControl : BaseStyledPanel, ISupportInitialize
    {
        #region Constants (3)
        private const int DEF_HEADER_HEIGHT = 19;
        private const int DEF_GLYPH_WIDTH = 40;
        private const int MOVE_TOLERANCE = 5;
        #endregion

        #region Fields (30)
        private readonly TabControlMenuGlyph menuGlyph = null;
        private readonly TabControlCloseButton closeButton = null;
        private readonly StringFormat sf = null;
        private readonly TabPreview movePreview;

        private Rectangle stripButtonRect = Rectangle.Empty;
        private TabControlItem selectedItem = null;
        private TabControlItem tabAtMouseDown = null;

        private bool mouseEnteredTitle;
        private Point mouseDownPoint;

        private bool alwaysShowClose = true;
        private bool alwaysShowMenuGlyph = true;
        private bool showTabs = true;
        private bool showBorder = true;

        private bool isInitializing = false;
        private bool mouseDownAtMenuGliph = false;
        private bool mouseDownAtCloseGliph = false;

        public int HeaderHeight { get; private set; }

        private int startPosition = 10;

        public event TabControlItemClosingHandler TabControlItemClosing;
        public event TabControlItemChangedHandler TabControlItemSelectionChanged;
        public event TabControlMouseOnTitleHandler TabControlMouseOnTitle;
        public event TabControlMouseLeftTitleHandler TabControlMouseLeftTitle;

        public event EventHandler MenuItemsLoaded;
        public event EventHandler TabControlItemClosed;

        public event TabControlMouseEnteredTitleHandler TabControlMouseEnteredTitle;
        public event HandledEventHandler MenuItemsLoading;
        public event TabControlItemChangedHandler TabControlItemDetach;
        #endregion

        #region Public Methods (4)
        public void AddTab(TabControlItem tabItem)
        {
            // Add a tabpage to the first position
            if (Settings.InvertTabPageOrder)
                this.Items.Insert(0, tabItem);
            // Add a tabpage to the last position in the sequence
            else
                this.Items.Add(tabItem);
            
            tabItem.Dock = DockStyle.Fill;
        }

        public void RemoveTab(TabControlItem tabItem)
        {
            int tabIndex = this.Items.IndexOf(tabItem);
            bool wasSelected = tabItem.Selected;

            if (tabIndex >= 0)
            {
                this.UnSelectItem(tabItem);
                this.Items.Remove(tabItem);
            }

            if (wasSelected)
            {
                if (this.Items.Count > 0)
                {
                    if (this.RightToLeft == RightToLeft.No)
                    {
                        if (this.Items[tabIndex - 1] != null)
                        {
                            this.SelectedItem = this.Items[tabIndex - 1];
                        }
                        else
                        {
                            this.SelectedItem = this.Items.FirstVisible;
                        }
                    }
                    else
                    {
                        if (this.Items[tabIndex + 1] != null)
                        {
                            this.SelectedItem = this.Items[tabIndex + 1];
                        }
                        else
                        {
                            this.SelectedItem = this.Items.LastVisible;
                        }
                    }
                }
                else
                    this.SelectedItem = null;
            }
        }

        public void CloseTab(TabControlItem tabItem)
        {
            if (tabItem != null)
            {
                this.SelectedItem = tabItem;
                TabControlItemClosingEventArgs args = new TabControlItemClosingEventArgs();
                this.OnTabControlItemClosing(args);

                if (this.SelectedItem != null && !args.Cancel && this.SelectedItem.CanClose)
                {
                    this.RemoveTab(this.SelectedItem);
                    this.OnTabControlItemClosed(EventArgs.Empty);
                }
            }
        }

        public TabControlItem GetTabItemByPoint(Point pt)
        {
            TabControlItem item = null;

            for (int i = 0; i < this.Items.Count; i++)
            {
                TabControlItem current = this.Items[i];

                if (current.StripRect.Contains(pt))
                {
                    item = current;
                }
            }

            return item;
        }
        #endregion

        #region Internal Methods (1)
        internal void OnDrawTabPage(Graphics g, TabControlItem currentItem)
        {
            bool isFirstTab = this.Items.IndexOf(currentItem) == 0;
            Font currentFont = this.Font;

            if (currentItem == this.SelectedItem)
                currentFont = new Font(this.Font, FontStyle.Bold);

            SizeF textSize = g.MeasureString(currentItem.Title, currentFont, new SizeF(200, 10), this.sf);
            textSize.Width += 20;

            RectangleF buttonRect = currentItem.StripRect;
            GraphicsPath path = new GraphicsPath();
            LinearGradientBrush brush = null;
            const int mtop = 3;

            #region Draw Not Right-To-Left Tab
            if (this.RightToLeft == RightToLeft.No)
            {
                if (currentItem == this.SelectedItem || isFirstTab)
                {
                    path.AddLine(buttonRect.Left - 10, buttonRect.Bottom - 1, buttonRect.Left + (buttonRect.Height / 2) - 4, mtop + 4);
                }
                else
                {
                    path.AddLine(buttonRect.Left, buttonRect.Bottom - 1, buttonRect.Left, buttonRect.Bottom - (buttonRect.Height / 2) - 2);
                    path.AddLine(buttonRect.Left, buttonRect.Bottom - (buttonRect.Height / 2) - 3, buttonRect.Left + (buttonRect.Height / 2) - 4, mtop + 3);
                }

                path.AddLine(buttonRect.Left + (buttonRect.Height / 2) + 2, mtop, buttonRect.Right - 3, mtop);
                path.AddLine(buttonRect.Right, mtop + 2, buttonRect.Right, buttonRect.Bottom - 1);
                path.AddLine(buttonRect.Right - 4, buttonRect.Bottom - 1, buttonRect.Left, buttonRect.Bottom - 1);
                path.CloseFigure();
                try
                {
                    if (currentItem == this.SelectedItem)
                    {
                        brush = new LinearGradientBrush(buttonRect, currentItem.TabColor, SystemColors.Window, LinearGradientMode.Vertical);
                    }
                    else
                    {
                        brush = new LinearGradientBrush(buttonRect, currentItem.TabColor, SystemColors.Window, LinearGradientMode.Vertical);
                    }
                }
                catch (Exception)
                {

                }
                g.FillPath(brush, path);
                Pen pen = SystemPens.ControlDark;
                if (currentItem == this.SelectedItem)
                {
                    pen = new Pen(this.ToolStripRenderer.ColorTable.MenuStripGradientBegin);
                }
                g.DrawPath(pen, path);
                if (currentItem == this.SelectedItem)
                {
                    pen.Dispose();
                }

                if (currentItem == this.SelectedItem)
                {
                    g.DrawLine(new Pen(brush), buttonRect.Left - 9, buttonRect.Height + 2, buttonRect.Left + buttonRect.Width - 1, buttonRect.Height + 2);
                }

                PointF textLoc = new PointF(buttonRect.Left + buttonRect.Height - 4, buttonRect.Top + (buttonRect.Height / 2) - (textSize.Height / 2) - 3);
                RectangleF textRect = buttonRect;
                textRect.Location = textLoc;
                textRect.Width = (float)buttonRect.Width - (textRect.Left - buttonRect.Left) - 4;
                textRect.Height = textSize.Height + currentFont.Size / 2;

                HeaderHeight = Convert.ToInt32(buttonRect.Height + buttonRect.Location.Y);

                if (currentItem == this.SelectedItem)
                {
                    //textRect.Y -= 2;
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(this.ForeColor), textRect, this.sf);
                }
                else
                {
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(this.ForeColor), textRect, this.sf);
                }
            }
            #endregion

            #region Draw Right-To-Left Tab
            if (this.RightToLeft == RightToLeft.Yes)
            {
                if (currentItem == this.SelectedItem || isFirstTab)
                {
                    path.AddLine(buttonRect.Right + 10, buttonRect.Bottom - 1, buttonRect.Right - (buttonRect.Height / 2) + 4, mtop + 4);
                }
                else
                {
                    path.AddLine(buttonRect.Right, buttonRect.Bottom - 1, buttonRect.Right, buttonRect.Bottom - (buttonRect.Height / 2) - 2);
                    path.AddLine(buttonRect.Right, buttonRect.Bottom - (buttonRect.Height / 2) - 3, buttonRect.Right - (buttonRect.Height / 2) + 4, mtop + 3);
                }

                path.AddLine(buttonRect.Right - (buttonRect.Height / 2) - 2, mtop, buttonRect.Left + 3, mtop);
                path.AddLine(buttonRect.Left, mtop + 2, buttonRect.Left, buttonRect.Bottom - 1);
                path.AddLine(buttonRect.Left + 4, buttonRect.Bottom - 1, buttonRect.Right, buttonRect.Bottom - 1);
                path.CloseFigure();

                if (currentItem == this.SelectedItem)
                {
                    brush = new LinearGradientBrush(buttonRect, currentItem.TabColor, SystemColors.Window, LinearGradientMode.Vertical);
                }
                else
                {
                    brush = new LinearGradientBrush(buttonRect, currentItem.TabColor, SystemColors.Window, LinearGradientMode.Vertical);
                }

                g.FillPath(brush, path);
                g.DrawPath(SystemPens.ControlDark, path);

                if (currentItem == this.SelectedItem)
                {
                    g.DrawLine(new Pen(brush), buttonRect.Right + 9, buttonRect.Height + 2, buttonRect.Right - buttonRect.Width + 1, buttonRect.Height + 2);
                }

                PointF textLoc = new PointF(buttonRect.Left + 2, buttonRect.Top + (buttonRect.Height / 2) - (textSize.Height / 2) - 2);
                RectangleF textRect = buttonRect;
                textRect.Location = textLoc;
                textRect.Width = (float)buttonRect.Width - (textRect.Left - buttonRect.Left) - 10;
                textRect.Height = textSize.Height + currentFont.Size / 2;

                HeaderHeight = Convert.ToInt32(buttonRect.Height + buttonRect.Location.Y);

                if (currentItem == this.SelectedItem)
                {
                    textRect.Y -= 1;
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(this.ForeColor), textRect, this.sf);
                }
                else
                {
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(this.ForeColor), textRect, this.sf);
                }

                //g.FillRectangle(Brushes.Red, textRect);
            }
            #endregion

            currentItem.IsDrawn = true;
        }
        #endregion

        #region Protected Methods (8)
        protected override void OnClick(EventArgs e)
        {
            if (this.IsMouseModdleClick(e))
                this.CloseTabAtCurrentCursor();

            base.OnClick(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (this.isInitializing)
                return;

            this.UpdateLayout();
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.UpdateLayout();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.SetDefaultSelected();

            if (this.showBorder)
            {
                Rectangle borderRc = base.ClientRectangle;
                borderRc.Width--;
                borderRc.Height--;
                e.Graphics.DrawRectangle(SystemPens.ControlDark, borderRc);
            }

            this.startPosition = this.RightToLeft == RightToLeft.No ? 10 : this.stripButtonRect.Right;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            #region Draw Pages
            if (this.showTabs)
            {
                TabControlItem selectedTabItem = this.SelectedItem;

                // todo bug 32353 - first calculate all, then identify ragne of tabs to draw, and finally paint them 
                for (int i = 0; i < this.Items.Count; i++)
                {
                    TabControlItem currentItem = this.Items[i];
                    if (!currentItem.Visible && !this.DesignMode)
                        continue;

                    this.OnCalcTabPage(e.Graphics, currentItem);
                    currentItem.IsDrawn = false;

                    // delay drawing active item to the end
                    if (currentItem == selectedTabItem)
                        continue;

                    if (!this.AllowDraw(currentItem))
                        continue;

                    this.OnDrawTabPage(e.Graphics, currentItem);
                }
                if (selectedTabItem != null && this.AllowDraw(selectedTabItem))
                {
                    try
                    {
                        this.OnDrawTabPage(e.Graphics, selectedTabItem);
                    }
                    catch (Exception)
                    {
                        //black hole this for now
                    }
                }
            }
            #endregion

            #region Draw UnderPage Line
            if (this.showTabs && this.showBorder)
            {
                using (Pen pen = new Pen(this.ToolStripRenderer.ColorTable.MenuStripGradientBegin))
                {
                    if (this.RightToLeft == RightToLeft.No)
                    {
                        if (this.Items.DrawnCount == 0 || this.Items.VisibleCount == 0)
                        {
                            e.Graphics.DrawLine(pen, new Point(0, DEF_HEADER_HEIGHT), new Point(this.ClientRectangle.Width, DEF_HEADER_HEIGHT));
                        }
                        else if (this.SelectedItem != null && this.SelectedItem.IsDrawn)
                        {
                            Point end = new Point((int)this.SelectedItem.StripRect.Left - 9, DEF_HEADER_HEIGHT);
                            e.Graphics.DrawLine(pen, new Point(0, DEF_HEADER_HEIGHT), end);
                            end.X += (int)this.SelectedItem.StripRect.Width + 10;
                            e.Graphics.DrawLine(pen, end, new Point(this.ClientRectangle.Width, DEF_HEADER_HEIGHT));
                        }
                    }
                    else
                    {
                        if (this.Items.DrawnCount == 0 || this.Items.VisibleCount == 0)
                        {
                            e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, DEF_HEADER_HEIGHT), new Point(this.ClientRectangle.Width, DEF_HEADER_HEIGHT));
                        }
                        else if (this.SelectedItem != null && this.SelectedItem.IsDrawn)
                        {
                            Point end = new Point((int)this.SelectedItem.StripRect.Left, DEF_HEADER_HEIGHT);
                            e.Graphics.DrawLine(pen, new Point(0, DEF_HEADER_HEIGHT), end);
                            end.X += (int)this.SelectedItem.StripRect.Width + 20;
                            e.Graphics.DrawLine(pen, end, new Point(this.ClientRectangle.Width, DEF_HEADER_HEIGHT));
                        }
                    }
                }
            }
            #endregion

            #region Draw Menu and Close Glyphs
            if (this.showTabs)
            {
                if ((this.AlwaysShowMenuGlyph && this.Items.VisibleCount > 0) || this.Items.DrawnCount > this.Items.VisibleCount)
                    this.menuGlyph.DrawGlyph(e.Graphics);

                if (this.AlwaysShowClose || (this.SelectedItem != null && this.SelectedItem.CanClose))
                    this.closeButton.DrawCross(e.Graphics);
            }
            #endregion
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                bool handled = this.HandleMenuGlipMouseUp(e);
                handled |= this.HandleCloseButtonMouseUp(e);
                handled |= this.HandleTabDetach(e);

                // It is highly important to call this method after the above ones
                // to avoid messing up the tab order.
                this.HandleTablItemMouseUpActions(e);

                if (!handled)
                    base.OnMouseUp(e);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
            finally
            {
                this.tabAtMouseDown = null;
                this.mouseDownAtMenuGliph = false;
                this.mouseDownAtCloseGliph = false;
                this.movePreview.Hide();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.mouseDownPoint = e.Location;
                this.tabAtMouseDown = this.GetTabItemByPoint(this.mouseDownPoint);

                // Show Tabs menu
                if (this.MouseIsOnMenuGliph(e))
                    this.mouseDownAtMenuGliph = true;

                // close by click on close button
                if (this.MouseIsOnCloseButton(e))
                    this.mouseDownAtCloseGliph = true;
            }

            // custom handling
            if (!this.mouseDownAtCloseGliph && !this.mouseDownAtMenuGliph && this.tabAtMouseDown == null)
                base.OnMouseDown(e);

            this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.HandleDrawMenuGliph(e);
            this.HandleDrawCloseButton(e);
            this.HandleMouseInTitle(e);
            this.HandlePreviewMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.menuGlyph.IsMouseOver = false;
            this.Invalidate(this.menuGlyph.Rect);
            this.closeButton.IsMouseOver = false;
            this.Invalidate(this.closeButton.Rect);
        }
        #endregion

        #region Private Methods (40)
        private bool AllowDraw(TabControlItem item)
        {
            bool result = true;

            if (this.RightToLeft == RightToLeft.No)
            {
                if (item.StripRect.Right >= this.stripButtonRect.Width)
                    result = false;
            }
            else
            {
                if (item.StripRect.Left <= this.stripButtonRect.Left)
                    return false;
            }

            return result;
        }

        private bool IsMouseModdleClick(EventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            return mouse != null && mouse.Button == MouseButtons.Middle;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.AllowDrop = true;
            this.ResumeLayout(false);

        }

        private void CloseTabAtCurrentCursor()
        {
            TabControlItem selectedTab = this.GetTabItemByPoint(this.PointToClient(Cursor.Position));
            this.CloseTab(selectedTab);
        }

        private bool HandleTabDetach(MouseEventArgs e)
        {
            bool outside = this.IsMouseOutsideHeader(e.Location);
            if (outside)
            {
                if (this.TabControlItemDetach != null)
                {
                    var args = new TabControlItemChangedEventArgs(this.tabAtMouseDown);
                    this.TabControlItemDetach(args);
                }

                return true;
            }

            return false;
        }

        private bool IsMouseOutsideHeader(Point location)
        {
            bool outsideY = location.Y < -MOVE_TOLERANCE || (DEF_HEADER_HEIGHT + MOVE_TOLERANCE) < location.Y;
            bool outsideX = location.X < -MOVE_TOLERANCE || (this.Width + MOVE_TOLERANCE) < location.X;
            return outsideY || outsideX;
        }

        private bool HandleCloseButtonMouseUp(MouseEventArgs e)
        {
            if (this.mouseDownAtCloseGliph && this.MouseIsOnCloseButton(e))
            {
                this.CloseTab(this.SelectedItem);
                return true;
            }

            return false;
        }

        private bool HandleMenuGlipMouseUp(MouseEventArgs e)
        {
            if (this.mouseDownAtMenuGliph && this.MouseIsOnMenuGliph(e))
            {
                this.ShowTabsMenu();
                return true;
            }

            return false;
        }

        private void HandleTablItemMouseUpActions(MouseEventArgs e)
        {
            if (this.tabAtMouseDown != null)
            {
                TabControlItem upItem = this.GetTabItemByPoint(e.Location);
                if (upItem != null && upItem == this.tabAtMouseDown)
                {
                    this.SelectedItem = upItem;
                }
                else
                    this.SwapTabItems(e.X, upItem);
            }
        }

        private void SwapTabItems(int mouseX, TabControlItem upItem)
        {
            int downIndex = this.Items.IndexOf(this.tabAtMouseDown);
            int newIndex = this.Items.IndexOf(upItem);

            int upCentre = 48 + newIndex * 87;
            if (downIndex < newIndex)
            {
                newIndex--;
                upCentre += 10;
            }

            if (mouseX >= upCentre)
            {
                newIndex++;
            }

            if (newIndex > this.Items.Count - 1)
            {
                newIndex = this.Items.Count - 1;
            }

            if (newIndex <= 0)
            {
                newIndex = 0;
            }

            this.Items.Remove(this.tabAtMouseDown);
            this.Items.Insert(newIndex, this.tabAtMouseDown);
        }

        private bool MouseIsOnMenuGliph(MouseEventArgs e)
        {
            return this.menuGlyph.Rect.Contains(e.Location);
        }

        private bool MouseIsOnCloseButton(MouseEventArgs e)
        {
            return this.closeButton.Rect.Contains(e.Location);
        }

        private void ShowTabsMenu()
        {
            HandledEventArgs args = new HandledEventArgs(false);
            this.OnMenuItemsLoading(args);

            if (!args.Handled)
            {
                this.OnMenuItemsLoad(EventArgs.Empty);
            }

            this.OnMenuShow();
        }

        private void OnTabControlMouseOnTitle(TabControlMouseOnTitleEventArgs e)
        {
            if (this.TabControlMouseOnTitle != null)
            {
                this.TabControlMouseOnTitle(e);
            }
        }

        private void OnTabControlMouseEnteredTitle(TabControlMouseOnTitleEventArgs e)
        {
            if (this.TabControlMouseEnteredTitle != null)
            {
                this.TabControlMouseEnteredTitle(e);
            }
        }

        private void OnTabControlMouseLeftTitle(TabControlMouseOnTitleEventArgs e)
        {
            if (this.TabControlMouseLeftTitle != null)
            {
                this.TabControlMouseLeftTitle(e);
            }
        }

        private void OnTabControlItemClosing(TabControlItemClosingEventArgs e)
        {
            if (this.TabControlItemClosing != null)
                this.TabControlItemClosing(e);
        }

        private void OnTabControlItemClosed(EventArgs e)
        {
            if (this.TabControlItemClosed != null)
                this.TabControlItemClosed(this, e);
        }

        private void SetDefaultSelected()
        {
            if (this.selectedItem == null && this.Items.Count > 0)
                this.SelectedItem = this.Items[0];

            for (int i = 0; i < this.Items.Count; i++)
            {
                TabControlItem itm = this.Items[i];
                itm.Dock = DockStyle.Fill;
            }
        }

        private void SelectItem(TabControlItem tabItem)
        {
            tabItem.Dock = DockStyle.Fill;
            tabItem.Visible = true;
            tabItem.Selected = true;
        }

        private void UnSelectItem(TabControlItem tabItem)
        {
            tabItem.Selected = false;
        }

        private void OnMenuItemsLoading(HandledEventArgs e)
        {
            if (this.MenuItemsLoading != null)
                this.MenuItemsLoading(this, e);
        }

        private void OnMenuShow()
        {
            if (this.Menu.Visible == false && this.Menu.Items.Count > 0)
            {
                if (this.RightToLeft == RightToLeft.No)
                {
                    this.Menu.Show(this, new Point(this.menuGlyph.Rect.Left, this.menuGlyph.Rect.Bottom + 2));
                }
                else
                {
                    this.Menu.Show(this, new Point(this.menuGlyph.Rect.Right, this.menuGlyph.Rect.Bottom + 2));
                }

                this.MenuOpen = true;
            }
        }

        private void OnMenuItemsLoaded(EventArgs e)
        {
            if (this.MenuItemsLoaded != null)
                this.MenuItemsLoaded(this, e);
        }

        private void OnMenuItemsLoad(EventArgs e)
        {
            this.Menu.RightToLeft = this.RightToLeft;
            this.Menu.Items.Clear();

            List<ToolStripMenuItem> list = new List<ToolStripMenuItem>();

            for (int i = 0; i < this.Items.Count; i++)
            {
                TabControlItem item = this.Items[i];

                if (!item.Visible)
                    continue;

                ToolStripMenuItem tItem = new ToolStripMenuItem(item.Title);

                tItem.Tag = item;

                if (item.Selected)
                    tItem.Select();

                list.Add(tItem);
            }

            // Sort by caption, else do nothing i.e. sorted by call sequence not by caption!!!!!
            if (Terminals.Configuration.Files.Main.Settings.Settings.SortTabPagesByCaption)
                list.Sort(CompareSortText);

            this.Menu.Items.AddRange(list.ToArray());
            this.OnMenuItemsLoaded(EventArgs.Empty);
        }

        private void OnMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TabControlItem clickedItem = (TabControlItem)e.ClickedItem.Tag;

            if (clickedItem != null)
            {
                this.SelectedItem = clickedItem;
                this.Items.MoveTo(0, clickedItem);
            }
        }

        private void OnMenuVisibleChanged(object sender, EventArgs e)
        {
            if (this.Menu.Visible == false)
            {
                this.MenuOpen = false;
            }
        }

        private void HandlePreviewMove(MouseEventArgs e)
        {
            if (this.tabAtMouseDown != null)
            {
                this.UpdateMovePreviewLocation(e);
                this.ShowTabPreview(e);
            }
        }

        private void UpdateMovePreviewLocation(MouseEventArgs e)
        {
            Point newLocation = this.PointToScreen(e.Location);
            newLocation.X -= this.mouseDownPoint.X;
            newLocation.Y -= this.mouseDownPoint.Y;
            this.movePreview.Location = newLocation;
        }

        private void ShowTabPreview(MouseEventArgs e)
        {
            bool movedOutOfTolerance = this.MovedOutOfTolerance(e);

            if (movedOutOfTolerance)
            {
                bool toDetach = this.IsMouseOutsideHeader(e.Location);
                this.movePreview.UpdateDetachState(toDetach);
                this.movePreview.Show(this.tabAtMouseDown);
            }
        }

        private bool MovedOutOfTolerance(MouseEventArgs e)
        {
            int xDelta = Math.Abs(this.mouseDownPoint.X - e.Location.X);
            int yDelta = Math.Abs(this.mouseDownPoint.Y - e.Location.Y);
            bool movedOutOfTolerance = xDelta > MOVE_TOLERANCE || yDelta > MOVE_TOLERANCE;
            return movedOutOfTolerance;
        }

        private void HandleMouseInTitle(MouseEventArgs e)
        {
            TabControlItem item = this.GetTabItemByPoint(e.Location);

            if (item != null)
            {
                var inTitle = item.LocationIsInTitle(e.Location);
                TabControlMouseOnTitleEventArgs args = new TabControlMouseOnTitleEventArgs(item);

                if (inTitle)
                {
                    this.OnTabControlMouseOnTitle(args);

                    if (!this.mouseEnteredTitle)
                    {
                        this.mouseEnteredTitle = true;
                        this.OnTabControlMouseEnteredTitle(args);
                    }
                }
                else if (this.mouseEnteredTitle)
                {
                    this.mouseEnteredTitle = false;
                    this.OnTabControlMouseLeftTitle(args);
                }
            }
        }

        private void HandleDrawMenuGliph(MouseEventArgs e)
        {
            if (this.MouseIsOnMenuGliph(e))
            {
                this.menuGlyph.IsMouseOver = true;
                this.Invalidate(this.menuGlyph.Rect);
            }
            else
            {
                if (this.menuGlyph.IsMouseOver && !this.MenuOpen)
                {
                    this.menuGlyph.IsMouseOver = false;
                    this.Invalidate(this.menuGlyph.Rect);
                }
            }
        }

        private void HandleDrawCloseButton(MouseEventArgs e)
        {
            if (this.MouseIsOnCloseButton(e))
            {
                this.closeButton.IsMouseOver = true;
                this.Invalidate(this.closeButton.Rect);
            }
            else
            {
                if (this.closeButton.IsMouseOver)
                {
                    this.closeButton.IsMouseOver = false;
                    this.Invalidate(this.closeButton.Rect);
                }
            }
        }

        private void OnCalcTabPage(Graphics g, TabControlItem currentItem)
        {
            Font currentFont = this.Font;

            if (currentItem == this.SelectedItem)
                currentFont = new Font(this.Font, FontStyle.Bold);

            SizeF textSize = g.MeasureString(currentItem.Title, currentFont, new SizeF(200, 10), this.sf);
            textSize.Width += 20;

            if (this.RightToLeft == RightToLeft.No)
            {
                RectangleF buttonRect = new RectangleF(this.startPosition, 3, textSize.Width, 17);
                currentItem.StripRect = buttonRect;
                this.startPosition += (int)textSize.Width;
            }
            else
            {
                RectangleF buttonRect = new RectangleF(this.startPosition - textSize.Width + 1, 3, textSize.Width - 1, 17);
                currentItem.StripRect = buttonRect;
                this.startPosition -= (int)textSize.Width;
            }
        }

        private void UpdateLayout()
        {
            if (this.RightToLeft == RightToLeft.No)
            {
                this.sf.Trimming = StringTrimming.EllipsisCharacter;
                this.sf.FormatFlags |= StringFormatFlags.NoWrap;
                this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;

                this.stripButtonRect = new Rectangle(0, 0, this.ClientSize.Width - DEF_GLYPH_WIDTH - 2, 10);
                this.menuGlyph.Rect = new Rectangle(this.ClientSize.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
                this.closeButton.Rect = new Rectangle(this.ClientSize.Width - 20, 2, 16, 15);
            }
            else
            {
                this.sf.Trimming = StringTrimming.EllipsisCharacter;
                this.sf.FormatFlags |= StringFormatFlags.NoWrap;
                this.sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                this.stripButtonRect = new Rectangle(DEF_GLYPH_WIDTH + 2, 0, this.ClientSize.Width - DEF_GLYPH_WIDTH - 15, 10);
                this.menuGlyph.Rect = new Rectangle(20 + 4, 2, 16, 16);//this.ClientSize.Width - 20, 2, 16, 16);
                this.closeButton.Rect = new Rectangle(4, 2, 16, 15);//ClientSize.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
            }

            int borderWidth = (this.showBorder ? 1 : 0);
            int headerWidth = (this.showTabs ? DEF_HEADER_HEIGHT + 1 : 1);

            this.DockPadding.Top = headerWidth;
            this.DockPadding.Bottom = borderWidth;
            this.DockPadding.Right = borderWidth;
            this.DockPadding.Left = borderWidth;
        }

        private void OnTabControlItemChanged(TabControlItemChangedEventArgs e)
        {
            if (this.TabControlItemSelectionChanged != null)
                this.TabControlItemSelectionChanged(e);
        }

        private void OnCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            TabControlItem itm = (TabControlItem)e.Element;

            if (e.Action == CollectionChangeAction.Add)
            {
                this.Controls.Add(itm);
                this.OnTabControlItemChanged(new TabControlItemChangedEventArgs(itm));
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                this.Controls.Remove(itm);
                this.OnTabControlItemChanged(new TabControlItemChangedEventArgs(itm));
            }
            else
            {
                this.OnTabControlItemChanged(new TabControlItemChangedEventArgs(itm));
            }

            this.UpdateLayout();
            this.Invalidate();
        }

        private static int CompareSortText(ToolStripMenuItem x, ToolStripMenuItem y)
        {
            return x.Text.CompareTo(y.Text);
        }
        #endregion

        #region Constructor (1)
        public TabControl()
        {
            MenuOpen = false;
            this.BeginInit();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.Selectable, true);

            this.Items = new TabControlItemCollection();
            this.Items.CollectionChanged += new CollectionChangeEventHandler(this.OnCollectionChanged);
            base.Size = new Size(350, 200);

            this.Menu = new ContextMenuStrip();
            this.Menu.Renderer = this.ToolStripRenderer;
            this.Menu.ItemClicked += new ToolStripItemClickedEventHandler(this.OnMenuItemClicked);
            this.Menu.VisibleChanged += new EventHandler(this.OnMenuVisibleChanged);

            this.menuGlyph = new TabControlMenuGlyph(this.ToolStripRenderer);
            this.closeButton = new TabControlCloseButton(this.ToolStripRenderer);
            this.Font = new Font("Tahoma", 8.25f, FontStyle.Regular);
            this.sf = new StringFormat();
            this.movePreview = new TabPreview(this);
            this.movePreview.AllowDrop = true;

            this.EndInit();

            this.UpdateLayout();
        }
        #endregion

        #region Properties (11)
        public bool ShowToolTipOnTitle { get; set; }

        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.All)]
        public TabControlItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (this.selectedItem == value)
                    return;

                if (value == null && this.Items.Count > 0)
                {
                    TabControlItem itm = this.Items[0];

                    if (itm.Visible)
                    {
                        this.selectedItem = itm;
                        this.selectedItem.Selected = true;
                        this.selectedItem.Dock = DockStyle.Fill;
                    }
                }
                else
                {
                    this.selectedItem = value;
                }

                foreach (TabControlItem itm in this.Items)
                {
                    if (itm == this.selectedItem)
                    {
                        this.SelectItem(itm);
                        itm.Dock = DockStyle.Fill;
                        itm.Show();
                    }
                    else
                    {
                        this.UnSelectItem(itm);
                        itm.Hide();
                    }
                }

                if (this.selectedItem != null)
                    this.SelectItem(this.selectedItem);

                this.Invalidate();
                this.OnTabControlItemChanged(new TabControlItemChangedEventArgs(this.selectedItem));
            }
        }

        [DefaultValue(typeof(Size), "350,200")]
        public new Size Size
        {
            set
            {
                if (base.Size == value)
                    return;

                base.Size = value;
                this.UpdateLayout();
            }
            get
            {
                return base.Size;
            }
        }

        [DefaultValue(true)]
        private bool AlwaysShowMenuGlyph
        {
            get { return this.alwaysShowMenuGlyph; }
            set
            {
                if (this.alwaysShowMenuGlyph == value)
                    return;

                this.alwaysShowMenuGlyph = value;
                this.Invalidate();
            }
        }

        [DefaultValue(true)]
        public bool AlwaysShowClose
        {
            get { return this.alwaysShowClose; }
            set
            {
                if (this.alwaysShowClose == value)
                    return;

                this.alwaysShowClose = value;
                this.Invalidate();
            }
        }

        [DefaultValue(true)]
        public bool ShowTabs
        {
            get
            {
                return this.showTabs;
            }
            set
            {
                if (this.showTabs != value)
                {
                    this.showTabs = value;
                    this.Invalidate();
                    this.UpdateLayout();
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowBorder
        {
            get
            {
                return this.showBorder;
            }
            set
            {
                if (this.showBorder != value)
                {
                    this.showBorder = value;
                    this.Invalidate();
                    this.UpdateLayout();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControlItemCollection Items { get; private set; }

        /// <summary>
        /// DesignerSerializationVisibility
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private new ControlCollection Controls
        {
            get { return base.Controls; }
        }

        [Browsable(false)]
        public ContextMenuStrip Menu { get; private set; }

        [Browsable(false)]
        public bool MenuOpen { get; private set; }
        #endregion

        #region ISupportInitialize Members (2)
        public void BeginInit()
        {
            this.isInitializing = true;
        }

        public void EndInit()
        {
            this.isInitializing = false;
        }
        #endregion
    }
}
