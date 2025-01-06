using DAL.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implement
{
    internal class ContestFieldsRepository : GenericRepository<ContestFields>, IContestFieldsRepository
    {
        public ContestFieldsRepository(StandardContest2023Context context) : base(context)
        {
        }
    }
}
