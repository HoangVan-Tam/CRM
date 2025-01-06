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
    public class ContestFieldsService : IContestFieldsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ContestFieldsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FunctionResults<List<FieldsForNewContest>>> LoadAllColumn()
        {
            FunctionResults<List<FieldsForNewContest>> response = new FunctionResults<List<FieldsForNewContest>>();
            try
            {
                var allColumn = await _unitOfWork.ContestFields.GetAllAsync();
                response.Data = _mapper.Map<List<FieldsForNewContest>>(allColumn);
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
