using System.Data;

namespace DAL;

public static class SqlTypeHelper
{
    public static SqlDbType GetSqlDbType(string type)
    {
        return type.ToLower() switch
        {
            "nvarchar" => SqlDbType.NVarChar,
            "varchar" => SqlDbType.VarChar,
            "int" => SqlDbType.Int,
            "datetime" => SqlDbType.DateTime,
            "bit" => SqlDbType.Bit,
            "decimal" => SqlDbType.Decimal,
            "float" => SqlDbType.Float,
            _ => throw new ArgumentException($"Unsupported SQL type: {type}")
        };
    }
}
