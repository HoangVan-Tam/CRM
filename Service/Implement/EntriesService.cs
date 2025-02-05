using AutoMapper;
using CsvHelper;
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
using Entities.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Services.Common;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class EntriesService : IEntriesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private SqlConnection _sqlConnection;
        private IConfiguration _config;

        public EntriesService(IUnitOfWork unitOfWork, IMapper mapper, SqlConnection sqlConnection, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sqlConnection = sqlConnection;
            _config = config;
            _sqlConnection.ConnectionString = _config.GetConnectionString("BaseDB");
        }

        public async Task<FunctionResults<List<Dictionary<string, object>>>> GetAllEntriesAsync(string ContestUniqueCode, Option option)
        {
            FunctionResults<List<Dictionary<string, object>>> response = new FunctionResults<List<Dictionary<string, object>>>();
            await _sqlConnection.OpenAsync();
            try
            {
                ContestUniqueCode = "010101_TIGER";
                var contest = await _unitOfWork.Contest.FindAsync(p => p.ContestUniqueCode == ContestUniqueCode);
                var entryExclusionFields = contest.EntryExclusionFields.Split(",").ToList();
                response.Data = await _unitOfWork.LinqToSQL.GetAllEntries(ContestUniqueCode, option, entryExclusionFields, _sqlConnection);
                await _sqlConnection.CloseAsync();
            }
            catch (Exception ex)
            {
                await _sqlConnection.CloseAsync();
                response.IsSuccess = false;
                response.Error = ex.Message;
            }
            return response;
        }

        public async Task<FunctionResults<string>> PurgeSelectedEntriesAsync(string ContestUniqueCode, List<int> entriesID)
        {
            FunctionResults<string> response = new FunctionResults<string>();
            await _sqlConnection.OpenAsync();
            try
            {
                await _unitOfWork.LinqToSQL.PurgeSelectedEntries(ContestUniqueCode, String.Join(", ", entriesID.ConvertAll<string>(x => x.ToString())), _sqlConnection);
                await _sqlConnection.CloseAsync();
                response.Data = "Deleted Selected Entries";
            }
            catch (Exception ex)
            {
                await _sqlConnection.CloseAsync();
                response.IsSuccess = false;
                response.Error = ex.Message;
            }
            return response;
        }

        public async Task<FunctionResults<string>> PurgeAllEntriesAsync(string ContestUniqueCode)
        {
            FunctionResults<string> response = new FunctionResults<string>();
            await _sqlConnection.OpenAsync();
            try
            {
                await _unitOfWork.LinqToSQL.PurgeAllEntries(ContestUniqueCode, _sqlConnection);
                await _sqlConnection.CloseAsync();
                response.Data = "Deleted All Entries";
            }
            catch (Exception ex)
            {
                await _sqlConnection.CloseAsync();
                response.IsSuccess = false;
                response.Error = ex.Message;
            }
            return response;
        }

        public async Task<FunctionResults<List<Dictionary<string, object>>>> PickWinnerAsync(string ContestUniqueCode, Option option)
        {
            FunctionResults<List<Dictionary<string, object>>> response = new FunctionResults<List<Dictionary<string, object>>>();
            //await _sqlConnection.OpenAsync();
            //try
            //{
            //    response.Data = await _unitOfWork.LinqToSQL.GetAllEntries(ContestEntriesTableName, option, _sqlConnection);
            //    await _sqlConnection.CloseAsync();
            //}
            //catch (Exception ex)
            //{
            //    await _sqlConnection.CloseAsync();
            //    response.IsSuccess = false;
            //    response.Error = ex.Message;
            //}
            return response;
        }

        /// <summary>
        /// Save entries to database
        /// </summary>
        /// <param name="ContestUniqueCode"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<FunctionResults<string>> SaveEntries(string ContestUniqueCode, string columns, string values, IBrowserFile file)
        {
            FunctionResults<string> response = new FunctionResults<string>();
            await _sqlConnection.OpenAsync();
            try
            {
                var completionColumns = "DateEntry, EntryText, IsValid, IsVerified, IsRejected, Reason, Response, VerificationCode, Chances, EntrySource, ";
                completionColumns = completionColumns + columns;
                var dateEntry = DateTime.UtcNow;
                var completionValue = "'" + dateEntry + "', " + "'', " + "'1', "
                    + "'0', " + "'0', " + "'', " + "'', " + "'', " + "'1', " + "'WEB', ";
                completionValue = completionValue + values;
                var nameTable = "BC_" + ContestUniqueCode;
                await _unitOfWork.LinqToSQL.InsertAsync(nameTable, completionColumns, completionValue, _sqlConnection);
                response.Message = "Insert Entry Successfully";
                await _sqlConnection.CloseAsync();
            }
            catch (Exception ex)
            {
                await _sqlConnection.CloseAsync();
                response.IsSuccess = false;
                response.Error = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ContestUniqueCode"></param>
        /// <returns></returns>
        public async Task<FunctionResults<byte[]>> GetEntriesCSV(string ContestUniqueCode)
        {
            FunctionResults<byte[]> response = new FunctionResults<byte[]>();
            await _sqlConnection.OpenAsync();
            try
            {
                ContestUniqueCode = "010101_TIGER";
                var contest = await _unitOfWork.Contest.FindAsync(p => p.ContestUniqueCode == ContestUniqueCode);
                var entryExclusionFields = contest.EntryExclusionFields.Split(",").ToList();
                var dataEntries = await _unitOfWork.LinqToSQL.GetAllEntries(ContestUniqueCode, entryExclusionFields, _sqlConnection);
                response.Data = Common.Helper.ExportToCsv(dataEntries);
                //using (var writer = new StreamWriter("entries.csv"))
                //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                //{
                //    foreach (var dic in data)
                //    {
                //        foreach (var pair in dic)
                //        {
                //            csv.WriteField(pair.Value);                           
                //        }
                //        csv.NextRecord();
                //    }
                //    csv.Flush();
                //}
                await _sqlConnection.CloseAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Error = ex.Message;
                await _sqlConnection.CloseAsync();
            }
            return response;
        }

        //public async Task<FunctionResults<string>> APISubmitEntry(Parameters parameters, string contestUniqueCode)
        //{
        //    // DateEntry, EntryText, IsValid, Reason, Response, VerificationCode, Chances, EntrySource,
        //    var contest = await _unitOfWork.Contest.FindAsync(p => p.ContestUniqueCode == contestUniqueCode, c => c.ContestFieldDetails, c => c.ContestFieldDetails.Select(o => o.RegexValidation));
        //    if (contest != null)
        //    {
        //        var props = new Dictionary<string, object> { { "IsVerified", 0 }, { "IsRejected", 0 } };
        //        var nameTable = "BC_" + contestUniqueCode;
        //        if (parameters.EntrySource.Equals("Sms", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            props.Add("EntrySource", "SMS");
        //        }
        //        else if (parameters.EntrySource.Equals("Whatsapp", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            props.Add("EntrySource", "Whatsapp");
        //        }
        //        FunctionResults<string> response = new FunctionResults<string>();
        //        var CleanedMessage = Helper.CleanMessage(parameters, contest.Keyword);
        //        var CreatedOn = parameters.CreatedOn != null && parameters.CreatedOn != "" ? DateTime.Parse(parameters.CreatedOn.ToString()).ToUniversalTime() : System.DateTime.UtcNow;
        //        props.Add("DateEntry", CreatedOn);

        //        var ValidationResults = PreMatchFieldValidations(contest, CreatedOn);
        //        if (!ValidationResults.IsSuccess)
        //        {
        //            props.Add("IsValid", false);
        //            props.Add("Reasons", ValidationResults.Error);
        //            props.Add("Response", ValidationResults.Message);
        //        }

        //        #region SpecificRegexMatching
        //        Regex regex = new Regex(contest.ValidationRegexFull, RegexOptions.IgnoreCase);
        //        var MatchedMessage = regex.Match(CleanedMessage.Trim());
        //        // Set Fields which dont require validation
        //        props.Add("MobileNo", parameters.MobileNo);
        //        props.Add("EntryText", parameters.Message.ToString());
        //        props.Add("FileLink", (parameters.EntrySource == "MMS" || parameters.EntrySource == "Whatsapp") && !string.IsNullOrEmpty(parameters.FileLink) ? parameters.FileLink.ToString() : "");

        //        if (MatchedMessage.Success)
        //        {
        //            //Matched fields will now have a space infront of them because the space is now inside the regex of each field.
        //            var MatchedResultList = MatchedMessage.Groups.Cast<Group>().Select(match => match.Value).Skip(1).ToList().Select(s => s.Trim()).ToList();

        //            var FieldsL = contest.SMSSubmitFields.Split(',').ToList();
        //            for (int k = 0; k < FieldsL.Count; k++)
        //            {
        //                //logic will handle invalid or blank amounts
        //                var field = contest.ContestFieldDetails.Where(p => p.FieldName == FieldsL[k]).FirstOrDefault();
        //                if (field != null)
        //                {
        //                    //Other fields which require special Regex
        //                    if (FieldsL[k] == "ReceiptNo")
        //                    {
        //                        var regexReceipt = new Regex(@field.RegexValidation.Pattern);
        //                        Regex rgx = new Regex("[^a-zA-Z0-9]");
        //                        var str = rgx.Replace(MatchedResultList[k], "");

        //                        //var AMTmatch = regexReceipt.Match(MatchedResultList[k]);
        //                        var rgxMatch = regexReceipt.Match(str);

        //                        if (rgxMatch.Success)
        //                        {
        //                            props.Add(FieldsL[k], rgxMatch.Value);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var regexPattern = field.RegexValidation.Pattern;
        //                        var rgx = new Regex(@regexPattern);
        //                        var matchRegex = rgx.Match(MatchedResultList[k]);
        //                        if (matchRegex.Success)
        //                        {
        //                            props.Add(FieldsL[k], MatchedResultList[k]);
        //                        }
        //                    }
        //                }
        //            }
        //            ValidationResults = null;
        //            ValidationResults = ValidationLogics(FunctRes.Entry, FunctRes.Entry.EntrySource);
        //            if (!ValidationResults.IsSuccessful)
        //            {
        //                FunctRes.Entry.IsValid = false;

        //                FunctRes.ListOfReasonsForPossibleFailures.Add(ValidationResults.ReasonForFailure);
        //                FunctRes.ListOfResponsesForPossibleFailures.Add(ValidationResults.ResponseForFailure);

        //            }

        //            FunctRes.IsSavable = true;
        //        }
        //        #endregion
        //    }
        //    return new FunctionResults<string>();
        //}

        //public FunctionResults<string> ValidationLogics(Contest contest, string type)
        //{
        //    //Validation

        //    #region Validation

        //    var regex = new Regex("");
        //    var Match = regex.Match("");

        //    if (type == "WEB") //Only online Entries need to validate Field By Field.
        //    {
        //        regex = new Regex(ValidationRegexMobileNo, RegexOptions.IgnoreCase);
        //        Match = regex.Match(Entry.MobileNo.Trim());

        //        if (!Match.Success)
        //        {
        //            return new ValidationResult(false) { ReasonForFailure = "Mobile is Invalid!", ResponseForFailure = "Mobile is Invalid!"/*errMessage*/ };
        //        }

        //        regex = new Regex(ValidationRegexName, RegexOptions.IgnoreCase);
        //        Match = regex.Match(Entry.Name.Trim());

        //        if (!Match.Success)
        //        {
        //            return new ValidationResult(false) { ReasonForFailure = "Name is Invalid!", ResponseForFailure = "Name is Invalid!"/*errMessage*/ };
        //        }

        //        //regex = new Regex(ValidationRegexNRIC, RegexOptions.IgnoreCase);
        //        //Match = regex.Match(Entry.NRIC.Trim());

        //        //if (!Match.Success)
        //        //{
        //        //    return new ValidationResult(false) { ReasonForFailure = "NRIC is Invalid!", ResponseForFailure = "NRIC is Invalid!"/*ErrorMessageGeneric*/ };
        //        //}


        //        //regex = new Regex(ValidationRegexReceiptNo, RegexOptions.IgnoreCase);
        //        //Match = regex.Match(Entry.ReceiptNo.Trim());

        //        //if (!Match.Success)
        //        //{
        //        //    return new ValidationResult(false) { ReasonForFailure = "ReceiptNo is Invalid!", ResponseForFailure = ErrorMessageGeneric };
        //        //}


        //        //regex = new Regex(ValidationRegexEmail, RegexOptions.IgnoreCase);
        //        //Match = regex.Match(Entry.Email.Trim());

        //        //if (!Match.Success)
        //        //{
        //        //return new ValidationResult(false) { ReasonForFailure = "Email is Invalid!", ResponseForFailure = ErrorMessageGeneric };
        //        //}
        //    }
        //    if (type == "Whatsapp")
        //    {
        //        if (string.IsNullOrEmpty(Entry.FileLink))
        //        {
        //            return new ValidationResult(false) { ReasonForFailure = "Image is required!", ResponseForFailure = errMessage };
        //        }
        //    }
        //    #endregion

        //    if (Convert.ToDecimal(Entry.Amount) < TierAmount)
        //    {
        //        return new ValidationResult(false) { ReasonForFailure = "Amount is below " + TierAmount.ToString() + "!", ResponseForFailure = ErrorMessageAmount.Replace("{tierAmount}", TierAmount.ToString()) };
        //    }

        //    var EntryMobile = (Entry.MobileNo.Substring(0, 1) == "+" ?/*2*/ Entry.MobileNo.Substring(1, Entry.MobileNo.Length - 1) :/*2*/ Entry.MobileNo);

        //    //Remove leading 0 on receiptNo
        //    Entry.ReceiptNo = Entry.ReceiptNo.TrimStart('0');

        //    //Check for duplicates


        //    /*Check For Receipt*/
        //    //var ReceiptCheckQuery = db.BC_241226_POKKACNY25.Where(s => s.IsValid == true).Where(s => s.ReceiptNo.ToUpper() == Entry.ReceiptNo.ToUpper());
        //    //var ReceiptCheckCount = ReceiptCheckQuery.Count();
        //    //if (ReceiptCheckCount > 0)
        //    //{
        //    //    /* When repeated message contains {uploadlink} */
        //    //    return new ValidationResult(false)
        //    //    {
        //    //        ReasonForFailure = "Repeated entry!",
        //    //        ResponseForFailure = RepeatedMessageSMS.Replace("{uploadlink}", UploadLink + "?i=" + ReceiptCheckQuery.FirstOrDefault().VerificationCode)
        //    //    };

        //    //    // return new ValidationResult(false) { ReasonForFailure = "Repeated entry!", ResponseForFailure = RepeatedMessageSMS };
        //    //}

        //    /*Check For Receipt AND Mobile*/
        //    var ReceiptMobileCheckQuery = db.BC_241226_POKKACNY25.Where(s => s.IsValid == true).Where(s => s.ReceiptNo.ToUpper() == Entry.ReceiptNo.ToUpper()
        //    &&
        //    (s.MobileNo.Substring(0, 1) == "+" ?/*1*/ s.MobileNo.Substring(1, s.MobileNo.Length - 1) ==
        //    EntryMobile
        //    :/*1*/  s.MobileNo ==
        //    EntryMobile));
        //    var ReceiptMobileCheckCount = ReceiptMobileCheckQuery.Count();
        //    if (ReceiptMobileCheckCount > 0)
        //    {
        //        /* When repeated message contains {uploadlink} */

        //        return new ValidationResult(false)
        //        {
        //            ReasonForFailure = "Repeated entry!",
        //            ResponseForFailure = RepeatedMessageSMS.Replace("{uploadlink}", UploadLink + "?i=" + ReceiptMobileCheckQuery.FirstOrDefault().VerificationCode)
        //        };

        //        //return new ValidationResult(false) { ReasonForFailure = "Repeated entry!", ResponseForFailure = RepeatedMessageSMS };
        //    }
        //    //if managed to pass all logics then return true
        //    return new ValidationResult(true);
        //}

        public FunctionResults<string> PreMatchFieldValidations(Contest contest, DateTime dt)
        {
            //Campaign Date Checking
            if (dt < contest.TestDate || dt > contest.EndDate)
            {

                return new FunctionResults<string>()
                {
                    IsSuccess = false,
                    Error = "Not In Campaign Period",
                    Message = "Error : Not within campaign period (From " + contest.TestDate.ToString("dd MMM yyyy") + " to " + contest.EndDate.ToString("dd MMM yyyy") + ")"
                };
            }
            //Campaign Date Checking
            return new FunctionResults<string>()
            {
                IsSuccess = true,
            };
        }
    }
}