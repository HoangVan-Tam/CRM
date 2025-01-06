using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Entities.DTO;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Text;

namespace Services.Common
{
    public static class Helper
    {
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
    }
}
