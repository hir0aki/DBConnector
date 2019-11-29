using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBConnector
{
    public partial class frmDebug : Form
    {
        public frmDebug(object commandObj)
        {
            InitializeComponent();

            try
            {
                if (commandObj is SqlCommand)
                {
                    SqlCommand sqlcmd = commandObj as SqlCommand;
                    txtQuery.Text = sqlcmd.CommandText;

                    foreach (SqlParameter parameter in sqlcmd.Parameters)
                    {
                        dataGridViewParameters.Rows.Add(parameter.ParameterName, parameter.Value.ToString());
                    }
                }
                else if (commandObj is OdbcCommand)
                {
                    OdbcCommand sqlcmd = commandObj as OdbcCommand;
                    txtQuery.Text = sqlcmd.CommandText;

                    foreach (OdbcParameter parameter in sqlcmd.Parameters)
                    {
                        dataGridViewParameters.Rows.Add(parameter.ParameterName, parameter.Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}