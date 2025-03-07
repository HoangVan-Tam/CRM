using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Constants.GlobalConstants;

namespace Common
{
    public static class MessagesHelpers
    {
        public static string GetResponse(ProcessEntryStatus status, Contest contest, string entrySource, bool isAddOnPage = false)
        {
            if (contest == null || string.IsNullOrWhiteSpace(entrySource))
                return "Invalid contest or entry source";

            string entrySourceKey = entrySource.ToUpper();

            var responses = new Dictionary<ProcessEntryStatus, Func<string>>()
    {
        { ProcessEntryStatus.Success, () => entrySourceKey switch
            {
                "WEB" => isAddOnPage ? contest.ValidOnlineCompletionResponse : contest.ValidOnlinePageResponse,
                "SMS" => contest.ValidSmsresponse,
                "WHATSAPP" => contest.ValidWhatsappResponse,
                _ => "Entry source not found"
            }
        },
        { ProcessEntryStatus.FieldInvalid, () => entrySourceKey switch
            {
                "SMS" => contest.InvalidSmsresponse,
                "WHATSAPP" => contest.InvalidWhatsappResponse,
                _ => "Entry source not found"
            }
        },
        { ProcessEntryStatus.Repeated, () => entrySourceKey switch
            {
                "WEB" => contest.RepeatedOnlinePageResponse,
                "SMS" => contest.RepeatedSmsresponse,
                "WHATSAPP" => contest.RepeatedWhatsappResponse,
                _ => "Entry source not found"
            }
        },
        { ProcessEntryStatus.AmountInvalid, () => contest.ErrorMessageAmount },
        { ProcessEntryStatus.NotInCampaignPeriod, () => "Not In Campaign Period" }
    };

            return responses.TryGetValue(status, out var responseFunc) ? responseFunc() : "Unknown Process Entry Status.";
        }

    }
}
