using AutoMapper;
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class ContestService : IContestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private SqlConnection _sqlConnection;
        private IConfiguration _config;
        public ContestService(IUnitOfWork unitOfWork, IMapper mapper, SqlConnection sqlConnection, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sqlConnection = sqlConnection;
            _config = config;
            _sqlConnection.ConnectionString = _config.GetConnectionString("BaseDB");
        }

        public async Task<FunctionResults<List<ContestOverView>>> LoadAllContestAsync()
        {
            FunctionResults<List<ContestOverView>> response = new FunctionResults<List<ContestOverView>>();
            try
            {
                var lstContests = await _unitOfWork.Contest.GetAllAsync();
                if (lstContests.Count() > 0)
                {
                    response.Data = _mapper.Map<List<ContestOverView>>(lstContests);
                }
                else
                {
                    response.Message = "No Contests Found!";
                }
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<FunctionResults<List<string>>> CreateNewContestAsync(NewContestInfomation newContestIfno)
        {
            FunctionResults<List<string>> response = new FunctionResults<List<string>>();
            await _sqlConnection.OpenAsync();
            var transaction = _sqlConnection.BeginTransaction();
            try
            {
                var newContest = _mapper.Map<Contest>(newContestIfno);
                var contestUniqueCode = (newContest.StartDate.ToString("yyMMdd") + "_" + newContest.Keyword).ToUpper();
                newContest.ContestUniqueCode = contestUniqueCode;
                await _unitOfWork.Contest.InsertAsync(newContest);
                foreach (var col in newContestIfno.contestFields)
                {
                    col.ContestUniqueCode = contestUniqueCode;
                    var contestColumnDetail = _mapper.Map<ContestFieldDetails>(col);
                    contestColumnDetail.ContestID = newContest.ContestID;
                    await _unitOfWork.ContestFieldDetail.InsertAsync(contestColumnDetail);
                }
                await _unitOfWork.LinqToSQL.CreateContestTableAsync(contestUniqueCode, newContestIfno.contestFields, transaction, Constants.TYPETABLE.ENTRIES);
                await _unitOfWork.LinqToSQL.CreateContestTableAsync(contestUniqueCode, null, transaction, Constants.TYPETABLE.WINNERS);
                await _unitOfWork.LinqToSQL.CreateContestTableAsync(contestUniqueCode, null, transaction, Constants.TYPETABLE.LOG);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                await _sqlConnection.CloseAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _sqlConnection.CloseAsync();
                response.Error = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<FunctionResults<object>> CheckDoesContestExist(string contestUniqueCode)
        {
            FunctionResults<object> response = new FunctionResults<object>();
            try
            {
                var contest = await _unitOfWork.Contest.FindAsync(p => p.ContestUniqueCode == contestUniqueCode);
                if (contest != null)
                {
                    response.Data = true;
                }
                else
                {
                    response.Data = false;
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
