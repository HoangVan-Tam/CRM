using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class Parameters
    {
        public string EntrySource { get; set; }
        public bool SendResponse { get; set; }

        //Standard Params for SMS/MMS
        public string CreatedOn { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public string FileLink { get; set; }
    }
}
