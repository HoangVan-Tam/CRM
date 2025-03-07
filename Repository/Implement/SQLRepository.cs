
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
using Entities.Helper;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            queryString = queryString.Replace("values", string.Join(",", props.Keys.Select(p => "@" + p)));
            var sqlParams = props.Select(p => new SqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray();
            await _context.Database.ExecuteSqlRawAsync(queryString, sqlParams);
        }

        /// <summary>
        /// Inserts a new entry into the dynamically generated contest table.
        /// </summary>
        /// <param name="contest">The contest object containing contest details.</param>
        /// <param name="props">A dictionary containing column values to be inserted.</param>
        /// <param name="tableColumns">A collection of metadata about table columns.</param>
        /// <param name="transaction">The database transaction to ensure atomicity.</param>
        /// <returns>An asynchronous task representing the operation.</returns>
        public async Task InsertEntriesAsync(
            Contest contest,
            Dictionary<string, object> props,
            IEnumerable<ColumnMetadata> tableColumns,
            IDbContextTransaction transaction)
        {
            // Generate a unique verification code using the current UTC timestamp and a GUID substring
            var verificationCode = DateTime.UtcNow.ToString("MMddff") + Guid.NewGuid().ToString().Substring(0, 4);

            // Add the verification code to the properties dictionary
            props.Add("VerificationCode", verificationCode);

            // Define the target table name dynamically based on the contest unique code
            var tableName = "BC_" + contest.ContestUniqueCode;

            // Construct the base SQL INSERT query
            string queryString = "INSERT INTO " + tableName + " (columns) VALUES (values)";

            // Replace placeholder "columns" with actual column names
            queryString = queryString.Replace("columns", string.Join(",", tableColumns.Select(p => p.ColumnName)));

            // Replace placeholder "values" with parameterized values to prevent SQL injection
            queryString = queryString.Replace("values", string.Join(",", tableColumns.Select(p => "@" + p.ColumnName)));

            // Extract column names from metadata
            var tableColumnNames = tableColumns.Select(p => p.ColumnName);

            // Prepare SQL parameters with proper SQL data types and values
            var sqlParams = tableColumns.Select(p =>
            {
                var sqlDbType = SqlTypeHelper.GetSqlDbType(p.DataType ?? "nvarchar");
                var value = SqlTypeHelper.GetDefaultValue(sqlDbType);

                // Assign actual value if it exists in props, otherwise use the default value
                if (props.ContainsKey(p.ColumnName))
                {
                    value = props[p.ColumnName] ?? value;
                }

                return new SqlParameter("@" + p.ColumnName, sqlDbType) { Value = value };
            }).ToArray();

            // Get database connection from the context
            var connection = _context.Database.GetDbConnection();

            // Open connection if it's not already open
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            // Execute the SQL command within the provided transaction
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction.GetDbTransaction(); // Link the transaction
                command.CommandText = queryString;
                command.Parameters.AddRange(sqlParams);

                await command.ExecuteNonQueryAsync(); // Execute the INSERT query
            }
        }

        public async Task CreateContestTableAsync(string tableName, List<FieldsForNewContest> columns, GlobalConstants.TYPETABLE type, IDbContextTransaction transaction)
        {
            string queryString = "";
            switch (type)
            {
                case GlobalConstants.TYPETABLE.ENTRIES:
                    queryString = GlobalConstants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD.Replace("230101_KEYWORD", tableName);
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
                    queryString = GlobalConstants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD_Winner.Replace("230101_KEYWORD", tableName);
                    break;
                case GlobalConstants.TYPETABLE.LOG:
                    queryString = GlobalConstants.DBSCRIPT_CREATE_TABLE_BC_230101_KEYWORD_Logs.Replace("230101_KEYWORD", tableName);
                    break;
                default: break;

            }

            var lstCmd = queryString.Split("GO", StringSplitOptions.RemoveEmptyEntries).ToList();

            var connection = _context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
            foreach (var item in lstCmd)
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction.GetDbTransaction(); // Link transaction
                    command.CommandText = item;
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Dictionary<string, object>>> GetEntriesByCondition(
            string tableName,
            Dictionary<string, object> conditionProps,
            IDbContextTransaction transaction)
        {

            var queryString = $"SELECT * FROM {tableName} WHERE ";

            var conditions = new List<string>();
            var sqlParams = new List<DbParameter>();

            foreach (var kvp in conditionProps)
            {
                string paramName = $"@{kvp.Key}";
                conditions.Add($"{kvp.Key} = {paramName}");

                var param = _context.Database.GetDbConnection().CreateCommand().CreateParameter();
                param.ParameterName = paramName;
                param.Value = kvp.Value;
                sqlParams.Add(param);
            }

            queryString += string.Join(" AND ", conditions);

            var result = new List<Dictionary<string, object>>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.Transaction = transaction.GetDbTransaction();
                command.CommandText = queryString;

                foreach (var param in sqlParams)
                {
                    command.Parameters.Add(param);
                }

                if (command.Connection.State == ConnectionState.Closed)
                {
                    await command.Connection.OpenAsync();
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        result.Add(row);
                    }
                }
            }
            return result;
        }

        public async Task<List<Dictionary<string, object>>> GetAllEntries(string tableName, Option option, List<string> entryExclusionFields)
        {
            //tableName = "[BC_010101_TIGER]";
            //var skipRow = option.PageSize * (option.PageNumber - 1);
            //string queryString = Constants.DBSCRIPT_GET_ALL_ENTRIES.Replace("[BC_230101_KEYWORD]", tableName).Replace("{SkipRow}", skipRow.ToString()).Replace("{TakeRow}", option.PageSize.ToString());
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

        public async Task<List<Dictionary<string, object>>> GetAllEntries(string tableName, List<string> entryExclusionFields)
        {
            return null;
            //tableName = "[BC_010101_TIGER]";
            //string queryString = Constants.DBSCRIPT_GET_ALL_ENTRIES_NOPAGING.Replace("[BC_230101_KEYWORD]", tableName);
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

        public async Task PurgeSelectedEntries(string tableName, string entriesID)
        {
            //tableName = "[BC_010101_TIGER]";
            //string queryString = Constants.DBSCRIPT_PURGE_SELECTED_ENTRIES.Replace("[BC_230101_KEYWORD]", tableName).Replace("{entriesID}", entriesID);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = _sqlConnection;
            //cmd.CommandText = queryString;
            //await cmd.ExecuteNonQueryAsync();
        }

        public async Task PurgeAllEntries(string tableName)
        {
            //tableName = "[BC_010101_TIGER]";
            //string queryString = Constants.DBSCRIPT_PURGE_ALL_ENTRIES.Replace("[BC_230101_KEYWORD]", tableName);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = _sqlConnection;
            //cmd.CommandText = queryString;
            //await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Dictionary<string, object>>> FindEntriesAsync(string tableName, Dictionary<string, object> props, List<ColumnMetadata> tableColumns, IDbContextTransaction transaction)
        {
            var dictionaries = new List<Dictionary<string, object>>();
            var conditionCmds = new List<string>();
            foreach (var item in props)
            {
                conditionCmds.Add(item.Key + " = @" + item.Key);
            }
            string queryString = GlobalConstants.DBSCRIPT_SELECT_ENTRIES_BY_CONDITION.Replace("@table", tableName);
            queryString = queryString.Replace("@condition", string.Join(" and ", conditionCmds));

            var connection = _context.Database.GetDbConnection();
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
                        var dictionary = new Dictionary<string, object>();
                        for (int i = 0; i < tableColumns.Count(); i++)
                        {
                            dictionary.Add(tableColumns[i].ColumnName, reader.GetValue(i));
                        }
                        dictionaries.Add(dictionary);
                    }
                }
            }
            return dictionaries;
        }

        public async Task<List<ColumnMetadata>> GetTableColumnsAsync(
             string contestUniqueCode,
             IDbContextTransaction transaction)
        {
            var columns = new List<ColumnMetadata>();
            string query = GlobalConstants.DBSCRIPT_SELECT_COLUMN_METADATA;

            var connection = _context.Database.GetDbConnection();

            // Không đóng kết nối khi hoàn thành, chỉ lấy dữ liệu
            var command = connection.CreateCommand();
            command.Transaction = transaction.GetDbTransaction(); // Liên kết transaction
            command.CommandText = query;

            var param = new SqlParameter("@TableName", DbType.String) { Value = "BC_" + contestUniqueCode };
            command.Parameters.Add(param);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                columns.Add(new ColumnMetadata
                {
                    ColumnName = reader["COLUMN_NAME"].ToString(),
                    DataType = reader["DATA_TYPE"].ToString()
                });
            }

            return columns;
        }


        public async Task UpdateEntriesAsync(
            string tableName,
            string verificationCode,
            List<ColumnMetadata> tableColumns,
            Dictionary<string, object> updatedProperties,
            IDbContextTransaction dbTransaction)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName), "Table name cannot be null or empty.");

            if (updatedProperties == null || updatedProperties.Count == 0)
                throw new ArgumentException("No properties provided to update.", nameof(updatedProperties));

            if (dbTransaction == null)
                throw new ArgumentNullException(nameof(dbTransaction));

            var setClause = string.Join(", ", updatedProperties.Keys.Select(key => $"{key} = @{key}"));

            string updateQuery = $"UPDATE {tableName} SET {setClause} WHERE EntryID = @EntryID";

            var sqlParams = tableColumns.Select(p =>
            {
                var sqlDbType = SqlTypeHelper.GetSqlDbType(p.DataType ?? "nvarchar");
                var value = SqlTypeHelper.GetDefaultValue(sqlDbType);

                // Assign actual value if it exists in props, otherwise use the default value
                if (updatedProperties.ContainsKey(p.ColumnName))
                {
                    value = updatedProperties[p.ColumnName] ?? value;
                }

                return new SqlParameter("@" + p.ColumnName, sqlDbType) { Value = value };
            }).ToList();

            sqlParams.Add(new SqlParameter("@VerificationCode", SqlDbType.NVarChar) { Value = verificationCode });

            await _context.Database.ExecuteSqlRawAsync(updateQuery, sqlParams);
        }

    }
}
