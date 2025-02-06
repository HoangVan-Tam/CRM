using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class FieldsForNewContest
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string ContestUniqueCode { get; set; }

        public string FieldName { get; set; }

        public string FieldType { get; set; }
        string _fieldLabel;
        public string FieldLabel
        {
            get => _fieldLabel;
            set
            {
                var temp = "";
                _fieldLabel = value;
                var strings = _fieldLabel.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strings)
                {
                    temp = temp + item.Substring(0, 1).ToUpper() + item.Substring(1);
                }
                FieldName = temp;
            }
        }
        public int RegexValidationID { get; set; }
        public int Order { get; set; }
        public bool ShowOnlinePage { get; set; } = false;
        public bool ShowOnlineCompletion { get; set; } = false;
        public bool IsRequired { get; set; } = false;
    }
}
