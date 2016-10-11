/*
 * NTPClient
 * Copyright (C)2001 Valer BOCAN <vbocan@dataman.ro>
 * All Rights Reserved
 * 
 * This code is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * To fully understand the concepts used herein, I strongly
 * recommend that you read the RFC 2030.
 * 
 * Borrowed from:
 * http://www.codeguru.com/Csharp/Csharp/cs_date_time/timeroutines/article.php/c4207/
 * 
 */

using Kohl.Framework.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace Terminals.Network.NTP
{
    // Leap indicator field values

    //Mode field values

    // Stratum field values

    /// <summary>
    ///     NTPClient is a C# class designed to connect to time servers on the Internet.
    ///     The implementation of the protocol is based on the RFC 2030.
    ///  
    ///     Public class members:
    /// 
    ///     LeapIndicator - Warns of an impending leap second to be inserted/deleted in the last
    ///     minute of the current day. (See the _LeapIndicator enum)
    ///  
    ///     VersionNumber - Version number of the protocol (3 or 4).
    ///  
    ///     Mode - Returns mode. (See the _Mode enum)
    ///  
    ///     Stratum - Stratum of the clock. (See the _Stratum enum)
    ///  
    ///     PollInterval - Maximum interval between successive messages.
    ///  
    ///     Precision - Precision of the clock.
    ///  
    ///     RootDelay - Round trip time to the primary reference source.
    ///  
    ///     RootDispersion - Nominal error relative to the primary reference source.
    ///  
    ///     ReferenceID - Reference identifier (either a 4 character string or an IP address).
    ///  
    ///     ReferenceTimestamp - The time at which the clock was last set or corrected.
    ///  
    ///     OriginateTimestamp - The time at which the request departed the client for the server.
    ///  
    ///     ReceiveTimestamp - The time at which the request arrived at the server.
    ///  
    ///     Transmit Timestamp - The time at which the reply departed the server for client.
    ///  
    ///     RoundTripDelay - The time between the departure of request and arrival of reply.
    ///  
    ///     LocalClockOffset - The offset of the local clock relative to the primary reference
    ///     source.
    ///  
    ///     Initialize - Sets up data structure and prepares for connection.
    ///  
    ///     Connect - Connects to the time server and populates the data structure.
    ///  
    ///     IsResponseValid - Returns true if received data is valid and if comes from
    ///     a NTP-compliant time server.
    ///  
    ///     -----------------------------------------------------------------------------
    ///     Structure of the standard NTP header (as described in RFC 2030)
    ///     1                   2                   3
    ///     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |LI | VN  |Mode |    Stratum    |     Poll      |   Precision   |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                          Root Delay                           |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                       Root Dispersion                         |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                     Reference Identifier                      |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                                                               |
    ///     |                   Reference Timestamp (64)                    |
    ///     |                                                               |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                                                               |
    ///     |                   Originate Timestamp (64)                    |
    ///     |                                                               |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                                                               |
    ///     |                    Receive Timestamp (64)                     |
    ///     |                                                               |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                                                               |
    ///     |                    Transmit Timestamp (64)                    |
    ///     |                                                               |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                 Key Identifier (optional) (32)                |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                                                               |
    ///     |                                                               |
    ///     |                 Message Digest (optional) (128)               |
    ///     |                                                               |
    ///     |                                                               |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  
    ///     -----------------------------------------------------------------------------
    ///  
    ///     NTP Timestamp Format (as described in RFC 2030)
    ///     1                   2                   3
    ///     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                           Seconds                             |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///     |                  Seconds Fraction (0-padded)                  |
    ///     +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </summary>
    public class NTPClient
    {
        // NTP Data Structure Length
        private const byte NTPDataLength = 48;
        // NTP Data Structure (as described in RFC 2030)

        // Offset constants for timestamps in the data structure
        private const byte offReferenceID = 12;
        private const byte offReferenceTimestamp = 16;
        private const byte offOriginateTimestamp = 24;
        private const byte offReceiveTimestamp = 32;
        private const byte offTransmitTimestamp = 40;
        public const string DefaultTimeServer = "time.nist.gov";
        private readonly string TimeServer;
        private byte[] NTPData = new byte[NTPDataLength];
        private DateTime ReceptionTimestamp;

        private NTPClient(string host)
        {
            this.TimeServer = host;
        }

        // Leap Indicator
        public LeapIndicator LeapIndicator
        {
            get
            {
                // Isolate the two most significant bits
                byte val = (byte)(this.NTPData[0] >> 6);
                switch (val)
                {
                    case 0:
                        return LeapIndicator.NoWarning;
                    case 1:
                        return LeapIndicator.LastMinute61;
                    case 2:
                        return LeapIndicator.LastMinute59;
                    default:
                        return LeapIndicator.Alarm;
                }
            }
        }

        // Version Number
        private byte VersionNumber
        {
            get
            {
                // Isolate bits 3 - 5
                byte val = (byte)((this.NTPData[0] & 0x38) >> 3);
                return val;
            }
        }

        // Mode
        private Mode Mode
        {
            get
            {
                // Isolate bits 0 - 3
                byte val = (byte)(this.NTPData[0] & 0x7);
                switch (val)
                {
                    case 1:
                        return Mode.SymmetricActive;
                    case 2:
                        return Mode.SymmetricPassive;
                    case 3:
                        return Mode.Client;
                    case 4:
                        return Mode.Server;
                    case 5:
                        return Mode.Broadcast;
                    default:
                        return Mode.Unknown;
                }
            }
        }

        // Stratum
        private Stratum Stratum
        {
            get
            {
                byte val = this.NTPData[1];
                if (val == 0)
                    return Stratum.Unspecified;
                if (val == 1) return Stratum.PrimaryReference;
                if (val <= 15) return Stratum.SecondaryReference;
                return Stratum.Reserved;
            }
        }

        // Poll Interval
        public uint PollInterval
        {
            get { return (uint)Math.Round(Math.Pow(2, this.NTPData[2])); }
        }

        // Precision (in milliseconds)
        public double Precision
        {
            get { return (1000 * Math.Pow(2, this.NTPData[3])); }
        }

        // Root Delay (in milliseconds)
        public double RootDelay
        {
            get
            {
                int temp = 0;
                temp = 256 * (256 * (256 * this.NTPData[4] + this.NTPData[5]) + this.NTPData[6]) + this.NTPData[7];
                return 1000 * (((double)temp) / 0x10000);
            }
        }

        // Root Dispersion (in milliseconds)
        public double RootDispersion
        {
            get
            {
                int temp = 0;
                temp = 256 * (256 * (256 * this.NTPData[8] + this.NTPData[9]) + this.NTPData[10]) + this.NTPData[11];
                return 1000 * (((double)temp) / 0x10000);
            }
        }

        // Reference Identifier
        public string ReferenceID
        {
            get
            {
                string val = "";

                switch (this.Stratum)
                {
                    case Stratum.Unspecified:
                    case Stratum.PrimaryReference:
                        val += Convert.ToChar(this.NTPData[offReferenceID + 0]);
                        val += Convert.ToChar(this.NTPData[offReferenceID + 1]);
                        val += Convert.ToChar(this.NTPData[offReferenceID + 2]);
                        val += Convert.ToChar(this.NTPData[offReferenceID + 3]);
                        break;

                    case Stratum.SecondaryReference:
                        switch (this.VersionNumber)
                        {
                            case 3: // Version 3, Reference ID is an IPv4 address
                                string Address = this.NTPData[offReferenceID + 0].ToString() + "." +
                                                 this.NTPData[offReferenceID + 1].ToString() + "." +
                                                 this.NTPData[offReferenceID + 2].ToString() + "." +
                                                 this.NTPData[offReferenceID + 3].ToString();
                                try
                                {
                                    IPAddress RefAddr = IPAddress.Parse(Address);
                                    //IPHostEntry Host = Dns.GetHostByAddress(RefAddr);
                                    IPHostEntry Host = Dns.GetHostEntry(RefAddr);
                                    val = Host.HostName + " (" + Address + ")";
                                }
                                catch (Exception e)
                                {
                                    val = "N/A";
                                    Log.Error(
                                string.Format("Error parsing and looking up DNS for IP address '{0}'.", Address),
                                        e);
                                }

                                break;

                            case 4: // Version 4, Reference ID is the timestamp of last update
                                DateTime time = this.ComputeDate(this.GetMilliSeconds(offReferenceID));
                                // Take care of the time zone
                                long offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Ticks;
                                TimeSpan offspan = TimeSpan.FromTicks(offset);
                                val = (time + offspan).ToString();
                                break;

                            default:
                                val = "N/A";
                                break;
                        }

                        break;
                }

                return val;
            }
        }

        // Reference Timestamp
        public DateTime ReferenceTimestamp
        {
            get
            {
                DateTime time = this.ComputeDate(this.GetMilliSeconds(offReferenceTimestamp));
                // Take care of the time zone
                long offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Ticks;
                TimeSpan offspan = TimeSpan.FromTicks(offset);
                return time + offspan;
            }
        }

        // Originate Timestamp
        private DateTime OriginateTimestamp
        {
            get { return this.ComputeDate(this.GetMilliSeconds(offOriginateTimestamp)); }
        }

        // Receive Timestamp
        private DateTime ReceiveTimestamp
        {
            get
            {
                DateTime time = this.ComputeDate(this.GetMilliSeconds(offReceiveTimestamp));
                // Take care of the time zone
                long offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Ticks;
                TimeSpan offspan = TimeSpan.FromTicks(offset);
                return time + offspan;
            }
        }

        // Transmit Timestamp
        private DateTime TransmitTimestamp
        {
            get
            {
                DateTime time = this.ComputeDate(this.GetMilliSeconds(offTransmitTimestamp));
                // Take care of the time zone
                long offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Ticks;
                TimeSpan offspan = TimeSpan.FromTicks(offset);
                return time + offspan;
            }

            set { this.SetDate(offTransmitTimestamp, value); }
        }

        // Reception Timestamp

        // Round trip delay (in milliseconds)
        public int RoundTripDelay
        {
            get
            {
                TimeSpan span = (this.ReceiveTimestamp - this.OriginateTimestamp) +
                                (this.ReceptionTimestamp - this.TransmitTimestamp);

                try
                {
                    return (int)span.TotalMilliseconds;
                }
                catch
                {
                    return -1;
                }
            }
        }

        // Local clock offset (in milliseconds)
        private int LocalClockOffset
        {
            get
            {
                TimeSpan span = (this.ReceiveTimestamp - this.OriginateTimestamp) -
                                (this.ReceptionTimestamp - this.TransmitTimestamp);
                return (int)(span.TotalMilliseconds / 2);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetSystemTime([In] ref SystemTime st);

        // Compute date, given the number of milliseconds since January 1, 1900
        private DateTime ComputeDate(ulong milliseconds)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(milliseconds);
            DateTime time = new DateTime(1900, 1, 1);
            time += span;
            return time;
        }

        // Compute the number of milliseconds, given the offset of a 8-byte array
        private ulong GetMilliSeconds(byte offset)
        {
            ulong intpart = 0, fractpart = 0;

            for (int i = 0; i <= 3; i++)
            {
                intpart = 256 * intpart + this.NTPData[offset + i];
            }

            for (int i = 4; i <= 7; i++)
            {
                fractpart = 256 * fractpart + this.NTPData[offset + i];
            }

            ulong milliseconds = intpart * 1000 + (fractpart * 1000) / 0x100000000L;
            return milliseconds;
        }

        // Compute the 8-byte array, given the date
        private void SetDate(byte offset, DateTime date)
        {
            ulong intpart = 0, fractpart = 0;
            DateTime StartOfCentury = new DateTime(1900, 1, 1, 0, 0, 0); // January 1, 1900 12:00 AM

            ulong milliseconds = (ulong)(date - StartOfCentury).TotalMilliseconds;
            intpart = milliseconds / 1000;
            fractpart = ((milliseconds % 1000) * 0x100000000L) / 1000;

            ulong temp = intpart;
            for (int i = 3; i >= 0; i--)
            {
                this.NTPData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }

            temp = fractpart;
            for (int i = 7; i >= 4; i--)
            {
                this.NTPData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }
        }

        // Initialize the NTPClient data
        private void Initialize()
        {
            // Set version number to 4 and Mode to 3 (client)
            this.NTPData[0] = 0x1B;
            // Initialize all other fields with 0
            for (int i = 1; i < 48; i++)
            {
                this.NTPData[i] = 0;
            }

            // Initialize the transmit timestamp
            this.TransmitTimestamp = DateTime.Now;
        }

        // Connect to the time server
        private void Connect()
        {
            try
            {
                //IPHostEntry hostadd = System.Net.Dns.Resolve(TimeServer);
                IPHostEntry hostadd = Dns.GetHostEntry(this.TimeServer);
                IPEndPoint EPhost = new IPEndPoint(hostadd.AddressList[0], 123);
                UdpClient TimeSocket = new UdpClient { Client = { ReceiveTimeout = 1000 } };
                TimeSocket.Connect(EPhost);
                Thread.Sleep(1000);
                this.Initialize();
                TimeSocket.Send(this.NTPData, this.NTPData.Length);
                Thread.Sleep(1000);
                this.NTPData = TimeSocket.Receive(ref EPhost);
                if (!this.IsResponseValid())
                {
                    //throw new Exception("Invalid response from " + TimeServer);
                }

                this.ReceptionTimestamp = DateTime.Now;
            }
            catch (SocketException e)
            {
                //throw new Exception(e.Message);
                Log.Error("Socket Exception", e);
            }
        }

        // Check if the response from server is valid
        private bool IsResponseValid()
        {
            if (this.NTPData.Length < NTPDataLength || this.Mode != Mode.Server)
            {
                return false;
            }
            return true;
        }

        // The URL of the time server we're connecting to

        public static NTPClient GetTime(string TimeServer = DefaultTimeServer)
        {
            NTPClient client = new NTPClient(TimeServer);
            client.Connect();
            return client;
        }

        public static NTPClient GetAndSetTime()
        {
            return GetAndSetTime(DefaultTimeServer);
        }

        public static NTPClient GetAndSetTime(string TimeServer)
        {
            NTPClient client = GetTime(TimeServer);
            DateTime setTime =
                TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now.AddMilliseconds(client.LocalClockOffset));

            SystemTime st = new SystemTime
            {
                Year = (short)setTime.Year,
                Month = (short)setTime.Month,
                Day = (short)setTime.Day,
                Hour = (short)setTime.Hour,
                Minute = (short)setTime.Minute,
                Second = (short)setTime.Second,
                Milliseconds = (short)setTime.Millisecond
            };

            SetSystemTime(ref st);

            return client;
        }
    }
}