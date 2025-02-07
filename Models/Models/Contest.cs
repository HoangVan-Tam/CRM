using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Contest
    {
        [Key]
        public string ContestID { get; set; } = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("HHmmssff");
        [Required]
        public string ContestUniqueCode { get; set; }
        public string NameContest { get; set; }
        public string? DescriptionContest { get; set; }
        public string Keyword { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public int AppId { get; set; }
        public string AppSecret { get; set; }
        public string SMSSubmitFields { get; set; }
        public string? ValidationRegexFull { get; set; }
        public string? ValidSmsresponse { get; set; }
        public string? InvalidSmsresponse { get; set; }
        public string? RepeatedSmsresponse { get; set; }
        public string? ValidWhatsappResponse { get; set; }
        public string? InvalidWhatsappResponse { get; set; }
        public string? RepeatedWhatsappResponse { get; set; }
        public string? ValidOnlinePageResponse { get; set; }
        public string? RepeatedOnlinePageResponse { get; set; }
        public string? ValidOnlineCompletionResponse { get; set; }
        public string? ErrorMessageAmount { get; set; }
        public string? MissingFieldResponse { get; set; }
        public string? EntryExclusionFields { get; set; }
        public string? WinnerExclusionFields { get; set; }
        public string? RepeatValidation { get; set; }
        public decimal Amount { get; set; }
        public decimal TierAmount { get; set; } = 0;

        public virtual ICollection<ContestFieldDetails> ContestFieldDetails { get; set; }
    }
}
