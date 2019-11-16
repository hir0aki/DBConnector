using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector
{
    public class MSSQLServer : IDBConnector, IDisposable
    {
        private IntPtr nativeResource = Marshal.AllocHGlobal(100);
        public string ConnectionString { get; }
        public List<Values> listValuesForQuery { get; }
        public List<Values> listValuesWhere { get; }

        private SqlConnection sqlcn;

        public MSSQLServer(string connectionString)
        {
            ConnectionString = connectionString;
            sqlcn = new SqlConnection(ConnectionString);
            listValuesForQuery = new List<Values>();
            listValuesWhere = new List<Values>();
        }
        public void AddValuesForQuery(string columnName, object value)
        {

            Values valuesQuery = new Values
            {
                Name = columnName,
                Value = value
            };
            listValuesForQuery.Add(valuesQuery);
        }
        public void AddValuesWhere(string columnName, object value)
        {
            Values valuesWhere = new Values
            {
                Name = columnName,
                Value = value
            };
            listValuesWhere.Add(valuesWhere);
        }
        public int ExecuteQuery(string query)
        {
            SqlCommand sqlcmd = new SqlCommand(query, sqlcn);
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            try
            {
                int rowsAff = sqlcmd.ExecuteNonQuery();
                sqlcmd.Dispose();
                sqlcn.Close();
                return rowsAff;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                throw new Exception(ex.Message, ex);
            }
        }
        
        public bool Insert(string tableName)
        {
            if (listValuesForQuery.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueForQuery);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"INSERT INTO {tableName} (");
            StringBuilder valuesStr = new StringBuilder(") VALUES (");
            foreach (var value in listValuesForQuery)
            {
                sqlQuery.Append($"{value.Name},");
                valuesStr.Append($"@{value.Name},");
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            sqlQuery.Replace(",", string.Empty, sqlQuery.Length - 1, 1);
            valuesStr.Replace(",", string.Empty, valuesStr.Length - 1, 1);
            valuesStr.Append(")");
            sqlQuery.Append(valuesStr);
            sqlcmd.CommandText = sqlQuery.ToString();
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            try
            {
                int rowsAff = sqlcmd.ExecuteNonQuery();
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesForQuery.Clear();
                if (rowsAff > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesForQuery.Clear();
                throw new Exception(ex.Message, ex);
            }
        }

        public int Update(string tableName)
        {
            if (listValuesForQuery.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueForQuery);
            }
            if (listValuesWhere.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueWhere);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"UPDATE {tableName} SET ");
            foreach (var value in listValuesForQuery)
            {
                sqlQuery.Append($"{value.Name} = @{value.Name},");
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            sqlQuery.Replace(",", string.Empty, sqlQuery.Length - 1, 1);
            sqlQuery.Append(" WHERE ");
            foreach (var value in listValuesWhere)
            {
                sqlQuery.Append($"{value.Name} = @{value.Name} AND ");
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            sqlcmd.CommandText = sqlQuery.ToString();
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            try
            {
                int rowsAff = sqlcmd.ExecuteNonQuery();
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesForQuery.Clear();
                listValuesWhere.Clear();
                return rowsAff;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesForQuery.Clear();
                listValuesWhere.Clear();
                throw new Exception(ex.Message, ex);
            }
        }

        public int Delete(string tableName)
        {
            if (listValuesWhere.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueWhere);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"DELETE {tableName} WHERE ");
            foreach (var value in listValuesWhere)
            {
                sqlQuery.Append($"{value.Name} = @{value.Name} AND ");
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            sqlcmd.CommandText = sqlQuery.ToString();
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            try
            {
                int rowsAff = sqlcmd.ExecuteNonQuery();
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesWhere.Clear();
                return rowsAff;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesWhere.Clear();
                throw new Exception(ex.Message, ex);
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (sqlcn != null)
                {
                    sqlcn.Dispose();
                    sqlcn = null;
                }
            }
            // free native resources if there are any.
            if (nativeResource != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(nativeResource);
                nativeResource = IntPtr.Zero;
            }
        }
    }
}