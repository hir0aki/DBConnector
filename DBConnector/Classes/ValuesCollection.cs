using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Classes
{
    public class ValuesCollection : System.Collections.ObjectModel.Collection<Values>
    {
        public ValuesCollection Add(string columnName, object value)
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
            Add(valuesQuery);
            return this;
        }
    }
}
