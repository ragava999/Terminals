using System.Drawing;
using System.Windows.Forms;

namespace ExplorerBrowser
{
    public class ControlStyler : UserControl
    {
        private ControlStyle controlStyle;

        public ControlStyle ControlStyle
        {
            get { return this.controlStyle; }
            set
            {
                this.controlStyle = value;

                if (this.cmbStyles != null)
                    RefreshView();
            }
        }

        public Control FirstControl { get; set; }

        public Control SecondControl { get; set; }

        public Control ThirdControl { get; set; }

        public Control FourthControl { get; set; }

        public ControlStyler()
        {
            this.Dock = DockStyle.Fill;
        }

        public new void Show()
        {
            this.RefreshView();
        }

        private void RefreshView()
        {
            this.SuspendLayout();

            if (contContent == null)
            {
                this.InitializeComponent();
                this.cmbStyles.Items.Clear();
                this.cmbStyles.Items.AddRange(System.Enum.GetNames(typeof(ControlStyle)));
                this.cmbStyles.Text = System.Enum.GetName(typeof(ControlStyle), controlStyle);
                this.cmbStyles.SelectedIndexChanged += this.cmbStyles_SelectedIndexChanged;
            }
            else
            {
                this.contContent.Panel2.Controls.Clear();
            }

            // Set the background color.
            if (FirstControl != null)
            {
                this.contContent.BackColor = FirstControl.BackColor;
                this.contContent.Panel1.BackColor = FirstControl.BackColor;
            }
            else if (SecondControl != null)
            {
                this.contContent.BackColor = SecondControl.BackColor;
                this.contContent.Panel1.BackColor = SecondControl.BackColor;
            }
            else if (ThirdControl != null)
            {
                this.contContent.BackColor = ThirdControl.BackColor;
                this.contContent.Panel1.BackColor = ThirdControl.BackColor;
            }
            else if (FourthControl != null)
            {
                this.contContent.BackColor = FourthControl.BackColor;
                this.contContent.Panel1.BackColor = FourthControl.BackColor;
            }

            //  +---+---+
            //  |       |
            //  +       +
            //  |       |
            //  +---+---+
            if (this.controlStyle == ControlStyle.Single)
            {
                if (FirstControl != null)
                    this.contContent.Panel2.Controls.Add(this.FirstControl);
            }
            //  +---+---+
            //  |   |   |
            //  +   +   +
            //  |   |   |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.DualVertical)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Width / 2;

                if (FirstControl != null)
                    splitContainer1.Panel1.Controls.Add(this.FirstControl);

                if (SecondControl != null)
                    splitContainer1.Panel2.Controls.Add(this.SecondControl);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  |       |
            //  +---+---+
            //  |       |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.DualHorizontal)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Orientation = Orientation.Horizontal;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Height / 2;

                if (FirstControl != null)
                    splitContainer1.Panel1.Controls.Add(this.FirstControl);

                if (SecondControl != null)
                    splitContainer1.Panel2.Controls.Add(this.SecondControl);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  |       |
            //  +---+---+
            //  |   |   |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.TrippleTop)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Orientation = Orientation.Horizontal;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Height / 2;

                if (FirstControl != null)
                    splitContainer1.Panel1.Controls.Add(this.FirstControl);

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Vertical;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Width / 2;

                if (SecondControl != null)
                    splitContainer2.Panel1.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer2.Panel2.Controls.Add(this.ThirdControl);

                splitContainer1.Panel2.Controls.Add(splitContainer2);
                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  |   |   |
            //  +---+---+
            //  |       |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.TrippleBottom)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Orientation = Orientation.Horizontal;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Height / 2;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Vertical;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Width / 2;

                splitContainer1.Panel1.Controls.Add(splitContainer2);

                if (FirstControl != null)
                    splitContainer2.Panel1.Controls.Add(this.FirstControl);

                if (SecondControl != null)
                    splitContainer2.Panel2.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer1.Panel2.Controls.Add(this.ThirdControl);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +--+-+--+
            //  |  | |  |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.TrippleVertical)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Orientation = Orientation.Vertical;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Width / 3;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Vertical;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Width / 2;

                if (FirstControl != null)
                    splitContainer1.Panel1.Controls.Add(this.FirstControl);

                splitContainer1.Panel2.Controls.Add(splitContainer2);

                if (SecondControl != null)
                    splitContainer2.Panel1.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer2.Panel2.Controls.Add(this.ThirdControl);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  +---+---+
            //  +---+---+
            //  +---+---+
            else if (this.controlStyle == ControlStyle.TrippleHorizontal)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Orientation = Orientation.Horizontal;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Height / 3;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Horizontal;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Height / 2;

                if (FirstControl != null)
                    splitContainer1.Panel1.Controls.Add(this.FirstControl);

                splitContainer1.Panel2.Controls.Add(splitContainer2);

                if (SecondControl != null)
                    splitContainer2.Panel1.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer2.Panel2.Controls.Add(this.ThirdControl);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  |   |   |
            //  +   +---+
            //  |   |   |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.TrippleLeft)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Width / 2;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Horizontal;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Height / 2;

                if (FirstControl != null)
                    splitContainer1.Panel1.Controls.Add(this.FirstControl);

                splitContainer1.Panel2.Controls.Add(splitContainer2);

                if (SecondControl != null)
                    splitContainer2.Panel1.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer2.Panel2.Controls.Add(this.ThirdControl);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  |   |   |
            //  +---+   +
            //  |   |   |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.TrippleRight)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Width / 2;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Horizontal;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Height / 2;

                splitContainer1.Panel1.Controls.Add(splitContainer2);

                if (FirstControl != null)
                    splitContainer1.Panel2.Controls.Add(this.FirstControl);

                if (SecondControl != null)
                    splitContainer2.Panel1.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer2.Panel2.Controls.Add(this.ThirdControl);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  |   |   |
            //  +---+---+
            //  |   |   |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.Quad)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Orientation = Orientation.Horizontal;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Height / 2;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Vertical;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Width / 2;

                SplitContainer splitContainer3 = new SplitContainer();
                splitContainer3.Dock = DockStyle.Fill;
                splitContainer3.Orientation = Orientation.Vertical;
                splitContainer3.Location = new Point(0, 0);
                splitContainer3.TabIndex = 0;
                splitContainer3.SplitterDistance = splitContainer3.Size.Width / 2;

                if (FirstControl != null)
                    splitContainer2.Panel1.Controls.Add(this.FirstControl);

                if (SecondControl != null)
                    splitContainer2.Panel2.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer3.Panel1.Controls.Add(this.ThirdControl);

                if (FourthControl != null)
                    splitContainer3.Panel2.Controls.Add(this.FourthControl);

                splitContainer1.Panel1.Controls.Add(splitContainer2);
                splitContainer1.Panel2.Controls.Add(splitContainer3);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +-+-+-+-+
            //  | | | | |
            //  +---+---+
            else if (this.controlStyle == ControlStyle.QuadVertical)
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Width / 2;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Vertical;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Width / 2;

                SplitContainer splitContainer3 = new SplitContainer();
                splitContainer3.Dock = DockStyle.Fill;
                splitContainer3.Orientation = Orientation.Vertical;
                splitContainer3.Location = new Point(0, 0);
                splitContainer3.TabIndex = 0;
                splitContainer3.SplitterDistance = splitContainer3.Size.Width / 2;

                if (FirstControl != null)
                    splitContainer2.Panel1.Controls.Add(this.FirstControl);

                if (SecondControl != null)
                    splitContainer2.Panel2.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer3.Panel1.Controls.Add(this.ThirdControl);

                if (FourthControl != null)
                    splitContainer3.Panel2.Controls.Add(this.FourthControl);

                splitContainer1.Panel1.Controls.Add(splitContainer2);
                splitContainer1.Panel2.Controls.Add(splitContainer3);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }
            //  +---+---+
            //  +---+---+
            //  +---+---+
            //  +---+---+
            //  +---+---+
            else // QuadHorizontal
            {
                SplitContainer splitContainer1 = new SplitContainer();
                splitContainer1.Dock = DockStyle.Fill;
                splitContainer1.Orientation = Orientation.Horizontal;
                splitContainer1.Location = new Point(0, 0);
                splitContainer1.TabIndex = 0;
                splitContainer1.SplitterDistance = splitContainer1.Size.Height / 2;

                SplitContainer splitContainer2 = new SplitContainer();
                splitContainer2.Dock = DockStyle.Fill;
                splitContainer2.Orientation = Orientation.Horizontal;
                splitContainer2.Location = new Point(0, 0);
                splitContainer2.TabIndex = 0;
                splitContainer2.SplitterDistance = splitContainer2.Size.Height / 2;

                SplitContainer splitContainer3 = new SplitContainer();
                splitContainer3.Dock = DockStyle.Fill;
                splitContainer3.Orientation = Orientation.Horizontal;
                splitContainer3.Location = new Point(0, 0);
                splitContainer3.TabIndex = 0;
                splitContainer3.SplitterDistance = splitContainer3.Size.Height / 2;

                if (FirstControl != null)
                    splitContainer2.Panel1.Controls.Add(this.FirstControl);

                if (SecondControl != null)
                    splitContainer2.Panel2.Controls.Add(this.SecondControl);

                if (ThirdControl != null)
                    splitContainer3.Panel1.Controls.Add(this.ThirdControl);

                if (FourthControl != null)
                    splitContainer3.Panel2.Controls.Add(this.FourthControl);

                splitContainer1.Panel1.Controls.Add(splitContainer2);
                splitContainer1.Panel2.Controls.Add(splitContainer3);

                this.contContent.Panel2.Controls.Add(splitContainer1);
            }

