namespace Terminals.Connections
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    ///     Description of TcpProtocol.
    /// </summary>
    public class TcpProtocol
    {
        #region Public Delegates (2)

        public delegate void DataIndicate(byte[] data);

        public delegate void Disconnect();

        #endregion

        #region Public Events (2)

        public event DataIndicate OnDataIndicated;
        public event Disconnect OnDisconnect;

        #endregion

        #region Public Constructors (1)

        public TcpProtocol(NetworkStream stream)
        {
            this.stream = stream;
            State s = new State(stream);
            this.stream.BeginRead(s.buffer, 0, s.buffer.Length, this.OnRead, s);
        }

        #endregion

        #region Public Methods (1)

        public void RequestData(byte[] data)
        {
            this.stream.Write(data, 0, data.Length);
        }

        #endregion

        #region Private Fields (1)

        private readonly NetworkStream stream;

        #endregion

        #region Private Classes (1)

        private class State
        {
            public readonly byte[] buffer;
            public readonly NetworkStream stream;

            public State(NetworkStream stream)
            {
                this.stream = stream;
                this.buffer = new byte[2048];
            }
        }

        #endregion

        #region Private Methods (1)

        private void OnRead(IAsyncResult ar)
        {
            State s = (State) ar.AsyncState;
            try
            {
                int n = s.stream.EndRead(ar);
                if (n > 0 && this.OnDataIndicated != null)
                {
                    byte[] obuf = new byte[n];
                    Array.Copy(s.buffer, obuf, obuf.Length);
                    this.OnDataIndicated(obuf);
                }
                this.stream.BeginRead(s.buffer, 0, s.buffer.Length,
                                      this.OnRead, s);
            }
            catch (Exception)
            {
                if (this.OnDisconnect != null) this.OnDisconnect();
            }
        }

        #endregion
    }
}