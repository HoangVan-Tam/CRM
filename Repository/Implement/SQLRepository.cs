
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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


        public async Task InsertAsync(string contestUniqueCode, Dictionary<string, object> props)
        {
            var tableName = "BC_" + contestUniqueCode;
            string queryString = "INSERT INTO " + tableName + " {columns} " + "VALUES " + "{values}";
            queryString = queryString.Replace("columns", string.Join(",", props.Keys).Replace("@", ""));
            queryString = queryString.Replace("values", string.Join(",", props.Keys));
            var sqlParams = props.Select(p => new SqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray();
            await _context.Database.ExecuteSqlRawAsync(queryString, sqlParams);
        }

        public async Task CreateContestTableAsync(string nameTable, List<FieldsForNewContest> columns, Constants.TYPETABLE type)
        {
            string queryString = "";
            switch (type)
            {
                case Constants.TYPETABLE.ENTRIES:
                    queryString = Constants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD.Replace("230101_KEYWORD", nameTable);
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
                case Constants.TYPETABLE.WINNERS:
                    queryString = Constants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD_Winner.Replace("230101_KEYWORD", nameTable);
                    break;
                case Constants.TYPETABLE.LOG:
                    queryString = Constants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD_Logs.Replace("230101_KEYWORD", nameTable);
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

        public async Task<Dictionary<string, object>> FindEntries(string contestUniqueCode, Dictionary<string, object> props, IDbTransaction transaction = null)
        {
            var dictionary = new Dictionary<string, object>();
            var conditionCmds = new List<string>();
            foreach (var item in props)
            {
                conditionCmds.Add(item.Key.Replace("@", "") + " = " + item.Key);
            }
            string queryString = Constants.DBSCRIPT_SELECT_ENTRIES_BY_CONDITION.Replace("@table", "[BC_" + contestUniqueCode + "]");
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
                        command.Transaction = (DbTransaction?)transaction;
                    }
                    command.CommandText = queryString;
                    foreach (var item in props)
                    {
                        var param = new SqlParameter(item.Key, SqlDbType.NVarChar) { Value = item.Value};
                        command.Parameters.Add(param);
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

            //List<Dictionary<string, object>> lstDictionaries = new List<Dictionary<string, object>>();
            //var sqlParams = props.Select(p => new SqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray();
            //var connection = _context.Database.GetDbConnection();
            //await connection.OpenAsync();
            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = "SELECT Id, Name FROM Contests";
            //    using (var reader = await command.ExecuteReaderAsync())
            //    {
            //        while (await reader.ReadAsync())
            //        {
            //            dictionary.Add(reader.GetString(0), reader.GetValue(1));
            //        }
            //    }
            //}
            //await connection.CloseAsync();
            return dictionary;
        }
    }
}
