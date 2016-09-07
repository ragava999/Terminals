using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Terminals.Forms
{
    public partial class SelectDatabase : Form
    {
        public SelectDatabase()
        {
            InitializeComponent();
        }

        public string ConnectionString
        {
            get
            {
                return string.Format(
                    "Data Source={0}; Initial Catalog={1}; Integrated Security=SSPI",
                    txtServerName.Text ?? "",
                    cmbDatabase.Text) ?? "";
            }
        }

        public string MappingQuery
        {
            get
            {
                return txtMappingQuery.Text;
            }
        }

        private void btnDatabase_Click(object sender, EventArgs e)
        {
            if (!TryConnect())
                return;

            this.cmbDatabase.Items.Clear();

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb');", connection);

                SqlDataReader reader = command.ExecuteReader();

                while (!reader.Read())
                {
                    cmbDatabase.Items.Add(reader["name"]);
                }
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            TryConnect(true);
        }

        private bool TryConnect(bool showInfo = false)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        if (showInfo)
                            MessageBox.Show("Connection to the database server has been created successfully.", "Connection status", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return true;
                    }
                    else
                    {
                        if (showInfo)
                            MessageBox.Show("Error opening a connection to the database.", "Connection status", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (showInfo)
                    MessageBox.Show("Error opening a connection to the database: " + ex.Message, "Connection status", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }
    }
}
