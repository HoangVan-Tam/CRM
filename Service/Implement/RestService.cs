using Entities.DTO;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class RestService : IRestService
    {
        public RestService() { }
        public async Task<FunctionResults<string>> GetAndPostFunction(Parameters parameters)
        {
            // try
            // {
            //     //UserID and Keyword
            //     DateTime dt = DateTime.UtcNow;

            //     var Result = Repo.SubmitEntry(body);
            //     string response = string.Empty;

            //     if (body.SendResponse && (Result.IsSendSMS && Repo.AppID != "" && Repo.AppSecret != ""))/* && body.EntrySource !="API" */ //UnComment this to prevent API Entries from sending responses. 
            //     {
            //         if ((Repo.AppID != "" && Repo.AppSecret != "") &&
            //                 Result.Entry.EntrySource.Equals("SMS", StringComparison.InvariantCultureIgnoreCase))
            //         {
            //             response = GeneralFunctions.SendSms(Convert.ToInt32(Repo.AppID), new Guid(Repo.AppSecret), body.MobileNo.ToString(),
            //                         Result.Entry.Response);
            //         }
            //         else if (Result.Entry.EntrySource.Equals("Whatsapp", StringComparison.InvariantCultureIgnoreCase))
            //         {
            //             response = GeneralFunctions.SendWhatsapp(body.MobileNo.ToString(),
            //                       "text", Result.Entry.Response);
            //         }

            //     }

            //     return "OK";

            // }
            // catch (Exception ex)
            // {
            //     var ErrMsg = "Error : " + ex.Message + ex.StackTrace.ToString();
            //     Repo.ErrorLog(ErrMsg);
            //     return ErrMsg;
            // }

            return new FunctionResults<string>();
        }
    }
}
