using DBConnector.Classes;
using System.Data;


namespace DBConnector
{
    public interface IDBConnector
    {
        //Properties
        ColumnsSelectCollection ColumnsSelect { get; }
        ValuesWhereCollection ValuesWhere { get; }
        ValuesCollection Values { get; }
        ParametersSPCollection ParametersSP { get; }
        QuerysTransactionCollection QuerysTransaction { get; }

        bool Debug { get; set; }
        //Methods
        /// <summary>
        /// Ejecuta cualquier query que se pase como parametro y regresal el número de rows afectados. 
        /// </summary>
        /// <param name="query">Query para ejecutar</param>
        /// <returns></returns>
        int ExecuteQuery(string query);
        /// <summary>
        /// Ejecuta el SP especificado y regresa el número de rows afectados. Para agregar parametros se necesita llamar el método Add de la colección ParametersSP antes de ejecutar esta función.
        /// </summary>
        /// <param name="spName">Nombre del SP</param>
        /// <returns></returns>
        int ExecuteSP(string spName);
        /// <summary>
        /// Ejecuta el SP especificado y regresa los resultados en un DataTable. Para agregar parametros se necesita llamar el método Add de la colección ParametersSP antes de ejecutar esta función.
        /// </summary>
        /// <param name="spName">Nombre del SP</param>
        /// <returns></returns>
        DataTable ExecuteSPToDataTable(string spName);
        /// <summary>
        /// Ejecuta una serie de querys como transacciones y regresa true si todos los querys se ejecutan correctamente. Los querys a ejecutar se agregan mediante el método Add de la colección QuerysTransaction.
        /// </summary>
        /// <returns></returns>
        bool ExecuteSQLTransactions();
        /// <summary>
        /// Ejecuta un SELECT y regresa los resultados en un DataTable. Para especificar los campos a regresar necesita llamar el método Add de la colección ColumnsSelect o pasar el parametro allColumns = true. Para filtrar los datos necesita llamar el método Add de la colección ValuesWhere.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla a regresar</param>
        /// <param name="allColumns">Si se especifica como true, regresa todos los campos de la tabla</param>
        /// <param name="top">Limitar la cantidad de datos a regresar</param>
        /// <param name="groupByColumnName">Nombres de las columnas para agrupar</param>
        /// <param name="orderBy">Método de ordenamiento</param>
        /// <param name="orderByColumnName">Nombre de las columnas para ordenar</param>
        /// <returns></returns>
        DataTable SelectToDataTable(string tableName, bool allColumns = false, int top = 0, string groupByColumnName = "", SQLOrderBy orderBy = SQLOrderBy.None, string orderByColumnName = "", bool distinct = false);
        /// <summary>
        /// Regresa el query especificado en un DataTable.
        /// </summary>
        /// <param name="query">Query para ejecutar</param>
        /// <returns></returns>
        DataTable SelectQueryToDataTable(string query);
        /// <summary>
        /// Regresa el primer valor del resultado de la columna especificada en la tabla especificada como un objeto. Se puede especificar una función SQL para aplicar a la columna. Para filtrar los datos necesita llamar el método AddValuesWhere.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <param name="sqlFunction">Función SQL a aplicar</param>
        /// <returns></returns>
        object SelectSingleValue(string tableName, string columnName, SQLFunction sqlFunction = SQLFunction.None);
        /// <summary>
        /// Regresa el primer valor del resultado del query especificado en un objeto.
        /// </summary>
        /// <param name="query">Query para ejecutar</param>
        /// <returns></returns>
        object SelectQuerySingleValue(string query);
        /// <summary>
        /// Realiza un Insert en la tabla especificada y regresa true si se insertaron los datos. Para especificar los datos a insertar se necesita llamar el método Add de la colección Values.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla para realizar el Insert</param>
        /// <returns></returns>
        bool Insert(string tableName);
        /// <summary>
        /// Realiza un Insert y regresa el valor insertado de la columna especificada (ODBC y MSSQL). En el caso de MySQL no es necesario especificar el parametro identityColumn ya que automaticamente regresa el valor del campo AutoIdentity. Para especificar los datos a insertar se necesita llamar el método AddValuesQuery antes de llamar esta función.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla para realizar el Insert</param>
        /// <param name="identityColumn">Nombre de la columna para regresar el valor insertado (solo ODBC y MSSQL). En el caso de MySql este parametro es ignorado</param>
        /// <returns></returns>
        string InsertWithIdentityReturn(string tableName, string identityColumn);
        /// <summary>
        /// Realiza un Update en la tabla especificada y regresa el número de rows afectados. Para especificar los campos y valores a actualizar se necesita llamar el método Add de la colección Values. Para filtrar los datos a actualizar necesita llamar el método Add de la colección ValuesWhere.
        /// </summary>
        /// <param name="tableName">El nombre de tabla para realizar el Update</param>
        /// <returns></returns>
        int Update(string tableName);
        /// <summary>
        /// Realizar un Delete en la tabla especificada y regresa el número de rows afectados. Para filtrar los datos a eliminar necesita llamar el método Add de la colección ValuesWhere.
        /// </summary>
        /// <param name="tableName">El nombre de la tabla para realizar el Delete</param>
        /// <returns></returns>
        int Delete(string tableName);
        //Secondary Methods
        /// <summary>
        /// Método para agregar los valores que son usado por otros métodos.
        /// </summary>
        /// <param name="columnName">Nombre de la columna</param>
        /// <param name="value">Valor de la columna</param>
        //IDBConnector AddValues(string columnName, object value);
        /// <summary>
        /// Método para agregar los parametros del SP.
        /// </summary>
        /// <param name="parameterName">Nombre del parametro</param>
        /// <param name="value">Valor del parametro</param>
        //IDBConnector AddParameterSP(string parameterName, object value);
        /// <summary>
        /// Método para especificar los datos a filtrar en otros métodos.
        /// </summary>
        /// <param name="columnName">Nombre de la columna</param>
        /// <param name="sqlComparisonOperator">Tipo de comparador</param>
        /// <param name="value">Valor de la columna</param>
        /// <param name="parameterName">Nombre del parametro (Necesario en caso de repetir el nombre de la columna)</param>
        //IDBConnector AddValuesWhere(string columnName, SQLComparisonOperator sqlComparisonOperator, object value, string parameterName = "");
        /// <summary>
        /// Método para agregar los nombres de las columnas para los métodos Select.
        /// </summary>
        /// <param name="columnName">Nombre de la columna</param>
        //IDBConnector AddColumnsSelect(string columnName);
        /// <summary>
        /// Método para agregar los querys que se ejecutarán en el método ExecuteSQLTransactions.
        /// </summary>
        /// <param name="query">Query para ejecutar</param>
        //IDBConnector AddQuerySQLTransaction(string query);
        /// <summary>
        /// Método para hacer Dispose del objeto actual.
        /// </summary>
        void Dispose();
        //Transaction Methods
        /// <summary>
        /// Método para especificar que las siguientes operaciones a la BD seran parte de una transacción.
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// Método para hacer Commit de las últimas operaciones en la BD.
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// Método para hacer Rollback de las últimas operaciones en la BD.
        /// </summary>
        void RollbackTransaction();
    }

