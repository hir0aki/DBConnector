using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector
{
    public interface IDBConnector
    {
        string ConnectionString { get; }
        List<Values> listValuesForQuery { get; }
        List<Values> listValuesWhere { get; }
        int ExecuteQuery(string query);
        bool Insert(string tableName);
        int Update(string tableName);
        int Delete(string tableName);
        void AddValuesForQuery(string columnName, object value);
        void AddValuesWhere(string columnName, object value);
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
}
