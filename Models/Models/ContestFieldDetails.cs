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
        public int ID { get; set; }
        public bool? ShowOnlinePage { get; set; }
        public bool? ShowOnlineCompletion { get; set; }
        public bool? IsRequired { get; set; }
        public string? FieldLabel { get; set; }
        [Required]
        public int Order { get; set; }

        [ForeignKey("FieldRefID")]
        public int FieldID { get; set; }
        public virtual ContestFields Field { get; set; }
        [ForeignKey("ContestRefID")]
        public int ContestID { get; set; }
        public virtual Contest Contest { get; set; }
        [ForeignKey("RegexValidationRefID")]
        public string RegexID { get; set; }
        public virtual RegexValidation RegexValidation { get; set; }
    }
}
