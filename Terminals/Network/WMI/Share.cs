/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 15:01
 */

namespace Terminals.Network.WMI
{
    /// <summary>
    ///     Description of Share.
    /// </summary>
    public class Share
    {
        private string type;
        public string AccessMask { private get; set; }

        public string MaximumAllowed { private get; set; }

        public string InstallDate { private get; set; }

        public string Description { private get; set; }

        public string Caption { private get; set; }

        public string AllowMaximum { private get; set; }

        public string Name { private get; set; }

        public string Path { private get; set; }

        public string Status { private get; set; }

        public string Type
        {
            get
            {
                switch (this.type)
                {
                    case "0":
                        return "Disk Drive";
                    case "1":
                        return "Print Queue";
                    case "2":
                        return "Device";
                    case "3":
                        return "IPC";
                    case "2147483648":
                        return "Disk Drive Admin";
                    case "2147483649":
                        return "Print Queue Admin";
                    case "2147483650":
                        return "Device Admin";
                    case "2147483651":
                        return "IPC Admin";
                    default:
                        return this.type;
                }
            }
            set { this.type = value; }
        }
    }
}