using Kohl.Framework.Logging;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Terminals.Properties;

namespace Terminals.Network.WMI
{
    /// <summary>
    ///     Summary description for Form1.
    /// </summary>
    public partial class WmiControl : UserControl
    {
        private bool stilRunning = true;
        private ArrayList history = new ArrayList();

        public WmiControl()
        {
            this.InitializeComponent();
            this.Form1_Resize(null, null);

        }

        private string Username { get; set; }
        private string Password { get; set; }
        private string Computer { get; set; }

        private string HistoryPath
        {
            get { return Application.StartupPath + "\\WMITestClientHistory.txt"; }
        }

        public static DataTable WMIToDataTable(string query, string computer, string username, string password)
        {
            string qry = query;

            ManagementObjectSearcher searcher;
            ObjectQuery queryObj = new ObjectQuery(qry);

            if (username != string.Empty && password != string.Empty && computer != string.Empty &&
                !computer.StartsWith(@"\\localhost"))
            {
                ConnectionOptions oConn = new ConnectionOptions { Username = username, Password = password };

                if (!computer.StartsWith(@"\\"))
                    computer = @"\\" + computer;

                if (!computer.ToLower().EndsWith(@"\root\cimv2"))
                    computer += @"\root\cimv2";

                ManagementScope oMs = new ManagementScope(computer, oConn);

                searcher = new ManagementObjectSearcher(oMs, queryObj);
            }
            else
            {
                searcher = new ManagementObjectSearcher(queryObj);
            }

            DataTable dt = new DataTable();

            bool needsSchema = true;
            int length = 0;
            object[] values = null;

            foreach (ManagementObject share in searcher.Get())
            {
                if (needsSchema)
                {
                    foreach (PropertyData p in share.Properties)
                    {
                        DataColumn col = new DataColumn(p.Name, ConvertCimType(p.Type));
                        dt.Columns.Add(col);
                    }

                    length = share.Properties.Count;
                    needsSchema = false;
                }

                if (values == null)
                    values = new object[length];

                int x = 0;

                foreach (PropertyData p in share.Properties)
                {
                    if (p.Type == CimType.DateTime)
                    {
                        values[x] = ManagementDateTimeConverter.ToDateTime(p.Value.ToString());
                    }
                    else
                    {
                        values[x] = p.Value;
                    }

                    x++;
                }

                dt.Rows.Add(values);
                values = null;
            }

            return dt;
        }

        private static Type ConvertCimType(CimType ctValue)
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

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.progressBar1.Width = this.Width - 10;
        }

        private void IncrementBar()
        {
            if (this.progressBar1.Value >= this.progressBar1.Maximum)
                this.progressBar1.Value = 0;

            this.progressBar1.Value++;
        }

        private void AddToHistory()
        {
            if (!this.history.Contains(this.QueryTextBox.Text))
            {
                this.QueryTextBox.Items.Add(this.QueryTextBox.Text);
                this.history.Add(this.QueryTextBox.Text);
            }
        }

