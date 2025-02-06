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

namespace Services.Implement
{
    public class RegexValidationService : IRegexValidationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RegexValidationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FunctionResults<List<RegexValidation>>> LoadAllRegexAsync()
        {
            FunctionResults<List<RegexValidation>> response = new FunctionResults<List<RegexValidation>>();
            try
            {
                var lstRegex = await _unitOfWork.RegexValidation.GetAllAsync();
                response.Data = lstRegex.ToList();
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<FunctionResults<RegexValidation>>
        CreateNewRegexValidation(NewRegexValidation newRegexValidation)
        {
            FunctionResults<RegexValidation> response = new FunctionResults<RegexValidation>();
            try
            {
                var newRegex = _mapper.Map<RegexValidation>(newRegexValidation);
                var isExistRegexName = await _unitOfWork.RegexValidation.FindAsync(p => p.Name == newRegex.Name);
                if (isExistRegexName != null)
                {
                    response.IsSuccess = false;
                    response.Error = "Name of the regex is repeated";
                    return response;
                }
                await _unitOfWork.RegexValidation.InsertAsync(newRegex);
                await _unitOfWork.SaveAsync();
                response.Data = newRegex;
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
