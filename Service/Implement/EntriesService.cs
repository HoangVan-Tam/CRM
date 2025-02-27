﻿using AutoMapper;
using Azure;
using CsvHelper;
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
using Entities.Helper;
using Entities.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
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
                response.Data = await _unitOfWork.SQL.GetAllEntries(ContestUniqueCode, option, entryExclusionFields);
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
                await _unitOfWork.SQL.PurgeSelectedEntries(ContestUniqueCode, String.Join(", ", entriesID.ConvertAll<string>(x => x.ToString())));
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
                await _unitOfWork.SQL.PurgeAllEntries(ContestUniqueCode);
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
            //    response.Data = await _unitOfWork.SQL.GetAllEntries(ContestEntriesTableName, option, _sqlConnection);
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
                //  await _unitOfWork.SQL.InsertAsync(nameTable, completionColumns, completionValue);
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
                var dataEntries = await _unitOfWork.SQL.GetAllEntries(ContestUniqueCode, entryExclusionFields);
                response.Data = Common.Helpers.ExportToCsv(dataEntries);
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

        public async Task<FunctionResults<Dictionary<string, object>>> APISubmitEntry(Parameters body, Contest contest)
        {
            await _unitOfWork.BeginEfTransactionAsync();
            var res = new FunctionResults<Dictionary<string, object>>();
            try
            {
                var props = new Dictionary<string, object>();
                var tableColumns = await _unitOfWork.SQL.GetTableColumnsAsync(body.ContestUniqueCode, _unitOfWork.CurrentEfTransaction);
                if (body.EntrySource.Equals("Sms", StringComparison.InvariantCultureIgnoreCase))
                {
                    props.Add("EntrySource", "SMS");
                }
                else if (body.EntrySource.Equals("Whatsapp", StringComparison.InvariantCultureIgnoreCase))
                {
                    props.Add("EntrySource", "Whatsapp");
                }
                FunctionResults<string> response = new FunctionResults<string>();
                var CleanedMessage = Helpers.CleanMessage(body, contest.Keyword);
                var CreatedOn = body.CreatedOn != null && body.CreatedOn != "" ? DateTime.Parse(body.CreatedOn.ToString()).ToUniversalTime() : System.DateTime.UtcNow;
                props.Add("DateEntry", CreatedOn);

                var ValidationResults = PreMatchFieldValidations(contest, CreatedOn);
                if (!ValidationResults.IsSuccess)
                {
                    props.Add("IsValid", false);
                    props.Add("Reasons", ValidationResults.Error);
                    props.Add("ProcessEntryStatus", GlobalConstants.ProcessEntryStatus.NotInCampaignPeriod);
                }

                #region SpecificRegexMatching
                Regex regex = new Regex(contest.ValidationRegexFull, RegexOptions.IgnoreCase);
                var MatchedMessage = regex.Match(CleanedMessage.Trim());

                // Set Fields which dont require validation
                props.Add("FileLink", (body.EntrySource == "MMS" || body.EntrySource == "Whatsapp") && !string.IsNullOrEmpty(body.FileLink) ? body.FileLink.ToString() : "");

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
                                    props.Add("" + FieldsL[k], MatchedResultList[k]);
                                }
                            }
                        }
                    }
                    //Check Repeat Validation
                    var uniqueFields = contest.ContestFieldDetails.Where(p => p.IsUnique == true);
                    var isUniqueEntry = await CheckUniqueOfEntryAsync(body.ContestUniqueCode, uniqueFields, props, tableColumns, _unitOfWork.CurrentEfTransaction);
                    if (!isUniqueEntry)
                    {
                        props.Add("IsValid", false);
                        props.Add("ProcessEntryStatus", GlobalConstants.ProcessEntryStatus.Repeated);
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
                        var CurrentFieldRegex = contest.ContestFieldDetails.Where(p => p.FieldName == FieldsL[k]).Select(p => p.RegexValidation.Pattern).FirstOrDefault();
                        if (CurrentFieldRegex != null)
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
                    props.Add("ProcessEntryStatus", GlobalConstants.ProcessEntryStatus.FieldInvalid);
                }
                #endregion

                //upload filelink to azure blob
                if (props["FileLink"]?.ToString() != "" && props["EntrySource"]?.ToString() == "Whatsapp")
                {
                    //get filename
                    Uri uri = new Uri(props["FileLink"].ToString());
                    string filename = string.Empty;

                    filename = System.IO.Path.GetFileName(uri.LocalPath);

                    using (var webClient = new WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(props["FileLink"].ToString());

                        string fl;
                        //var fileresult = SendByteToAzureStorage(FunctRes.Entry, imageBytes, filename, out fl);

                        // if (!fileresult.Valid)
                        // {
                        //     return fileresult;
                        // }

                        // props["FileLink"] = fl;
                    }
                }

                await _unitOfWork.SQL.InsertEntriesAsync(contest.ContestUniqueCode, props, tableColumns, _unitOfWork.CurrentEfTransaction);
                res.Data = props;
                //var temp = SaveEntry(props);

                await _unitOfWork.CommitAsync();
                return res;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                res.Error = ex.Message;
                res.IsSuccess = false;
                return res;
            }
        }

        public async Task<bool> CheckUniqueOfEntryAsync(string contestUniqueCode, IEnumerable<ContestFieldDetails> uniqueFields, Dictionary<string, object> props, List<ColumnMetadata> tableColumns, IDbContextTransaction transaction)
        {

            var uniqueProps = new Dictionary<string, object>();
            foreach (var item in uniqueFields)
            {
                uniqueProps.Add(item.FieldName, props[item.FieldName]);
            }
            var entries = await _unitOfWork.SQL.FindEntries(contestUniqueCode, uniqueProps, tableColumns, transaction);
            if (entries.Count() > 0)
            {
                return false;
            }
            return true;

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