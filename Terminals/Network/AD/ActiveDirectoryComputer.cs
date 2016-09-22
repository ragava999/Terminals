using System;
using System.DirectoryServices;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using Terminals.Connections;

namespace Terminals.Network.AD
{
    public class ActiveDirectoryComputer
    {
        private const string NAME = "name";
        private const string OS = "operatingSystem";
        private const string DN = "distinguishedName";

        private ActiveDirectoryComputer()
        {
            this.Protocol = typeof(RDPConnection).GetProtocolName();
            this.ComputerName = String.Empty;
            this.OperatingSystem = String.Empty;
            this.Tags = String.Empty;
            this.Notes = String.Empty;
        }

        private String ComputerName { get; set; }
        private String OperatingSystem { get; set; }

        private String Protocol { get; set; }
        private String Tags { get; set; }
        private String Notes { get; set; }

        public static ActiveDirectoryComputer FromDirectoryEntry(String domain, DirectoryEntry computer)
        {
            ActiveDirectoryComputer comp = new ActiveDirectoryComputer { Tags = domain };

            if (computer.Properties != null)
            {
                comp.NameFromEntry(computer);
                comp.OperationSystemFromEntry(computer);
                comp.DistinquishedNameFromEntry(computer);
            }

            return comp;
        }

        private void NameFromEntry(DirectoryEntry computer)
        {
            PropertyValueCollection nameValues = computer.Properties[NAME];
            String name = computer.Name.Replace("CN=", "");
            if (nameValues != null && nameValues.Count > 0)
            {
                name = nameValues[0].ToString();
            }
            this.ComputerName = name;
        }

        private void OperationSystemFromEntry(DirectoryEntry computer)
        {
            PropertyValueCollection osValues = computer.Properties[OS];
            if (osValues != null && osValues.Count > 0)
            {
                this.Tags += "," + osValues[0];
                this.OperatingSystem = osValues[0].ToString();
            }
        }

        private void DistinquishedNameFromEntry(DirectoryEntry computer)
        {
            PropertyValueCollection dnameValues = computer.Properties[DN];
            if (dnameValues != null && dnameValues.Count > 0)
            {
                string distinguishedName = dnameValues[0].ToString();
                if (distinguishedName.Contains("OU=Domain Controllers"))
                {
                    this.Tags += ",Domain Controllers";
                }
            }
        }

        public FavoriteConfigurationElement ToFavorite(String domain)
        {
            FavoriteConfigurationElement favorite = new FavoriteConfigurationElement(this.ComputerName)
            {
                Name = this.ComputerName,
                ServerName = this.ComputerName,
                UserName = Environment.UserName,
                DomainName = domain,
                Tags = this.Tags,
                Port = ConnectionManager.GetPort(this.Protocol),
                Protocol = this.Protocol,
                Notes = this.Notes
            };
            return favorite;
        }
    }
}