using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class Option
    {
        public string ContestUniqueCode;
        public int PageNumber;
        public int PageSize;
        public DateTime StartDate;
        public DateTime EndDate;
        public object EntryValidity;
    }
}
