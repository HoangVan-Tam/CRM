using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class OnlinePageInfomation
    {
        public Field Field;
        public bool IsOnlinePage;
        public bool IsOnlineCompletion;
        public string Regex;
        public string FieldLabel;
    }
    public class Field
    {
        public string FieldName;
        public string FieldType;
        public bool IsRequired;
    }
}
