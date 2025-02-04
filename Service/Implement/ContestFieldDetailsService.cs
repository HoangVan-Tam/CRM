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
    public class ContestFieldDetailsService : IContestFieldDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ContestFieldDetailsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FunctionResults<List<OnlinePageInfomation>>> GetAllFielsdOfTheContestForOnlinePageAsync(string contestUniqueCode)
        {
            FunctionResults<List<OnlinePageInfomation>> response = new FunctionResults<List<OnlinePageInfomation>>();
            try
            {
                var lstContests = await _unitOfWork.ContestFieldDetail.FindAllWithIncludeAsync(p => p.Contest.ContestUniqueCode == contestUniqueCode && p.ShowOnlinePage == true, x => x.RegexValidation);
                if (lstContests.Count() > 0)
                {
                    response.Data = _mapper.Map<List<OnlinePageInfomation>>(lstContests);
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
