using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IRestService
    {
        Task<FunctionResults<string>> GetAndPostFunction(Parameters parameters);
    }
}
