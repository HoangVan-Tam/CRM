using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Net;
using System.Net.Http.Formatting;

namespace SMSDOME_Standard_Contest_BlazorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RESTController : ControllerBase
    {
        private IRestService _restService;
        public RESTController(IRestService restService) 
        {
            _restService = restService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Add/")]
        public async Task<IActionResult> Add([FromBody] FormDataCollection body)
        {
            var rtn = _restService.GetAndPostFunction(new Parameters
            {
                CreatedOn = body.Get("CreatedOn") == null ? null : body["CreatedOn"].ToString(),
                MobileNo = body.Get("MobileNo") == null ? null : body["MobileNo"].ToString(),
                Message = body.Get("Message") == null ? null : body["Message"].ToString(),
                FileLink = body.Get("FileLink") == null ? null : body["FileLink"].ToString(),
                EntrySource = (body["FileLink"] != null && body["FileLink"].ToString() != "") ? "MMS" : "SMS",
                SendResponse = true,
            });
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Add/")]
        public async Task<IActionResult> Add(string contestUniqueCode, string createdon, string MobileNo, string Message, string FileLink = "")
        { 
            var res = _restService.GetAndPostFunction(new Parameters
            {
                CreatedOn = createdon,
                MobileNo = MobileNo,
                Message = Message,
                FileLink = FileLink,
                EntrySource = (FileLink != null && FileLink.ToString() != "") ? "MMS" : "SMS",
                SendResponse = true,
                ContestUniqueCode = contestUniqueCode
            });
            return Ok();
        }
    }
}
