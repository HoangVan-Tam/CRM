using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helper
{
    public class OutboundMessage
    {
        public string ContestId { get; set; }
        public string MobileNo { get; set; }
        public string MessageType { get; set; }
        public string MessageText { get; set; }
    }
}
