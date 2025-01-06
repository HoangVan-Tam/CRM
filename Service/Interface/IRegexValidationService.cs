using Entities.DTO;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IRegexValidationService
    {
        Task<FunctionResults<List<RegexValidation>>> LoadAllRegexAsync();
        Task<FunctionResults<RegexValidation>> CreateNewRegexValidation(NewRegexValidation newRegexValidation);
    }
}
