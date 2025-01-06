using DAL.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implement
{
    public class ContestFieldDetailsRepository : GenericRepository<ContestFieldDetails>, IContestFieldDetailsRepository
    {
        public ContestFieldDetailsRepository(StandardContest2023Context context) : base(context)
        {
        }
    }
}
