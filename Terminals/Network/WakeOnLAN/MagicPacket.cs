using Kohl.Framework.Security;
using System;
using System.Globalization;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Terminals.Network.WakeOnLAN
{
    public class MagicPacket
    {
        public enum ShutdownCommands
        {
            LogOff = 0,
            ForcedLogOff = 4,
            Shutdown = 1,
            ForcedShutdown = 5,
            Reboot = 2,
            ForcedReboot = 6,
            PowerOff = 8,
            ForcedPowerOff = 12
        }

        private const int HEADER = 6;
        private const int BYTELENGHT = 6;
        private const int MAGICPACKETLENGTH = 16;
        private readonly byte[] magicPacketPayload;

        private readonly IPEndPoint wolEndPoint;
        private readonly IPAddress wolIPAddr = IPAddress.Broadcast;
        private readonly byte[] wolMacAddr;
        private readonly int wolPortAddr = 7;

        public MagicPacket(String macAddress)
        {
            this.wolMacAddr = Mac2Byte(macAddress);
            this.magicPacketPayload = CreatePayload(this.wolMacAddr);
            this.wolEndPoint = new IPEndPoint(this.wolIPAddr, this.wolPortAddr);
        }

        public MagicPacket(String macAddress, String strPortAddress)
        {
            this.wolMacAddr = Mac2Byte(macAddress);
            this.magicPacketPayload = CreatePayload(this.wolMacAddr);
            this.wolPortAddr = Convert.ToInt16(strPortAddress, 10);
            this.wolEndPoint = new IPEndPoint(this.wolIPAddr, this.wolPortAddr);
        }

        public string macAddress
        {
            get
            {
                String strMacAdress = String.Empty;
                for (Int32 i = 0; i < this.wolMacAddr.Length; i++)
                {
                    strMacAdress += this.wolMacAddr[i].ToString("X2");
                }

                return strMacAdress;
            }
        }

        private static Byte[] Mac2Byte(String strMacAddress)
        {
            String macAddr = String.Empty;
            Byte[] macBytes = new Byte[BYTELENGHT];
            //remove all non 0-9, A-F, a-f characters
            macAddr = Regex.Replace(strMacAddress, @"[^0-9A-Fa-f]", "");
            //check if it is now a valid mac adress
            if (macAddr.Length != BYTELENGHT * 2)
                throw new ArgumentException("Mac Adress must be " + (BYTELENGHT * 2).ToString() +
                                            " digits of 0-9, A-F, a-f characters in length.");

            for (Int32 i = 0; i < macBytes.Length; i++)
            {
                String hex = new String(new[] { macAddr[i * 2], macAddr[i * 2 + 1] });
                macBytes[(i)] = byte.Parse(hex, NumberStyles.HexNumber);
            }

            return macBytes;
        }

        private static Byte[] CreatePayload(Byte[] macAddress)
        {
            Byte[] payloadData = new Byte[HEADER + MAGICPACKETLENGTH * BYTELENGHT];
            for (Int32 i = 0; i < HEADER; i++)
            {
                payloadData[i] = Byte.Parse("FF", NumberStyles.HexNumber);
            }

            for (Int32 i = 0; i < MAGICPACKETLENGTH; i++)
            {
                for (Int32 j = 0; j < BYTELENGHT; j++)
                {
                    payloadData[((i * BYTELENGHT) + j) + HEADER] = macAddress[j];
                }
            }

            return payloadData;
        }

        public Int32 WakeUp()
        {
            return SendUDP(this.magicPacketPayload, this.wolEndPoint);
        }

        private static Int32 SendUDP(Byte[] Payload, IPEndPoint EndPoint)
        {
            Int32 byteSend;

            //create a new client socket...
            Socket socketClient = new Socket(EndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                //open connection...
                socketClient.Connect(EndPoint);
                //send MagicPacket(TM)...
                byteSend = socketClient.Send(Payload, 0, Payload.Length, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                throw ex;
            }
            finally
            {
                socketClient.Close();
            }

            return byteSend;
        }

        /// <summary>
        ///     Send a shutdown command to a (remote) computer.
        /// </summary>
        /// <param name="machineName"> The machinename or ip-address of computer to send shutdown command to. </param>
        /// <param name="shutdownCommand"> Shutdown type command. </param>
        /// <param name="credentials"> Optional network credentials for the (remote) computer. </param>
        /// <returns> 0 if the shutdown was succesfully send, else another integer value. </returns>
        /// <exception cref="ManagementException">An unhandled managed error occured.</exception>
        /// <exception cref="UnauthorizedAccessException">Access was denied.</exception>
        public static Int32 ForceShutdown(String machineName, ShutdownCommands shutdownCommand, Credential credential = null)
        {
            Int32 result = -1;

            ConnectionOptions options = new ConnectionOptions();
            if (credential != null)
            {
                options.EnablePrivileges = true;
                options.Username = credential.UserNameWithDomain;
                options.Password = credential.Password;
            }

            ManagementScope scope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", machineName), options);
            scope.Connect();

            SelectQuery query = new SelectQuery("Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject os in searcher.Get())
            {
                ManagementBaseObject inParams = os.GetMethodParameters("Win32Shutdown");
                inParams["Flags"] = (Int32)shutdownCommand;

                ManagementBaseObject outParams = os.InvokeMethod("Win32Shutdown", inParams, null);
                result = Convert.ToInt32(outParams.Properties["returnValue"].Value);

                return result;
            }

            return result;
        }
    }
}