using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.DirectoryServices.ActiveDirectory;
using Kohl.Framework.Logging;

namespace Terminals.Network.DNS
{
    public static class AdapterInfo
    {
        private static List<String> dnsServers = null;

        public static void LoadDnsServers()
        {
            dnsServers = new List<string>();

            Log.Info("Precaching DNS servers");

            try
            {
                List<Adapter> adapters = GetAdapters();
                dnsServers.AddRange(from a in adapters where a.IPEnabled where a.DNSServerSearchOrder != null from server in a.DNSServerSearchOrder select "Local: " + server);

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

                dnsServers.Add("Google: 8.8.8.8");
                dnsServers.Add("Google: 8.8.4.4");
                dnsServers.Add("Dnsadvantage: 156.154.70.1");
                dnsServers.Add("Dnsadvantage: 156.154.71.1");
                dnsServers.Add("OpenDNS: 208.67.222.222");
                dnsServers.Add("OpenDNS: 208.67.220.220");
                dnsServers.Add("Norton: 198.153.192.1");
                dnsServers.Add("Norton: 198.153.194.1");
                dnsServers.Add("Verizon: 4.2.2.1");
                dnsServers.Add("Verizon: 4.2.2.2");
                dnsServers.Add("Verizon: 4.2.2.3");
                dnsServers.Add("Verizon: 4.2.2.4");
                dnsServers.Add("Verizon: 4.2.2.5");
                dnsServers.Add("Verizon: 4.2.2.6");

                using (var forest = Forest.GetCurrentForest())
                {
                    foreach (Domain domain in forest.Domains)
                    {
                        try
                        {
                            // One domain controller per domain is fully enough
                            /*
                             foreach (DomainController dc in domain.DomainControllers)
                                  dnsServers.Add("Domain Controller (" + domain.Name + "): " + dc.IPAddress);
                            */
                            dnsServers.Add("Domain Controller (" + domain.Name + "): " + domain.DomainControllers[0].IPAddress);
                        }
                        catch
                        {
                            Log.Debug("Skipping Active Directory server " + domain.Name + " for DNS lookups.");
                        }
                        finally
                        {
                            domain.Dispose();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error("The DNS server lookup failed.", exc);
            }
        }

        public static List<String> DnsServers
        {
            get
            {
                if (dnsServers != null)
                    return dnsServers;

                LoadDnsServers();
                return dnsServers;
            }
        }

        public static List<Adapter> GetAdapters()
        {
            List<Adapter> adapterList = new List<Adapter>();

            ObjectQuery q = new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(q);
            foreach (ManagementObject share in searcher.Get())
            {
                Adapter ad = new Adapter { PropertyData = share };
                adapterList.Add(ad);
            }

            return adapterList;
        }
    }
}