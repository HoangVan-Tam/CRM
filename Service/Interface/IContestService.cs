using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IContestService
    {
        Task<FunctionResults<List<ContestOverView>>> LoadAllContestAsync();
        Task<FunctionResults<List<string>>> CreateNewContestAsync(NewContestInfomation newContestIfno, List<FieldsForNewContest> lstColumns);
        Task<FunctionResults<object>> CheckDoesContestExist(string contestUniqueCode);
    }
}
