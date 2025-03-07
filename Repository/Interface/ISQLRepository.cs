using Entities.Constants;
using Entities.DTO;
using Entities.Helper;
using Entities.Models;
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
        Task<List<Dictionary<string, object>>> GetEntriesByCondition(string tableName, Dictionary<string, object> conditionProps, IDbContextTransaction transaction);
        Task InsertEntriesAsync(Contest contest, Dictionary<string, object> props, IEnumerable<ColumnMetadata> tableColumns, IDbContextTransaction transaction);
        Task UpdateEntriesAsync(string tableName, string verificationCode, List<ColumnMetadata> tableColumns, Dictionary<string, object> updatedProperties, IDbContextTransaction dbTransaction);
        Task InsertLogAsync(string contestUniqueCode, Dictionary<string, object> props);
        Task CreateContestTableAsync(string tableName, List<FieldsForNewContest> columns, GlobalConstants.TYPETABLE type, IDbContextTransaction transaction);
        Task<List<Dictionary<string, object>>> GetAllEntries(string tableName, Option option, List<string> entryExclusionFields);
        Task<List<Dictionary<string, object>>> GetAllEntries(string tableName, List<string> entryExclusionFields);
        Task PurgeSelectedEntries(string tableName, string entriesID);
        Task PurgeAllEntries(string tableName);
        Task<List<Dictionary<string, object>>> FindEntriesAsync(string tableName, Dictionary<string, object> props, List<ColumnMetadata> tableColumns, IDbContextTransaction transaction);
        Task<List<ColumnMetadata>> GetTableColumnsAsync(string contestUniqueCode, IDbContextTransaction transaction);
    }
}
