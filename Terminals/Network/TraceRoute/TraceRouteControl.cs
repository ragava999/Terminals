using Kohl.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Terminals.Network.TraceRoute
{
    public partial class TraceRouteControl : UserControl
    {
        #region Fields

        private readonly MethodInvoker DoUpdateForm;
        private List<TraceRouteHopData> hopList = new List<TraceRouteHopData>();
        private GraphPane myPane;
        private Boolean traceRunning;
        private TraceRoute tracert;

        #endregion

        #region Constructors

        public TraceRouteControl()
        {
            this.InitializeComponent();
            this.DoUpdateForm = this.UpdateForm;

            this.InitializeGraph();


            this.dataGridView1.Columns.Add("1", "Count");
            this.dataGridView1.Columns.Add("2", "Address");
            this.dataGridView1.Columns.Add("3", "Roundtrip time");
            this.dataGridView1.Columns.Add("4", "Status");
            this.dataGridView1.Columns.Add("5", "Host name");
            this.dataGridView1.Columns.Add("6", "Time");
        }

        #endregion

        #region Form Events

        private void TraceRoute_Load(object sender, EventArgs e)
        {
            this.TextHost.Focus();
        }

        private void TraceRoute_Resize(object sender, EventArgs e)
        {
            this.SetSize();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.TextHost.Text.Trim()))
            {
                this.TextHost.Focus();
                return;
            }
            this.TextHost.Text = this.TextHost.Text.Trim();

            this.ButtonStart.Enabled = false;
            this.ButtonStop.Enabled = true;
            this.TextHost.Enabled = false;
            Application.DoEvents();

            if (!this.traceRunning)
            {
                if (this.tracert == null)
                {
                    this.tracert = new TraceRoute();
                    this.tracert.Completed += this.tracert_Completed;
                    this.tracert.RouteHopFound += this.tracert_RouteHopFound;
                }

                this.tracert.Destination = this.TextHost.Text;
                this.tracert.ResolveNames = this.ResolveCheckBox.Checked;

                try
                {
                    this.tracert.Start();
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    this.ResetForm();
                    MessageBox.Show(ex.Message, "Ups ...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.traceRunning = true;
            }

            this.hopList = new List<TraceRouteHopData>();
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            this.tracert.Cancel();
            this.ResetForm();
        }

        private void TextHost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                if (!this.traceRunning)
                    this.ButtonStart.PerformClick();
            }
        }

        #endregion

        #region Developer made methods

        private void tracert_Completed(object sender, EventArgs e)
        {
            this.traceRunning = false;
            this.ResetForm();
        }

        private void tracert_RouteHopFound(object sender, RouteHopFoundEventArgs e)
        {
            this.hopList = this.tracert.Hops;
            this.Invoke(this.DoUpdateForm);
        }

        /// <summary>
        ///     Reset the form control to start properties.
        /// </summary>
        private void ResetForm()
        {
            this.ButtonStart.Enabled = true;
            this.ButtonStop.Enabled = false;
            this.TextHost.Enabled = true;
            this.TextHost.Focus();
            this.TextHost.SelectAll();
        }

        public void ForceTrace(String hostName)
        {
            this.TextHost.Text = hostName;
            this.ButtonStart.PerformClick();
        }

        /// <summary>
        ///     Update form control with new data.
        /// </summary>
        private void UpdateForm()
        {
            TraceRouteHopData data = this.hopList[this.hopList.Count - 1];

            this.dataGridView1.Rows.Add(new object[] { data.Count, data.Address, data.RoundTripTime, data.Status, data.HostName, data.Time });

            if (this.dataGridView1.Rows.Count > 1)
                this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;

            this.UpdateGraph();
        }

        #endregion

        #region Graph Control

        private void InitializeGraph()
        {
            this.myPane = this.ZGraph.GraphPane;
            // Set the titles and axis labels
            this.myPane.Title.Text = "Trace Route results";
            this.myPane.XAxis.Title.Text = "Counter";
            this.myPane.YAxis.Title.Text = "Time, Milliseconds";

            // Show the x axis grid
            this.myPane.XAxis.MajorGrid.IsVisible = true;

            // Make the Y axis scale red
            this.myPane.YAxis.Scale.FontSpec.FontColor = Color.Blue;
            this.myPane.YAxis.Title.FontSpec.FontColor = Color.Blue;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            this.myPane.YAxis.MajorTic.IsOpposite = false;
            this.myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            this.myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            this.myPane.YAxis.Scale.Align = AlignP.Inside;

            // Fill the axis background with a gradient
            this.myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);

            // Add a text box with instructions
            TextObj text = new TextObj(
                "Zoom: left mouse & drag\nPan: middle mouse & drag\nContext Menu: right mouse",
                0.02f, 0.15f, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom)
            {
                FontSpec =
                                       {
                                           Size = 8,
                                           StringAlignment = StringAlignment.Near
                                       }
            };
            this.myPane.GraphObjList.Add(text);

            // Enable scrollbars if needed
            this.ZGraph.IsShowHScrollBar = true;
            this.ZGraph.IsShowVScrollBar = true;

            // OPTIONAL: Show tooltips when the mouse hovers over a point
            this.ZGraph.IsShowPointValues = true;
            this.ZGraph.PointValueEvent += this.MyPointValueHandler;

            // OPTIONAL: Add a custom context menu item
            this.ZGraph.ContextMenuBuilder += this.MyContextMenuBuilder;

            // OPTIONAL: Handle the Zoom Event

            // Size the control to fit the window
            this.SetSize();
        }

        private void UpdateGraph()
        {
            // Make up some data points based on the Sine function
            PointPairList list = new PointPairList();
            PointPairList avgList = new PointPairList();
            Int32 x = 1;
            Int64 yMax = 0;
            Int64 sum = 0;

            foreach (TraceRouteHopData p in this.hopList)
            {
                if (p.RoundTripTime > yMax)
                    yMax = p.RoundTripTime;

                list.Add(x, p.RoundTripTime, p.Address.ToString());

                sum += p.RoundTripTime;
                avgList.Add(x, (Int32)(sum / x));
                x++;
            }

            this.myPane.Title.Text = String.Format("Trace Route results for {0}", this.TextHost.Text);

            // Manually set the axis range
            this.myPane.YAxis.Scale.Min = 0;
            this.myPane.YAxis.Scale.Max = yMax;
            this.myPane.XAxis.Scale.Min = 0;
            this.myPane.XAxis.Scale.Max = x;

            this.myPane.CurveList.Clear();
            LineItem myCurve = this.myPane.AddCurve(this.TextHost.Text, list, Color.Blue, SymbolType.Diamond);
            LineItem avgCurve = this.myPane.AddCurve("Average", avgList, Color.Red, SymbolType.Diamond);

            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);
            // Associate this curve with the Y2 axis
            myCurve.IsY2Axis = true;

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            this.ZGraph.AxisChange();
            // Make sure the Graph gets redrawn
            this.ZGraph.Invalidate();
        }

        /// <summary>
        ///     Display customized tooltips when the mouse hovers over a point
        /// </summary>
        private string MyPointValueHandler(ZedGraphControl control, GraphPane pane, CurveItem curve, int iPt)
        {
            try
            {
                // Get the PointPair that is under the mouse
                PointPair pt = curve[iPt];

                return String.Format("{0} is {1:f2} milliseconds at {2:f1}", pt.Tag, pt.Y, pt.X);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return String.Empty;
        }

        private void SetSize()
        {
            this.ZGraph.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            this.ZGraph.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 20);
        }

        /// <summary>
        ///     Customize the context menu by adding a new item to the end of the menu
        /// </summary>
        private void MyContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt,
                                          ZedGraphControl.ContextMenuObjectState objState)
        {
            //ToolStripMenuItem item = new ToolStripMenuItem();
            //item.Name = "add-beta";
            //item.Tag = "add-beta";
            //item.Text = "Add a new Beta Point";
            //item.Click += new System.EventHandler(AddBetaPoint);

            //menuStrip.Items.Add(item);
        }

        #endregion
    }
}