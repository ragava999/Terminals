using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Kohl.Framework.Localization;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Connection.ScreenCapture
{
    public class Capture
    {
        public static string FlickrApiKey = "9362619635c6f6c20e7c14fe4b67c2a0";
        public static string FlickrSharedSecretKey = "ac8f3c60be0812b6";
        private string comments;
        private string filepath;

        public Capture()
        {
        }

        public Capture(string filePath)
        {
            this.FilePath = filePath;
        }

        private string CommentsFileName
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(this.FilePath),
                                    string.Format(Localization.Text("CaputureManager.Capture.CommentsFileName", typeof(Capture)),
                                                  Path.GetFileName(this.filepath)));
            }
        }

        public string Name
        {
            get { return Path.GetFileName(this.FilePath); }
        }

        public Image Image
        {
            get
            {
                string copy = Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()),
                                            Path.GetFileName(this.filepath));
                if (!File.Exists(copy)) File.Copy(this.filepath, copy);
                using (Image i = Image.FromFile(copy))
                {
                    return (Image) i.Clone();
                }

                return null;
            }
        }

        public string Comments
        {
            get
            {
                if (this.comments == null)
                {
                    if (File.Exists(this.CommentsFileName))
                    {
                        string copy = Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()),
                                                   Path.GetFileName(this.CommentsFileName));
                        if (!File.Exists(copy))
                            File.Copy(this.CommentsFileName, copy);

                        using (FileStream fs = new FileStream(copy, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
                        {
                            using (StreamReader stream = new StreamReader(fs))
                            {
                                this.comments = stream.ReadToEnd();
                            }
                        }
                    }
                }

                return this.comments;
            }

            set { this.comments = value; }
        }

        public string FilePath
        {
            get { return this.filepath; }

            set { this.filepath = value; }
        }

        public void Delete()
        {
            try
            {
                if (File.Exists(this.FilePath)) File.Delete(this.FilePath);
                if (File.Exists(this.CommentsFileName)) File.Delete(this.CommentsFileName);
            }
            catch (Exception ec)
            {
                Log.Error(Localization.Text("CaputureManager.Capture.Delete", typeof(Capture)), ec);
            }
        }

        public void Move(string Destination)
        {
            try
            {
                if (File.Exists(Destination))
                {
                    File.Delete(this.FilePath);
                }
                else
                {
                    File.Move(this.FilePath, Destination);
                }

                Destination = string.Format(Localization.Text("CaputureManager.Capture.CommentsFileName", typeof(Capture)), Destination);

                if (File.Exists(Destination))
                {
                    File.Delete(this.CommentsFileName);
                }
                else
                {
                    File.Move(this.CommentsFileName, Destination);
                }
            }
            catch (Exception exc)
            {
                Log.Error(Localization.Text("CaputureManager.Capture.Move_Error", typeof(Capture)), exc);
            }
        }

        public void Save()
        {
            if (File.Exists(this.CommentsFileName)) File.Delete(this.CommentsFileName);
            if (this.Comments != null && this.Comments.Trim() != string.Empty)
            {
                File.WriteAllText(this.CommentsFileName, this.comments);
            }
        }

        public void Show()
        {
            Process.Start(this.FilePath);
        }
    }
}