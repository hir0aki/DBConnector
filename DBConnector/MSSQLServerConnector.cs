﻿using DBConnector.Classes;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DBConnector
{
    public class MSSQLServerConnector : IDBConnector, IDisposable
    {
        private string _connectionString;
        private bool _activeTransaction = false;
        private IntPtr nativeResource = Marshal.AllocHGlobal(100);
        private SqlConnection sqlcn;
        private DataTable dataTable;
        private SqlTransaction _sqlTransactionGlobal;
        public ColumnsSelectCollection ColumnsSelect { get; }
        public ValuesWhereCollection ValuesWhere { get; }
        public ValuesCollection Values { get; }
        public ParametersSPCollection ParametersSP { get; }
        public QuerysTransactionCollection QuerysTransaction { get; }
        public bool Debug { get; set; } = false;

        public MSSQLServerConnector(string connectionString)
        {
            _connectionString = connectionString;
            sqlcn = new SqlConnection(_connectionString);
            //listValuesQuery = new List<Values>();
            //listParametersSP = new List<Values>();
            //listValuesWhere = new List<ValuesWhere>();
            //listColumnsSelect = new List<string>();
            //listQuerysTransaction = new List<string>();
            ColumnsSelect = new ColumnsSelectCollection();
            ValuesWhere = new ValuesWhereCollection();
            Values = new ValuesCollection();
            ParametersSP = new ParametersSPCollection();
            QuerysTransaction = new QuerysTransactionCollection();
        }
        ////////////////Methods
        public int ExecuteQuery(string query)
        {
            if (string.IsNullOrEmpty(query?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            SqlCommand sqlcmd = new SqlCommand(query, sqlcn);
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            } 
            else
            {
                sqlcn.Open();
            }
            try
            {
                int rowsAff = sqlcmd.ExecuteNonQuery();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return rowsAff;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
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
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            foreach (var value in ParametersSP)
            {
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            try
            {
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                int rowsAff = sqlcmd.ExecuteNonQuery();
                ParametersSP.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return rowsAff;
            }
            catch (Exception ex)
            {
                ParametersSP.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                throw new Exception(ex.Message, ex);
            }
        }
        public DataTable ExecuteSPToDataTable(string spName)
        {
            if (string.IsNullOrEmpty(spName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            SqlCommand sqlcmd = new SqlCommand(spName, sqlcn);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            foreach (var value in ParametersSP)
            {
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            try
            {
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                SqlDataReader reader = sqlcmd.ExecuteReader();
                dataTable = new DataTable();
                dataTable.Load(reader);
                ParametersSP.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                ParametersSP.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                throw new Exception(ex.Message, ex);
            }
        }
        public bool ExecuteSQLTransactions()
        {
            if (QuerysTransaction.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddQuerySQLTransaction);
            }
            sqlcn.Open();
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            SqlTransaction sqlTransaction = sqlcn.BeginTransaction();
            sqlcmd.Transaction = sqlTransaction;
            try
            {
                foreach (string query in QuerysTransaction)
                {
                    sqlcmd.CommandText = query;
                    sqlcmd.ExecuteNonQuery();
                }
                QuerysTransaction.Clear();
                sqlTransaction.Commit();
                sqlTransaction.Dispose();
                sqlcmd.Dispose();
                sqlcn.Close();
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
                    QuerysTransaction.Clear();
                    sqlTransaction.Dispose();
                    sqlcmd.Dispose();
                    sqlcn.Close();
                    throw new Exception(ex2.Message, ex2);
                }
                QuerysTransaction.Clear();
                sqlTransaction.Dispose();
                sqlcmd.Dispose();
                sqlcn.Close();
                throw new Exception(ex.Message, ex);
            }
        }
        public DataTable SelectToDataTable(string tableName, bool allColumns = false, int top = 0, string groupByColumnName = "", SQLOrderBy orderBy = SQLOrderBy.None, string orderByColumnName = "", bool distinct = false)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (!allColumns)
            {
                if (ColumnsSelect.Count == 0)
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
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            StringBuilder sqlQuery = new StringBuilder("SELECT ");
            if (distinct)
            {
                sqlQuery.Append("DISTINCT ");
            }
            if (top > 0)
            {
                sqlQuery.Append($"TOP {top} ");
            }
            if (allColumns)
            {
                sqlQuery.Append($"* FROM {tableName}");
            }
            else
            {
                foreach (string column in ColumnsSelect)
                {
                    sqlQuery.Append($"{column},");
                }
                sqlQuery.Replace(",", string.Empty, sqlQuery.Length - 1, 1);
                sqlQuery.Append($" FROM {tableName}");
            }
            if (ValuesWhere.Count > 0)
            {
                sqlQuery.Append(" WHERE ");
                foreach (var value in ValuesWhere)
                {
                    //string prefix = (value.SQLComparisonOperator != SQLComparisonOperator.Like) ? "@" : string.Empty;
                    sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                    sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
                }
                sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            }
            if (!string.IsNullOrEmpty(groupByColumnName?.Trim()))
            {
                sqlQuery.Append($" GROUP BY {groupByColumnName}");
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
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                sqlDataAdapter.Fill(dataTable);
                ColumnsSelect.Clear();
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                sqlDataAdapter.Dispose();
                return dataTable;
            }
            catch (Exception ex)
            {
                ColumnsSelect.Clear();
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                sqlDataAdapter.Dispose();
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
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
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
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
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
            if (ValuesWhere.Count > 0)
            {
                sqlQuery.Append(" WHERE ");
                foreach (var value in ValuesWhere)
                {
                    sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                    sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
                }
                sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            }
            sqlcmd.CommandText = sqlQuery.ToString();
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            try
            {
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                object data = sqlcmd.ExecuteScalar();
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return data;
            }
            catch (Exception ex)
            {
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
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
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            try
            {
                object data = sqlcmd.ExecuteScalar();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return data;
            }
            catch (Exception ex)
            {
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                throw new Exception(ex.Message, ex);
            }
        }
        public bool Insert(string tableName)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (Values.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueForQuery);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"INSERT INTO {tableName} (");
            StringBuilder valuesStr = new StringBuilder(") VALUES (");
            foreach (var value in Values)
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
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            try
            {
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                int rowsAff = sqlcmd.ExecuteNonQuery();
                Values.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
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
                Values.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                throw new Exception(ex.Message, ex);
            }
        }
        public string InsertWithIdentityReturn(string tableName, string identityColumn)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()) || string.IsNullOrEmpty(identityColumn?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (Values.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueForQuery);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"INSERT INTO {tableName} (");
            StringBuilder valuesStr = new StringBuilder($") OUTPUT INSERTED.{identityColumn} VALUES (");
            foreach (var value in Values)
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
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            try
            {
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                string insertedId = sqlcmd.ExecuteScalar().ToString();
                Values.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return insertedId;
            }
            catch (Exception ex)
            {
                Values.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                throw new Exception(ex.Message, ex);
            }
        }
        public int Update(string tableName)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (Values.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueForQuery);
            }
            if (ValuesWhere.Count == 0)
            {
                throw new Exception(Properties.Resources.exceptionAddValueWhere);
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"UPDATE {tableName} SET ");
            foreach (var value in Values)
            {
                sqlQuery.Append($"{value.Name} = @{value.Name},");
                sqlcmd.Parameters.AddWithValue($"@{value.Name}", value.Value);
            }
            sqlQuery.Replace(",", string.Empty, sqlQuery.Length - 1, 1);
            sqlQuery.Append(" WHERE ");
            foreach (var value in ValuesWhere)
            {
                sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
            }
            sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            sqlcmd.CommandText = sqlQuery.ToString();
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            try
            {
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                int rowsAff = sqlcmd.ExecuteNonQuery();
                Values.Clear();
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return rowsAff;
            }
            catch (Exception ex)
            {
                Values.Clear();
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                throw new Exception(ex.Message, ex);
            }
        }
        public int Delete(string tableName, bool allRows = false)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            if (!allRows)
            {
                if (ValuesWhere.Count == 0)
                {
                    throw new Exception(Properties.Resources.exceptionAddValueWhere);
                }
            }
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcn;
            StringBuilder sqlQuery = new StringBuilder($"DELETE {tableName}");
            if (!allRows)
            {
                sqlQuery.Append($" WHERE ");
                foreach (var value in ValuesWhere)
                {
                    sqlQuery.Append($"{value.Name} {value.SQLComparisonOperator} @{value.ParameterName} AND ");
                    sqlcmd.Parameters.AddWithValue($"@{value.ParameterName}", value.Value);
                }
                sqlQuery.Replace(" AND ", string.Empty, sqlQuery.Length - 5, 5);
            }
            sqlcmd.CommandText = sqlQuery.ToString();
            if (_activeTransaction)
            {
                sqlcmd.Transaction = _sqlTransactionGlobal;
            }
            else
            {
                sqlcn.Open();
            }
            try
            {
                if (Debug)
                {
                    frmDebug frmDebug = new frmDebug(sqlcmd);
                    DialogResult dialogResult = frmDebug.ShowDialog();
                    frmDebug.Dispose();
                }
                int rowsAff = sqlcmd.ExecuteNonQuery();
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                return rowsAff;
            }
            catch (Exception ex)
            {
                ValuesWhere.Clear();
                sqlcmd.Dispose();
                if (!_activeTransaction)
                {
                    sqlcn.Close();
                }
                throw new Exception(ex.Message, ex);
            }
        }
        ////////////////Transaction Methods
        public void BeginTransaction()
        {
            _activeTransaction = true;
            sqlcn.Open();
            _sqlTransactionGlobal = sqlcn.BeginTransaction();
        }
        public void CommitTransaction()
        {
            _activeTransaction = false;
            _sqlTransactionGlobal.Commit();
            sqlcn.Close();
        }
        public void RollbackTransaction()
        {
            _activeTransaction = false;
            _sqlTransactionGlobal.Rollback();
            sqlcn.Close();
        }
        public void CleanValues()
        {
            ColumnsSelect.Clear();
            ValuesWhere.Clear();
            Values.Clear();
            ParametersSP.Clear();
            QuerysTransaction.Clear();
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
                if (_sqlTransactionGlobal != null)
                {
                    _sqlTransactionGlobal.Dispose();
                    _sqlTransactionGlobal = null;
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