namespace Terminals.TerminalServices
{
	using System;
	using System.Configuration;
	using System.Reflection;

	[System.ComponentModel.Category("Client")]
    public class Client
    {
    	[System.ComponentModel.Category("Client")]
        public string Status { get; set; }

        [System.ComponentModel.Category("Client")]
        public string UserName { get; set; }

        [System.ComponentModel.Category("Client")]
        public string StationName { get; set; }

        [System.ComponentModel.Category("Client")]
        public string DomianName { get; set; }

        [System.ComponentModel.Category("Client")]
        public string ClientName { get; set; }

        [System.ComponentModel.Category("Client")]
        public string AddressFamily { get; set; }

        [System.ComponentModel.Category("Client")]
        public string Address { get; set; }
		
        [System.ComponentModel.Category("Client")]
        public uint SessionId { get; set; }
        
        public override string ToString()
        {
            return string.Format("Domain:{0}, Client:{1}, Station:{2}, Address:{3}, Username:{4}, Status:{5}",
                                 this.DomianName, this.ClientName, this.StationName, this.Address, this.UserName,
                                 this.Status);
        }
    }
}