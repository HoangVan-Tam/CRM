using Entities.DTO;
using Entities.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUtilityService
    {
        void SetValue(Microsoft.AspNetCore.Components.ChangeEventArgs e, PropertyInfo propertyInfo, object o);
        byte[] ExportToCsv(List<Dictionary<string, object>> data);
        Task<string> UploadFileAsync(IBrowserFile file, string receiptNo);
        Dictionary<string, TValue> ToDictionary<TValue>(object obj);
        Type GetType(string type);
        string NormalizeSpaces(string value);
        string CleanMessage(Parameters param, string keyword);
        Task<string> SendSms(Contest contest, string receivers, string content);
        Task<string> SendWhatsapp(Contest contest, string mobileNo, string messageType, string messageText);
    }

}
