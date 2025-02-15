using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DAL.Implement;
using DAL.Interface;
using Entities.DTO;
using Entities.Helper;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Services.Common
{
    public static class Helper
    {
        private static AppSettings _appSettings;
        private static IUnitOfWork _unitOfWork;

        public static void Initialize(AppSettings appSettings, IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings;
            _unitOfWork = unitOfWork;
        }

        public static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }

        public static void SetValue(Microsoft.AspNetCore.Components.ChangeEventArgs e, PropertyInfo propertyInfo, object o)
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

        public static Type GetType(string type)
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

        public static byte[] ExportToCsv(List<Dictionary<string, object>> data)
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

        public static async Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream)
        {
            try
            {
                //var container = new BlobContainerClient(blobStorageconnection, blobContainerName);
                //var createResponse = await container.CreateIfNotExistsAsync();
                //if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                //    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                //var blob = container.GetBlobClient(strFileName);
                //await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                //await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contecntType });
                //var urlString = blob.Uri.ToString();
                //return urlString;
                return "";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string NormalizeSpaces(string value)
        {
            value = Regex.Replace(value, @"[\n\r\t]", " ");  // Convert all  \n = CR(Carriage Return)   \r = LF(Line Feed)   \t = tab to a single space.
            value = Regex.Replace(value, @"\s+", " ");       // Convert all whitespaces to a single space.
            return value.Trim();
        }

        public static string CleanMessage(Parameters param, string Keyword)
        {
            //Remove Keyword and Append Number to String.
            var json = param.Message.ToString();
            json = json.Replace("<>", " ").Replace("\r\n", " ").Replace("\n\r", " ").Replace("\n", " ").Replace("\r", " ");
            json = NormalizeSpaces(json).Trim();

            if (json.Trim().ToUpper().StartsWith(Keyword.ToUpper() + " "))
            {
                var KeywordNFirstSpace = json.Split(' ')[0];
                json = json.Remove(0, KeywordNFirstSpace.Length);
            }
            json = json.Trim();

            json = param.MobileNo.ToString() + " " + json;

            return json;
        }

        public static async Task<string> SendSms(Contest contest, string receivers, string content)
        {
            try
            {
                // create the web request with the url to the web
                // service with the method name added to the end
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create("http://www.smsdome.com/api/http/sendsms.aspx");

                // add the parameters as key valued pairs making
                // sure they are URL encoded where needed
                ASCIIEncoding encoding = new ASCIIEncoding();
                //byte[] postData = encoding.GetBytes("CreatedOn=" + dt + "&MobileNo=" + MobileNo + "&Message=" + Message);
                byte[] postData = encoding.GetBytes("AppID=" +  contest.AppId + "&AppSecret=" + contest.AppSecret + "&receivers=" + receivers + "&content=" + HttpUtility.UrlEncode(content) + "&responseformat=XML");
                httpReq.ContentType = "application/x-www-form-urlencoded";
                httpReq.Method = "POST";
                httpReq.ContentLength = postData.Length;

                // convert the request to a steeam object and send it on its way
                Stream ReqStrm = httpReq.GetRequestStream();
                ReqStrm.Write(postData, 0, postData.Length);
                ReqStrm.Close();

                // get the response from the web server and
                // read it all back into a string variable
                HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
                StreamReader respStrm = new StreamReader(httpResp.GetResponseStream(), Encoding.ASCII);
                string result = respStrm.ReadToEnd();
                httpResp.Close();
                respStrm.Close();

                // Get Credits used for sms
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                XmlNodeList parentNode = xmlDoc.GetElementsByTagName("receiver");
                var creditsUsed = "";
                foreach (XmlNode node in parentNode)
                {
                    creditsUsed = node.Attributes["credits"].Value;
                }

                var log = new Dictionary<string, object>();
                log.Add("LogDate", DateTime.UtcNow);
                log.Add("Recipient", receivers);
                log.Add("Content", content);
                log.Add("LogType", "SMS");
                log.Add("CreditsUsed", creditsUsed == "" ? "0" : creditsUsed);
                await _unitOfWork.SQL.InsertAsync("BC" + contest.ContestUniqueCode, log);

                return result;
                // show the result the test box for testing purposes
                //string result2 = result;

                ////////////////////////////////////////////////////////

            }
            catch (Exception ex)
            {
                return ex.ToString();
                //WriteToLogFile("Campaign Error: " + ex.Message);
            }
        }

        public static async Task<string> SendWhatsapp(Contest contest, string mobileNo, string messageType, string messageText)
        {
            try
            {     
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //call outbound url
                string uri = _appSettings.OutboundURL;

                var log = new Dictionary<string, object>();
                log.Add("LogDate", DateTime.UtcNow);
                log.Add("Recipient", mobileNo);
                log.Add("Content", messageText);
                log.Add("LogType", "Whatsapp");
                log.Add("CreditsUsed", "1");
                await _unitOfWork.SQL.InsertAsync("BC" + contest.ContestUniqueCode, log);

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
                string content = string.Empty;

                using (var response = (HttpWebResponse)httpReq.GetResponse())
                using (var receiveStream = response.GetResponseStream())
                using (var reader = new StreamReader(receiveStream))
                {
                    content = reader.ReadToEnd();
                    // parse your content, etc.
                }
                return content;

            }
            catch (WebException ex)
            {
                return "Exception : " + ex.Message;
            }
        }

        public static async Task<List<ColumnMetadata>> GetTableColumnsAsync(string contestUniqueCode)
        {
            var columns = new List<ColumnMetadata>();
            string query = @"
            SELECT COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @TableName";

            var connection = _unitOfWork.GetDbConnection();
            using (var command = new SqlCommand(query, (SqlConnection)connection, (SqlTransaction)_unitOfWork.CurrentDbTransaction))
            {
                command.Parameters.AddWithValue("@TableName", "BC_" + contestUniqueCode);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        columns.Add(new ColumnMetadata
                        {
                            ColumnName = reader["COLUMN_NAME"].ToString(),
                            DataType = reader["DATA_TYPE"].ToString()
                        });
                    }
                }
            }
            return columns;
        }
    }
}
