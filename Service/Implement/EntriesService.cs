using AutoMapper;
using Azure;
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
using System.Collections;
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

        public async Task<FunctionResults<string>> APISubmitEntry(Parameters parameters, string contestUniqueCode)
        {
            // DateEntry, EntryText, IsValid, Reason, Response, VerificationCode, Chances, EntrySource,
            var contest = await _unitOfWork.Contest.FindAsync(p => p.ContestUniqueCode == contestUniqueCode, c => c.ContestFieldDetails, c => c.ContestFieldDetails.Select(o => o.RegexValidation));
            if (contest != null)
            {
                var props = new Dictionary<string, object>();
                var nameTable = "BC_" + contestUniqueCode;
                if (parameters.EntrySource.Equals("Sms", StringComparison.InvariantCultureIgnoreCase))
                {
                    props.Add("EntrySource", "SMS");
                }
                else if (parameters.EntrySource.Equals("Whatsapp", StringComparison.InvariantCultureIgnoreCase))
                {
                    props.Add("EntrySource", "Whatsapp");
                }
                FunctionResults<string> response = new FunctionResults<string>();
                var CleanedMessage = Helper.CleanMessage(parameters, contest.Keyword);
                var CreatedOn = parameters.CreatedOn != null && parameters.CreatedOn != "" ? DateTime.Parse(parameters.CreatedOn.ToString()).ToUniversalTime() : System.DateTime.UtcNow;
                props.Add("DateEntry", CreatedOn);

                var ValidationResults = PreMatchFieldValidations(contest, CreatedOn);
                if (!ValidationResults.IsSuccess)
                {
                    props.Add("IsValid", false);
                    props.Add("Reasons", ValidationResults.Error);
                    props.Add("Response", ValidationResults.Message);
                }

                #region SpecificRegexMatching
                Regex regex = new Regex(contest.ValidationRegexFull, RegexOptions.IgnoreCase);
                var MatchedMessage = regex.Match(CleanedMessage.Trim());

                // Set Fields which dont require validation
                props.Add("MobileNo", parameters.MobileNo);
                props.Add("EntryText", parameters.Message.ToString());
                props.Add("FileLink", (parameters.EntrySource == "MMS" || parameters.EntrySource == "Whatsapp") && !string.IsNullOrEmpty(parameters.FileLink) ? parameters.FileLink.ToString() : "");

                if (MatchedMessage.Success)
                {
                    //Matched fields will now have a space infront of them because the space is now inside the regex of each field.
                    var MatchedResultList = MatchedMessage.Groups.Cast<Group>().Select(match => match.Value).Skip(1).ToList().Select(s => s.Trim()).ToList();

                    var FieldsL = contest.SMSSubmitFields.Split(',').ToList();
                    for (int k = 0; k < FieldsL.Count; k++)
                    {
                        //logic will handle invalid or blank amounts
                        var field = contest.ContestFieldDetails.Where(p => p.FieldName == FieldsL[k]).FirstOrDefault();
                        if (field != null)
                        {
                            //Other fields which require special Regex
                            if (FieldsL[k] == "ReceiptNo")
                            {
                                var regexReceipt = new Regex(@field.RegexValidation.Pattern);
                                Regex rgx = new Regex("[^a-zA-Z0-9]");
                                var str = rgx.Replace(MatchedResultList[k], "");

                                //var AMTmatch = regexReceipt.Match(MatchedResultList[k]);
                                var rgxMatch = regexReceipt.Match(str);

                                if (rgxMatch.Success)
                                {
                                    props.Add(FieldsL[k], rgxMatch.Value);
                                }
                            }
                            else
                            {
                                var regexPattern = field.RegexValidation.Pattern;
                                var rgx = new Regex(@regexPattern);
                                var matchRegex = rgx.Match(MatchedResultList[k]);
                                if (matchRegex.Success)
                                {
                                    props.Add(FieldsL[k], MatchedResultList[k]);
                                }
                            }
                        }
                    }
                    //Check Repeat Validation
                    var uniqueFields = contest.ContestFieldDetails.Where(p => p.IsUnique == true);
                    var isUniqueEntry = await CheckUniqueOfEntryAsync(contestUniqueCode, uniqueFields, props);
                    if (!isUniqueEntry)
                    {
                        props.Add("IsValid", false);
                        props.Add("ProcessEntryStatus", Constants.ProcessEntryStatus.Repeated);
                    }
                    props.Add("IsSaveable", false);
                }
                else
                {
                    //If Invalid, for loop form regex on the fly to determine which field have error(use Key) RegexKeyPairValue,

                    var FieldsL = contest.SMSSubmitFields.Split(',').ToList();
                    var BuildingRegex = "";
                    for (int k = 0; k < FieldsL.Count; k++)
                    {
                        var CurrentFieldRegex = contest.ContestFieldDetails.Where(p=>p.FieldName == FieldsL[k]).Select(p=>p.RegexValidation.Pattern).FirstOrDefault();
                        if(CurrentFieldRegex != null)
                        {
                            if (BuildingRegex == "")
                            {
                                BuildingRegex = "(" + CurrentFieldRegex.Remove(CurrentFieldRegex.Length - 1).Remove(0, 1) + ")";
                            }
                            else
                            {
                                BuildingRegex = BuildingRegex + "(" + " " + CurrentFieldRegex.Remove(CurrentFieldRegex.Length - 1).Remove(0, 1) + ")";
                            }

                            var TestingRegex = k == FieldsL.Count - 1 ? "^" + BuildingRegex + "$" : "^" + BuildingRegex + "( ?)";

                            Regex InvalidedRegex = new Regex(TestingRegex, RegexOptions.IgnoreCase);
                            var MatchNowOrNot = InvalidedRegex.Match(CleanedMessage.Trim());

                            if (!MatchNowOrNot.Success)
                            {
                                //Validation has failed at this field, therefore we assume this field to be the one causing the error.
                                props.Add("Reason", FieldsL[k] + " Is Not Valid!");
                            }
                        }
                    }

                    props.Add("IsValid", false);
                    props.Add("IsSaveable", false);
                    props.Add("ProcessEntryStatus", Constants.ProcessEntryStatus.FieldInvalid);
                }
                #endregion

                //upload filelink to azure blob
                if (props["FileLink"].ToString() != "" && props["EntrySource"].ToString() == "Whatsapp")
                {
                    //get filename
                    Uri uri = new Uri(props["FileLink"].ToString());
                    string filename = string.Empty;

                    filename = System.IO.Path.GetFileName(uri.LocalPath);

                    using (var webClient = new WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(props["FileLink"].ToString());

                        string fl;
                        var fileresult = SendByteToAzureStorage(FunctRes.Entry, imageBytes, filename, out fl);

                        if (!fileresult.Valid)
                        {
                            return fileresult;
                        }

                        props["FileLink"] = fl;
                    }
                }




                //Decide whether to save entryfields based on validity
                int EntryID;

                var temp = SaveEntry(props);
            }
            return new FunctionResults<string>();
        }

        public async Task<bool> CheckUniqueOfEntryAsync(string contestUniqueCode, IEnumerable<ContestFieldDetails> uniqueFields, Dictionary<string, object> props)
        {
            await _sqlConnection.OpenAsync();
            try
            {
                var uniqueProps = new Dictionary<string, object>();
                foreach (var item in uniqueFields)
                {
                    uniqueProps.Add(item.FieldName, props[item.FieldName]);
                }
                var entries = await _unitOfWork.LinqToSQL.FindEntries(contestUniqueCode, uniqueProps, _sqlConnection);
                if(entries.Count() > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                await _sqlConnection.CloseAsync();
                return false;
            }
        }

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