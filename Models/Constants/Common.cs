using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Constants
{
    public static partial class GlobalConstants
    {
        public enum ProcessEntryStatus
        {
            Success,
            FieldInvalid,
            Repeated,
            AmountInvalid,
            NotInCampaignPeriod
        }
        public enum TypeSubmitForm
        {
            OnlinePage,
            OnlineCompletionPage
        }
    }
}
