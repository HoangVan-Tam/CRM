using Entities.Constants;
using Entities.DTO;
using Entities.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
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
        Task InsertEntriesAsync(string contestUniqueCode, Dictionary<string, object> props, List<ColumnMetadata> tableColumns, IDbContextTransaction transaction);
        Task InsertLogAsync(string contestUniqueCode, Dictionary<string, object> props);
        Task CreateContestTableAsync(string nameTable, List<FieldsForNewContest> columns, GlobalConstants.TYPETABLE type);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, Option option, List<string> entryExclusionFields);
        Task<List<Dictionary<string, object>>> GetAllEntries(string nameTable, List<string> entryExclusionFields);
        Task PurgeSelectedEntries(string nameTable, string entriesID);
        Task PurgeAllEntries(string nameTable);
        Task<Dictionary<string, object>> FindEntries(string contestUniqueCode, Dictionary<string, object> props, List<ColumnMetadata> tableColumns, IDbContextTransaction transaction);
        Task<List<ColumnMetadata>> GetTableColumnsAsync(string contestUniqueCode, IDbContextTransaction transaction);
    }
}
