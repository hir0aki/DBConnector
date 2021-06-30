using MySql.Data.MySqlClient;
using System;
using System.Data.Odbc;
using System.Data.SqlClient;
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
                else if (commandObj is MySqlCommand)
                {
                    MySqlCommand sqlcmd = commandObj as MySqlCommand;
                    txtQuery.Text = sqlcmd.CommandText;

                    foreach (MySqlParameter parameter in sqlcmd.Parameters)
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