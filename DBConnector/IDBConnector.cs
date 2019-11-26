using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector
{
    public interface IDBConnector
    {
        //Properties
        string ConnectionString { get; }
        List<Values> listValuesQuery { get; }
        List<Values> listParametersSP { get; }
        List<ValuesWhere> listValuesWhere { get; }
        List<string> listColumnsSelect { get; }
        List<string> listQuerysTransaction { get; }
        //Methods
        int ExecuteQuery(string query);
        int ExecuteSP(string spName);
        bool ExecuteSQLTransactions();
        DataTable SelectToDataTable(string tableName, bool allColumns = false, SQLOrderBy orderBy = SQLOrderBy.None, string orderByColumnName = "");
        DataTable SelectQueryToDataTable(string query);
        object SelectSingleValue(string tableName, string columnName, SQLFunction sqlFunction = SQLFunction.None);
        object SelectQuerySingleValue(string query);
        bool Insert(string tableName);
        string InsertWithIdentityReturn(string tableName, string identityColumn);
        int Update(string tableName);
        int Delete(string tableName);
        //Secondary Methods
        void AddValuesQuery(string columnName, object value);
        void AddParameterSP(string parameterName, object value);
        void AddValuesWhere(string columnName, SQLComparisonOperator sqlComparisonOperator, object value, string parameterName = "");
        void AddColumnsSelect(string columnName);
        void AddQuerySQLTransaction(string query);
        void Dispose();
    }

    public class Values
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class ValuesWhere
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SQLComparisonOperator SQLComparisonOperator { get; set;}
        public string ParameterName { get; set; }
    }

    public enum DBConnectionType : int
    {
        SQLServer = 1,
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
