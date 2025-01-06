using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IContestFieldDetailsService
    {
        Task<FunctionResults<List<OnlinePageInfomation>>> GetAllFielsdOfTheContestForOnlinePageAsync(string contestUniqueCode);
    }
}
