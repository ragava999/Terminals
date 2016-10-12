using Kohl.Framework.Converters;
using Kohl.Framework.Logging;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Keys;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Manager;
using Terminals.Properties;
using Terminals.SSHClient;
using WalburySoftware;

namespace Terminals.Connections
{
    public class TerminalConnection : Connection.Connection
    {
        private Socket client;
        private Boolean connected;
        private Protocol sshProtocol;
        private TerminalEmulator term;

        protected override Image[] images
        {
            get { return new Image[] { Resources.TELNET }; }
        }

        public override ushort Port
        {
            get { return 22; }
        }

        public override bool Connected
        {
            get { return this.connected; }
        }

        protected override void ChangeDesktopSize(DesktopSize desktopSize, System.Drawing.Size size)
        {
        }

        public override Boolean Connect()
        {
            this.connected = false;

            String protocol = "unknown";

            try
            {
                if (typeof(TerminalConnection).IsEqual(this.Favorite.Protocol))
                {
                    protocol = typeof(TerminalConnection).GetProtocolName();
                }
                else
                {
                    protocol = (this.Favorite.Ssh1) ? "SSH1" : "SSH2";
                }

                this.InvokeIfNecessary(delegate { this.term = new TerminalEmulator(); });

                this.Embed(this.term);

                this.term.BackColor = FavoriteConfigurationElement.TranslateColor(this.Favorite.ConsoleBackColor);
                this.term.ForeColor = FavoriteConfigurationElement.TranslateColor(this.Favorite.ConsoleTextColor);
                this.term.BlinkColor = this.term.CursorColor = FavoriteConfigurationElement.TranslateColor(this.Favorite.ConsoleCursorColor);

                if (this.term.InvokeRequired)
                    this.term.Invoke(new MethodInvoker(delegate { this.term.Font = FontParser.FromString(this.Favorite.ConsoleFont); }));
                else
                    this.term.Font = FontParser.FromString(this.Favorite.ConsoleFont);

                this.term.Rows = this.Favorite.ConsoleRows;
                this.term.Columns = this.Favorite.ConsoleCols;

                this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.client.Connect(this.Favorite.ServerName, this.Favorite.Port);

                if (!this.Favorite.Credential.IsSetUserName || !this.Favorite.Credential.IsSetPassword)
                {
                    Log.Fatal(string.Format("Please set user name and password in your {0} connection properties.", this.Favorite.Protocol.ToLower()));
                    return false;
                }

                if (typeof(TerminalConnection).IsEqual(this.Favorite.Protocol))
                {
                    this.ConfigureTelnetConnection(this.Favorite.Credential.UserName, this.Favorite.Credential.Password);
                }
                else
                {
                    this.ConfigureSshConnection(this.Favorite.Credential.UserName, this.Favorite.Credential.Password);
                }

                if (this.term.InvokeRequired)
                    this.term.Invoke(new MethodInvoker(delegate { this.term.Focus(); }));
                else
                    this.term.Focus();

                return this.connected = true;
            }
            catch (Exception exc)
            {
                Log.Fatal(string.Format("Terminals was unable to create the {0} connection.", protocol), exc);
                return this.connected = false;
            }
        }

        private void ConfigureTelnetConnection(string userName, string password)
        {
            TcpProtocol tcpProtocol = new TcpProtocol(new NetworkStream(this.client));
            TelnetProtocol p = new TelnetProtocol();
            tcpProtocol.OnDataIndicated += p.IndicateData;
            tcpProtocol.OnDisconnect += this.OnDisconnected;
            p.TerminalType = this.term.TerminalType;
            p.Username = userName;
            p.Password = password;
            p.OnDataIndicated += this.term.IndicateData;
            p.OnDataRequested += tcpProtocol.RequestData;
            this.term.OnDataRequested += p.RequestData;
            this.connected = this.client.Connected;
        }

        private void ConfigureSshConnection(string userName, string password)
        {
            this.sshProtocol = new Protocol();
            this.sshProtocol.setTerminalParams(this.term.TerminalType, this.term.Rows, this.term.Columns);
            this.sshProtocol.OnDataIndicated += this.term.IndicateData;
            this.sshProtocol.OnDisconnect += this.OnDisconnected;
            this.term.OnDataRequested += this.sshProtocol.RequestData;

            String key = String.Empty;

            KeyConfigElement keyConfigElement = Settings.SSHKeys.Keys[this.Favorite.KeyTag];

            if (keyConfigElement != null)
                key = keyConfigElement.Key;

            this.sshProtocol.setProtocolParams(this.Favorite.AuthMethod, userName, password, key, this.Favorite.Ssh1);

            this.sshProtocol.Connect(this.client);
            this.connected = true; // SSH will throw if fails
        }

        private void OnDisconnected()
        {
            if (!connected)
                return;

            Log.Fatal(String.Format("{0} connection \"{1}\" has been lost.",
                                    this.Favorite.Protocol, this.Favorite.Name));
            this.connected = false;
            this.CloseTabPage();

            InvokeIfNecessary(() => base.Disconnect());
        }

        public override void Disconnect()
        {
            this.connected = false;

            try
            {
                this.client.Close();
                if (this.sshProtocol != null)
                    this.sshProtocol.OnConnectionClosed();
            }
            catch (Exception ex)
            {
                Log.Error(
                    string.Format("Unable to disconnect from the {0} connection named \"{1}\".", this.Favorite.Protocol,
                                  this.Favorite.Name), ex);
            }
        }
    }
}