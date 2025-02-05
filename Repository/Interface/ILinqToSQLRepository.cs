using Entities.DTO;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface ILinqToSQLRepository
    {
        Task CreateEntriesTableAsync(string nameTable, List<FieldsForNewContest> columns, SqlConnection sqlConnection, SqlTransaction sqlTransaction);
        Task CreateWinnerTableAsync(string nameTable, SqlConnection sqlConnection, SqlTransaction sqlTransaction);
        Task InsertAsync(string tableName, string columns, string value, SqlConnection sqlConnection);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, Option option, List<string> entryExclusionFields,  SqlConnection sqlConnection);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, List<string> entryExclusionFields, SqlConnection sqlConnection);
        Task PurgeSelectedEntries(string nameTable, string entriesID, SqlConnection sqlConnection);
        Task PurgeAllEntries(string nameTable, SqlConnection sqlConnection);
    }
}
