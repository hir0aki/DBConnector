using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Classes
{
    public class ValuesWhere
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SQLComparisonOperator SQLComparisonOperator { get; set; }
        public string ParameterName { get; set; }
    }
}
