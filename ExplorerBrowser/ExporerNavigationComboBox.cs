using System.Drawing;
using System.Windows.Forms;

namespace ExplorerBrowser
{
    internal partial class ExporerNavigationComboBox : UserControl
    {
        private readonly DataComboBox pathComboBox = new DataComboBox
                                                            {
                                                                BackgroundImageLayout = ImageLayout.Center,
                                                                Cursor = Cursors.IBeam,
                                                                Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                                                                ForeColor = Color.SteelBlue,
                                                                ImeMode = ImeMode.On,
                                                                Location = new Point(3, 5),
                                                                TabIndex = 1
                                                            };

        public ComboBox.ObjectCollection Items
        {
            get
            {
                if (this.pathComboBox == null)
                    return null;

                return this.pathComboBox.Items;
            }
        }


        public new DataComboBox.Element Text
        {
            get { return this.pathComboBox.Text; }
            set { this.pathComboBox.Text = value; }
        }

        public ExporerNavigationComboBox()
        {
            this.Resize += ExporerNavigationControl_Resize;

            this.Controls.Clear();

            InitializeComponent();

            this.picExporerNavigationComboBoxButton.BackgroundImage = Resources.ExporerNavigationComboBoxButton_Inactive;

            int start = picExporerNavigationComboBox.Width;
            int end = picExporerNavigationComboBoxButton.Location.X;

            this.pathComboBox.Size = new Size(end, 16);
            this.pathComboBox.KeyDown += pathComboBox_KeyDown;

            this.Controls.Add(this.pathComboBox);

            for (int i = start; i < end; i++)
            {
                this.Controls.Add(new PictureBox
                                      {
                                          BackgroundImage = Resources.ExporerNavigationComboBox_Pixel,
                                          Location = new Point(i, 0),
                                          BackColor = Color.Transparent
                                      });
            }
        }

        void pathComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.OnClick(e);
                this.OnMouseClick(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
            }
        }

        void ExporerNavigationControl_Resize(object sender, System.EventArgs e)
        {
            this.pathComboBox.Size = new Size(picExporerNavigationComboBoxButton.Location.X, 16);
        }

        public new Size Size
        {
            get
            {
                return new Size(base.Size.Width, 30);
            }
            set
            {
                base.Size = new Size(value.Width, 30);
            }
        }

        private void picTextBoxEnd_MouseHover(object sender, System.EventArgs e)
        {
            this.picExporerNavigationComboBoxButton.BackgroundImage = Resources.ExporerNavigationComboBoxButton_Hover;
        }

        private void picTextBoxEnd_MouseLeave(object sender, System.EventArgs e)
        {
            this.picExporerNavigationComboBoxButton.BackgroundImage = Resources.ExporerNavigationComboBoxButton_Inactive;
        }

        private void picTextBoxEnd_MouseUp(object sender, MouseEventArgs e)
        {
            this.picExporerNavigationComboBoxButton.BackgroundImage = Resources.ExporerNavigationComboBoxButton_Inactive;
        }

        private void picTextBoxEnd_MouseDown(object sender, MouseEventArgs e)
        {
            this.picExporerNavigationComboBoxButton.BackgroundImage = Resources.ExporerNavigationComboBoxButton_Click;
        }

        private void picTextBoxEnd_MouseClick(object sender, MouseEventArgs e)
        {
            this.OnClick(e);
            this.OnMouseClick(e);
        }
    }
}