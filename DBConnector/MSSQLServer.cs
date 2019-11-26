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
        public List<Values> listParametersSP { get; }
        public List<ValuesWhere> listValuesWhere { get; }
        public List<string> listColumnsSelect { get; }
        public List<string> listQuerysTransaction { get; }

        private IntPtr nativeResource = Marshal.AllocHGlobal(100);
        private SqlConnection sqlcn;
        private DataTable dataTable;


        public MSSQLServer(string connectionString)
        {
            ConnectionString = connectionString;
            sqlcn = new SqlConnection(ConnectionString);
            listValuesQuery = new List<Values>();
            listParametersSP = new List<Values>();
            listValuesWhere = new List<ValuesWhere>();
            listColumnsSelect = new List<string>();
            listQuerysTransaction = new List<string>();
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
        public void AddParameterSP(string parameterName, object value)
        {
            if (string.IsNullOrEmpty(parameterName?.Trim()) || value == null)
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            Values parameterSP = new Values
            {
                Name = parameterName,
                Value = value
            };
            listParametersSP.Add(parameterSP);
        }
        public void AddValuesWhere(string columnName, SQLComparisonOperator sqlComparisonOperator, object value, string parameterName = "")
        {
            if (string.IsNullOrEmpty(columnName?.Trim()) || value == null || sqlComparisonOperator == null)
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            ValuesWhere valuesWhere = new ValuesWhere
            {
                Name = columnName,
                Value = value,
                SQLComparisonOperator = sqlComparisonOperator,
                ParameterName = (string.IsNullOrEmpty(parameterName?.Trim()) ? columnName : parameterName)
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
        public void AddQuerySQLTransaction(string query)
        {
            if (string.IsNullOrEmpty(query?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            listQuerysTransaction.Add(query);
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

        public int ExecuteSP(string spName)
        {
            if (string.IsNullOrEmpty(spName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            SqlCommand sqlcmd = new SqlCommand(spName, sqlcn);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            foreach (var value in listParametersSP)
            {
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            try
            {
                int rowsAff = sqlcmd.ExecuteNonQuery();
                sqlcmd.Dispose();
                sqlcn.Close();
                listParametersSP.Clear();
                return rowsAff;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                sqlcn.Close();
                listParametersSP.Clear();
                throw new Exception(ex.Message, ex);
            }
        }
        public bool ExecuteSQLTransactions()
        {
            if (listQuerysTransaction.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddQuerySQLTransaction);
            }
            if (sqlcn.State == ConnectionState.Open)
            {
                sqlcn.Close();
            }
            sqlcn.Open();
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            SqlTransaction sqlTransaction;
            sqlTransaction = sqlcn.BeginTransaction();
            sqlcmd.Transaction = sqlTransaction;
            try
            {
                foreach (string query in listQuerysTransaction)
                {
                    sqlcmd.CommandText = query;
                    sqlcmd.ExecuteNonQuery();
                }
                sqlTransaction.Commit();
                sqlTransaction.Dispose();
                sqlcmd.Dispose();
                sqlcn.Close();
                listQuerysTransaction.Clear();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    sqlTransaction.Rollback();
                }
                catch (Exception ex2)
                {
                    sqlTransaction.Dispose();
                    sqlcmd.Dispose();
                    sqlcn.Close();
                    listQuerysTransaction.Clear();
                    throw new Exception(ex2.Message, ex2);
                }
                sqlTransaction.Dispose();
                sqlcmd.Dispose();
                sqlcn.Close();
                listQuerysTransaction.Clear();
                throw new Exception(ex.Message, ex);
            }
        }
        public DataTable SelectToDataTable(string tableName, bool allColumns = false, SQLOrderBy orderBy = SQLOrderBy.None, string orderByColumnName = "")
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
            if (orderBy != SQLOrderBy.None && string.IsNullOrEmpty(orderByColumnName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
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
                    //string prefix = (value.SQLComparisonOperator != SQLComparisonOperator.Like) ? "@" : string.Empty;
                    sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                    sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
                }
                sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            }
            switch (orderBy)
            {
                case SQLOrderBy.None:
                    break;
                case SQLOrderBy.ASC:
                    sqlQuery.Append($" ORDER BY {orderByColumnName} ASC");
                    break;
                case SQLOrderBy.DESC:
                    sqlQuery.Append($" ORDER BY {orderByColumnName} DESC");
                    break;
                default:
                    break;
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
                    sqlQuery.Append($"TOP 1 {columnName}");
                    break;
                case SQLFunction.MAX:
                    sqlQuery.Append($"ISNULL(MAX({columnName}),0)");
                    break;
                case SQLFunction.MIN:
                    sqlQuery.Append($"ISNULL(MIN({columnName}),0)");
                    break;
                case SQLFunction.COUNT:
                    sqlQuery.Append($"COUNT({columnName})");
                    break;
                case SQLFunction.SUM:
                    sqlQuery.Append($"SUM({columnName})");
                    break;
                case SQLFunction.AVG:
                    sqlQuery.Append($"AVG({columnName})");
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
                    sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                    sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
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
                sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
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
                sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
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