    public enum DBConnectionType : int
    {
        SQLServer = 0,
        ODBCSql = 1,
        MySql = 2
    };

    public enum SQLFunction : int
    {
        None = 0,
        MAX = 1,
        MIN = 2,
        COUNT = 3,
        SUM = 4,
        AVG = 5
    }

    public enum SQLOrderBy : int
    {
        None = 0,
        ASC = 1,
        DESC = 2
    }

    public sealed class SQLComparisonOperator
    {
        private readonly string name;
        private readonly int value;

        public static readonly SQLComparisonOperator EqualTo = new SQLComparisonOperator(1, "=");
        public static readonly SQLComparisonOperator GreaterThan = new SQLComparisonOperator(2, ">");
        public static readonly SQLComparisonOperator LessThan = new SQLComparisonOperator(3, "<");
        public static readonly SQLComparisonOperator GreaterThanOrEqualTo = new SQLComparisonOperator(4, ">=");
        public static readonly SQLComparisonOperator LessThanOrEqualTo = new SQLComparisonOperator(5, "<=");
        public static readonly SQLComparisonOperator NotEqualTo = new SQLComparisonOperator(6, "<>");
        public static readonly SQLComparisonOperator Like = new SQLComparisonOperator(7, "LIKE");

        private SQLComparisonOperator(int value, string name)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return name;
        }
    }
}