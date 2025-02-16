using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Constants
{
    public partial class GlobalConstants
    {
        public static readonly Dictionary<string, object> DROPDOWNLIST_FIELDTYPE = new Dictionary<string, object>() { { "String", "String" }, { "DateTime", "DateTime" }, { "Int", "Int" }, { "Decimal", "Decimal" }, { "Boolean", "Boolean" } };
    }
}
