
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implement
{
    public class LinqToSQLRepository : ILinqToSQLRepository
    {
        public LinqToSQLRepository()
        {
        }
        public async Task InsertAsync(string tableName, string columns, string value, SqlConnection sqlConnection)
        {
            string queryString = "INSERT INTO " + tableName + " (" + columns + ") " + "VALUES " + "(" + value + ")";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = queryString;
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task CreateContestTableAsync(string nameTable, List<FieldsForNewContest> columns, SqlConnection sqlConnection, SqlTransaction sqlTransaction, Constants.TYPETABLE  type)
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
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.Transaction = sqlTransaction;
            var lstCmd = queryString.Split("GO").ToList();
            foreach (var item in lstCmd)
            {
                cmd.CommandText = item;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, Option option, List<string> entryExclusionFields, SqlConnection sqlConnection)
        {
            nameTable = "[BC_010101_TIGER]";
            var skipRow = option.PageSize * (option.PageNumber - 1);
            string queryString = Constants.DBSCRIPT_GET_ALL_ENTRIES.Replace("[BC_230101_KEYWORD]", nameTable).Replace("{SkipRow}", skipRow.ToString()).Replace("{TakeRow}", option.PageSize.ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = queryString;
            List<Dictionary<string, object>> lstDictionaries = new List<Dictionary<string, object>>();
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    var keyValuePair = Enumerable.Range(0, reader.FieldCount)
                 .ToDictionary(reader.GetName, reader.GetValue).Where(p => !entryExclusionFields.Any(o => p.Key.Contains(o))).ToDictionary(p=>p.Key, p=>p.Value);
                    lstDictionaries.Add(keyValuePair);
                }
            }
            return lstDictionaries;
        }

        public async Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, List<string> entryExclusionFields, SqlConnection sqlConnection)
        {
            nameTable = "[BC_010101_TIGER]";
            string queryString = Constants.DBSCRIPT_GET_ALL_ENTRIES_NOPAGING.Replace("[BC_230101_KEYWORD]", nameTable);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = queryString;
            List<Dictionary<string, object>> lstDictionaries = new List<Dictionary<string, object>>();
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    var keyValuePair = Enumerable.Range(0, reader.FieldCount)
                 .ToDictionary(reader.GetName, reader.GetValue).Where(p => !entryExclusionFields.Any(o => p.Key.Contains(o))).ToDictionary(p => p.Key, p => p.Value);
                    lstDictionaries.Add(keyValuePair);
                }
            }
            return lstDictionaries;
        }

        public async Task PurgeSelectedEntries(string nameTable, string entriesID, SqlConnection sqlConnection)
        {
            nameTable = "[BC_010101_TIGER]";
            string queryString = Constants.DBSCRIPT_PURGE_SELECTED_ENTRIES.Replace("[BC_230101_KEYWORD]", nameTable).Replace("{entriesID}", entriesID);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = queryString;
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task PurgeAllEntries(string nameTable, SqlConnection sqlConnection)
        {
            nameTable = "[BC_010101_TIGER]";
            string queryString = Constants.DBSCRIPT_PURGE_ALL_ENTRIES.Replace("[BC_230101_KEYWORD]", nameTable);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandText = queryString;
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Dictionary<string, object>>> FindEntries (string nameTable,Dictionary<string, object> props, SqlConnection sqlConnection)
        {
            var conditionCmds = new List<string>();
            foreach(var item in props)
            {
                conditionCmds.Add(item.Key + " = " + item.Value.ToString());
            }
            string queryString = Constants.DBSCRIPT_SELECT_ENTRIES_BY_CONDITION.Replace("[BC_230101_KEYWORD]", nameTable);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@table", nameTable);
            cmd.Parameters.AddWithValue("@condition", string.Join("and", conditionCmds));
            cmd.Connection = sqlConnection;
            cmd.CommandText = queryString;
            List<Dictionary<string, object>> lstDictionaries = new List<Dictionary<string, object>>();
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    var keyValuePair = Enumerable.Range(0, reader.FieldCount)
                 .ToDictionary(reader.GetName, reader.GetValue).ToDictionary(p => p.Key, p => p.Value);
                    lstDictionaries.Add(keyValuePair);
                }
            }
            return lstDictionaries;
        }
    }
}
