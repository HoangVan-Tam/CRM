using DAL.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implement
{
    public class ContestRepository : GenericRepository<Contest>, IContestRepository
    {
        public ContestRepository(StandardContest2023Context context) : base(context) 
        { 
        } 
    }
}
