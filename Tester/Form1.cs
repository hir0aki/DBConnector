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
            connector.AddValuesForQuery("Servidor", "SERVER");
            connector.AddValuesForQuery("Usuario", "User");
            connector.AddValuesForQuery("Password", "PAss");
            connector.AddValuesForQuery("BaseDeDatos", "DB");
            bool insert = connector.Insert("BaseDeDatos");

            connector.AddValuesForQuery("Servidor", "127.0.0.1");
            connector.AddValuesForQuery("Usuario", "Usuario");
            connector.AddValuesWhere("BaseDeDatos", "DB");
            int rows = connector.Update("BaseDeDatos");

            connector.AddValuesWhere("Servidor", "127.0.0.1");
            int rowsDelete = connector.Delete("BaseDeDatos");
            connector.Dispose();
        }
    }
}
