
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IUnitOfWork
    {
        IContestRepository Contest { get; }
        IContestFieldDetailsRepository ContestFieldDetail { get; }
        ILinqToSQLRepository LinqToSQL{ get; }
        IContestFieldsRepository ContestFields { get; }
        IRegexValidationRepository RegexValidation { get; }
        void Save();
        Task SaveAsync();
    }
}
