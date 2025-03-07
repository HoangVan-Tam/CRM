using Entities.DTO;
using Entities.Models;
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
        Task<FunctionResults<Dictionary<string, object>>> GetEntryByVerificationCode(string ContestUniqueCode, string verificationCode);
        Task<FunctionResults<string>> InsertEntry(string contestUniqueCode, Dictionary<string, object> props, IBrowserFile file);
        Task<FunctionResults<List<Dictionary<string, object>>>> GetAllEntriesAsync(string ContestUniqueCode, Option option);
        Task<FunctionResults<string>> PurgeSelectedEntriesAsync(string ContestEntriesTableName, List<int> entriesID);
        Task<FunctionResults<string>> PurgeAllEntriesAsync(string ContestEntriesTableName);
        Task<FunctionResults<byte[]>> GetEntriesCSV(string ContestUniqueCode);
        Task<FunctionResults<Dictionary<string, object>>> SubmitEntryAPI(Parameters body, Contest contest);
        Task<FunctionResults<string>> CompleteEntry(string contestCode, string verificationCode, Dictionary<string, object> entryData, IBrowserFile file);
    }
}
