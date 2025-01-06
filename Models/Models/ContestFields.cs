using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class ContestFields
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string FieldName { get; set; } = null!;
        public string FieldType { get; set; } = null!;

        public virtual ICollection<ContestFieldDetails> ContestFieldDetails { get; set; }
    }
}
