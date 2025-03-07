using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class FormField
    {
        public string? FieldLabel { get; set; }
        public string? FieldName { get; set; }
        public string? FormControl { get; set; }
        public string? FieldType { get; set; }
        public int Order { get; set; }
        public string Pattern { get; set; }
        public bool IsRequired { get; set; }
    }
}
