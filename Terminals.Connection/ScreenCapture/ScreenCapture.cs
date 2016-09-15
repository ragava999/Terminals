using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Native;

namespace Terminals.Connection.ScreenCapture
{
    public class ScreenCapture
    {
        private readonly PrintDocument doc = new PrintDocument();

        private ImageFormatHandler formatHandler;
        private Bitmap image;
        private Bitmap[] images;

        public ScreenCapture()
        {
            this.doc.PrintPage += this.printPage;
            this.formatHandler = new ImageFormatHandler();
        }

        public ScreenCapture(ImageFormatHandler formatHandler)
        {
            this.doc.PrintPage += this.printPage;
            this.formatHandler = formatHandler;
        }

        public ImageFormatHandler FormatHandler
        {
            set { this.formatHandler = value; }
        }

        public virtual Bitmap Capture(Form window, string filename, ImageFormatTypes format)
        {
            return this.Capture(window, filename, format, false);
        }

        public virtual Bitmap Capture(Form window, string filename, ImageFormatTypes format, bool onlyClient)
        {
            this.Capture(window, onlyClient);
            this.Save(filename, format);
            return this.images[0];
        }

        public virtual Bitmap Capture(IntPtr handle, string filename, ImageFormatTypes format)
        {
            this.Capture(handle);
            this.Save(filename, format);
            return this.images[0];
        }

        public virtual Bitmap CaptureControl(Control window, string filename, ImageFormatTypes format)
        {
            this.CaptureControl(window);

            if (Settings.EnableCaptureToFolder)
                this.Save(filename, format);

            return this.images[0];
        }

        public virtual Bitmap CaptureControl(Control window)
        {
            Rectangle rc = window.RectangleToScreen(window.DisplayRectangle);
            return this.capture(window, rc);
        }

        public virtual Bitmap Capture(Form window, bool onlyClient)
        {
            if (!onlyClient)
                return this.Capture(window);

            Rectangle rc = window.RectangleToScreen(window.ClientRectangle);
            return this.capture(window, rc);
        }

        public virtual Bitmap Capture(Form window)
        {
            Rectangle rc = new Rectangle(window.Location, window.Size);
            return this.capture(window, rc);
        }

