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
        public static void CheckAndPerformUpgradeIfAllowedInSettingsAndBinaryIsNewer(Control invoker, CommandLineArgs commandLine)
        {
            Invoker = invoker;

            if (Invoker == null)
                throw new Exception("Error invoker must not be null.");

            ThreadPool.QueueUserWorkItem(PerformUpgradeIfNewer, commandLine);
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
        private static void CheckForNewRelease(CommandLineArgs commandLineArgs, bool forceCheck = false)
        {
            if (forceCheck == false)
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
                        //don't run the update if the file is today or later .. if we have checked today or not
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

                    XmlReader responseReader = XmlReader.Create(Configuration.Url.GitHubReleasesFeed);

                    Log.Debug("Loading syndication feed.");

                    SyndicationFeed feed = SyndicationFeed.Load(responseReader);

                    // Only the first element would be enough normally - but to be sure
                    // check all of the releases
                    foreach (SyndicationItem item in feed.Items)
                    {
                        // Don't check this release - it's deprecated
                        if (item.Title.Text.ToUpperInvariant().Contains("DEPRECATED"))
                            continue;

                        // Don't check this release - it's broken
                        if (item.Title.Text.ToUpperInvariant().Contains("BROKEN"))
                            continue;

                        string title = item.Title.Text;
                        string version = title.Replace("Terminals", "").Trim();
                        DateTimeOffset feedDate = item.LastUpdatedTime;

                        Log.Debug("Release '" + title + "' has been discovered.");

                        try
                        {
                            feedDate = Convert.ToDateTime(Web.HTTPAsString(string.Format(Configuration.Url.GitHubLatestRelease_Binary, version).Replace("Terminals.zip", "TerminalsBuildDate")).Replace("?", ""));
                            Log.Info("Received release date \"" + feedDate.ToString() + "\" from the TerminalsBuildDate file stored on GitHub in " + title);
                        }
                        catch (Exception ex)
                        {
                            Log.Debug("Unable to get the release date from the TerminalsBuildDate file stored on GitHub in " + title + ". Using the syndication feed date.", ex);
                        }

                        //check the date the item was build (stored in TerminalsBuildDate), if not available check when it has been published (date in syndication feed).  
                        // If the build date is old than the feed date, then it's probably a new build!
                        if (new Version(version) >= AssemblyInfo.Version && feedDate > AssemblyInfo.BuildDate)
                        {
                            dynamic content = item.Content;

                            // Skip prereleases if we aren't already on a prerelease of the same version
                            if (AssemblyInfo.Version < new Version(version) && content.Text.ToLowerInvariant().Contains("this is just a preview"))
                                continue;

                            Log.Debug("Release '" + title + "'(" + feedDate.ToString() + ") is newer than the currently installed Terminals version '" + AssemblyInfo.Version + "'(" + AssemblyInfo.BuildDate.ToString() + ").");

                            FormsExtensions.InvokeIfNecessary(Invoker, () => { MainForm.ReleaseAvailable = true; MainForm.ReleaseDescription = title; });

                            Settings.UpdateSource = string.Format(Configuration.Url.GitHubLatestRelease_Binary, version);
                            commandLineArgs.AutomaticallyUpdate = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Unable to check for a new Terminals version on " + Configuration.Url.GitHubRepositry, ex);
                }

                File.WriteAllText(releaseFile, DateTime.Now.ToString());
            }
        }

        private static void PerformUpgradeIfNewer(object state)
        {
            PerformUpgradeIfNewer(state, false);
        }

        public static bool IsUpdateInProgress { get; private set; }

        public static void PerformUpgradeIfNewer(object state, bool forceUpgrade = false)
        {
            IsUpdateInProgress = false;

            CommandLineArgs commandLineArgs = (state as CommandLineArgs);

            try
            {
                CheckForNewRelease(commandLineArgs, forceUpgrade);
            }
            catch
            {
                Log.Warn("Failed to check for a new Terminals release.");
                return;
            }

            try
            {
                if (forceUpgrade || commandLineArgs.AutomaticallyUpdate)
                {
                    IsUpdateInProgress = true;

                    string url = Settings.UpdateSource;

                    string version = url.Substring(0, url.LastIndexOf("/"));
                    version = version.Substring(version.LastIndexOf("/") + 1, version.Length - version.LastIndexOf("/") - 1);

                    if (!Directory.Exists(AssemblyInfo.UpgradeDirectory))
                        Directory.CreateDirectory(AssemblyInfo.UpgradeDirectory);

                    string finalFolder = Path.Combine(AssemblyInfo.UpgradeDirectory, version);
                    if (!Directory.Exists(finalFolder))
                        Directory.CreateDirectory(finalFolder);

                    string filename = string.Format("{0}.zip", version);
                    filename = Path.Combine(AssemblyInfo.UpgradeDirectory, filename);
                    bool downloaded = true;

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

                            string updaterExe = Path.Combine(AssemblyInfo.Directory, "TerminalsUpdater.exe");

                            if (File.Exists(updaterExe))
                            {
                                // Use the new updater
                                File.Copy(Path.Combine(finalFolder, "TerminalsUpdater.exe"), updaterExe, true);

                                string args = String.Format("\"{0}\" \"{1}\"", finalFolder, AssemblyInfo.Directory);
                                Log.Debug("Starting TerminalsUpdater with arguments \"" + args + "\"");
                                IsUpdateInProgress = false;
                                Process.Start(updaterExe, args);
                            }
                            else
                                Log.Error("TerminalsUpdater.exe hasn't been found.");
                        }
                        else
                            Log.Warn("User has denied to upgrade Terminals from version '" + AssemblyInfo.Version + "'(" + AssemblyInfo.BuildDate.ToString() + ") to '" + version + ".");

                    }

                    IsUpdateInProgress = false;
                }
            }
            catch (Exception exc)
            {
                IsUpdateInProgress = false;
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