using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DAL.Interface;
using Entities.DTO;
using Entities.Helper;
using Entities.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.Interface;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Services.Common
{
    public class UtilityService : IUtilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppConfig _appConfig;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public UtilityService(IOptions<AppConfig> appConfig, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _appConfig = appConfig.Value;

            string blobConnectionString = _appConfig.AzureBlobStorage.ConnectionString
                .Replace("{accountName}", _appConfig.AzureBlobStorage.AccountName)
                .Replace("{accountKey}", _appConfig.AzureBlobStorage.AccountKey)
                .Replace("{endpointUrl}", _appConfig.AzureBlobStorage.EndpointUrl);
            _containerName = _appConfig.AzureBlobStorage.ContainerName;
            _blobServiceClient = new BlobServiceClient(blobConnectionString);
        }
        public Type GetType(string type)
        {
            switch (type)
            {
                case "String":
                    return typeof(string);
                case "DateTime":
                    return typeof(DateTime);
                case "Int32":
                    return typeof(int);
                case "Decimal":
                    return typeof(decimal);
                default: return null;
            }
        }
        public void SetValue(Microsoft.AspNetCore.Components.ChangeEventArgs e, PropertyInfo propertyInfo, object o)
        {
            var propertyType = propertyInfo.PropertyType;
            if (e.Value.GetType() == propertyType)
            {
                propertyInfo.SetValue(o, e.Value);
            }
            else
            {
                switch (propertyType.Name)
                {
                    case "String":
                        propertyInfo.SetValue(o, e.Value.ToString());
                        break;
                    case "DateTime":
                        propertyInfo.SetValue(o, Convert.ToDateTime(e.Value));
                        break;
                    case "Int32":
                        propertyInfo.SetValue(o, Convert.ToInt32(e.Value));
                        break;
                    case "Decimal":
                        propertyInfo.SetValue(o, Convert.ToDecimal(e.Value));
                        break;
                    default: break;
                }
            }
        }
        public Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
        }

        public string NormalizeSpaces(string value)
        {
            value = Regex.Replace(value, @"[\n\r\t]", " ");
            value = Regex.Replace(value, @"\s+", " ");
            return value.Trim();
        }

        public string CleanMessage(Parameters param, string keyword)
        {
            var json = param.Message.ToString();
            json = json.Replace("<>", " ").Replace("\r\n", " ").Replace("\n\r", " ").Replace("\n", " ").Replace("\r", " ");
            json = NormalizeSpaces(json).Trim();

            if (json.Trim().ToUpper().StartsWith(keyword.ToUpper() + " "))
            {
                var KeywordNFirstSpace = json.Split(' ')[0];
                json = json.Remove(0, KeywordNFirstSpace.Length);
            }
            json = json.Trim();
            json = param.MobileNo.ToString() + " " + json;
            return json;
        }

        public async Task<string> SendSms(Contest contest, string receivers, string content)
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create("http://www.smsdome.com/api/http/sendsms.aspx");
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postData = encoding.GetBytes("AppID=" + contest.AppId + "&AppSecret=" + contest.AppSecret +
                    "&receivers=" + receivers + "&content=" + HttpUtility.UrlEncode(content) + "&responseformat=XML");

                httpReq.ContentType = "application/x-www-form-urlencoded";
                httpReq.Method = "POST";
                httpReq.ContentLength = postData.Length;

                using (Stream ReqStrm = httpReq.GetRequestStream())
                {
                    ReqStrm.Write(postData, 0, postData.Length);
                }

                using (HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse())
                using (StreamReader respStrm = new StreamReader(httpResp.GetResponseStream(), Encoding.ASCII))
                {
                    string result = respStrm.ReadToEnd();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> SendWhatsapp(Contest contest, string mobileNo, string messageType, string messageText)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string uri = _appConfig.ExternalApiConfig.OutboundURL;

                var log = new Dictionary<string, object>
                {
                    { "LogDate", DateTime.UtcNow },
                    { "Recipient", mobileNo },
                    { "Content", messageText },
                    { "LogType", "Whatsapp" },
                    { "CreditsUsed", "1" }
                };

                await _unitOfWork.SQL.InsertLogAsync("BC" + contest.ContestUniqueCode, log);

                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(uri);
                httpReq.ContentType = "application/json; charset=utf-8";
                httpReq.Method = "POST";

                using (var streamWriter = new StreamWriter(httpReq.GetRequestStream()))
                {
                    OutboundMessage Outbound_webapp = new OutboundMessage
                    {
                        ContestId = contest.ContestID,
                        MobileNo = mobileNo,
                        MessageType = messageType,
                        MessageText = messageText
                    };

                    string outboundWebapp = JsonConvert.SerializeObject(Outbound_webapp);
                    streamWriter.Write(outboundWebapp);
                }

                using (var response = (HttpWebResponse)httpReq.GetResponse())
                using (var receiveStream = response.GetResponseStream())
                using (var reader = new StreamReader(receiveStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                return "Exception : " + ex.Message;
            }
        }

        public byte[] ExportToCsv(List<Dictionary<string, object>> data)
        {
            StringBuilder sbRtn = new StringBuilder();
            var header = "";
            foreach (var item in data[0])
            {
                if (header == "")
                {
                    header = string.Format(item.Key);
                }
                else
                {
                    header = header + string.Format("," + item.Key);
                }
            }
            sbRtn.AppendLine(header);
            foreach (var dic in data)
            {
                var listResult = "";
                foreach (var pair in dic)
                {
                    var formatCell = pair.Value;
                    if (formatCell.GetType() == DateTime.UtcNow.GetType())
                    {
                        formatCell = (Convert.ToDateTime(formatCell).ToString("dd MMM yyyy HH:mm:ss"));
                    }
                    else
                    {
                        formatCell = formatCell.ToString().Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                    }
                    if (listResult == "")
                    {
                        listResult = string.Format(formatCell.ToString());
                    }
                    else
                    {
                        listResult = listResult + string.Format("," + formatCell.ToString());
                    }
                }
                sbRtn.AppendLine(listResult);
            }
            return Encoding.UTF8.GetBytes(sbRtn.ToString());
        }
        public async Task<string> UploadFileAsync(IBrowserFile file, string receiptNo)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var dt = DateTime.UtcNow;
            string fileNamePrefix = dt.ToString("yyMMdd_HHmmssfff_");// + '_';
            string fileName = $"{fileNamePrefix}{receiptNo}{Path.GetExtension(file.Name)}";
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using (Stream stream = file.OpenReadStream())
            {
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType // Lấy MIME Type từ IBrowserFile
                };

                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }
            return blobClient.Uri.ToString();
        }
    }
}
