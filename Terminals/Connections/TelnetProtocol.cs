using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Terminals.Connections
{
    /// <summary>
    ///     Description of TelnetProtocol.
    /// </summary>
    public class TelnetProtocol
    {
        #region Public Fields (3)

        public string Password;
        public string TerminalType;
        public string Username;

        #endregion

        #region Public Delegates (2)

        public delegate void DataIndicate(byte[] data);

        public delegate void DataRequest(byte[] data);

        #endregion

        #region Public Events (2)

        public event DataIndicate OnDataIndicated;
        public event DataRequest OnDataRequested;

        #endregion

        #region Public Constructors (1)

        public TelnetProtocol()
        {
            this.local = new Options();
            this.remote = new Options();
            this.local.supported[(int)OPT.TTYPE] = true; // terminal type
            this.remote.supported[(int)OPT.ECHO] = true; // echo
        }

        #endregion

        #region Public Methods (2)

        public void RequestData(byte[] data)
        {
            if (this.first_tx)
            {
                this.first_tx = false;
                this.TelnetCmd(CMD.DO, (byte)OPT.ECHO);
            }

            // count the chars that look like IACs
            int n = data.Count(b => b == 255);

            if (n == 0)
            {
                // no 0xff chars just send on the buffer
                this._RequestData(data);
            }
            else
            {
                // some 0xff chars. copy array, doubling 0xff chars
                byte[] bytes = new byte[data.Length + n];
                int i = 0;
                foreach (byte b in data)
                {
                    if (b == 255)
                    {
                        bytes[i++] = b;
                    }
                    bytes[i++] = b;
                }
                this._RequestData(data);
            }
        }

        public void IndicateData(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            while (input.Position < input.Length)
            {
                int b = input.ReadByte();
                if ((byte)b == (byte)CMD.IAC)
                {
                    int cmd = input.ReadByte();
                    if (cmd < 0)
                        break;
                    if ((byte)cmd == (byte)CMD.IAC)
                    {
                        output.WriteByte((byte)cmd);
                    }
                    else
                    {
                        this.process_telnet_command((byte)cmd, input);
                    }
                }
                else
                {
                    output.WriteByte((byte)b);
                }
            }
            byte[] obuf = new byte[output.Length];
            Array.Copy(output.GetBuffer(), obuf, obuf.Length);
            this._IndicateData(obuf);
        }

        #endregion

        #region Private Enums (3)

        private enum CMD
        {
            SE = 240,
            NOP = 241,
            DM = 242,
            BRK = 243,
            IP = 244,
            AO = 245,
            AYT = 246,
            EC = 247,
            EL = 248,
            GA = 249,
            SB = 250,
            WILL = 251,
            WONT = 252,
            DO = 253,
            DONT = 254,
            IAC = 255,
        }

        private enum OPT
        {
            ECHO = 1, // echo
            SGA = 3, // suppress go ahead
            STATUS = 5, // status
            TM = 6, // timing mark
            TTYPE = 24, // terminal type
            NAWS = 31, // window size
            TSPEED = 32, // terminal speed
            LFLOW = 33, // remote flow control
            LINEMODE = 34, // linemode
            ENVIRON = 36, // environment variables
        }

        private enum tristate
        {
            not_set = 0,
            on = 1,
            off = 2
        }

        #endregion

        #region Private Fields (3)

        private readonly Options local;
        private readonly Options remote;
        private bool first_tx = true;

        #endregion

        #region Private Classes (1)

        private class Options
        {
            public readonly tristate[] in_effect;
            public readonly bool[] supported;

            public Options()
            {
                this.in_effect = new tristate[256];
                this.supported = new bool[256];
            }
        };

        #endregion

        #region Private Methods (6)

        private void _RequestData(byte[] data)
        {
            if (this.OnDataRequested != null) this.OnDataRequested(data);
        }

        private void _IndicateData(byte[] data)
        {
            if (this.OnDataIndicated != null) this.OnDataIndicated(data);
        }

        private void process_telnet_command(byte b, Stream s)
        {

            if (b < 240)
                return; // error
            int option = s.ReadByte();
            switch ((CMD)b)
            {
                case CMD.SE:
                case CMD.NOP:
                case CMD.DM:
                case CMD.BRK:
                case CMD.IP:
                case CMD.AO:
                case CMD.AYT:
                case CMD.EC:
                case CMD.EL:
                case CMD.GA:
                    break;
                case CMD.SB:
                    this.subnegotiate(s, option);
                    break;
                case CMD.WILL:
                    if (this.remote.supported[option])
                    {
                        if (this.remote.in_effect[option] != tristate.on)
                        {
                            this.remote.in_effect[option] = tristate.on;
                            this.TelnetCmd(CMD.DO, (byte)option);
                        }
                    }
                    else
                    {
                        this.TelnetCmd(CMD.DONT, (byte)option);
                    }
                    break;
                case CMD.WONT:
                    if (this.remote.supported[option])
                    {
                        if (this.remote.in_effect[option] != tristate.off)
                        {
                            this.remote.in_effect[option] = tristate.off;
                            this.TelnetCmd(CMD.DONT, (byte)option);
                        }
                    }
                    break;
                case CMD.DO:
                    if (this.local.supported[option])
                    {
                        if (this.local.in_effect[option] != tristate.on)
                        {
                            this.local.in_effect[option] = tristate.on;
                            this.TelnetCmd(CMD.WILL, (byte)option);
                        }
                    }
                    else
                    {
                        this.local.in_effect[option] = tristate.off;
                        this.TelnetCmd(CMD.WONT, (byte)option);
                    }
                    break;
                case CMD.DONT:
                    if (this.local.supported[option])
                    {
                        if (this.local.in_effect[option] != tristate.off)
                        {
                            this.local.in_effect[option] = tristate.off;
                            this.TelnetCmd(CMD.WONT, (byte)option);
                        }
                    }
                    break;
                case CMD.IAC:
                    byte[] ff = { (byte)CMD.IAC };
                    this._IndicateData(ff);
                    break;
                default: // can't happen
                    break;
            }
        }

        private void TelnetCmd(CMD cmd, byte option)
        {
            byte[] data = { (byte)CMD.IAC, (byte)cmd, option };
            this._RequestData(data);
        }

        private void TelnetSendSubopt(OPT option, string val)
        {
            byte[] bval = (new ASCIIEncoding()).GetBytes(val);
            byte[] data = new byte[6 + val.Length];
            int i = 0;
            data[i++] = (byte)CMD.IAC;
            data[i++] = (byte)CMD.SB;
            data[i++] = (byte)option;
            data[i++] = 0;
            bval.CopyTo(data, i);
            i += bval.Length;
            data[i++] = (byte)CMD.IAC;
            data[i++] = (byte)CMD.SE;
            this._RequestData(data);
        }

        private void subnegotiate(Stream s, int option)
        {
            int send = s.ReadByte();
            if (send == 0)
            {
                while (s.ReadByte() != 255)
                {
                    // not interested in values at the moment
                }

                s.ReadByte();
                return;
            }
            switch ((OPT)option) // what happens if its undefined ?
            {
                case OPT.TTYPE:
                    this.TelnetSendSubopt(OPT.TTYPE, this.TerminalType);
                    break;
                default:
                    break;
            }
            return;
        }

        #endregion
    }
}