using DBConnector;
using System;
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
            DBConnectionType dBConnectionType = DBConnectionType.ODBCSql;

            switch (dBConnectionType)
            {
                case DBConnectionType.SQLServer:
                    connector = new MSSQLServerConnector("Server=127.0.0.1;Database=PS2;User Id=ps2;Password=;");
                    break;
                case DBConnectionType.ODBCSql:
                    connector = new OdbcConnector("Dsn=PS2;uid=ps2");
                    break;
                case DBConnectionType.MySql:
                    connector = new MySqlConnector("Server=127.0.0.1;Database=ps3;Uid=ps3;Pwd=Seccionr3d;");
                    break;
                default:
                    break;
            }

            //connector.ColumnsSelect.Add("Nombre").Add("Apellido").Add("Edad").Add("FechaNac");

            //connector.ValuesWhere.Add("Servidor", SQLComparisonOperator.Like, "127.0.0.1").Add("MAQUINA1", SQLComparisonOperator.EqualTo, "localhost");

            connector.Debug = true;

            //connector.AddColumnsSelect("Nombre").AddColumnsSelect("Apellido").AddColumnsSelect("Edad").AddColumnsSelect("FechaNac");

            //connector.AddValuesWhere("Servidor", SQLComparisonOperator.EqualTo, "127.0.0.1").AddValuesWhere("MAQUINA1", SQLComparisonOperator.EqualTo, "localhost");

            //int rowsAff = connector.ExecuteQuery("DELETE BaseDeDatos");

            //connector.AddValuesQuery("Servidor", "SERVER");
            //connector.AddValuesQuery("Usuario", "User");
            //connector.AddValuesQuery("Password", "PAss");
            //connector.AddValuesQuery("BaseDeDatos", "DB");
            //string insertedId = connector.InsertWithIdentityReturn("BaseDeDatos", "Servidor");

            //connector.AddValuesQuery("Servidor", "127.0.0.1");
            //connector.AddValuesQuery("Usuario", "ps2");
            //connector.AddValuesQuery("Password", "****");
            //connector.AddValuesQuery("BaseDeDatos", "DB");
            //bool insert2 = connector.Insert("BaseDeDatos");

            //connector.AddValuesQuery("Servidor", "127.0.0.1");
            //connector.AddValuesQuery("Usuario", "Usuario");
            //connector.AddValuesWhere("BaseDeDatos", SQLComparisonOperator.EqualTo, "DB");
            //int rows = connector.Update("BaseDeDatos");

            //connector.AddValuesWhere("Servidor", SQLComparisonOperator.EqualTo, "127.0.0.1", );
            //int rowsDelete = connector.Delete("BaseDeDatos");

            //connector.AddColumnsSelect("Servidor");
            //connector.AddColumnsSelect("Usuario");
            //connector.AddValuesWhere("Servidor", SQLComparisonOperator.EqualTo, "127.0.0.1");
            //DataTable table1 = connector.SelectToDataTable("BaseDeDatos", orderBy: SQLOrderBy.ASC, orderByColumnName: "Servidor");

            //connector.AddValuesWhere("Servidor", SQLComparisonOperator.Like, "%S%");
            //DataTable tableLike = connector.SelectToDataTable("BaseDeDatos", true);

            //connector.AddValuesWhere("FECHACARGA", SQLComparisonOperator.GreaterThanOrEqualTo, new DateTime(2019, 09, 01));
            //connector.AddValuesWhere("FECHACARGA", SQLComparisonOperator.LessThanOrEqualTo, DateTime.Now, "fechaFin");
            //DataTable tableVentas = connector.SelectToDataTable("ConsumoGeneral", true, orderBy: SQLOrderBy.DESC, orderByColumnName: "FECHACARGA");

            //DataTable table2 = connector.SelectToDataTable("BaseDeDatos", true);

            //DataTable table4 = connector.SelectToDataTable("BaseDeDatos", true, 10);

            //DataTable table3 = connector.SelectQueryToDataTable("SELECT * FROM BaseDeDatos ORDER BY Servidor ASC");

            //connector.AddValuesWhere("SECUENCIA", SQLComparisonOperator.EqualTo, 789044);
            //string bomba = connector.SelectSingleValue("ConsumoGeneral", "BOMBA", SQLFunction.None).ToString();

            //string maxSecuencia = connector.SelectSingleValue("ConsumoGeneral", "SECUENCIA", SQLFunction.MAX).ToString();

            //connector.AddValuesWhere("DESCRIPCION", SQLComparisonOperator.EqualTo, "MAGNA 32011");
            //string count = connector.SelectSingleValue("ConsumoGeneral", "DESCRIPCION", SQLFunction.COUNT).ToString();

            //string fechaMAX = connector.SelectSingleValue("ConsumoGeneral", "FECHACARGA", SQLFunction.MAX).ToString();

            //string fechaMIN = connector.SelectSingleValue("ConsumoGeneral", "FECHACARGA", SQLFunction.MIN).ToString();

            //connector.AddParameterSP("fechaInicio", DateTime.Now);
            //connector.AddParameterSP("fechaFinal", DateTime.Now);
            //connector.AddParameterSP("turno", 1);
            //int rowsAff = connector.ExecuteSP("sp_CorteHistorico");

            //connector.AddQuerySQLTransaction("INSERT INTO BaseDeDatos VALUES ('MAQUINA1', 'User', 'Pass', 'PS2')");
            //connector.AddQuerySQLTransaction("INSERT INTO BaseDeDatos VALUES (MAQUINA2, 'User2', 'Pass2', 'PS3')");
            //connector.AddQuerySQLTransaction("UPDATE BaseDeDatos SET Servidor = 'MAQUINA3' WHERE Servidor = 'MAQUINA2'");
            //connector.AddQuerySQLTransaction("DELETE BaseDeDatos WHERE Servidor = 'MAQUINA1'");
            //bool transactions = connector.ExecuteSQLTransactions();

            try
            {
                //connector.BeginTransaction();

                //connector.AddValuesQuery("Servidor", "MAQUINA1");
                //connector.AddValuesQuery("Usuario", "User");
                //connector.AddValuesQuery("Password", "Pass");
                //connector.AddValuesQuery("BaseDeDatos", "PS3");
                //connector.Insert("BaseDeDatos");

                //connector.AddValuesQuery("Servidor", "MAQUINA2");
                //connector.AddValuesQuery("Usuario", "User2");
                //connector.AddValuesQuery("Password", "Pass2");
                //connector.AddValuesQuery("BaseDeDatos", "PS3");
                //connector.Insert("BaseDeDatos");

                //connector.AddValuesQuery("Servidor", "MAQUINA3");
                //connector.AddValuesWhere("Servidor", SQLComparisonOperator.EqualTo, "MAQUINA2", "nServidor");
                //connector.Update("BaseDeDatos");

                //connector.AddValuesWhere("Servidor", SQLComparisonOperator.EqualTo, "MAQUINA1");
                connector.Delete("BaseDeDatos", true);

                //connector.CommitTransaction();
            }
            catch (Exception ex)
            {
                //connector.RollbackTransaction();
                MessageBox.Show(ex.Message);
            }

            connector.Dispose();
        }

    }
}