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
        public string ConnectionString { get; }
        public List<Values> listValuesQuery { get; }
        public List<Values> listValuesWhere { get; }
        public List<string> listColumnsSelect { get; }

        private IntPtr nativeResource = Marshal.AllocHGlobal(100);
        private SqlConnection sqlcn;
        private DataTable dataTable;


        public MSSQLServer(string connectionString)
        {
            ConnectionString = connectionString;
            sqlcn = new SqlConnection(ConnectionString);
            listValuesQuery = new List<Values>();
            listValuesWhere = new List<Values>();
            listColumnsSelect = new List<string>();
        }
        public void AddValuesQuery(string columnName, object value)
        {
            if (string.IsNullOrEmpty(columnName?.Trim()) || value == null)
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            Values valuesQuery = new Values
            {
                Name = columnName,
                Value = value
            };
            listValuesQuery.Add(valuesQuery);
        }
        public void AddValuesWhere(string columnName, object value)
        {
            if (string.IsNullOrEmpty(columnName?.Trim()) || value == null)
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            Values valuesWhere = new Values
            {
                Name = columnName,
                Value = value
            };
            listValuesWhere.Add(valuesWhere);
        }
        public void AddColumnsSelect(string columnName)
        {
            if (string.IsNullOrEmpty(columnName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            listColumnsSelect.Add(columnName);
        }

        public int ExecuteQuery(string query)
        {
            if (string.IsNullOrEmpty(query?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
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
        public DataTable SelectToDataTable(string tableName, bool allColumns = false)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (!allColumns)
            {
                if (listColumnsSelect.Count == 0)
                {
                    throw new Exception(Properties.Resources.exceptionAddColumnsSelect);
                }
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder("SELECT ");
            if (allColumns)
            {
                sqlQuery.Append($"* FROM {tableName}");
            }
            else
            {
                foreach (string column in listColumnsSelect)
                {
                    sqlQuery.Append($"{column},");
                }
                sqlQuery.Replace(",", string.Empty, sqlQuery.Length - 1, 1);
                sqlQuery.Append($" FROM {tableName}");
            }
            if (listValuesWhere.Count > 0)
            {
                sqlQuery.Append(" WHERE ");
                foreach (var value in listValuesWhere)
                {
                    sqlQuery.Append($"{value.Name} = @{value.Name} AND ");
                    sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
                }
                sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            }
            sqlcmd.CommandText = sqlQuery.ToString();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlcmd);
            dataTable = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dataTable);
                sqlcmd.Dispose();
                sqlDataAdapter.Dispose();
                listColumnsSelect.Clear();
                listValuesWhere.Clear();
                return dataTable;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlDataAdapter.Dispose();
                listColumnsSelect.Clear();
                listValuesWhere.Clear();
                throw new Exception(ex.Message, ex);
            }
        }

        public DataTable SelectQueryToDataTable(string query)
        {
            if (string.IsNullOrEmpty(query?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            SqlCommand sqlcmd = new SqlCommand(query, sqlcn);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlcmd);
            dataTable = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dataTable);
                sqlcmd.Dispose();
                sqlDataAdapter.Dispose();
                return dataTable;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlDataAdapter.Dispose();
                throw new Exception(ex.Message, ex);
            }
        }

        public object SelectSingleValue(string tableName, string columnName, SQLFunction sqlFunction = SQLFunction.None)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()) || string.IsNullOrEmpty(columnName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder("SELECT ");
            switch (sqlFunction)
            {
                case SQLFunction.None:
                    sqlQuery.Append($"{columnName}");
                    break;
                case SQLFunction.MAX:
                    sqlQuery.Append($"ISNULL(MAX({columnName}),0)");
                    break;
                case SQLFunction.COUNT:
                    sqlQuery.Append($"COUNT({columnName})");
                    break;
                default:
                    sqlQuery.Append($"{columnName}");
                    break;
            }
            sqlQuery.Append($" FROM {tableName}");
            if (listValuesWhere.Count > 0)
            {
                sqlQuery.Append(" WHERE ");
                foreach (var value in listValuesWhere)
                {
                    sqlQuery.Append($"{value.Name} = @{value.Name} AND ");
                    sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
                }
                sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            }
            sqlcmd.CommandText = sqlQuery.ToString();
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            try
            {
                object data = sqlcmd.ExecuteScalar();
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesWhere.Clear();
                return data;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesWhere.Clear();
                throw new Exception(ex.Message, ex);
            }
        }
        public object SelectQuerySingleValue(string query)
        {
            if (string.IsNullOrEmpty(query?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            SqlCommand sqlcmd = new SqlCommand(query, sqlcn);
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            try
            {
                object data = sqlcmd.ExecuteScalar();
                sqlcmd.Dispose();
                sqlcn.Close();
                return data;
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
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (listValuesQuery.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueForQuery);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"INSERT INTO {tableName} (");
            StringBuilder valuesStr = new StringBuilder(") VALUES (");
            foreach (var value in listValuesQuery)
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
                listValuesQuery.Clear();
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
                listValuesQuery.Clear();
                throw new Exception(ex.Message, ex);
            }
        }
        public string InsertWithIdentityReturn(string tableName, string identityColumn)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()) || string.IsNullOrEmpty(identityColumn?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (listValuesQuery.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueForQuery);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"INSERT INTO {tableName} (");
            StringBuilder valuesStr = new StringBuilder($") OUTPUT INSERTED.{identityColumn} VALUES (");
            foreach (var value in listValuesQuery)
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
                string insertedId = sqlcmd.ExecuteScalar().ToString();
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesQuery.Clear();
                return insertedId;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesQuery.Clear();
                throw new Exception(ex.Message, ex);
            }
        }

        public int Update(string tableName)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (listValuesQuery.Count == 0)
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
            foreach (var value in listValuesQuery)
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
                listValuesQuery.Clear();
                listValuesWhere.Clear();
                return rowsAff;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                listValuesQuery.Clear();
                listValuesWhere.Clear();
                throw new Exception(ex.Message, ex);
            }
        }

        public int Delete(string tableName)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
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
                if (dataTable != null)
                {
                    dataTable.Dispose();
                    dataTable = null;
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