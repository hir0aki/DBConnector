using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Classes
{
    public class ValuesWhereCollection : System.Collections.ObjectModel.Collection<ValuesWhere>
    {
        public ValuesWhereCollection Add(string columnName, SQLComparisonOperator sqlComparisonOperator, object value, string parameterName = "")
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
            Add(valuesWhere);
            return this;
        }
    }
}
