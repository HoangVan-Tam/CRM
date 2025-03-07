using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Constants.GlobalConstants;

namespace Services.Interface
{
    public interface IContestFieldDetailsService
    {
        Task<FunctionResults<List<FormField>>> GetAllFieldsOfTheContestForFormAsync(string contestUniqueCode, TypeSubmitForm type);
    }
}
