using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Globalization;
using static Entities.Constants.GlobalConstants;

namespace DAL;

public static class SqlTypeHelper
{
    private static readonly Dictionary<string, SqlDbType> TypeMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "nvarchar", SqlDbType.NVarChar },
        { "varchar", SqlDbType.VarChar },
        { "ntext", SqlDbType.NText },
        { "datetime2", SqlDbType.DateTime2 },
        { "datetimeoffset", SqlDbType.DateTimeOffset },
        { "bigint", SqlDbType.BigInt },
        { "binary", SqlDbType.Binary },
        { "bit", SqlDbType.Bit },
        { "char", SqlDbType.Char },
        { "datetime", SqlDbType.DateTime },
        { "decimal", SqlDbType.Decimal },
        { "float", SqlDbType.Float },
        { "image", SqlDbType.Image },
        { "int", SqlDbType.Int },
        { "money", SqlDbType.Money },
        { "nchar", SqlDbType.NChar },
        { "real", SqlDbType.Real },
        { "uniqueidentifier", SqlDbType.UniqueIdentifier },
        { "smalldatetime", SqlDbType.SmallDateTime },
        { "smallint", SqlDbType.SmallInt },
        { "smallmoney", SqlDbType.SmallMoney },
        { "text", SqlDbType.Text },
        { "timestamp", SqlDbType.Timestamp },
        { "tinyint", SqlDbType.TinyInt },
        { "varbinary", SqlDbType.VarBinary },
        { "variant", SqlDbType.Variant },
        { "xml", SqlDbType.Xml },
        { "udt", SqlDbType.Udt },
        { "structured", SqlDbType.Structured },
        { "date", SqlDbType.Date },
        { "time", SqlDbType.Time }
    };

    public static SqlDbType? GetSqlDbType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return null;

        // Check in predefined mappings first
        if (TypeMappings.TryGetValue(type.Trim(), out var sqlDbType))
            return sqlDbType;

        // Attempt to parse directly
        if (Enum.TryParse(type.Trim(), true, out SqlDbType parsedType))
            return parsedType;

        return null;
    }

    public static object GetDefaultValue(SqlDbType? sqlDbType)
    {
        return sqlDbType switch
        {
            SqlDbType.BigInt => 0L,
            SqlDbType.Int => 0,
            SqlDbType.SmallInt => (short)0,
            SqlDbType.TinyInt => (byte)0,
            SqlDbType.Bit => false,
            SqlDbType.Decimal => 0.0m,
            SqlDbType.Float => 0.0,
            SqlDbType.Real => 0.0f,
            SqlDbType.Money => 0.0m,
            SqlDbType.SmallMoney => 0.0m,
            SqlDbType.DateTime => DateTime.UtcNow,
            SqlDbType.SmallDateTime => DateTime.UtcNow,
            SqlDbType.Date => DateTime.UtcNow.Date,
            SqlDbType.Time => TimeSpan.Zero,
            SqlDbType.DateTime2 => DateTime.UtcNow,
            SqlDbType.DateTimeOffset => DateTimeOffset.UtcNow,
            SqlDbType.UniqueIdentifier => Guid.NewGuid(),
            SqlDbType.Binary => new byte[0],
            SqlDbType.VarBinary => new byte[0],
            SqlDbType.Image => new byte[0],
            SqlDbType.Timestamp => new byte[8],
            SqlDbType.Char => string.Empty,
            SqlDbType.NChar => string.Empty,
            SqlDbType.VarChar => string.Empty,
            SqlDbType.NVarChar => string.Empty,
            SqlDbType.Text => string.Empty,
            SqlDbType.NText => string.Empty,
            SqlDbType.Xml => string.Empty,
            _ => DBNull.Value
        };
    }
}
