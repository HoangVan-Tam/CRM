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
    public interface ISQLRepository
    {
        Task InsertAsync(string contestUniqueCode, Dictionary<string, object> props);
        Task CreateContestTableAsync(string nameTable, List<FieldsForNewContest> columns, Constants.TYPETABLE type);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, Option option, List<string> entryExclusionFields);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, List<string> entryExclusionFields);
        Task PurgeSelectedEntries(string nameTable, string entriesID);
        Task PurgeAllEntries(string nameTable);
        Task<Dictionary<string, object>> FindEntries(string contestUniqueCode, Dictionary<string, object> props, IDbTransaction transaction = null);
    }
}
