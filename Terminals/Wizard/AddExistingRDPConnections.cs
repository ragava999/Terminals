using Kohl.Framework.Logging;
using Metro;
using Metro.Scanning;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Settings;
using Terminals.Connection.Manager;
using Terminals.Connections;
using Terminals.ExportImport.Import;

namespace Terminals.Wizard
{
    public partial class AddExistingRDPConnections : UserControl
    {
        public delegate void DiscoveryCompleted();

        private readonly MethodInvoker _miv;
        private readonly NetworkInterfaceList _nil;
        private readonly List<TcpSynScanner> _scannerList = new List<TcpSynScanner>(1275);
        private readonly object _uiElementsLock = new object();
        private IPAddress _endPointAddress;
        private int _pendingRequests;
        private int _scannerCount;

        public AddExistingRDPConnections()
        {
            this.DiscoveredConnections = new List<FavoriteConfigurationElement>();

            this.InitializeComponent();

            this.dataGridView1.Visible = false;
            this._miv = this.UpdateConnections;

            if (!Kohl.Framework.Info.MachineInfo.IsUnixOrMac)
                try
                {
                    this._nil = new NetworkInterfaceList();
                }
                catch (Exception exc)
                {
                    Log.Error("Could not new up Metro.NetworkInterfaceList in AddExistingRDPConnections", exc);
                }
        }

        public List<FavoriteConfigurationElement> DiscoveredConnections { get; private set; }
        public event DiscoveryCompleted OnDiscoveryCompleted;

        public void StartImport()
        {
            this.ImportFromRegistry();
            this.ScanInterfaceList();
        }

        private void ImportFromRegistry()
        {
            List<FavoriteConfigurationElement> favoritesFromRegistry = ImportRdpRegistry.Import();
            lock (this.DiscoveredConnections)
            {
                this.DiscoveredConnections.AddRange(favoritesFromRegistry);
            }
        }

        /// <summary>
        ///     then kick up the port scan for the entire subnet
        /// </summary>
        private void ScanInterfaceList()
        {
            if (this._nil != null)
            {
                try
                {
                    foreach (NetworkInterface face in this._nil.Interfaces)
                    {
                        if (face.IsEnabled && !face.isLoopback)
                        {
                            this._endPointAddress = face.Address;
                            break;
                        }
                    }

                    ThreadPool.QueueUserWorkItem(this.ScanSubnet, null);
                }
                catch (Exception e)
                {
                    Log.Error("Port Scan error", e);
                }
            }
        }

        public void CancelDiscovery()
        {
            try
            {
                if (this._scannerCount > 0)
                {
                    foreach (TcpSynScanner scanner in this._scannerList)
                    {
                        if (scanner.Running)
                            scanner.CancelScan();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("Cancel Discovery", e);
            }
        }

        private void Scanner_ScanComplete()
        {
            try
            {
                lock (this._uiElementsLock)
                {
                    this._scannerCount--;
                }

                this.Invoke(this._miv);
            }
            catch (Exception e)
            {
                Log.Error("Scanner Complete Error", e);
            }
        }

        private void ScanSubnet(object nullstate)
        {
            try
            {
                this._pendingRequests = 254 * 5;
                string ipAddress = this._endPointAddress.ToString();
                string start = ipAddress.Substring(0, ipAddress.LastIndexOf('.')) + ".";
                for (int x = 1; x < 255; x++)
                {
                    IPAddress address = IPAddress.Parse(start + x.ToString());
                    ThreadPool.QueueUserWorkItem(this.ScanMachine, address);
                }
            }
            catch (Exception e)
            {
                Log.Error("Scan Subnet Error", e);
            }

            this.Invoke(this._miv);
        }

        private void ScanMachine(object machine)
        {
            try
            {
                TcpSynScanner scanner = new TcpSynScanner(new IPEndPoint(this._endPointAddress, 0));
                scanner.PortReply += this.Scanner_PortReply;
                scanner.ScanComplete += this.Scanner_ScanComplete;

                IPAddress address = (IPAddress)machine;
                this._scannerList.Add(scanner);
                scanner.StartScan(address, ConnectionManager.GetPorts(), 1000, 100, true);
                this._scannerCount++;
            }
            catch (Exception)
            {
                lock (this._uiElementsLock)
                {
                    this._pendingRequests = this._pendingRequests - 5;
                }
            }

            if (!this.IsDisposed)
                this.Invoke(this._miv);

            Application.DoEvents();
        }

        private void Scanner_PortReply(IPEndPoint remoteEndPoint, TcpPortState state)
        {
            try
            {
                lock (this._uiElementsLock) this._pendingRequests--;
                if (state == TcpPortState.Opened)
                {
                    this.AddFavorite(remoteEndPoint);
                }

                this.Invoke(this._miv);
            }
            catch (Exception e)
            {
                Log.Error("Scanner Port Reply", e);
            }
        }

        private void AddFavorite(IPEndPoint endPoint)
        {
            try
            {
                string serverName = endPoint.Address.ToString();
                string connectionName = serverName;
                FavoriteConfigurationElement newFavorite = FavoritesFactory.CreateNewFavorite(connectionName, serverName,
                                                                                              typeof(RDPConnection)
                                                                                                  .GetProtocolName(),
                                                                                              ConnectionManager.GetPort(
                                                                                                  typeof(RDPConnection)
                                                                                                      .GetProtocolName()));
                this.AddFavoriteToDiscovered(newFavorite);
            }
            catch (Exception e)
            {
                Log.Error("Add Favorite Error", e);
            }
        }

        private void AddFavoriteToDiscovered(FavoriteConfigurationElement newFavorite)
        {
            lock (this.DiscoveredConnections)
            {
                this.DiscoveredConnections.Add(newFavorite);
            }
        }

        private void UpdateConnections()
        {
            try
            {
                this.ConnectionsCountLabel.Text = this.DiscoveredConnections.Count.ToString();
                this.PendingRequestsLabel.Text = this._pendingRequests.ToString();

                if (this._pendingRequests <= 0 && this.OnDiscoveryCompleted != null)
                    this.OnDiscoveryCompleted();

                Application.DoEvents();
            }
            catch (Exception e)
            {
                Log.Error("Update Connections", e);
            }
        }

        /// <summary>
        ///     hidden egg to show the connections.  Just click on the connections count label to show and update the list
        /// </summary>
        private void ConnectionsCountLabel_Click(object sender, EventArgs e)
        {
            try
            {
                List<BindingElement> list = new List<BindingElement>();
                foreach (FavoriteConfigurationElement elm in this.DiscoveredConnections)
                {
                    list.Add(new BindingElement
                    {
                        Element = string.Format("{0}:{1}", elm.ServerName, elm.Protocol)
                    });
                }

                this.dataGridView1.DataSource = list;
                this.dataGridView1.Visible = true;
                Application.DoEvents();
            }
            catch (Exception exc)
            {
                Log.Info("Connections Count Label", exc);
            }
        }
    }
}