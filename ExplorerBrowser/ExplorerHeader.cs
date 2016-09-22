using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ExplorerBrowser
{
    internal partial class ExplorerHeader : UserControl
    {
        public delegate void LogInfo(string message);
        public delegate void LogError(string message);

        private readonly int height = 0;

        private readonly LogInfo logInfo;
        private readonly LogError logError;
        private readonly Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser explorerBrowser = null;

        private Color colorTop = Color.FromArgb(152, 180, 208);
        private Color colorBottom = Color.FromArgb(185, 209, 234);

        public new Color BackColor
        {
            get { return this.ColorTop; }
            set { /* do nothing */ }
        }

        public Color ColorTop
        {
            get { return colorTop; }
            set { colorTop = value; }
        }

        private string initDirectory = null;

        public string InitDirectory
        {
            get
            {
                return NormalizeDriveName(initDirectory);
            }
            set
            {
                initDirectory = NormalizeDriveName(value);
            }
        }


        public static bool IsLogicalDrive(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // don't check only for C:\ path formats but check also for C: paths.
            return (from drive in System.IO.Directory.GetLogicalDrives() where drive.Contains(path) select true).Count() > 0;
        }

        public static string NormalizeDriveName(string path)
        {
            if (IsLogicalDrive(path))
            {
                // Normalize paths like C: -> C:\
                System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(path);
                return d.Root.ToString();
            }

            return path;
        }

        public Color ColorBottom
        {
            get { return colorBottom; }
            set { colorBottom = value; }
        }

        public string[] KnownFolders { get; private set; }

        public ExplorerHeader(Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser explorerBrowser)
        {
            // Initialize our logging delegates
            logInfo += delegate (string message) { };
            logError += delegate (string message) { };

            this.explorerBrowser = explorerBrowser;

            this.Paint += ExplorerHeader_Paint;

            InitializeComponent();

            this.height = this.Size.Height;

            // Optimize Painting.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            // initialize known folder combo box
            List<string> knownFolderList = new List<string>();
            foreach (IKnownFolder folder in Microsoft.WindowsAPICodePack.Shell.KnownFolders.All)
            {
                knownFolderList.Add(folder.CanonicalName);
            }
            knownFolderList.Sort();
            KnownFolders = knownFolderList.ToArray();

            this.pathEdit.Items.AddRange(KnownFolders);

            // setup explorerBrowser navigation events
            this.explorerBrowser.NavigationFailed += new EventHandler<NavigationFailedEventArgs>(this.explorerBrowser_NavigationFailed);
            this.explorerBrowser.NavigationComplete += new EventHandler<NavigationCompleteEventArgs>(this.explorerBrowser_NavigationComplete);

            // set up Navigation log event and button state
            this.explorerBrowser.NavigationLog.NavigationLogChanged += new EventHandler<NavigationLogEventArgs>(this.NavigationLog_NavigationLogChanged);

            this.explorerNavigationButtons1.HasLeftHistroy = false;
            this.explorerNavigationButtons1.HasRightHistroy = false;
            this.explorerNavigationButtons1.LeftButtonClick += LeftButtonClick;
            this.explorerNavigationButtons1.RightButtonClick += RightButtonClick;
            this.explorerNavigationButtons1.DropDownClick += DropDownClick;

            this.pathEdit.Click += navigateButton_Click;

            picDeleteHistory.BackgroundImage = Resources.DeleteHistory;
        }

        private void LeftButtonClick(object sender, EventArgs eventArgs)
        {
            // Move backwards through navigation log
            if (this.explorerNavigationButtons1.HasLeftHistroy)
                this.explorerBrowser.NavigateLogLocation(NavigationLogDirection.Backward);
        }

        private void RightButtonClick(object sender, EventArgs eventArgs)
        {
            // Move forwards through navigation log
            if (this.explorerNavigationButtons1.HasRightHistroy)
                this.explorerBrowser.NavigateLogLocation(NavigationLogDirection.Forward);
        }

        private void DropDownClick(object sender, EventArgs eventArgs)
        {
            this.contextMenu.Show(this.explorerNavigationButtons1, new Point(this.explorerNavigationButtons1.Width - 20, this.explorerNavigationButtons1.Height));
        }

        void ExplorerHeader_Paint(object sender, PaintEventArgs e)
        {
            // Fill the controls background with a windows 7 like background style.
            e.Graphics
                .FillRectangle(
                    new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(0, this.pathEdit.Location.Y),
                                                                     ColorTop,
                                                                     ColorBottom), ClientRectangle);

            // Fill the remaining space with the brighter solid color.
            e.Graphics.FillRectangle(new SolidBrush(ColorBottom), 0, this.pathEdit.Location.Y, this.Width, this.Height);
        }

        void NavigationLog_NavigationLogChanged(object sender, NavigationLogEventArgs args)
        {
            // calculate button states
            if (args.CanNavigateBackwardChanged)
            {
                this.explorerNavigationButtons1.HasLeftHistroy = this.explorerBrowser.NavigationLog.CanNavigateBackward;
            }
            if (args.CanNavigateForwardChanged)
            {
                this.explorerNavigationButtons1.HasRightHistroy = this.explorerBrowser.NavigationLog.CanNavigateForward;
            }

            // This event is BeginInvoked to decouple the explorerBrowser UI from this UI
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                // update history combo box
                if (args.LocationsChanged)
                {
                    this.contextMenu.Items.Clear();
                    foreach (ShellObject shobj in this.explorerBrowser.NavigationLog.Locations)
                    {
                        logInfo("Navigation: " + shobj.Name);
                        this.contextMenu.Items.Add(shobj.Name);
                    }
                }

                for (int i = 0; i < this.contextMenu.Items.Count; i++)
                    this.contextMenu.Items[i].Image = null;

                if (this.explorerBrowser.NavigationLog.CurrentLocationIndex != -1)
                    this.contextMenu.Items[this.explorerBrowser.NavigationLog.CurrentLocationIndex].Image =
                        Resources._checked;
            }));
        }

        void explorerBrowser_NavigationComplete(object sender, NavigationCompleteEventArgs args)
        {
            // This event is BeginInvoked to decouple the explorerBrowser UI from this UI
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                // update event history
                if (args.NewLocation == null)
                {
                    logError("Error retrieving navigation location.");
                    return;
                }
                else
                    logInfo("Navigation completed. New Location = " + args.NewLocation.Name);

                if (args.NewLocation != null)
                    this.pathEdit.Text = new DataComboBox.Element() { Text = args.NewLocation.ParsingName.StartsWith("::{") ? args.NewLocation.Name : args.NewLocation.ParsingName, Value = args.NewLocation.ParsingName };
            }));
        }

        void explorerBrowser_NavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            // This event is BeginInvoked to decouple the explorerBrowser UI from this UI
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                // update event history
                logError("Navigation failed. Failed Location = " + ((args.FailedLocation == null) ? "(unknown)" : args.FailedLocation.Name));

                for (int i = 0; i < this.contextMenu.Items.Count; i++)
                    this.contextMenu.Items[i].Image = null;

                if (this.explorerBrowser.NavigationLog.CurrentLocationIndex != -1)
                    this.contextMenu.Items[this.explorerBrowser.NavigationLog.CurrentLocationIndex].Image =
                        Resources._checked;
            }));
        }

        public bool DisallowNavigation { get; set; }

        private void navigateButton_Click(object sender, EventArgs e)
        {
            if (this.pathEdit.Text.Value.StartsWith("::{"))
            {
                try
                {
                    IKnownFolder kf = KnownFolderHelper.FromParsingName(this.pathEdit.Text.Value);

                    this.explorerBrowser.Navigate((ShellObject)kf);
                }
                catch
                {
                    logError("Navigation not possible. Failed to retrieve known folder.");
                }
                return;
            }

            if (!this.pathEdit.Items.Contains(this.pathEdit.Text.Value))
            {
                NavigateToFolderOrArchive(this.pathEdit.Text.Value);
            }
            else
            {
                NavigateToSpecialFolder(this.pathEdit.Text.Value);
            }
        }

        public void NavigateToSpecialFolder(string knownFolder)
        {
            try
            {
                // Navigate to a known folder
                IKnownFolder kf = KnownFolderHelper.FromCanonicalName(knownFolder);

                this.explorerBrowser.Navigate((ShellObject)kf);
            }
            catch (COMException)
            {
                logError("Navigation not possible.");
            }
        }

        public void NavigateToFolderOrArchive(string url)
        {
            if (string.IsNullOrEmpty(InitDirectory))
                InitDirectory = url;

            try
            {
                // navigate to specific folder
                this.explorerBrowser.Navigate(ShellFileSystemFolder.FromFolderPath(url));
            }
            catch
            {
                try
                {
                    // Navigates to a specified file (must be a container file to work, i.e., ZIP, CAB)         
                    this.explorerBrowser.Navigate(ShellFile.FromFilePath(url));
                }
                catch (Exception)
                {
                    logError("Navigation not possible.");
                }
            }
        }

        private void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.explorerBrowser.NavigateLogLocation(this.contextMenu.Items.IndexOf(e.ClickedItem));
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // clear navigatiofn log
            this.explorerBrowser.NavigationLog.ClearLog();
            this.explorerNavigationButtons1.HasLeftHistroy = false;
            this.explorerNavigationButtons1.HasRightHistroy = false;

            if (string.IsNullOrEmpty(InitDirectory))
                this.explorerBrowser.Navigate((ShellObject)Microsoft.WindowsAPICodePack.Shell.KnownFolders.Desktop);
            else
                NavigateToFolderOrArchive(InitDirectory);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            picDeleteHistory.BackgroundImage = Resources.DeleteHistory3;
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            picDeleteHistory.BackgroundImage = Resources.DeleteHistory2;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            picDeleteHistory.BackgroundImage = Resources.DeleteHistory;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            picDeleteHistory.BackgroundImage = Resources.DeleteHistory;
        }

        private void ExplorerHeader_Resize(object sender, EventArgs e)
        {
            if (this.Size.Height != height)
                this.Size = new Size(this.Size.Width, height);
        }
    }
}