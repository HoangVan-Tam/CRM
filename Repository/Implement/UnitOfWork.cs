using DAL.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implement
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _disposed;
        private StandardContest2023Context _context;
        private IContestRepository _contestRepository;
        private IContestFieldDetailsRepository _contestFieldDetailRepository;
        private IContestFieldsRepository _contestFieldsRepository;
        private IRegexValidationRepository _regexValidationRepository;
        private ILinqToSQLRepository _linqToSQLRepository;
        public UnitOfWork(StandardContest2023Context context, ILinqToSQLRepository linqToSQLRepository)
        {
            _context = context;
            _linqToSQLRepository = linqToSQLRepository;
        }
        public IContestRepository Contest { 
            get 
            {
                return _contestRepository = _contestRepository ?? new ContestRepository(_context);
            } 
        }

        public IContestFieldDetailsRepository ContestFieldDetail
        {
            get
            {
                return _contestFieldDetailRepository = _contestFieldDetailRepository ?? new ContestFieldDetailsRepository(_context);
            }
        }

        public ILinqToSQLRepository LinqToSQL
        {
            get
            {
                return _linqToSQLRepository;
            }
        }

        public IContestFieldsRepository ContestFields
        {
            get
            {
                return _contestFieldsRepository = _contestFieldsRepository ?? new ContestFieldsRepository(_context);
            }
        }

        public IRegexValidationRepository RegexValidation
        {
            get
            {
                return _regexValidationRepository = _regexValidationRepository ?? new RegexValidationRepository(_context);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}
