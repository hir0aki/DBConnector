using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Classes
{
    public class ColumnsSelectCollection : System.Collections.ObjectModel.Collection<string>
    {
        public new ColumnsSelectCollection Add(string columnName)
        {
            if (string.IsNullOrEmpty(columnName?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            base.Add(columnName);
            return this;
        }
    }
}
