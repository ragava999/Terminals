using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace Terminals.Network.WMI
{
    public partial class Services : UserControl
    {
        private readonly List<ManagementObject> list = new List<ManagementObject>();

        public Services()
        {
            this.InitializeComponent();

        }

        private void LoadServices(string Username, string Password, string Computer)
        {
            const string qry = "select AcceptPause, AcceptStop, Caption, CheckPoint, CreationClassName, Description, DesktopInteract, DisplayName, ErrorControl, ExitCode, InstallDate, Name, PathName, ProcessId,ServiceSpecificExitCode, ServiceType, Started, StartMode, StartName, State, Status, SystemCreationClassName, SystemName, TagId, WaitHint from win32_service";

            ManagementObjectSearcher searcher;
            ObjectQuery query = new ObjectQuery(qry);

            if (Username != "" && Password != "" && Computer != "" && !Computer.StartsWith(@"\\localhost"))
            {
                ConnectionOptions oConn = new ConnectionOptions { Username = Username, Password = Password };

                if (!Computer.StartsWith(@"\\")) Computer = @"\\" + Computer;

                if (!Computer.ToLower().EndsWith(@"\root\cimv2")) Computer = Computer + @"\root\cimv2";

                ManagementScope oMs = new ManagementScope(Computer, oConn);

                searcher = new ManagementObjectSearcher(oMs, query);
            }
            else
            {
                searcher = new ManagementObjectSearcher(query);
            }

            DataTable dt = new DataTable();

            bool needsSchema = true;
            int length = 0;
            object[] values = null;

            this.list.Clear();

            foreach (ManagementObject share in searcher.Get())
            {
                Share s = new Share();

                this.list.Add(share);

                if (needsSchema)
                {
                    foreach (PropertyData p in share.Properties)
                    {
                        DataColumn col = new DataColumn(p.Name, this.ConvertCimType(p.Type));
                        dt.Columns.Add(col);
                    }

                    length = share.Properties.Count;
                    needsSchema = false;
                }

                if (values == null) values = new object[length];

                int x = 0;

                foreach (PropertyData p in share.Properties)
                {
                    if (p != null && x < length)
                    {
                        values[x] = p.Value;
                        x++;
                    }
                }

                dt.Rows.Add(values);
                values = null;
            }

            this.dataGridView1.DataSource = dt;
        }


        private void Services_Load(object sender, EventArgs e)
        {
            this.LoadServices("", "", "");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.LoadServices(this.wmiServerCredentials1.Username, this.wmiServerCredentials1.Password,
                              this.wmiServerCredentials1.SelectedServer);
        }

        private Type ConvertCimType(CimType ctValue)
        {
            Type tReturnVal = null;

            switch (ctValue)
            {
                case CimType.Boolean:
                    tReturnVal = typeof(Boolean);
                    break;
                case CimType.Char16:
                    tReturnVal = typeof(String);
                    break;
                case CimType.DateTime:
                    tReturnVal = typeof(DateTime);
                    break;
                case CimType.Object:
                    tReturnVal = typeof(Object);
                    break;
                case CimType.Real32:
                    tReturnVal = typeof(Decimal);
                    break;
                case CimType.Real64:
                    tReturnVal = typeof(Decimal);
                    break;
                case CimType.Reference:
                    tReturnVal = typeof(Object);
                    break;
                case CimType.SInt16:
                    tReturnVal = typeof(Int16);
                    break;
                case CimType.SInt32:
                    tReturnVal = typeof(Int32);
                    break;
                case CimType.SInt8:
                    tReturnVal = typeof(Int16);
                    break;
                case CimType.String:
                    tReturnVal = typeof(String);
                    break;
                case CimType.UInt16:
                    tReturnVal = typeof(UInt16);
                    break;
                case CimType.UInt32:
                    tReturnVal = typeof(UInt32);
                    break;
                case CimType.UInt64:
                    tReturnVal = typeof(UInt64);
                    break;
                case CimType.UInt8:
                    tReturnVal = typeof(UInt16);
                    break;
            }
            return tReturnVal;
        }

        private ManagementObject FindWMIObject(string name, string propname)
        {
            return this.list.FirstOrDefault(obj => obj.Properties[propname].Value.ToString() == name);
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name =
                this.dataGridView1.Rows[this.dataGridView1.SelectedCells[0].RowIndex].Cells["Name"].Value.ToString();

            if (name != null && name != "")
            {
                ManagementObject obj = this.FindWMIObject(name, "Name");

                if (obj != null)
                {
                    obj.InvokeMethod("PauseService", null);
                    this.LoadServices(this.wmiServerCredentials1.Username, this.wmiServerCredentials1.Password,
                                      this.wmiServerCredentials1.SelectedServer);
                }
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name =
                this.dataGridView1.Rows[this.dataGridView1.SelectedCells[0].RowIndex].Cells["Name"].Value.ToString();

            if (name != null && name != "")
            {
                ManagementObject obj = this.FindWMIObject(name, "Name");

                if (obj != null)
                {
                    obj.InvokeMethod("StopService", null);
                    this.LoadServices(this.wmiServerCredentials1.Username, this.wmiServerCredentials1.Password,
                                      this.wmiServerCredentials1.SelectedServer);
                }
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name =
                this.dataGridView1.Rows[this.dataGridView1.SelectedCells[0].RowIndex].Cells["Name"].Value.ToString();

            if (name != null && name != "")
            {
                ManagementObject obj = this.FindWMIObject(name, "Name");

                if (obj != null)
                {
                    obj.InvokeMethod("StartService", null);
                    this.LoadServices(this.wmiServerCredentials1.Username, this.wmiServerCredentials1.Password,
                                      this.wmiServerCredentials1.SelectedServer);
                }
            }
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