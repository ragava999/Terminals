using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Kohl.Framework.Localization;
using Kohl.Framework.Logging;

namespace Terminals.Network.DNS
{
    public static class AdapterInfo
    {
        public static List<String> DnsServers
        {
            get
            {
                List<string> servers = new List<String>();
                try
                {
                    List<Adapter> adapters = GetAdapters();
                    servers.AddRange(from a in adapters where a.IPEnabled where a.DNSServerSearchOrder != null from server in a.DNSServerSearchOrder select "Local: " + server);
                }
                catch (Exception exc)
                {
                    Log.Error(Localization.Text("Network.DNS.AdapterInfo_Error"), exc);
                }

                servers.Add("Google: 8.8.8.8");
                servers.Add("Google: 8.8.4.4");
                servers.Add("Dnsadvantage: 156.154.70.1");
                servers.Add("Dnsadvantage: 156.154.71.1");
                servers.Add("OpenDNS: 208.67.222.222");
                servers.Add("OpenDNS: 208.67.220.220");
                servers.Add("Norton: 198.153.192.1");
                servers.Add("Norton: 198.153.194.1");
                servers.Add("Verizon: 4.2.2.1");
                servers.Add("Verizon: 4.2.2.2");
                servers.Add("Verizon: 4.2.2.3");
                servers.Add("Verizon: 4.2.2.4");
                servers.Add("Verizon: 4.2.2.5");
                servers.Add("Verizon: 4.2.2.6");

                /*
                 Free Public DNS Server

                => Service provider: Google
                 Google public dns server IP address:
                •8.8.8.8
                •8.8.4.4

                => Service provider:Dnsadvantage
                 Dnsadvantage free dns server list:
                •156.154.70.1
                •156.154.71.1

                => Service provider:OpenDNS
                 OpenDNS free dns server list / IP address:
                •208.67.222.222
                •208.67.220.220

                => Service provider:Norton
                 Norton free dns server list / IP address:
                •198.153.192.1
                •198.153.194.1

                => Service provider: GTEI DNS (now Verizon)
                 Public Name server IP address:
                •4.2.2.1
                •4.2.2.2
                •4.2.2.3
                •4.2.2.4
                •4.2.2.5
                •4.2.2.6

                 */

                return servers;
            }
        }

        public static List<Adapter> GetAdapters()
        {
            List<Adapter> adapterList = new List<Adapter>();

            ObjectQuery q = new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(q);
            foreach (ManagementObject share in searcher.Get())
            {
                Adapter ad = new Adapter {PropertyData = share};
                adapterList.Add(ad);
            }

            return adapterList;
        }
    }
}