using AutoMapper;
using DAL.Interface;
using Entities.DTO;
using Entities.Models;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Constants.GlobalConstants;

namespace Services.Implement
{
    public class ContestFieldDetailsService : IContestFieldDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ContestFieldDetailsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<FunctionResults<List<FormField>>> GetAllFieldsOfTheContestForFormAsync(string contestUniqueCode, TypeSubmitForm type)
        {
            FunctionResults<List<FormField>> response = new FunctionResults<List<FormField>>();
            try
            {
                var fields = new List<ContestFieldDetails>();
                if (type == TypeSubmitForm.OnlinePage)
                {
                    fields = await _unitOfWork.ContestFieldDetail.FindAllWithIncludeAsync(p => p.Contest.ContestUniqueCode == contestUniqueCode && p.ShowOnlinePage == true, x => x.RegexValidation);
                }
                else
                {
                    fields = await _unitOfWork.ContestFieldDetail.FindAllWithIncludeAsync(p => p.Contest.ContestUniqueCode == contestUniqueCode && p.ShowOnlineCompletion == true, x => x.RegexValidation);
                }
                if (fields.Count() > 0)
                {
                    response.Data = _mapper.Map<List<FormField>>(fields);
                }
                else
                {
                    response.Message = "No Columns Found!";
                }
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
    }
}
