namespace Terminals.Updates
{
    using CommandLine;
    using Configuration.Files.Main.Settings;
    using ICSharpCode.SharpZipLib.Zip;
    using Kohl.Framework.Info;
    using Kohl.Framework.Logging;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.ServiceModel.Syndication;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public static class UpdateManager
    {
        public static Control Invoker { get; set; }

        /// <summary>
        ///     Check for available application updates
        /// </summary>
        public static void CheckForUpdates(Control invoker, CommandLineArgs commandLine)
        {
            Invoker = invoker;

            if (Invoker == null)
                throw new Exception("Error invoker must not be null.");

            ThreadPool.QueueUserWorkItem(PerformCheck, commandLine);
        }

        private static string GetMD5HashFromFile(string file_name)
        {
            String tmpFile = file_name + ".tmp";
            File.Copy(file_name, tmpFile, true);
            Byte[] retVal = null;

            using (FileStream file = new FileStream(tmpFile, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(file);
                file.Close();
            }

            if (retVal != null)
            {
                StringBuilder s = new StringBuilder();
                foreach (Byte b in retVal)
                {
                    s.Append(b.ToString("x2").ToLower());
                }

                return s.ToString();
            }

            return null;
        }

        /// <summary>
        ///     check codeplex's rss feed to see if we have a new release available.
        /// </summary>
        private static void CheckForNewRelease(CommandLineArgs commandLineArgs)
        {
            if (!Settings.CheckForNewRelease)
            {
                Log.Debug("The user has choosen to deny Terminals the update check.");
                return;
            }

            Boolean checkForUpdate = true;
            string releaseFile = Path.Combine(AssemblyInfo.DirectoryConfigFiles, "LastUpdateCheck.txt");

            if (File.Exists(releaseFile))
            {
                string text = null;
                using (FileStream fs = new FileStream(releaseFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
                {
                    using (StreamReader stream = new StreamReader(fs))
                    {
                        text = stream.ReadToEnd().Trim(); ;
                    }
                }

                if (text != String.Empty)
                {
                    DateTime lastUpdate = DateTime.MinValue;
                    if (DateTime.TryParse(text, out lastUpdate))
                    {
                        //dont run the update if the file is today or later..if we have checked today or not
                        if (lastUpdate.Date >= DateTime.Now.Date)
                        {
                            Log.Debug("No need to check for a new Terminals release. The release has already been checked recently.");
                            checkForUpdate = false;
                        }
                    }
                }
            }

            if (checkForUpdate)
            {
                Log.Debug("Start to check for a new Terminals release.");

                try
                {
                    Log.Debug("Connecting to the internet to github.com to check if a new Terminals version is available.");

                    XmlReader responseReader = XmlReader.Create(Terminals.Configuration.Url.GitHubReleasesFeed);

                    Log.Debug("Loading syndication feed.");

                    SyndicationFeed feed = SyndicationFeed.Load(responseReader);

                    // Only the first element would be enough normally - but to be sure
                    // check all of the releases
                    foreach (SyndicationItem item in feed.Items)
                    {
                        string title = item.Title.Text;
                        DateTimeOffset date = item.LastUpdatedTime;

                        Log.Debug("Release '" + title + "' has been discovered.");

                        //check the date the item was published.  
                        //Is it after the currently executing application BuildDate? if so, then it is probably a new build!
                        if (date > AssemblyInfo.BuildDate)
                        {
                            Log.Debug("Release '" + title + "'(" + date.ToString() + ") is newer than the currently installed Terminals version '" + AssemblyInfo.Version + "'(" + AssemblyInfo.BuildDate.ToString() + ").");

                            System.Windows.Forms.FormsExtensions.InvokeIfNecessary(Invoker, () => { MainForm.ReleaseAvailable = true; MainForm.ReleaseDescription = title; });

                            string version = title.Replace("Terminals", "").Trim();
                            Settings.UpdateSource = string.Format(Terminals.Configuration.Url.GitHubLatestRelease_Binary, version);
                            commandLineArgs.AutomaticallyUpdate = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Unable to check for a new Terminals version on " + Terminals.Configuration.Url.GitHubRepositry, ex);
                }

                File.WriteAllText(releaseFile, DateTime.Now.ToString());
            }
        }

        private static void PerformCheck(object state)
        {
            CommandLineArgs commandLineArgs = (state as CommandLineArgs);

            try
            {
                CheckForNewRelease(commandLineArgs);
            }
            catch
            {
                Log.Warn("Failed to check for a new Terminals release.");
                return;
            }

            try
            {
                if (commandLineArgs.AutomaticallyUpdate)
                {
                    String url = Settings.UpdateSource;

                    String name = url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/") - 1);
                    String version = url.Substring(0, url.LastIndexOf("/"));
                    version = version.Substring(version.LastIndexOf("/") + 1, version.Length - version.LastIndexOf("/") - 1);

                    if (!Directory.Exists(AssemblyInfo.UpgradeDirectory))
                        Directory.CreateDirectory(AssemblyInfo.UpgradeDirectory);

                    String finalFolder = Path.Combine(AssemblyInfo.UpgradeDirectory, version);
                    if (!Directory.Exists(finalFolder))
                        Directory.CreateDirectory(finalFolder);

                    String filename = String.Format("{0}.zip", version);
                    filename = Path.Combine(AssemblyInfo.UpgradeDirectory, filename);
                    Boolean downloaded = true;

                    // if the file has already been downloaded
                    if (File.Exists(filename))
                        // and if it has already been extracted
                        if (Directory.Exists(finalFolder))
                        {
                            string buildDateFile = Path.Combine(finalFolder, "TerminalsBuildDate");

                            // and the build date exists
                            // check if we need to download the new release (i.e. the extract is old)
                            // might be because of TerminalVersion remains the same and the build date
                            // has changed e.g. this might occur in cases of backports.
                            if (File.Exists(buildDateFile))
                            {
                                string buildDate = null;
                                using (FileStream fs = new FileStream(buildDateFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (StreamReader stream = new StreamReader(fs))
                                    {
                                        buildDate = stream.ReadToEnd().Trim(); ;
                                    }
                                }

                                // if we got some result
                                if (!string.IsNullOrWhiteSpace(buildDate))
                                {
                                    DateTime buildDateResult = DateTime.MinValue;

                                    // check if the result is a valid date time value -> if not download the new build
                                    if (!DateTime.TryParse(buildDate, out buildDateResult))
                                        downloaded = DownloadNewBuild(url, filename);

                                    // if the build date is equal to the min value or the build date is equal or lower to the current build date or this build is older than one month
                                    else if (buildDateResult == DateTime.MinValue || buildDateResult.Equals(new DateTime(1970, 1, 1, 0, 0, 0)) || buildDateResult <= AssemblyInfo.BuildDate || AssemblyInfo.BuildDate < DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0, 0)))
                                        downloaded = DownloadNewBuild(url, filename);
                                }
                                // no valid date -> download new one
                                else
                                    downloaded = DownloadNewBuild(url, filename);
                            }
                            // no build date file has been found -> delete everything i.e. the extracted zip
                            // and the zip file itself
                            else
                            {
                                Directory.Delete(finalFolder, true);
                                File.Delete(filename);
                                downloaded = DownloadNewBuild(url, filename);
                            }
                        }
                        // if it hasn't been extracted delete the downloaded zip file
                        else
                        {
                            File.Delete(filename);
                            downloaded = DownloadNewBuild(url, filename);
                        }
                    else
                        downloaded = DownloadNewBuild(url, filename);

                    if (downloaded && File.Exists(filename))
                    {
                        FastZip fz = new FastZip();
                        fz.ExtractZip(filename, finalFolder, null);
                        Log.Debug("Terminals release " + version + " has been extracted to \"" + finalFolder + "\"");

                        if (
                            MessageBox.Show(
                                "A new build is available, would you like to install it now",
                                "New Build", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Log.Debug("User has choosen to upgrade Terminals from version '" + AssemblyInfo.Version + "'(" + AssemblyInfo.BuildDate.ToString() + ") to '" + version + ".");

                            string updaterExe = Path.Combine(AssemblyInfo.Directory, AssemblyInfo.Title + "Updater.exe");

                            if (File.Exists(updaterExe))
                            {
                                String args = String.Format("\"{0}\" \"{1}\"", finalFolder, AssemblyInfo.Directory);
                                Log.Debug("Starting TerminalsUpdater with arguments \"" + args + "\"");
                                Process.Start(updaterExe, args);
                            }
                            else
                                Log.Error("TerminalsUpdater.exe hasn't been found.");
                        }
                        else
                            Log.Warn("User has denied to upgrade Terminals from version '" + AssemblyInfo.Version + "'(" + AssemblyInfo.BuildDate.ToString() + ") to '" + version + ".");

                    }
                }
            }
            catch (Exception exc)
            {
                Log.Fatal("Failed during update.", exc);
            }
        }

        private static DirectoryInfo FindFileInFolder(DirectoryInfo Path, String Filename)
        {
            if (Path.GetFiles(Filename).Length > 0)
                return Path;

            return Path.GetDirectories().Select(dir => FindFileInFolder(dir, Filename)).FirstOrDefault(found => found != null);
        }

        private static bool DownloadNewBuild(String Url, String Filename)
        {
            Log.Info("Downloading new Terminals release \"" + Url + "\" to \"" + Filename + "\"");
            return Web.SaveHTTPToFile(Url, Filename);
        }
    }
}