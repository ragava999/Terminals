using System.Drawing;
using System.Windows.Forms;

namespace ExplorerBrowser
{
    public class Explorer : UserControl
    {
        private readonly Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser explorerBrowser = new Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser();
        private readonly ExplorerHeader explorerHeader;
        private readonly System.Windows.Forms.SplitContainer splitContainer1 = new System.Windows.Forms.SplitContainer();

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

        public string[] KnownFolders
        {
            get
            {
                if (explorerHeader == null || explorerHeader.KnownFolders == null)
                    return new string[] { };

                return explorerHeader.KnownFolders;
            }
        }

        public Color ColorTop
        {
            get { return (base.BackColor = this.explorerHeader.ColorTop); }
            set { base.BackColor = this.explorerHeader.ColorTop = value; }
        }

        public new Color BackColor
        {
            get { return this.explorerHeader.ColorTop; }
            set { /* Do nothing */ }
        }

        public string InitDirectory
        {
            get
            {
                return this.explorerHeader.InitDirectory;
            }
            set
            {
                this.explorerHeader.InitDirectory = value;
            }
        }

        public Color ColorBottom
        {
            get { return this.explorerHeader.ColorBottom; }
            set { this.explorerHeader.ColorBottom = value; }
        }

        public void NavigateToInitDirectory()
        {
            if (!string.IsNullOrEmpty(InitDirectory))
            {
                this.explorerHeader.NavigateToFolderOrArchive(InitDirectory);
            }
        }

        public void NavigateToSpecialFolder(string knownFolder)
        {
            this.explorerHeader.NavigateToSpecialFolder(knownFolder);
        }

        public void NavigateToFolderOrArchive(string url)
        {
            this.explorerHeader.NavigateToFolderOrArchive(url);
        }

        public Explorer()
        {
            explorerHeader = new ExplorerHeader(explorerBrowser);

            this.SuspendLayout();

            this.explorerHeader.ColorBottom = this.ColorBottom;
            this.explorerHeader.ColorTop = this.ColorTop;
            this.explorerHeader.DisallowNavigation = false;
            this.explorerHeader.Dock = DockStyle.Fill;
            this.explorerHeader.InitDirectory = null;
            this.explorerHeader.Location = new Point(0, 0);
            this.explorerHeader.TabIndex = 1;
            this.explorerBrowser.TabStop = true;

            this.explorerBrowser.Dock = DockStyle.Fill;
            this.explorerBrowser.PropertyBagName = "Oliver Kohl D.Sc. Explorer";
            this.explorerBrowser.TabIndex = 2;
            this.explorerBrowser.TabStop = false;

            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.FixedPanel = FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new Point(0, 0);
            this.splitContainer1.Orientation = Orientation.Horizontal;
            this.splitContainer1.SplitterDistance = this.explorerHeader.Height;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            this.explorerBrowser.TabStop = false;

            this.splitContainer1.Panel1.Controls.Add(this.explorerHeader);
            this.splitContainer1.Panel1.Size = new Size(this.explorerHeader.Width, this.explorerHeader.Height);
            this.splitContainer1.Panel2.Controls.Add(this.explorerBrowser);

            this.Controls.Add(splitContainer1);

            this.AutoScaleMode = AutoScaleMode.Font;
            this.Name = "Explorer";
            this.AutoSize = true;
            this.BackColor = Color.Transparent;

            this.ResumeLayout(false);
        }
    }
}