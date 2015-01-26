/*
 * Created by SharpDevelop.
 * User: CableJ01
 * Date: 18/01/2009
 * Time: 13:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Net.Sockets;
using Granados;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.SSHClient
{
    /// <summary>
    ///     Description of Protocol.
    /// </summary>
    public class Protocol : ISSHConnectionEventReceiver, ISSHChannelEventReceiver
    {
        #region Public Properties

        private string Username
        {
            set { this._params.UserName = value; }
        }

        private string Password
        {
            set { this._params.Password = value; }
        }

        private AuthenticationType AuthenticationType
        {
            set { this._params.AuthenticationType = value; }
        }

        private string Key
        {
            set
            {
                // tunnel this through to modified key classes!
                // rule will be if password is set but IdentifyFile
                // is null
                // then password is a base64 ProtectedData key
                this._params.Password = value;
            }
        }

        private SSHProtocol SSHProtocol
        {
            set { this._params.Protocol = value; }
        }

        private string TerminalName
        {
            set { this._params.TerminalName = value; }
        }

        private int TerminalWidth
        {
            set { this._params.TerminalWidth = value; }
        }

        private int TerminalHeight
        {
            set { this._params.TerminalHeight = value; }
        }

        #endregion

        #region Public Enums

        #endregion

        #region Public Fields

        #endregion

        #region Public Delegates

        public delegate void DataIndicate(byte[] data);

        public delegate void Disconnect();

        #endregion

        #region Public Events

        public event DataIndicate OnDataIndicated;
        public event Disconnect OnDisconnect;

        #endregion

        #region Public Constructors

        public Protocol()
        {
            this._params = new SSHConnectionParameter
                               {
                                   KeyCheck = delegate
                                                  {
                                                      //byte[] h = info.HostKeyMD5FingerPrint();
                                                      //foreach(byte b in h) Debug.Write(String.Format("{0:x2} ", b));
                                                      return true;
                                                  }
                               };
        }

        #endregion

        #region Public Methods

        public void OnData(byte[] data, int offset, int length)
        {
            if (this.OnDataIndicated != null)
            {
                byte[] obuf = new byte[length];
                Array.Copy(data, offset, obuf, 0, obuf.Length);
                this.OnDataIndicated(obuf);
            }
        }

        public void OnChannelError(Exception error)
        {
            //Debug.WriteLine("Channel ERROR: "+ error.Message);
        }

        public void OnChannelClosed()
        {
            //Debug.WriteLine("Channel closed");
            this._conn.Disconnect("");
            //_conn.AsyncReceive(this);
        }

        public void OnChannelEOF()
        {
            this._pf.Close();
            this.OnConnectionClosed();
            //Debug.WriteLine("Channel EOF");
        }

        public void OnExtendedData(int type, byte[] data)
        {
            //Debug.WriteLine("EXTENDED DATA");
        }

        public void OnChannelReady()
        {
            //    _ready = true;
        }

        public void OnMiscPacket(byte type, byte[] data, int offset, int length)
        {
        }

        public void OnDebugMessage(bool always_display, byte[] data)
        {
            //Debug.WriteLine("DEBUG: "+ Encoding.Default.GetString(data));
        }

        public void OnIgnoreMessage(byte[] data)
        {
            //Debug.WriteLine("Ignore: "+ Encoding.Default.GetString(data));
        }

        public void OnAuthenticationPrompt(string[] msg)
        {
            //Debug.WriteLine("Auth Prompt "+msg[0]);
        }

        public void OnError(Exception error)
        {
            //Debug.WriteLine("ERROR: "+ msg);
        }

        public void OnConnectionClosed()
        {
            //Debug.WriteLine("Connection closed");.
            if (this.OnDisconnect != null) this.OnDisconnect();
        }

        public void OnUnknownMessage(byte type, byte[] data)
        {
            //Debug.WriteLine("Unknown Message " + type);
        }

        public PortForwardingCheckResult CheckPortForwardingRequest(string host, int port, string originator_host,
                                                                    int originator_port)
        {
            PortForwardingCheckResult r = new PortForwardingCheckResult {allowed = true, channel = this};
            return r;
        }

        public void EstablishPortforwarding(ISSHChannelEventReceiver rec, SSHChannel channel)
        {
            this._pf = channel;
        }

        public void setTerminalParams(string type, int rows, int cols)
        {
            this._params = new SSHConnectionParameter();
            this.TerminalName = type;
            this.TerminalHeight = rows;
            this.TerminalWidth = cols;
        }

        public void setProtocolParams(
            AuthMethod authMethod,
            string userName,
            string pass,
            string key,
            bool SSH1)
        {
            // each following check can force to different authentication
            authMethod = this.CheckUserName(userName, authMethod);
            authMethod = this.CheckPublickKey(key, authMethod);
            authMethod = this.CheckPassword(pass, authMethod);
            this.AssingAuthentization(authMethod);
            this.ChooseProtocolVersion(SSH1);
            this._params.EventTracer = new SShTraceLissener();
        }

        private AuthMethod CheckUserName(String userName, AuthMethod authMethod)
        {
            if (userName == null)
                userName = "";
            this.Username = userName;

            if (userName == "") // can't do auto login without username
                return AuthMethod.KeyboardInteractive;

            return authMethod;
        }

        private AuthMethod CheckPublickKey(String key, AuthMethod authMethod)
        {
            if (authMethod == AuthMethod.PublicKey)
            {
                if (string.IsNullOrEmpty(key))
                    return AuthMethod.Password;

                this.Key = SSH2UserAuthKey.FromBase64String(key).toSECSHStyle("");
            }

            return authMethod;
        }

        private AuthMethod CheckPassword(String pass, AuthMethod authMethod)
        {
            if (pass == null)
                pass = "";
            this.Password = pass; // password always has to be set: required by grandados

            if (authMethod == AuthMethod.Password && pass == "")
                return AuthMethod.KeyboardInteractive;

            return authMethod;
        }

        private void AssingAuthentization(AuthMethod authMethod)
        {
            switch (authMethod)
            {
                case AuthMethod.Password:
                    this.AuthenticationType = AuthenticationType.Password;
                    break;
                case AuthMethod.PublicKey:
                    this.AuthenticationType = AuthenticationType.PublicKey;
                    break;

                default:
                    this.AuthenticationType = AuthenticationType.KeyboardInteractive;
                    break;
                    // granados doesnt support Host authentication
            }
        }

        private void ChooseProtocolVersion(bool SSH1)
        {
            this.SSHProtocol = SSH1 ? SSHProtocol.SSH1 : SSHProtocol.SSH2;
        }

        public void RequestData(byte[] data)
        {
            this._pf.Transmit(data, 0, data.Length);
        }

        public void Connect(Socket s)
        {
            this._params.WindowSize = 0x1000;
            this._conn = SSHConnection.Connect(this._params, this, s);
            this._pf = this._conn.OpenShell(this);
            SSHConnectionInfo ci = this._conn.ConnectionInfo;
        }

        public void OnChannelError(Exception error, string msg)
        {
            //Debug.WriteLine("Channel ERROR: "+ msg);
        }

        public void OnError(Exception error, string msg)
        {
        }

        #endregion

        #region Public Overrides

        #endregion

        #region Private Enums

        #endregion

        #region Private Fields

        private SSHConnection _conn;
        private SSHConnectionParameter _params;
        private SSHChannel _pf;

        #endregion
    }
}