using Entities.Constants;
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
        Task CreateContestTableAsync(string nameTable, List<FieldsForNewContest> columns, SqlTransaction sqlTransaction, Constants.TYPETABLE type);
        Task InsertAsync(string tableName, Dictionary<string, object> props);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, Option option, List<string> entryExclusionFields);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, List<string> entryExclusionFields);
        Task PurgeSelectedEntries(string nameTable, string entriesID);
        Task PurgeAllEntries(string nameTable);
        Task<List<Dictionary<string, object>>> FindEntries(string nameTable, Dictionary<string, object> props);
    }
}
