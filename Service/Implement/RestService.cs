using DAL.Interface;
using Entities.DTO;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class RestService : IRestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntriesService _entriesService;
        private readonly IUtilityService _utilityService;
        public RestService(IUnitOfWork unitOfWork, IEntriesService entriesService, IUtilityService utilityService)
        {
            _unitOfWork = unitOfWork;
            _entriesService = entriesService;
            _utilityService = utilityService;
        }
        public async Task<FunctionResults<string>> GetAndPostFunctionAsync(Parameters body)
        {
            var res = new FunctionResults<string>();
            try
            {
                //UserID and Keyword
                DateTime dt = DateTime.UtcNow;
                var contest = await _unitOfWork.Contest.GetContestWithContestFieldDetailsAndRegexValidationsAsync(body.ContestUniqueCode);
                if (contest != null)
                {
                    var Result = await _entriesService.SubmitEntryAPI(body, contest);
                    string response = string.Empty;

                    if (body.SendResponse && (Result.IsSuccess && contest.AppId.ToString() != "" && contest.AppSecret != ""))/* && body.EntrySource !="API" */ //UnComment this to prevent API Entries from sending responses. 
                    {
                        if ((contest.AppId.ToString() != "" && contest.AppSecret != "") &&
                                Result.Data["EntrySource"].ToString().Equals("SMS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            response = await _utilityService.SendSms(contest, body.MobileNo.ToString(),
                                        Result.Data["Response"].ToString());
                        }
                        else if (Result.Data["EntrySource"].ToString().Equals("Whatsapp", StringComparison.InvariantCultureIgnoreCase))
                        {
                            response = await _utilityService.SendWhatsapp(contest, body.MobileNo.ToString(),
                                      "text", Result.Data["Response"].ToString());
                        }
                    }
                    return res;
                }
                res.IsSuccess = false;
                return res;
            }
            catch (Exception ex)
            {
                var ErrMsg = "Error : " + ex.Message + ex.StackTrace.ToString();
                res.IsSuccess = false;
                res.Error = ErrMsg;
                return res;
            }
        }
    }
}
