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
        List<Values> listValuesWhere { get; }
        List<string> listColumnsSelect { get; }
        //Methods
        int ExecuteQuery(string query);
        int ExecuteSP(string spName);
        DataTable SelectToDataTable(string tableName, bool allColumns = false);
        DataTable SelectQueryToDataTable(string query);
        object SelectSingleValue(string tableName, string columnName, SQLFunction sqlFunction = SQLFunction.None);
        object SelectQuerySingleValue(string query);
        bool Insert(string tableName);
        string InsertWithIdentityReturn(string tableName, string identityColumn);
        int Update(string tableName);
        int Delete(string tableName);
        void AddValuesQuery(string columnName, object value);
        void AddParameterSP(string parameterName, object value);
        void AddValuesWhere(string columnName, object value);
        void AddColumnsSelect(string columnName);
        void Dispose();
    }

    public class Values
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public enum DBType : int
    {
        SQLServer = 1,
        MySql = 2
    };

    public enum SQLFunction : int
    {
        None = 0,
        MAX = 1,
        COUNT = 2
    }
}
