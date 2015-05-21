using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ExplorerBrowser
{
    internal partial class ExplorerNavigationButtons : UserControl
    {
        public EventHandler DropDownClick;
        public EventHandler LeftButtonClick;
        public EventHandler RightButtonClick;

        public ExplorerNavigationButtons()
        {
            InitializeComponent();

            // Don't allow someone to resize our application
            base.MinimumSize = this.MinimumSize;
            base.MaximumSize = this.MaximumSize;

            this.picLeft.Parent = picBackground;
            this.picRight.Parent = picBackground;
            this.picDropDown.Parent = picBackground;

            this.picLeft.BackgroundImage = LeftButtonImage;
            this.picRight.BackgroundImage = RightButtonImage;
            this.picDropDown.BackgroundImage = DropDownButtonImage;

            DropDownClick += (sender, args) => { };
            LeftButtonClick += (sender, args) => { };
            RightButtonClick += (sender, args) => { };
        }

        private bool hasLeftHistroy;

        public bool HasLeftHistroy
        {
            get { return hasLeftHistroy; }
            set
            {
                hasLeftHistroy = value;
                this.picRight.BackgroundImage = RightButtonImage;
                this.picLeft.BackgroundImage = LeftButtonImage;
                this.picDropDown.BackgroundImage = DropDownButtonImage;
            }
        }

        public Image LeftButtonImage
        {
            get
            {
                if (hasLeftHistroy)
                    return Resources.ExplorerNavigationButtons_Left_Active;

                return null;
            }
        }

        private bool hasRightHistroy;

        public bool HasRightHistroy
        {
            get { return hasRightHistroy; }
            set
            {
                hasRightHistroy = value;
                this.picRight.BackgroundImage = RightButtonImage;
                this.picLeft.BackgroundImage = LeftButtonImage;
            }
        }

        public Image RightButtonImage
        {
            get
            {
                if (hasRightHistroy)
                    return Resources.ExplorerNavigationButtons_Right_Active;

                return null;
            }
        }

        public bool HasDropDownHistroy
        {
            get { return (hasLeftHistroy || hasRightHistroy); }
        }

        public Image DropDownButtonImage
        {
            get
            {
                if (HasDropDownHistroy)
                    return Resources.ExplorerNavigationButtons_DropDown_Active;

                return null;
            }
        }

        /// <summary>
        /// Ruft die Größe ab, die die Untergrenze bildet, die <see cref="M:System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)"/> angeben kann, oder legt diese fest.
        /// </summary>
        /// 
        /// <returns>
        /// Ein geordnetes Paar vom Typ <see cref="T:System.Drawing.Size"/>, das die Breite und Höhe eines Rechtecks darstellt.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new Size MinimumSize
        {
            get { return this.Size; }
            set
            {
                // do nothing
            }
        }

        /// <summary>
        /// Ruft die Größe ab, die die Obergrenze bildet, die <see cref="M:System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)"/> angeben kann, oder legt diese fest.
        /// </summary>
        /// 
        /// <returns>
        /// Ein geordnetes Paar vom Typ <see cref="T:System.Drawing.Size"/>, das die Breite und Höhe eines Rechtecks darstellt.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new Size MaximumSize
        {
            get { return this.Size; }
            set
            {
                // do nothing
            }
        }

        private void picLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if (HasLeftHistroy)
                this.picLeft.BackgroundImage = Resources.ExplorerNavigationButtons_Left_Click;
        }

        private void picLeft_MouseHover(object sender, EventArgs e)
        {
            if (HasLeftHistroy)
                this.picLeft.BackgroundImage = Resources.ExplorerNavigationButtons_Left_Hover;
        }

        private void picLeft_MouseLeave(object sender, EventArgs e)
        {
            this.picLeft.BackgroundImage = LeftButtonImage;
        }

        private void picLeft_MouseUp(object sender, MouseEventArgs e)
        {
            this.picLeft.BackgroundImage = LeftButtonImage;
        }

        private void picLeft_MouseClick(object sender, MouseEventArgs e)
        {
            LeftButtonClick(sender, e);
        }

        private void picRight_MouseClick(object sender, MouseEventArgs e)
        {
            RightButtonClick(sender, e);
        }

        private void picRight_MouseDown(object sender, MouseEventArgs e)
        {
            if (HasRightHistroy)
                this.picRight.BackgroundImage = Resources.ExplorerNavigationButtons_Right_Click;
        }

        private void picRight_MouseHover(object sender, EventArgs e)
        {
            if (HasRightHistroy)
                this.picRight.BackgroundImage = Resources.ExplorerNavigationButtons_Right_Hover;
        }

        private void picRight_MouseLeave(object sender, EventArgs e)
        {
            this.picRight.BackgroundImage = RightButtonImage;
        }

        private void picRight_MouseUp(object sender, MouseEventArgs e)
        {
            this.picRight.BackgroundImage = RightButtonImage;
        }

        private void picDropDown_MouseClick(object sender, MouseEventArgs e)
        {
            DropDownClick(sender, e);
        }

        private void picDropDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (HasDropDownHistroy)
                this.picDropDown.BackgroundImage = Resources.ExplorerNavigationButtons_DropDown_Click;
        }

        private void picDropDown_MouseHover(object sender, EventArgs e)
        {
            if (HasDropDownHistroy)
                this.picDropDown.BackgroundImage = Resources.ExplorerNavigationButtons_DropDown_Hover;
        }

        private void picDropDown_MouseLeave(object sender, EventArgs e)
        {
            this.picDropDown.BackgroundImage = DropDownButtonImage;
        }

        private void picDropDown_MouseUp(object sender, MouseEventArgs e)
        {
            this.picDropDown.BackgroundImage = DropDownButtonImage;
        }
    }
}