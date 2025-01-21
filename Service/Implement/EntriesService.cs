using AutoMapper;
using CsvHelper;
using DAL.Interface;
using Entities.Constants;
using Entities.DTO;
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

        public Task<FunctionResults<string>> APISubmitEntry(Parameters parameters, string contestUniqueCode)
        {
            // DateEntry, EntryText, IsValid, Reason, Response, VerificationCode, Chances, EntrySource,
            var defaultProps = new Dictionary<string, object> { { "IsVerified", 0 }, { "IsRejected", 0 }, { "DateEntry", DateTime.UtcNow } };
            var nameTable = "BC_" + contestUniqueCode;
            if (parameters.EntrySource.Equals("Sms", StringComparison.InvariantCultureIgnoreCase))
            {
                defaultProps.Add("EntrySource", "SMS");
            }
            else if (parameters.EntrySource.Equals("Whatsapp", StringComparison.InvariantCultureIgnoreCase))
            {
                defaultProps.Add("EntrySource", "Whatsapp");
            }
            FunctionResults<string> response = new FunctionResults<string>();
            var CleanedMessage = Helper.CleanMessage(parameters);
        }
    }
}
