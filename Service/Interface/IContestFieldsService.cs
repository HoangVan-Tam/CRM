using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IContestFieldsService
    {
        Task<FunctionResults<List<FieldsForNewContest>>> LoadAllColumn();
    }
}