        private void SaveHistory()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string historyItem in this.history)
            {
                if (historyItem != "")
                    sb.Append(historyItem + "\r\n");
            }

            StreamWriter sw = new StreamWriter(this.HistoryPath, false);

            sw.Write(sb.ToString());

            sw.Close();
        }

        private void LoadHistory(string path)
        {
            string realPath = path;

            if (string.IsNullOrEmpty(realPath))
                realPath = this.HistoryPath;

            if (File.Exists(realPath))
            {
                StreamReader sr = new StreamReader(realPath);

                string contents = sr.ReadToEnd();

                sr.Close();

                string[] items = Regex.Split(contents, "\r\n");

                foreach (string item in items)
                {
                    if (item != string.Empty)
                    {
                        this.history.Add(item);
                        this.QueryTextBox.Items.Add(item);
                    }
                }
            }
        }

        private void QueryButton_Click(object sender, EventArgs e)
        {
            this.AddToHistory();

            this.stilRunning = true;
            this.QueryButton.Enabled = false;
            this.StopButton.Enabled = true;

            try
            {
                this.treeView1.Nodes.Clear();

                if (this.QueryTextBox.Text != string.Empty)
                {
                    string qry = this.QueryTextBox.Text;
                    ManagementObjectSearcher searcher;
                    ObjectQuery query = new ObjectQuery(qry);

                    if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password) &&
                        !string.IsNullOrEmpty(this.Computer) && !this.Computer.StartsWith(@"\\localhost"))
                    {
                        ConnectionOptions oConn = new ConnectionOptions
                        {
                            Username = this.Username,
                            Password = this.Password
                        };

                        ManagementScope oMs = new ManagementScope(this.Computer, oConn);

                        searcher = new ManagementObjectSearcher(oMs, query);
                    }
                    else
                    {
                        searcher = new ManagementObjectSearcher(query);
                    }

                    TreeNode root = new TreeNode(qry) { Tag = "RootNode" };

                    root.Expand();
                    this.treeView1.Nodes.Add(root);

                    foreach (ManagementObject share in searcher.Get())
                    {
                        TreeNode item = new TreeNode(share.ClassPath.ClassName) { Tag = "ClassNode" };

                        root.Nodes.Add(item);

                        foreach (PropertyData p in share.Properties)
                        {
                            bool isLocal = p.IsLocal;
                            string type = p.Type.ToString();
                            string origin = p.Origin;
                            string name = p.Name;

                            Application.DoEvents();

                            bool IsArray = p.IsArray;

                            string val = "NULL";

                            if (p.Value != null)
                                val = p.Value.ToString();

                            TreeNode node = new TreeNode(name) { Tag = "PropertyNode" };

                            string display = "";

                            if (type.ToLower() == "string")
                            {
                                display = "Value='" + val + "'";
                            }
                            else
                            {
                                display = "Value=" + val;
                            }

                            TreeNode ValueNode = new TreeNode(display) { Tag = "ValueNode" };
                            TreeNode TypeNode = new TreeNode("Type='" + type + "'") { Tag = "ValueNode" };
                            TreeNode localNode = new TreeNode("IsLocal=" + isLocal) { Tag = "ValueNode" };
                            TreeNode OriginNode = new TreeNode("Origin='" + origin + "'") { Tag = "ValueNode" };
                            TreeNode IsArrayNode = new TreeNode("IsArray=" + IsArray) { Tag = "ValueNode" };

                            node.Nodes.Add(ValueNode);
                            node.Nodes.Add(TypeNode);
                            node.Nodes.Add(localNode);
                            node.Nodes.Add(OriginNode);
                            node.Nodes.Add(IsArrayNode);

                            if (IsArray && p.Value != null)
                            {
                                Array a = (Array)p.Value;

                                for (int x = 0; x < a.Length; x++)
                                {
                                    string v = "";
                                    if (a.GetValue(x) != null)
                                        v = a.GetValue(x).ToString();

                                    TreeNode arrayNode = new TreeNode(name + "[" + x + "]=" + v) { Tag = "ArrayNode" };

                                    IsArrayNode.Nodes.Add(arrayNode);

                                    this.IncrementBar();
                                }
                            }

                            this.IncrementBar();

                            item.Nodes.Add(node);
                            Application.DoEvents();

                            if (!this.stilRunning)
                                break;
                        }

                        if (!this.stilRunning)
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Info("Query Button Failed", exc);
                MessageBox.Show("Error Thrown:" + exc.Message);
            }

            this.progressBar1.Value = 0;
            this.QueryButton.Enabled = true;
            this.StopButton.Enabled = false;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (sender != null)
            {
                string queryString = string.Empty;
                TreeNode n = ((TreeView)sender).SelectedNode;

                if (n != null)
                {
                    string nodeType = string.Empty;

                    if (n.Tag != null) nodeType = n.Tag.ToString();

                    string ntext = n.Text;

                    if (ntext.IndexOf("(") > 0) ntext = ntext.Substring(0, ntext.IndexOf("(") - 1).Trim();

                    switch (nodeType)
                    {
                        case "ClassNode":
                            queryString = "select * from " + ntext;
                            break;

                        case "PropertyNode":
                            TreeNode p = n.Parent;
                            string pnPText = p.Text;

                            if (pnPText.IndexOf("(") > 0)
                                pnPText = pnPText.Substring(0, pnPText.IndexOf("(") - 1).Trim();

                            queryString = "select " + ntext + " from " + pnPText;
                            break;

                        case "ValueNode":
                            TreeNode p1 = n.Parent;
                            TreeNode pp = p1.Parent;

                            string ppText = pp.Text;
                            string p1Text = p1.Text;

                            if (ppText.IndexOf("(") > 0)
                                ppText = ppText.Substring(0, ppText.IndexOf("(") - 1).Trim();

                            if (p1Text.IndexOf("(") > 0)
                                p1Text = p1Text.Substring(0, p1Text.IndexOf("(") - 1).Trim();

                            if (ntext.Substring(0, 5).ToLower() == "value")
                            {
                                queryString = "select * from " + ppText + " where " + p1Text + "=" +
                                              ntext.Replace("Value=", string.Empty);
                            }
                            else
                            {
                                queryString = "select " + p1Text + " from " + ppText; // + " where " + n.Text;
                            }

                            break;

                        case "RootNode":
                            if (this.treeView1.Nodes == null || this.treeView1.Nodes.Count == 0)
                                this.treeView1.Nodes.Add("Query");

                            TreeNode root = this.treeView1.Nodes[0];
                            queryString = root.Text;
                            break;

                        default:
                            if (nodeType != null && nodeType != string.Empty)
                                MessageBox.Show(nodeType);

                            break;
                    }

                    this.QueryTextBox.Text = queryString;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.stilRunning = false;
            this.QueryButton.Enabled = true;
            this.StopButton.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.QueryTextBox.Items.Clear();
            this.QueryTextBox.Text = "select * from CIM_System";
            this.LoadHistory(null);
            this.BasicTreemenuItem_Click(null, null);
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            this.SaveHistory();
        }

        private void ExitmenuItem_Click(object sender, EventArgs e)
        {
            this.SaveHistory();
            Application.Exit();
        }

        private void SavemenuItem_Click(object sender, EventArgs e)
        {
            this.SaveHistory();
            MessageBox.Show("History Saved.");
        }

        private void ClearmenuItem_Click(object sender, EventArgs e)
        {
            this.QueryTextBox.Items.Clear();
            this.history = new ArrayList();
            File.Delete(this.HistoryPath);
            MessageBox.Show("History Cleared.");
        }

        private void LoadmenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.CheckFileExists = true;
            this.openFileDialog1.CheckPathExists = true;
            this.openFileDialog1.DefaultExt = "*.txt";
            this.openFileDialog1.InitialDirectory = Application.StartupPath;
            this.openFileDialog1.Multiselect = false;
            this.openFileDialog1.Title = "Locate History File...";
            DialogResult result = this.openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.LoadHistory(this.openFileDialog1.FileName);
            }
        }

        private void LoginMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm frm = new LoginForm
            {
                UserName = this.Username,
                Password = this.Password,
                MachineName = this.Computer
            };

            frm.ShowDialog(this);
            if (!frm.Cancelled)
            {
                this.Username = frm.UserName;
                this.Password = frm.Password;
                this.Computer = frm.MachineName;
            }
        }

        private void LoadNode(XmlNode n, TreeNode tn)
        {
            if (n != null)
            {
                string nAlt = null;
                string nType = null;

                try
                {
                    string nText = n.Attributes["Text"].Value;
                    this.IncrementBar();
                    Application.DoEvents();

                    try
                    {
                        if (n.Attributes["Alt"] != null)
                            nAlt = n.Attributes["Alt"].Value;
                    }
                    catch (Exception exc)
                    {
                        Log.Error("Alt Attribute", exc);
                        nAlt = nText;
                    }

                    try
                    {
                        if (n.Attributes["Type"] != null)
                            nType = n.Attributes["Type"].Value;
                    }
                    catch (Exception exc)
                    {
                        Log.Error("Type Attributes", exc);
                        nType = nAlt;
                    }

                    if (string.IsNullOrEmpty(nAlt))
                    {
                        tn.Text = nText;
                    }
                    else
                    {
                        tn.Text = nText + " (" + nAlt + ")";
                    }

                    tn.Tag = nType;

                    if (n.HasChildNodes)
                    {
                        foreach (XmlNode newXmlNode in n.ChildNodes)
                        {
                            TreeNode child = new TreeNode();
                            this.LoadNode(newXmlNode, child);
                            tn.Nodes.Add(child);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.Error("Load Node Failed", ee);
                }
            }
        }

        private void BasicTreemenuItem_Click(object sender, EventArgs e)
        {
            this.LoadBasicTree(Resources.BasicTree);
        }

        private void LoadBasicTree(string xml)
        {
            this.progressBar1.Value = 0;

            this.IncrementBar();
            this.treeView2.Nodes.Clear();
            Application.DoEvents();
            this.IncrementBar();
            Application.DoEvents();

            this.IncrementBar();

            Application.DoEvents();
            XmlDocument x = new XmlDocument();

            try
            {
                x.LoadXml(xml);
            }
            catch (Exception xexc)
            {
                Log.Error("Load Basic Tree Failed", xexc);
            }

            XmlNode n = x.SelectSingleNode("/tree");
            TreeNode root = new TreeNode();
            this.treeView2.Nodes.Add(root);
            Application.DoEvents();

            this.LoadNode(n, root);

            root.Expand();

            if (root.Nodes != null && root.Nodes.Count > 0)
                root.Nodes[0].Expand();

            this.progressBar1.Value = 0;
        }

        private void treeView2_DoubleClick(object sender, EventArgs e)
        {
            if (this.treeView2.Nodes[0].Text == "To load-> Double Click")
            {
                this.BasicTreemenuItem_Click(null, null);
            }
            else
            {
                this.treeView1.Nodes.Clear();
                Application.DoEvents();
                this.treeView1_DoubleClick(this.treeView2, null);
                Application.DoEvents();
                this.QueryButton_Click(null, null);
                Application.DoEvents();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
                        if(tabControl1.SelectedTab.Name==processesTab.Name) {
                System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcesses();
                treeView3.Nodes.Clear();
                foreach(System.Diagnostics.Process p in procs) {
                    treeView3.Nodes.Add(new System.Windows.Forms.TreeNode(p.ProcessName));
                    Application.DoEvents();          
                }
            }
        */
        }

        private void treeView2_Click(object sender, EventArgs e)
        {
            //System.Windows.Forms.TreeNode rootNode = treeView2.SelectedNode;
            //if (rootNode != null)
            //{
            //    if (rootNode.Nodes == null || rootNode.Nodes.Count <= 0)
            //    {
            //        if (rootNode.Tag != null && Convert.ToString(rootNode.Tag) == "ClassNode")
            //        {
            //            string ntext = rootNode.Text;
            //            if (ntext.IndexOf("(") > 0) ntext = ntext.Substring(0, ntext.IndexOf("(") - 1).Trim();
            //            string qry = "select * from " + ntext;
            //            Application.DoEvents();
            //            System.Management.ManagementObjectSearcher searcher;
            //            System.Management.ObjectQuery query = new System.Management.ObjectQuery(qry);

            //            if (Username != "" && Password != "" && Computer != "")
            //            {
            //                System.Management.ConnectionOptions oConn = new System.Management.ConnectionOptions();
            //                oConn.Username = Username;
            //                oConn.Password = Password;
            //                System.Management.ManagementScope oMs = new System.Management.ManagementScope(Computer, oConn);
            //                searcher = new System.Management.ManagementObjectSearcher(oMs, query);
            //            }
            //            else
            //            {
            //                searcher = new System.Management.ManagementObjectSearcher(query);
            //            }
            //            System.Windows.Forms.TreeNode root = new TreeNode(qry);
            //            root.Tag = "RootNode";
            //            rootNode.Nodes.Add(root);
            //            root.Expand();
            //            rootNode.Expand();
            //            string path = "";
            //            Application.DoEvents();

            //            foreach (System.Management.ManagementObject share in searcher.Get())
            //            {
            //                System.Windows.Forms.TreeNode item = new TreeNode(share.ClassPath.ClassName);
            //                item.Tag = "ClassNode";
            //                root.Nodes.Add(item);
            //                path = "Enumerating:" + share.ClassPath.ClassName;
            //                foreach (System.Management.PropertyData p in share.Properties)
            //                {
            //                    bool isLocal = p.IsLocal;
            //                    string type = p.Type.ToString();
            //                    string origin = p.Origin;
            //                    string name = p.Name;
            //                    path = "Enumerating:" + share.ClassPath.ClassName + "," + name;
            //                    Application.DoEvents();

            //                    bool IsArray = p.IsArray;
            //                    string val = "NULL";
            //                    if (p.Value != null) val = p.Value.ToString();
            //                    System.Windows.Forms.TreeNode node = new TreeNode(name);
            //                    node.Tag = "PropertyNode";
            //                    string display = "";
            //                    if (type.ToLower() == "string")
            //                    {
            //                        display = "Value='" + val + "'";
            //                    }
            //                    else
            //                    {
            //                        display = "Value=" + val;
            //                    }
            //                    System.Windows.Forms.TreeNode ValueNode = new TreeNode(display);
            //                    ValueNode.Tag = "ValueNode";
            //                    System.Windows.Forms.TreeNode TypeNode = new TreeNode("Type='" + type + "'");
            //                    TypeNode.Tag = "ValueNode";
            //                    System.Windows.Forms.TreeNode localNode = new TreeNode("IsLocal=" + isLocal);
            //                    localNode.Tag = "ValueNode";
            //                    System.Windows.Forms.TreeNode OriginNode = new TreeNode("Origin='" + origin + "'");
            //                    OriginNode.Tag = "ValueNode";
            //                    System.Windows.Forms.TreeNode IsArrayNode = new TreeNode("IsArray=" + IsArray);
            //                    IsArrayNode.Tag = "ValueNode";

            //                    node.Nodes.Add(ValueNode);
            //                    node.Nodes.Add(TypeNode);
            //                    node.Nodes.Add(localNode);
            //                    node.Nodes.Add(OriginNode);
            //                    node.Nodes.Add(IsArrayNode);
            //                    Application.DoEvents();
            //                    if (IsArray && p.Value != null)
            //                    {
            //                        System.Array a = (System.Array)p.Value;
            //                        for (int x = 0; x < a.Length; x++)
            //                        {
            //                            string v = "";
            //                            if (a.GetValue(x) != null) v = a.GetValue(x).ToString();
            //                            System.Windows.Forms.TreeNode arrayNode = new TreeNode(name + "[" + x + "]=" + v);
            //                            arrayNode.Tag = "ArrayNode";
            //                            IsArrayNode.Nodes.Add(arrayNode);
            //                            IncrementBar();
            //                        }
            //                    }
            //                    IncrementBar();

            //                    item.Nodes.Add(node);
            //                    Application.DoEvents();
            //                    if (!this.StilRunning) break;

            //                }
            //                if (!this.StilRunning) break;
            //            }
            //        }
            //    }
            //}
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            this.LoginMenuItem_Click(null, null);
            this.ConnectionLabel.Text = this.Computer;
        }

        private void QueryTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.QueryButton_Click(null, null);
            }
        }
    }
}