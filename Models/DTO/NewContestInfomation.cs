using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class NewContestInfomation
    {
        public string ContestUniqueCode { get; set; }
        [Required]
        public string NameContest { get; set; }
        [Required]
        public string? DescriptionContest { get; set; }
        [Required]
        public string Keyword { get; set; }
        [Required]
        public DateTime TestDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime TerminationDate { get; set; } = DateTime.UtcNow;
        [Required]
        public int AppId { get; set; }
        [Required]
        public string AppSecret { get; set; }
        [Required]
        public bool HasSMSSubmission { get; set; }
        [Required]
        public bool HasWhatsAppSubmission { get; set; }
        [Required]
        public bool HasOnlinePage { get; set; }
        [Required]
        public bool HasOnlineCompletion { get; set; }

        //SMS response
        public string ValidSMSResponse { get; set; }
        public string InvalidSMSResponse { get; set; }
        public string RepeatedSMSResponse { get; set; }

        //Whatsapp response 
        public string ValidWhatsappResponse { get; set; }
        public string InvalidWhatsappResponse { get; set; }
        public string RepeatedWhatsappResponse { get; set; }
        public string ValidationRegexFull { get; set; }
        public string SMSFields { get; set; }

        //Online page response
        public string ValidOnlinePageResponse { get; set; }
        public string RepeatedOnlinePageResponse { get; set; }

        //Online Completion response
        public string ValidOnlineCompletionResponse { get; set; }

        public string MissingFieldResponse { get; set; }
        public string EntryExclusionFields { get; set; } = "Response,NRIC_NoPrefix,VerificationCode,Chances,NRIC,EntryText,DateVerified,DateRejected,DateResent,IsRejected,IsVerified";
        public string WinnerExclusionFields { get; set; } = "Response,NRIC_NoPrefix,VerificationCode,Chances,EntrySource,NRIC,IsValid,IsRejected,DateVerified,DateRejected,DateResent,IsVerified,Reason,EntryText";

        List<FieldsForNewContest> contestFields { get; set; }
    }
}
