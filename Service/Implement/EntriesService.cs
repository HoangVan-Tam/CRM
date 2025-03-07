using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Common;
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
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Polly;
using Services.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private readonly IUtilityService _utilityService;
        private readonly IMapper _mapper;
        private SqlConnection _sqlConnection;
        private IConfiguration _config;

        public EntriesService(IUnitOfWork unitOfWork, IMapper mapper, SqlConnection sqlConnection, IOptions<AppConfig> appConfig, IConfiguration config, IUtilityService utilityService)
        {
            _unitOfWork = unitOfWork;
            _utilityService = utilityService;
            _mapper = mapper;
            _sqlConnection = sqlConnection;
            _config = config;
            _sqlConnection.ConnectionString = _config.GetConnectionString("BaseDB");
        }

        public async Task<FunctionResults<Dictionary<string, object>>> GetEntryByVerificationCode(string ContestUniqueCode, string verificationCode)
        {
            await _unitOfWork.BeginEfTransactionAsync();
            FunctionResults<Dictionary<string, object>> response = new FunctionResults<Dictionary<string, object>>();

            try
            {
                var conditionProps = new Dictionary<string, object>();
                conditionProps.Add("VerificationCode", verificationCode);
                var tableName = "BC_" + ContestUniqueCode;
                var entries = await _unitOfWork.SQL.GetEntriesByCondition(tableName, conditionProps, _unitOfWork.CurrentEfTransaction);
                if (entries != null)
                {
                    response.Data = entries.FirstOrDefault();
                }
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.IsSuccess = false;
                response.Error = ex.Message;
            }
            return response;
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
        /// Inserts an entry into the database while performing validation and uniqueness checks.
        /// </summary>
        /// <param name="contestUniqueCode">Unique code of the contest.</param>
        /// <param name="props">Dictionary containing entry details.</param>
        /// <param name="file">Optional file to be uploaded.</param>
        /// <returns>FunctionResults containing success status and message.</returns>
        public async Task<FunctionResults<string>> InsertEntry(string contestUniqueCode, Dictionary<string, object> props, IBrowserFile file)
        {
            var response = new FunctionResults<string>();

            // Start a database transaction to ensure atomicity
            await _unitOfWork.BeginEfTransactionAsync();
            try
            {
                // Fetch contest details
                var contest = await _unitOfWork.Contest
     .FindAsync(p => p.ContestUniqueCode.ToLower() == contestUniqueCode.ToLower());

                // Retrieve table columns dynamically
                var tableColumns = await _unitOfWork.SQL.GetTableColumnsAsync(contestUniqueCode, _unitOfWork.CurrentEfTransaction);

                // Initialize default values
                props["EntrySource"] = "WEB";
                props["IsValid"] = true;
                props["ProcessEntryStatus"] = GlobalConstants.ProcessEntryStatus.Success;

                // Perform pre-match field validation
                var validationResults = PreMatchFieldValidations(contest, DateTime.UtcNow);
                if (!validationResults.IsSuccess)
                {
                    props["IsValid"] = false;
                    props["Reason"] = validationResults.Error;
                    props["ProcessEntryStatus"] = GlobalConstants.ProcessEntryStatus.NotInCampaignPeriod;
                }

                // Check if the entry is unique based on predefined contest fields
                var uniqueFields = contest.ContestFieldDetails.Where(p => p.IsUnique == true);
                var isUniqueEntry = await CheckUniqueOfEntryAsync(contestUniqueCode, uniqueFields, props, tableColumns, _unitOfWork.CurrentEfTransaction);
                if (!isUniqueEntry)
                {
                    props["IsValid"] = false;
                    props["ProcessEntryStatus"] = GlobalConstants.ProcessEntryStatus.Repeated;
                }

                // Upload file to Azure if necessary
                if (!string.IsNullOrEmpty(props["FileLink"]?.ToString()))
                {
                    props["FileLink"] = await _utilityService.UploadFileAsync(file, props["ReceiptNumber"].ToString());
                }

                // Generate response message based on entry processing status
                if (props["ProcessEntryStatus"] is GlobalConstants.ProcessEntryStatus status)
                {
                    props["Response"] = MessagesHelpers.GetResponse(status, contest, props["EntrySource"].ToString());
                }

                // Insert the entry into the database, excluding the "EntryID" column
                if (Convert.ToBoolean(props["IsValid"]))
                {
                    await _unitOfWork.SQL.InsertEntriesAsync(contest, props, tableColumns.Where(p => p.ColumnName != "EntryID"), _unitOfWork.CurrentEfTransaction);
                }

                // Commit the transaction after successful insertion
                response.Message = props["Response"] == null ? "" : props["Response"].ToString();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of an error
                await _unitOfWork.RollbackAsync();
                response.IsSuccess = false;
                response.Error = $"Error inserting entry: {ex.Message}";
            }

            return response;
        }

        public async Task<FunctionResults<string>> CompleteEntry(string contestCode, string verificationCode, Dictionary<string, object> entryData, IBrowserFile file)
        {
            var result = new FunctionResults<string>();

            await _unitOfWork.BeginEfTransactionAsync();
            try
            {
                // Fetch contest details
                var contest = await _unitOfWork.Contest.FindAsync(p => p.ContestUniqueCode.ToLower() == contestCode.ToLower());
                var entrySearchParams = new Dictionary<string, object>();
                entrySearchParams.Add("MobileNumber", entryData["MobileNumber"]);
                entrySearchParams.Add("VerificationCode", verificationCode);
                var searchConditions = new List<ColumnMetadata>();
                searchConditions.Add(new ColumnMetadata() { ColumnName = "MobileNumber", DataType = "NvarChar" });
                searchConditions.Add(new ColumnMetadata() { ColumnName = "VerificationCode", DataType = "NvarChar" });
                var tableName = "BC_" + contestCode;
                var existingEntries = await _unitOfWork.SQL.FindEntriesAsync(tableName, entrySearchParams, searchConditions, _unitOfWork.CurrentEfTransaction);
                if (existingEntries.Count == 0)
                {
                    result.Message = "Unable to find Entry.";
                }
                else
                {
                    // Upload file to Azure if necessary
                    if (!string.IsNullOrEmpty(entryData["FileLink"]?.ToString()))
                    {
                        entryData["FileLink"] = await _utilityService.UploadFileAsync(file, entryData["ReceiptNumber"].ToString());
                    }

                    var tableColumns = await _unitOfWork.SQL.GetTableColumnsAsync(contestCode, _unitOfWork.CurrentEfTransaction);
                    tableColumns = tableColumns.Where(p => entryData.Keys.Contains(p.ColumnName)).ToList();
                    await _unitOfWork.SQL.UpdateEntriesAsync(tableName, verificationCode, tableColumns, entryData, _unitOfWork.CurrentEfTransaction);
                }

                // Commit the transaction after successful insertion
                result.Message = contest.ValidOnlineCompletionResponse;
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of an error
                await _unitOfWork.RollbackAsync();
                result.IsSuccess = false;
                result.Error = $"Error inserting entry: {ex.Message}";
            }

            return result;
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
                response.Data = _utilityService.ExportToCsv(dataEntries);
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

        /// <summary>
        /// Processes and validates an entry submission for a contest.
        /// - Validates input fields using regex and predefined criteria.
        /// - Checks uniqueness of the entry.
        /// - Uploads files to Azure if required.
        /// - Saves the validated entry into the database.
        /// - Handles errors with proper transaction management.
        /// </summary>
        public async Task<FunctionResults<Dictionary<string, object>>> SubmitEntryAPI(Parameters body, Contest contest)
        {
            await _unitOfWork.BeginEfTransactionAsync();
            var res = new FunctionResults<Dictionary<string, object>>();
            var props = new Dictionary<string, object>
    {
        { "IsValid", true },
        { "ProcessEntryStatus", GlobalConstants.ProcessEntryStatus.Success },
        { "EntrySource", body.EntrySource.Equals("Sms", StringComparison.InvariantCultureIgnoreCase) ? "SMS" :
                         body.EntrySource.Equals("Whatsapp", StringComparison.InvariantCultureIgnoreCase) ? "Whatsapp" : "" },
        { "FileLink", (body.EntrySource == "MMS" || body.EntrySource == "Whatsapp") && !string.IsNullOrEmpty(body.FileLink) ? body.FileLink : "" },
        { "DateEntry", string.IsNullOrEmpty(body.CreatedOn) ? DateTime.UtcNow : DateTime.Parse(body.CreatedOn).ToUniversalTime() }
    };

            try
            {
                var tableColumns = await _unitOfWork.SQL.GetTableColumnsAsync(body.ContestUniqueCode, _unitOfWork.CurrentEfTransaction);
                var cleanedMessage = _utilityService.CleanMessage(body, contest.Keyword);

                // Validate contest entry timing
                var validationResults = PreMatchFieldValidations(contest, (DateTime)props["DateEntry"]);
                if (!validationResults.IsSuccess)
                {
                    props["IsValid"] = false;
                    props["ProcessEntryStatus"] = GlobalConstants.ProcessEntryStatus.NotInCampaignPeriod;
                    props["Reason"] = validationResults.Error;
                }

                // Perform regex validation on input fields
                var regex = new Regex(contest.ValidationRegexFull, RegexOptions.IgnoreCase);
                var matchedMessage = regex.Match(cleanedMessage.Trim());

                if (matchedMessage.Success)
                {
                    var matchedValues = matchedMessage.Groups.Cast<Group>().Skip(1).Select(g => g.Value.Trim()).ToList();
                    var fields = contest.SMSSubmitFields.Split(',').ToList();

                    for (int i = 0; i < fields.Count; i++)
                    {
                        var field = contest.ContestFieldDetails.FirstOrDefault(f => f.FieldName == fields[i]);
                        if (field != null && new Regex(field.RegexValidation.Pattern).IsMatch(matchedValues[i]))
                        {
                            props[fields[i]] = matchedValues[i];
                        }
                    }

                    // Check for unique entry constraint
                    var uniqueFields = contest.ContestFieldDetails.Where(f => f.IsUnique == true);
                    if (!await CheckUniqueOfEntryAsync(body.ContestUniqueCode, uniqueFields, props, tableColumns, _unitOfWork.CurrentEfTransaction))
                    {
                        props["IsValid"] = false;
                        props["ProcessEntryStatus"] = GlobalConstants.ProcessEntryStatus.Repeated;
                    }
                    props["IsSaveable"] = false;
                }
                else
                {
                    // Identify specific field validation failures
                    foreach (var field in contest.ContestFieldDetails)
                    {
                        if (!new Regex(field.RegexValidation.Pattern).IsMatch(cleanedMessage))
                        {
                            props["Reason"] = field.FieldName + " is not valid!";
                            break;
                        }
                    }
                    props["IsValid"] = false;
                    props["ProcessEntryStatus"] = GlobalConstants.ProcessEntryStatus.FieldInvalid;
                }

                // Upload file to Azure if necessary
                if (!string.IsNullOrEmpty(props["FileLink"].ToString()) && props["EntrySource"].ToString() == "Whatsapp")
                {
                    //props["FileLink"] = await Helpers.UploadFileToBlobAsync(props["FileLink"].ToString());
                }

                // Generate response message based on processing status
                if (props["ProcessEntryStatus"] is GlobalConstants.ProcessEntryStatus status)
                {
                    props["Response"] = MessagesHelpers.GetResponse(status, contest, props["EntrySource"].ToString());
                }

                // Save entry into database
                await _unitOfWork.SQL.InsertEntriesAsync(contest, props, tableColumns.Where(p => p.ColumnName != "EntryID"), _unitOfWork.CurrentEfTransaction);
                res.Data = props;

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
            var entries = await _unitOfWork.SQL.FindEntriesAsync(contestUniqueCode, uniqueProps, tableColumns, transaction);
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