
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
using Entities.Helper;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implement
{
    public class SQLRepository : ISQLRepository
    {
        private readonly StandardContest2023Context _context;

        public SQLRepository(StandardContest2023Context context)
        {
            _context = context;
        }

        public async Task InsertLogAsync(string contestUniqueCode, Dictionary<string, object> props)
        {
            var tableName = "BC_" + contestUniqueCode;
            string queryString = "INSERT INTO " + tableName + " {columns} " + "VALUES " + "{values}";
            queryString = queryString.Replace("columns", string.Join(",", props.Keys).Replace("@", ""));
            queryString = queryString.Replace("values", string.Join(",", props.Keys));
            var sqlParams = props.Select(p => new SqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray();
            await _context.Database.ExecuteSqlRawAsync(queryString, sqlParams);
        }
        public async Task InsertEntriesAsync(string contestUniqueCode, Dictionary<string, object> props, List<ColumnMetadata> tableColumns, IDbContextTransaction transaction)
        {
            var tableName = "BC_" + contestUniqueCode;
            string queryString = "INSERT INTO " + tableName + " {columns} " + "VALUES " + "{values}";
            queryString = queryString.Replace("columns", string.Join(",", props.Keys).Replace("@", ""));
            queryString = queryString.Replace("values", string.Join(",", props.Keys));
            var tableColumnNames = tableColumns.Select(p => p.ColumnName);
            var matchingProperties = props.Where(p => tableColumnNames.Contains(p.Key));
            var sqlParams = new List<SqlParameter>();
            foreach (var item in matchingProperties)
            {
                var tableColumnMetadata = tableColumns.Where(p => p.ColumnName == item.Key).FirstOrDefault();
                var sqlDbType = SqlTypeHelper.GetSqlDbType(tableColumnMetadata != null ? tableColumnMetadata.DataType : "nvarchar");
                var param = new SqlParameter(item.Key, sqlDbType) { Value = item.Value };
                sqlParams.Add(param);
            }
            await _context.Database.ExecuteSqlRawAsync(queryString, sqlParams);
        }

        public async Task CreateContestTableAsync(string nameTable, List<FieldsForNewContest> columns, GlobalConstants.TYPETABLE type)
        {
            string queryString = "";
            switch (type)
            {
                case GlobalConstants.TYPETABLE.ENTRIES:
                    queryString = GlobalConstants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD.Replace("230101_KEYWORD", nameTable);
                    if (columns.Count() == 0)
                    {
                        queryString = queryString.Replace("AddMoreColumn", "");
                    }
                    else
                    {
                        var additionalColumn = "";
                        foreach (var column in columns)
                        {
                            switch (column.FieldType)
                            {
                                case "String":
                                    additionalColumn = additionalColumn + "[" + column.FieldName + "] " + "[" + "nvarchar" + "] " + (column.FieldName == "FileLink" ? "(1500)" : "(250)") + (column.IsRequired ? "NOT NULL, " : ", ");
                                    break;
                                case "DateTime":
                                    additionalColumn = additionalColumn + "[" + column.FieldName + "] " + "[" + "datetime2" + "] " + (column.IsRequired ? "NOT NULL, " : ", ");
                                    break;
                                case "Int":
                                    additionalColumn = additionalColumn + "[" + column.FieldName + "] " + "[" + "int" + "] " + (column.IsRequired ? "NOT NULL, " : ", ");
                                    break;
                                case "Decimal":
                                    additionalColumn = additionalColumn + "[" + column.FieldName + "] " + "[" + "money" + "] " + (column.IsRequired ? "NOT NULL, " : ", ");
                                    break;
                                case "Boolean":
                                    additionalColumn = additionalColumn + "[" + column.FieldName + "] " + "[" + "bit" + "] " + (column.IsRequired ? "NOT NULL, " : ", ");
                                    break;
                                default: break;
                            }
                        }

                        queryString = queryString.Replace("AddMoreColumn", additionalColumn);
                    }
                    break;
                case GlobalConstants.TYPETABLE.WINNERS:
                    queryString = GlobalConstants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD_Winner.Replace("230101_KEYWORD", nameTable);
                    break;
                case GlobalConstants.TYPETABLE.LOG:
                    queryString = GlobalConstants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD_Logs.Replace("230101_KEYWORD", nameTable);
                    break;
                default: break;

            }
            var lstCmd = queryString.Split("GO", StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var item in lstCmd)
            {
                await _context.Database.ExecuteSqlRawAsync(item);
            }
        }

        public async Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, Option option, List<string> entryExclusionFields)
        {
            //nameTable = "[BC_010101_TIGER]";
            //var skipRow = option.PageSize * (option.PageNumber - 1);
            //string queryString = Constants.DBSCRIPT_GET_ALL_ENTRIES.Replace("[BC_230101_KEYWORD]", nameTable).Replace("{SkipRow}", skipRow.ToString()).Replace("{TakeRow}", option.PageSize.ToString());
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = _sqlConnection;
            //cmd.CommandText = queryString;
            //List<Dictionary<string, object>> lstDictionaries = new List<Dictionary<string, object>>();
            //using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            //{
            //    while (reader.Read())
            //    {
            //        var keyValuePair = Enumerable.Range(0, reader.FieldCount)
            //     .ToDictionary(reader.GetName, reader.GetValue).Where(p => !entryExclusionFields.Any(o => p.Key.Contains(o))).ToDictionary(p => p.Key, p => p.Value);
            //        lstDictionaries.Add(keyValuePair);
            //    }
            //}
            //return lstDictionaries;
            return null;
        }

        public async Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, List<string> entryExclusionFields)
        {
            return null;
            //nameTable = "[BC_010101_TIGER]";
            //string queryString = Constants.DBSCRIPT_GET_ALL_ENTRIES_NOPAGING.Replace("[BC_230101_KEYWORD]", nameTable);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = _sqlConnection;
            //cmd.CommandText = queryString;
            //List<Dictionary<string, object>> lstDictionaries = new List<Dictionary<string, object>>();
            //using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            //{
            //    while (reader.Read())
            //    {
            //        var keyValuePair = Enumerable.Range(0, reader.FieldCount)
            //     .ToDictionary(reader.GetName, reader.GetValue).Where(p => !entryExclusionFields.Any(o => p.Key.Contains(o))).ToDictionary(p => p.Key, p => p.Value);
            //        lstDictionaries.Add(keyValuePair);
            //    }
            //}
            //return lstDictionaries;
        }

        public async Task PurgeSelectedEntries(string nameTable, string entriesID)
        {
            //nameTable = "[BC_010101_TIGER]";
            //string queryString = Constants.DBSCRIPT_PURGE_SELECTED_ENTRIES.Replace("[BC_230101_KEYWORD]", nameTable).Replace("{entriesID}", entriesID);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = _sqlConnection;
            //cmd.CommandText = queryString;
            //await cmd.ExecuteNonQueryAsync();
        }

        public async Task PurgeAllEntries(string nameTable)
        {
            //nameTable = "[BC_010101_TIGER]";
            //string queryString = Constants.DBSCRIPT_PURGE_ALL_ENTRIES.Replace("[BC_230101_KEYWORD]", nameTable);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = _sqlConnection;
            //cmd.CommandText = queryString;
            //await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Dictionary<string, object>> FindEntries(string contestUniqueCode, Dictionary<string, object> props, List<ColumnMetadata> tableColumns, IDbContextTransaction transaction)
        {
            var dictionary = new Dictionary<string, object>();
            var conditionCmds = new List<string>();
            foreach (var item in props)
            {
                conditionCmds.Add(item.Key + " = @" + item.Key);
            }
            string queryString = GlobalConstants.DBSCRIPT_SELECT_ENTRIES_BY_CONDITION.Replace("@table", "[BC_" + contestUniqueCode + "]");
            queryString = queryString.Replace("@condition", string.Join(" and ", conditionCmds));

            using (var connection = _context.Database.GetDbConnection())
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }
                using (var command = connection.CreateCommand())
                {
                    if (transaction != null)
                    {
                        command.Transaction = transaction.GetDbTransaction();
                    }
                    command.CommandText = queryString;
                    foreach (var item in props)
                    {
                        var columnMetadata = tableColumns.Where(p => p.ColumnName == item.Key).FirstOrDefault();
                        if (columnMetadata != null)
                        {
                            var sqlDbType = SqlTypeHelper.GetSqlDbType(columnMetadata.DataType);
                            var param = new SqlParameter(item.Key, sqlDbType) { Value = item.Value };
                            command.Parameters.Add(param);
                        }
                    }
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dictionary.Add(reader.GetString(0), reader.GetValue(1));
                        }
                    }
                }
            }
            return dictionary;
        }

        public async Task<List<ColumnMetadata>> GetTableColumnsAsync(string contestUniqueCode, IDbContextTransaction transaction)
        {
            var columns = new List<ColumnMetadata>();
            string query = GlobalConstants.DBSCRIPT_SELECT_COLUMN_METADATA;

            var connection = _context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction.GetDbTransaction(); // Link transaction
                command.CommandText = query;

                var param = new SqlParameter("@TableName", DbType.String) { Value = "BC_" + contestUniqueCode };
                command.Parameters.Add(param);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        columns.Add(new ColumnMetadata
                        {
                            ColumnName = reader["COLUMN_NAME"].ToString(),
                            DataType = reader["DATA_TYPE"].ToString()
                        });
                    }
                }
            }

            return columns;
        }

    }
}
