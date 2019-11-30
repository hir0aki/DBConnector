using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Classes
{
    public class QuerysTransactionCollection : System.Collections.ObjectModel.Collection<string>
    {
        public new QuerysTransactionCollection Add(string query)
        {
            if (string.IsNullOrEmpty(query?.Trim()))
            {
                throw new Exception(Properties.Resources.exceptionParametersNullOrEmpty);
            }
            Add(query);
            return this;
        }
    }
}