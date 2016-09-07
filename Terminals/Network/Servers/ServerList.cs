/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 13:11
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Terminals.Network.Servers
{
    public partial class ServerList : UserControl
    {
        public ServerList()
        {
            this.InitializeComponent();
        }

        private void ServerList_Load(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            Application.DoEvents();
            List<KnownServers> list = new List<KnownServers>();
            Servers servers = new Servers(ServerType.All);

            foreach (string name in servers)
            {
                ServerType type = Servers.GetServerType(name);
                KnownServers s = new KnownServers {Name = name, Type = type};
                list.Add(s);
            }

            this.dataGridView1.DataSource = list;
        }

        private class KnownServers
        {
            public string Name { private get; set; }

            public ServerType Type { private get; set; }
        }
    }
}