using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Terminals.Connections
{
    /// <summary>
    ///     Description of TelnetStream.
    /// </summary>
    public class TelnetStream : Stream
    {
        #region Constructors (1)

        public TelnetStream(NetworkStream s)
        {
            this.first_tx = true;
            this.stream = s;
            this.local = new Options();
            this.remote = new Options();
            this.local.supported[(int) OPT.TTYPE] = true; // terminal type
            this.remote.supported[(int) OPT.ECHO] = true; // echo
        }

        #endregion

        #region Private Enums (1)

        private enum tristate
        {
            not_set = 0,
            on = 1,
            off = 2
        }

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

        private readonly Options local;
        private readonly Options remote;
        private readonly NetworkStream stream;
        private string TerminalType;
        private bool first_tx;

        public override long Position
        {
            get { return this.stream.Position; }
            set { this.stream.Position = value; }
        }

        public override long Length
        {
            get { return this.stream.Length; }
        }

        public override bool CanWrite
        {
            get { return this.stream.CanWrite; }
        }

        public override bool CanRead
        {
            get { return this.stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.stream.CanSeek; }
        }

        public override void Write(byte[] data, int offset, int length)
        {
            if (this.first_tx)
            {
                this.first_tx = false;
                if (this.remote.in_effect[(byte) OPT.ECHO] != tristate.on)
                    this.TelnetCmd(CMD.DO, (byte) OPT.ECHO);
            }
            int n = 0;
            // count the chars that look like IACs
            for (int i = 0; i < length; i++)
            {
                byte b = data[offset + i];
                if (b == 255)
                {
                    n++;
                }
            }
            if (n == 0)
            {
                // no 0xff chars just send on the buffer
                this.stream.Write(data, offset, length);
            }
            else
            {
                // some 0xff chars. write array, doubling 0xff chars
                for (int i = 0; i < length; i++)
                {
                    byte b = data[offset + i];
                    if (b == (byte) CMD.IAC)
                    {
                        this.stream.WriteByte(b);
                    }
                    this.stream.WriteByte(b);
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            MemoryStream m = new MemoryStream(buffer, offset, length);
            while (m.Position == 0)
            {
                while (this.stream.DataAvailable == false)
                    Thread.Sleep(500);
                while (this.stream.DataAvailable && m.Position < length)
                {
                    int b = this.stream.ReadByte();
                    if (b < 0)
                    {
                        break;
                    }
                    if ((byte) b == (byte) CMD.IAC)
                    {
                        int cmd = this.stream.ReadByte();
                        if (cmd < 0)
                            break;
                        if ((byte) cmd == (byte) CMD.IAC)
                        {
                            m.WriteByte((byte) cmd);
                        }
                        else
                        {
                            this.process_telnet_command((byte) cmd, this.stream);
                        }
                    }
                    else
                    {
                        m.WriteByte((byte) b);
                    }
                }
            }
            return (int) m.Position;
        }

        public override void SetLength(long l)
        {
            this.stream.SetLength(l);
        }

        public override long Seek(long amount, SeekOrigin from)
        {
            return this.stream.Seek(amount, from);
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        private void process_telnet_command(byte b, Stream s)
        {
            // TODO what if end of command sequence not in this buffer?
            if (b < 240)
                return; // error
            int option = s.ReadByte();
            switch ((CMD) b)
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
                            this.TelnetCmd(CMD.DO, (byte) option);
                        }
                    }
                    else
                    {
                        this.TelnetCmd(CMD.DONT, (byte) option);
                    }
                    break;
                case CMD.WONT:
                    if (this.remote.supported[option])
                    {
                        if (this.remote.in_effect[option] != tristate.off)
                        {
                            this.remote.in_effect[option] = tristate.off;
                            this.TelnetCmd(CMD.DONT, (byte) option);
                        }
                    }
                    break;
                case CMD.DO:
                    if (this.local.supported[option])
                    {
                        if (this.local.in_effect[option] != tristate.on)
                        {
                            this.local.in_effect[option] = tristate.on;
                            this.TelnetCmd(CMD.WILL, (byte) option);
                        }
                    }
                    else
                    {
                        this.local.in_effect[option] = tristate.off;
                        this.TelnetCmd(CMD.WONT, (byte) option);
                    }
                    break;
                case CMD.DONT:
                    if (this.local.supported[option])
                    {
                        if (this.local.in_effect[option] != tristate.off)
                        {
                            this.local.in_effect[option] = tristate.off;
                            this.TelnetCmd(CMD.WONT, (byte) option);
                        }
                    }
                    break;
                case CMD.IAC: // can't happen
                    break;
                default: // can't happen
                    break;
            }
        }

        private void TelnetCmd(CMD cmd, byte option)
        {
            byte[] data = {(byte) CMD.IAC, (byte) cmd, option};
            this.stream.Write(data, 0, data.Length);
        }

        private void TelnetSendSubopt(OPT option, string val)
        {
            byte[] bval = (new ASCIIEncoding()).GetBytes(val);
            byte[] data = new byte[6 + val.Length];
            int i = 0;
            data[i++] = (byte) CMD.IAC;
            data[i++] = (byte) CMD.SB;
            data[i++] = (byte) option;
            data[i++] = 0;
            bval.CopyTo(data, i);
            i += bval.Length;
            data[i++] = (byte) CMD.IAC;
            data[i++] = (byte) CMD.SE;
            this.stream.Write(data, 0, data.Length);
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
            switch ((OPT) option) // what happens if its undefined ?
            {
                case OPT.TTYPE:
                    this.TelnetSendSubopt(OPT.TTYPE, this.TerminalType);
                    break;
                default:
                    break;
            }
            return;
        }

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
    }
}