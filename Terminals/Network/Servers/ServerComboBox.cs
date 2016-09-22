/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 04.10.2012
 * Time: 13:09
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Terminals.Network.Servers
{
    /// <summary>
    ///     A ComboBox that uses Servers object to populate itself
    ///     with a list of servers.
    /// </summary>
    public class ServerComboBox : ComboBox
    {
        private readonly Servers servers;
        private bool autoRefresh;

        #region Properties

        /// <summary>
        ///     Server Type to search.  Can be one or more.
        /// </summary>
        public ServerType ServerType
        {
            get { return this.servers.Type; }
            set
            {
                this.servers.Type = value;
                if (this.autoRefresh)
                    this.Refresh();
            }
        }

        /// <summary>
        ///     Domain name to search.  Set to <code>null</code> for all.
        /// </summary>
        public string DomainName
        {
            get { return this.servers.DomainName; }
            set
            {
                this.servers.DomainName = value;
                if (this.autoRefresh)
                    this.Refresh();
            }
        }

        /// <summary>
        ///     If true, any changes to DomainName or ServerType will
        ///     cause the combobox to refresh it's data (default = false);
        /// </summary>
        public bool AutoRefresh
        {
            get { return this.autoRefresh; }
            set { this.autoRefresh = value; }
        }

        #endregion

        /// <summary>
        /// </summary>
        public ServerComboBox()
        {
            this.InitializeComponent();
            this.servers = new Servers();
            this.autoRefresh = false;
        }

        private void InitializeComponent()
        {
            this.Name = "ServerComboBox";
            this.Size = new Size(168, 24);
        }

        /// <summary>
        ///     Refreshes the ComboBox's data by enumerating the server list
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();

            this.BeginUpdate();
            this.Items.Clear();
            foreach (String name in this.servers)
            {
                this.Items.Add(name);
            }
            this.EndUpdate();
        }
    }
}