        private Bitmap capture(Control window, Rectangle rc)
        {
			if (Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
			{
				Log.Fatal("Screen caputure is only supported on Windows at the moment.");
				return new Bitmap(0, 0);
			}

            Bitmap memoryImage = null;
            this.images = new Bitmap[1];

            try
            {
                using (Graphics graphics = window.CreateGraphics())
                {
                    memoryImage = new Bitmap(rc.Width, rc.Height, graphics);

                    using (Graphics memoryGrahics = Graphics.FromImage(memoryImage))
                    {
                        memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Screen capture failed.", ex);
            }

            this.images[0] = memoryImage;
            return memoryImage;
        }

        private delegate Bitmap CaptureHandleDelegateHandler(IntPtr handle);

        public virtual Bitmap Capture(IntPtr handle)
        {
			if (Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
			{
				Log.Fatal("Screen caputure is only supported on Windows at the moment.");
				return new Bitmap(0,0);
			}

            WindowsApi.BringWindowToTop(handle);
            CaptureHandleDelegateHandler dlg = this.CaptureHandle;
            IAsyncResult result = dlg.BeginInvoke(handle, null, null);
            return dlg.EndInvoke(result);
        }

        protected virtual Bitmap CaptureHandle(IntPtr handle)
        {
			if (Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
			{
				Log.Fatal("Screen caputure is only supported on Windows at the moment.");
				return new Bitmap(0, 0);
			}

            Bitmap memoryImage = null;
            this.images = new Bitmap[1];
            try
            {
                using (Graphics graphics = Graphics.FromHwnd(handle))
                {
                    Rectangle rc = WindowsApi.GetWindowRect(handle);

                    if ((int) graphics.VisibleClipBounds.Width > 0 && (int) graphics.VisibleClipBounds.Height > 0)
                    {
                        memoryImage = new Bitmap(rc.Width, rc.Height, graphics);

                        using (Graphics memoryGrahics = Graphics.FromImage(memoryImage))
                        {
                            memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Screen capture failed.", ex);
            }

            this.images[0] = memoryImage;
            return memoryImage;
        }

        public virtual Bitmap[] Capture(CaptureType typeOfCapture, string filename, ImageFormatTypes format)
        {
            this.Capture(typeOfCapture);
            this.Save(filename, format);
            return this.images;
        }

        public virtual Bitmap[] Capture(CaptureType typeOfCapture)
        {
			if (Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
			{
				Log.Fatal("Screen caputure is only supported on Windows at the moment.");
				return new Bitmap[] { new Bitmap(0, 0)};
			}

            Bitmap memoryImage;
            int count = 1;

            try
            {
                Screen[] screens = Screen.AllScreens;
                Rectangle rc;
                switch (typeOfCapture)
                {
                    case CaptureType.PrimaryScreen:
                        rc = Screen.PrimaryScreen.Bounds;
                        break;
                    case CaptureType.VirtualScreen:
                        rc = SystemInformation.VirtualScreen;
                        break;
                    case CaptureType.WorkingArea:
                        rc = Screen.PrimaryScreen.WorkingArea;
                        break;
                    case CaptureType.AllScreens:
                        count = screens.Length;
                        typeOfCapture = CaptureType.WorkingArea;
                        rc = screens[0].WorkingArea;
                        break;
                    default:
                        rc = SystemInformation.VirtualScreen;
                        break;
                }

                this.images = new Bitmap[count];

                for (int index = 0; index < count; index++)
                {
                    if (index > 0)
                        rc = screens[index].WorkingArea;

                    memoryImage = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);

                    using (Graphics memoryGrahics = Graphics.FromImage(memoryImage))
                    {
                        memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                    }

                    this.images[index] = memoryImage;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Screen capture failed.", ex);
            }

            return this.images;
        }

        public virtual void Print()
        {
            if (this.images != null)
            {
                try
                {
                    for (int i = 0; i < this.images.Length; i++)
                    {
                        this.image = this.images[i];
                        this.doc.DefaultPageSettings.Landscape = (this.image.Width > this.image.Height);
                        this.doc.Print();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Print failed.", ex);
                    MessageBox.Show(ex.Message, "Print failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void printPage(object sender, PrintPageEventArgs e)
        {
            RectangleF rc = this.doc.DefaultPageSettings.Bounds;
            float ratio = this.image.Height/(float) (this.image.Width != 0 ? this.image.Width : 1);

            rc.Height = rc.Height - this.doc.DefaultPageSettings.Margins.Top -
                        this.doc.DefaultPageSettings.Margins.Bottom;
            rc.Y = rc.Y + this.doc.DefaultPageSettings.Margins.Top;

            rc.Width = rc.Width - this.doc.DefaultPageSettings.Margins.Left - this.doc.DefaultPageSettings.Margins.Right;
            rc.X = rc.X + this.doc.DefaultPageSettings.Margins.Left;

            if (rc.Height/rc.Width > ratio)
                rc.Height = rc.Width*ratio;
            else
                rc.Width = rc.Height/(ratio != 0 ? ratio : 1);

            e.Graphics.DrawImage(this.image, rc);
        }

        public virtual void Save(string filename, ImageFormatTypes format)
        {
            string directory = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);

            ext = this.formatHandler.GetDefaultFilenameExtension(format);

            if (ext.Length == 0)
            {
                format = ImageFormatTypes.imgPNG;
                ext = "png";
            }

            try
            {
                ImageCodecInfo info;
                EncoderParameters parameters = this.formatHandler.GetEncoderParameters(format, out info);

                for (int i = 0; i < this.images.Length; i++)
                {
                    if (this.images.Length > 1)
                    {
                        filename = string.Format("{0}\\{1}.{2:D2}.{3}",
                                                 directory, name, i + 1, ext);
                    }
                    else
                    {
                        filename = string.Format("{0}\\{1}.{2}",
                                                 directory, name, ext);
                    }

                    this.image = this.images[i];

                    if (parameters != null)
                    {
                        this.image.Save(filename, info, parameters);
                    }
                    else
                    {
                        this.image.Save(filename, ImageFormatHandler.GetImageFormat(format));
                    }
                }
            }
            catch (Exception ex)
            {
                string s = string.Format("Saving image to [{0}] in format [{1}] failed.\n{2}", filename,
                                         format.ToString(), ex);
                Log.Error(s, ex);
                MessageBox.Show(s, "Screen capture failed.",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}