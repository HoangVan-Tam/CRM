

using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IUnitOfWork
    {
        IDbConnection GetDbConnection();
        IDbTransaction CurrentDbTransaction { get; }
        IDbContextTransaction CurrentEfTransaction { get; }
        IContestRepository Contest { get; }
        IContestFieldDetailsRepository ContestFieldDetail { get; }
        ISQLRepository SQL{ get; }
        IContestFieldsRepository ContestFields { get; }
        IRegexValidationRepository RegexValidation { get; }
        void Save();
        Task SaveAsync();

        Task BeginEfTransactionAsync();
        Task BeginDbTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
