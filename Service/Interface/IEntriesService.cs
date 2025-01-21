using Entities.DTO;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IEntriesService
    {
        Task<FunctionResults<string>> SaveEntries(string ContestEntriesTableName, string columns, string values, IBrowserFile file);
        Task<FunctionResults<List<Dictionary<string, object>>>> GetAllEntriesAsync(string ContestUniqueCode, Option option);
        Task<FunctionResults<string>> PurgeSelectedEntriesAsync(string ContestEntriesTableName, List<int> entriesID);
        Task<FunctionResults<string>> PurgeAllEntriesAsync(string ContestEntriesTableName);
        Task<FunctionResults<byte[]>> GetEntriesCSV(string ContestUniqueCode);
        Task<FunctionResults<string>> SubmitEntry(Parameters parameters, string contestUniqueCode);
    }
}
