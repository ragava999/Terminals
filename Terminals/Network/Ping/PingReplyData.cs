using System;

namespace Terminals.Network.Ping
{
    /// <summary>
    ///     Represents data from ping reply.
    /// </summary>
    public class PingReplyData
    {
        public PingReplyData(Int64 count, String status, String hostname, String destination, Int32 bytes, Int32 ttl,
                             Int64 roundTripTime, string time)
        {
            this.Count = count;
            this.Status = status;
            this.Hostname = hostname;
            this.Destination = destination;
            this.Bytes = bytes;
            this.TimeToLive = ttl;
            this.RoundTripTime = roundTripTime;
            this.Time = time;
        }

        public Int64 Count { get; private set; }
        public String Status { get; private set; }
        public String Hostname { get; private set; }
        public String Destination { get; private set; }
        public Int32 Bytes { get; private set; }
        public Int32 TimeToLive { get; private set; }
        public Int64 RoundTripTime { get; private set; }
        public string Time { get; private set; }
    }
}