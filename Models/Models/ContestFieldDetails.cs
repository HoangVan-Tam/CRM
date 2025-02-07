using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class ContestFieldDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FieldDetailID { get; set; }
        public bool? ShowOnlinePage { get; set; }
        public bool? ShowOnlineCompletion { get; set; }
        public bool? IsUnique { get; set; }
        public bool? IsRequired { get; set; }
        public string? FieldLabel { get; set; }
        public string? FieldName { get; set; }
        public string? FormControl { get; set; }
        public string? FieldType { get; set; }
        [Required]
        public int Order { get; set; }

        [ForeignKey("ContestRefID")]
        public string ContestID { get; set; }
        public virtual Contest Contest { get; set; }

        [ForeignKey("RegexRefID")]
        public int RegexValidationID { get; set; }
        public virtual RegexValidation RegexValidation { get; set; }
    }
}
