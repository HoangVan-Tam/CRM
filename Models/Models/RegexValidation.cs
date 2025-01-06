using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class RegexValidation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Pattern { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<ContestFieldDetails> ContestFieldDetails { get; set; }
    }
}
