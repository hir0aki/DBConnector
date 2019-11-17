using DBConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester
{
    public partial class Form1 : Form
    {
        IDBConnector connector;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connector = new MSSQLServer("Server=127.0.0.1;Database=PS2;User Id=ps2;Password=;");

            //int rowsAff = connector.ExecuteQuery("DELETE BaseDeDatos");

            //connector.AddValuesQuery("Servidor", "SERVER");
            //connector.AddValuesQuery("Usuario", "User");
            //connector.AddValuesQuery("Password", "PAss");
            //connector.AddValuesQuery("BaseDeDatos", "DB");
            //bool insert = connector.Insert("BaseDeDatos");

            //connector.AddValuesQuery("Servidor", "127.0.0.1");
            //connector.AddValuesQuery("Usuario", "ps2");
            //connector.AddValuesQuery("Password", "****");
            //connector.AddValuesQuery("BaseDeDatos", "DB");
            //bool insert2 = connector.Insert("BaseDeDatos");

            //connector.AddValuesQuery("Servidor", "127.0.0.1");
            //connector.AddValuesQuery("Usuario", "Usuario");
            //connector.AddValuesWhere("BaseDeDatos", "DB");
            //int rows = connector.Update("BaseDeDatos");

            //connector.AddValuesWhere("Servidor", "127.0.0.1");
            //int rowsDelete = connector.Delete("BaseDeDatos");

            //connector.AddColumnsSelect("Servidor");
            //connector.AddColumnsSelect("Usuario");
            //connector.AddValuesWhere("Servidor", "127.0.0.1");
            //DataTable table1 = connector.SelectToDataTable("BaseDeDatos");

            //DataTable table2 = connector.SelectToDataTable("BaseDeDatos", true);

            //DataTable table3 = connector.SelectQueryToDataTable("SELECT * FROM BaseDeDatos ORDER BY Servidor ASC");

            connector.AddValuesWhere("SECUENCIA", 789044);
            string bomba = connector.SelectSingleValue("ConsumoGeneral", "BOMBA", SQLFunction.None).ToString();

            string maxSecuencia = connector.SelectSingleValue("ConsumoGeneral", "SECUENCIA", SQLFunction.MAX).ToString();

            connector.AddValuesWhere("DESCRIPCION", "MAGNA 32011");
            string count = connector.SelectSingleValue("ConsumoGeneral", "DESCRIPCION", SQLFunction.COUNT).ToString();

            connector.Dispose();
        }
    }
}
