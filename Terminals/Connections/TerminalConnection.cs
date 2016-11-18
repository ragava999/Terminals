using System.IO;
using System.Text;
using System.Threading;
using Kohl.Framework.Converters;
using Kohl.Framework.Logging;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using Terminals.Properties;
using WalburySoftware;
using Renci.SshNet;

namespace Terminals.Connections
{
    public class TerminalConnection : Connection.Connection
    {
        private dynamic client;
        private Boolean connected;
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

            String protocol = string.Empty;

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
            this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.client.Connect(this.Favorite.ServerName, this.Favorite.Port);

            TcpProtocol tcpProtocol = new TcpProtocol(new NetworkStream(this.client));
            TelnetProtocol p = new TelnetProtocol();
            tcpProtocol.OnDataIndicated += p.IndicateData;
            tcpProtocol.OnDisconnect += this.OnConnectionLost;
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
        	SshClient client = new SshClient(this.Favorite.ServerName, this.Favorite.Port, userName, password);

        	client.Connect();
        	
        	System.Collections.Generic.IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new System.Collections.Generic.Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            //termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

            ShellStream shellStream = client.CreateShellStream("xterm", (uint)this.Favorite.ConsoleCols, (uint)this.Favorite.ConsoleRows, (uint)this.Favorite.ConsoleCols, (uint)this.Favorite.ConsoleRows,/* 800, 600,*/ 512, termkvp);

            // Send the output from the ssh session to the terminal
            shellStream.DataReceived += (sender, e) => {
				if (Encoding.Default.GetString(e.Data).Equals("logout\r\n"))
					this.InvokeIfNecessary(()=> this.Disconnect());
				else
					this.term.IndicateData(e.Data);
			};

        	// Send the keys from the terminal emulator to the ssh protocol
        	this.term.OnDataRequested += (byte[] data) => { if (client != null) WriteStream(data, shellStream); };
            
        	this.connected = true;
        }
		
		private void WriteStream(byte[] cmd, ShellStream stream)
		{
		    var writer = new StreamWriter(stream, Encoding.Default);
		    
		    try
		    {
		    	writer.AutoFlush = true;
		    }
		    catch (ObjectDisposedException ex)
		    {
		    	Log.Debug("Disconnecting from disposed SSH session.");
		    	Disconnect();
		    	return;
		    }
		    
		    string command = Encoding.Default.GetString(cmd);
		    
	    	writer.Write(command.ToCharArray());
		    
		    while (stream.Length == 0)
		        Thread.Sleep(50);
		}
		
        private void OnConnectionLost()
        {
            if (!connected)
                return;

            Log.Fatal(String.Format("{0} connection \"{1}\" has been lost.", this.Favorite.Protocol, this.Favorite.Name));
            
            Disconnect();
        }

        public override void Disconnect()
        {
            this.connected = false;

            try
            {
            	if (client != null)
	            	if (client is SshClient)
	            	{
	            		client.Disconnect();
	            		client.Dispose();
	            	}
	            	else
	                	this.client.Close();
            	
            	if (term != null)
            		this.term.Dispose();
            	
            	this.CloseTabPage();

            	InvokeIfNecessary(() => base.Disconnect());
            	
            	Log.Info("Disconnected from " + this.Favorite.Protocol + " session.");
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