            this.ResumeLayout(false);
        }

        private SplitContainer contContent;
        private ComboBox cmbStyles;

        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private readonly System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.contContent = new System.Windows.Forms.SplitContainer();
            this.cmbStyles = new System.Windows.Forms.ComboBox();
            this.contContent.Panel1.SuspendLayout();
            this.contContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // contContent
            // 
            this.contContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contContent.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.contContent.IsSplitterFixed = true;
            this.contContent.Location = new System.Drawing.Point(0, 0);
            this.contContent.Name = "contContent";
            this.contContent.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // contContent.Panel1
            // 
            this.contContent.Panel1.Controls.Add(this.cmbStyles);
            this.contContent.Panel1MinSize = 22;
            this.contContent.Size = new System.Drawing.Size(422, 316);
            this.contContent.SplitterDistance = 22;
            this.contContent.SplitterWidth = 1;
            this.contContent.TabIndex = 0;
            // 
            // cmbStyles
            // 
            this.cmbStyles.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbStyles.FormattingEnabled = true;
            this.cmbStyles.Location = new System.Drawing.Point(0, 0);
            this.cmbStyles.Name = "cmbStyles";
            this.cmbStyles.Size = new System.Drawing.Size(121, 21);
            this.cmbStyles.TabIndex = 0;
            // 
            // ControlStyler
            // 
            this.Controls.Add(this.contContent);
            this.Name = "ControlStyler";
            this.Size = new System.Drawing.Size(422, 316);
            this.Load += new System.EventHandler(this.ControlStyler_Load);
            this.contContent.Panel1.ResumeLayout(false);
            this.contContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void cmbStyles_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.controlStyle = (ControlStyle)System.Enum.Parse(typeof(ControlStyle), this.cmbStyles.Text);
            RefreshView();
        }

        private void ControlStyler_Load(object sender, System.EventArgs e)
        {
            this.cmbStyles.Text = this.controlStyle.ToString();
        }
    }
}
