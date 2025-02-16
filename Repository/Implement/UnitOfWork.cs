using DAL.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implement
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _disposed;
        private StandardContest2023Context _context;
        private IDbConnection _dbConnection;
        private IDbContextTransaction _efConnection;
        private IDbTransaction _currentDbTransaction;
        private IDbContextTransaction _currentEfTransaction;
        private IContestRepository _contestRepository;
        private IContestFieldDetailsRepository _contestFieldDetailRepository;
        private IContestFieldsRepository _contestFieldsRepository;
        private IRegexValidationRepository _regexValidationRepository;
        private ISQLRepository _sql;
        public UnitOfWork(StandardContest2023Context context, ISQLRepository sql)
        {
            _context = context;
            _sql = sql;
        }
        public IContestRepository Contest
        {
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

        public ISQLRepository SQL
        {
            get
            {
                return _sql;
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

        public IDbTransaction CurrentDbTransaction => _currentDbTransaction;

        public IDbContextTransaction CurrentEfTransaction => _currentEfTransaction;

        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginDbTransactionAsync()
        {
            if (_currentDbTransaction != null)
                throw new InvalidOperationException("An ADO.NET transaction is already active.");

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
            _currentDbTransaction = await connection.BeginTransactionAsync();
            _dbConnection = connection;
        }

        public async Task BeginEfTransactionAsync()
        {
            if (_currentEfTransaction != null)
                throw new InvalidOperationException("An EF Core transaction is already active.");

            _currentEfTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_currentDbTransaction != null)
            {
                _currentDbTransaction.Commit();
                _currentDbTransaction.Dispose();
                _currentDbTransaction = null;
            }
            else if (_currentEfTransaction != null)
            {
                await _currentEfTransaction.CommitAsync();
                _currentEfTransaction.Dispose();
                _currentEfTransaction = null;
            }
            else
            {
                throw new InvalidOperationException("No transaction is active.");
            }
        }

        public async Task RollbackAsync()
        {
            if (_currentDbTransaction != null)
            {
                _currentDbTransaction.Rollback();
                _currentDbTransaction.Dispose();
                _currentDbTransaction = null;
            }
            else if (_currentEfTransaction != null)
            {
                await _currentEfTransaction.RollbackAsync();
                _currentEfTransaction.Dispose();
                _currentEfTransaction = null;
            }
            else
            {
                throw new InvalidOperationException("No transaction is active.");
            }
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

        public IDbConnection GetDbConnection()
        {
            return _dbConnection;
        }

        public IDbContextTransaction GetDbEfConnection()
        {
            return _efConnection;
        }
    }
}
