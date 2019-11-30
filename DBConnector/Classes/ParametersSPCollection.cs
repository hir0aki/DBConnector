using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Classes
{
    public class ParametersSPCollection : System.Collections.ObjectModel.Collection<Values>
    {
        public ParametersSPCollection Add(string parameterName, object value)
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
            Add(parameterSP);
            return this;
        }
    }
}
