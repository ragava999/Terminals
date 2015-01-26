using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DotRas;
using Kohl.Framework.Localization;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Properties;

namespace Terminals.Connections
{
    public class RASConnection : Connection.Connection
    {
        public delegate void LogHandler(string entry);

        private bool connected;

        private RasDialer rasDialer;
        private RasPhoneBook rasPhoneBook;

        protected override Image[] images
        {
            get { return new Image[] {Resources.RAS}; }
        }

        public override bool Connected
        {
            get { return this.connected; }
        }

        // Don't save the settings in a RAS phonebook e.g. in rasphone.pbk
        // Temporary create a phonebook that holds the information for the connection
        // which will be delete after disconnect.
        public bool LetTerminalsManageRasSettings { get; set; }

        // All users path
        // Current user path
        // Custom path
        private string PhonebookPath { get; set; }
        public event LogHandler OnLog;

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
        }

        private RasProperties rasProperties = null;

        public override bool Connect()
        {
            this.connected = false;

            try
            {
                this.rasProperties = new RasProperties(this.ParentForm, this);

                DirectoryInfo directoryInfo = (new FileInfo(this.GetType().Assembly.Location)).Directory;

                if ((directoryInfo == null || directoryInfo.Exists == false) && string.IsNullOrEmpty(this.PhonebookPath))
                {
                    rasProperties.Error("The phonebook path hasn't been set. Aborting RAS connection.");
                    return this.connected = false;
                }

                this.Embed(this.rasProperties);

                // The 'RasDevice.GetDeviceByName([string], [RasDeviceType])' method is obsolete as of DotRas (ChangeSet 93435):
                RasEntry entry = RasEntry.CreateVpnEntry(this.Favorite.Name, this.Favorite.ServerName,
                                                         RasVpnStrategy.Default, (from d in RasDevice.GetDevices()
                                                                                  where
                                                                                      d.DeviceType == RasDeviceType.Vpn &&
                                                                                      d.Name.ToUpper()
                                                                                       .Contains("(PPTP)")
                                                                                  select d).FirstOrDefault());

                // Create the Ras phonebook or upen it under the below mentioned path.
                string phonebookPath = this.PhonebookPath ?? Path.Combine(directoryInfo.FullName, "rasphone.pbk");

                this.rasPhoneBook = new RasPhoneBook();
                this.rasPhoneBook.Open(phonebookPath);

                this.rasProperties.RasEntry = entry;

                // Check if the entry hasn't been added.
                if (!this.rasPhoneBook.Entries.Contains(entry.Name))
                {
                    this.rasPhoneBook.Entries.Add(entry);
                }

                this.rasDialer = new RasDialer
                                     {
                                         PhoneBookPath = phonebookPath,
                                         // Set the credentials later ...
                                         Credentials = null,
                                         // Initialize with default values (same as the designer would do)
                                         EapOptions = new RasEapOptions(false, false, false),
                                         // Initialize with default values (same as the designer would do)
                                         Options = new RasDialOptions(false, false, false, false, false, false, false, false, false, false, false),
                                         EntryName = this.Favorite.Name
                                     };

                this.rasDialer.Error += this.rasDialer_Error;
                this.rasDialer.DialCompleted += this.rasDialer_DialCompleted;
                this.rasDialer.StateChanged += this.rasDialer_StateChanged;

                // Set the ras dialer credentials and checks if setting the credentials has been
                // successful; if not NULL will be set to the ras dealer credentials and in that
                // case we'll load the RAS connection from the phone book.
                if ((this.rasDialer.Credentials = this.Favorite.Credential) == null)
                {
                    rasProperties.Warn(Localization.Text("Connection.RASConnection.Connect_Warn"));

                    RasDialDialog rasDialDialog = new RasDialDialog
                                                      {
                                                          PhoneBookPath = phonebookPath,
                                                          EntryName = entry.Name
                                                      };

                    if (rasDialDialog.ShowDialog() == DialogResult.OK)
                    {
                        rasProperties.Info(string.Format("{0} {1}", "Thank you for providing the credentials." + Localization.Text("Connection.RASConnection.Connect_Info")));
                        return this.connected = true;
                    }

                    rasProperties.Error("Terminating RAS connection, either credentials haven't been supplied or error connecting to the target.");
                    return this.connected = false;
                }

                rasProperties.Info(Localization.Text("Connection.RASConnection.Connect_Info"));
                this.rasDialer.Dial();

                return this.connected = true;
            }
            catch (Exception ex)
            {
                rasProperties.Error(string.Format(Localization.Text("Connections.HTTPConnection.Connect_Error2"), this.Favorite.Protocol), ex);
                return this.connected = false;
            }
        }

        private void rasDialer_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (rasProperties != null)
                rasProperties.Info(string.Format(Localization.Text("Connection.RASConnection_ConnectionState"), e.State.ToString()));
        }

        private void rasDialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (rasProperties != null)
                    rasProperties.Info(Localization.Text("Connection.RASConnection_Cancelled"));
            }
            else if (e.TimedOut)
            {
                if (rasProperties != null)
                    rasProperties.Info(Localization.Text("Connection.RASConnection_Timeout"));
            }
            else if (e.Error != null)
            {
                if (rasProperties != null)
                    rasProperties.Info(e.Error.ToString());
            }
            else if (e.Connected)
            {
                this.connected = true;

                if (rasProperties != null)
                    rasProperties.Info(Localization.Text("Connection.RASConnection_Connected"));
            }

            if (!e.Connected)
            {
                // The connection was not connected, disable the disconnect button.
                this.connected = false;
            }
        }

        private void rasDialer_Error(object sender, ErrorEventArgs e)
        {
            Log.Error("RAS error.", e.GetException());
        }

        public override void Disconnect()
        {
            this.connected = false;

            if (this.rasDialer.IsBusy)
            {
                if (rasProperties != null)
                    rasProperties.Info(Localization.Text("Connection.RASConnection.Disconnect_Info1"));

                // The connection attempt has not been completed, cancel the attempt.
                this.rasDialer.DialAsyncCancel();
            }
            else
            {
                foreach (RasConnection connection in RasConnection.GetActiveConnections())
                {
                    if (rasProperties != null)
                        rasProperties.Info(Localization.Text("Connection.RASConnection.Disconnect_Info2"));

                    // The connection has been found, disconnect it.
                    connection.HangUp();
                }
            }

            if (rasProperties != null)
            {
                rasProperties.Dispose();
                rasProperties = null;
            }
        }
    }
}