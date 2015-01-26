using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Windows.Forms;
using Kohl.Framework.Localization;

namespace Terminals.Network.WMI
{
    public partial class NetworkShares : UserControl
    {
        public NetworkShares()
        {
            this.InitializeComponent();
            Localization.SetLanguage(this);
        }

        private void LoadShares(string Username, string Password, string Computer)
        {
            List<Share> shares = new List<Share>();

            StringBuilder sb = new StringBuilder();

            const string qry = "select * from win32_share";

            ManagementObjectSearcher searcher;

            ObjectQuery query = new ObjectQuery(qry);

            if (Username != "" && Password != "" && Computer != "" && !Computer.StartsWith(@"\\localhost"))
            {
                ConnectionOptions oConn = new ConnectionOptions {Username = Username, Password = Password};

                if (!Computer.StartsWith(@"\\")) Computer = @"\\" + Computer;

                if (!Computer.ToLower().EndsWith(@"\root\cimv2")) Computer = Computer + @"\root\cimv2";

                ManagementScope oMs = new ManagementScope(Computer, oConn);

                searcher = new ManagementObjectSearcher(oMs, query);
            }
            else
            {
                searcher = new ManagementObjectSearcher(query);
            }

            foreach (ManagementObject share in searcher.Get())
            {
                Share s = new Share();

                foreach (PropertyData p in share.Properties)
                {
                    switch (p.Name)
                    {
                        case "AccessMask":
                            if (p.Value != null) s.AccessMask = p.Value.ToString();
                            break;
                        case "MaximumAllowed":
                            if (p.Value != null) s.MaximumAllowed = p.Value.ToString();
                            break;
                        case "InstallDate":
                            if (p.Value != null) s.InstallDate = p.Value.ToString();
                            break;
                        case "Description":
                            if (p.Value != null) s.Description = p.Value.ToString();
                            break;
                        case "Caption":
                            if (p.Value != null) s.Caption = p.Value.ToString();
                            break;
                        case "AllowMaximum":
                            if (p.Value != null) s.AllowMaximum = p.Value.ToString();
                            break;
                        case "Name":
                            if (p.Value != null) s.Name = p.Value.ToString();
                            break;
                        case "Path":
                            if (p.Value != null) s.Path = p.Value.ToString();
                            break;
                        case "Status":
                            if (p.Value != null) s.Status = p.Value.ToString();
                            break;
                        case "Type":
                            if (p.Value != null) s.Type = p.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }

                shares.Add(s);
            }

            this.dataGridView1.DataSource = shares;
        }

        private void NetworkShares_Load(object sender, EventArgs e)
        {
            this.LoadShares("", "", "");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.LoadShares(this.wmiServerCredentials1.Username, this.wmiServerCredentials1.Password,
                            this.wmiServerCredentials1.SelectedServer);
        }

        private void wmiServerCredentials1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1_Click(null, null);
            }
        }
    }
}