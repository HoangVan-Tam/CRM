using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Constants
{
    public static partial class GlobalConstants
    {
        public const string REPEATVALIDATION_RECEIPTNO = "ReceipNo";
        public const string REPEATVALIDATION_RECEIPTNO_MOBILENO = "Receipt MobileNo";
        public enum RepeatValidation
        {
            ReceiptNo = 1,
            ReceipNo_MobileNo = 2,
        }

        public enum ProcessEntryStatus
        {
            Success,
            FieldInvalid,
            Repeated,
            AmountInvalid,
            NotInCampaignPeriod
        }
    }
}
