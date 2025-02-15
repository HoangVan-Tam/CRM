using DAL.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DAL.Implement
{
    public class ContestRepository : GenericRepository<Contest>, IContestRepository
    {
        private readonly StandardContest2023Context _context;
        public ContestRepository(StandardContest2023Context context) : base(context)
        { 
            _context = context;
        }

        public async Task<Contest> GetContestWithContestFieldDetailsAndRegexValidationsAsync(string contestUniqueCode)
        {
            return await _context.Contests
                .Include(c => c.ContestFieldDetails)
                .ThenInclude(d => d.RegexValidation)
                .FirstOrDefaultAsync(c => c.ContestUniqueCode == contestUniqueCode);
        }
    }
}
