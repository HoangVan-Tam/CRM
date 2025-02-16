using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Polly;
using Polly.Retry;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class TransactionService : ITransactionService
    {
        private readonly StandardContest2023Context _context;
        private readonly AsyncRetryPolicy _retryPolicy;

        public TransactionService(StandardContest2023Context context)
        {
            _context = context;
            _retryPolicy = Policy
                .Handle<DbUpdateException>() // error database
                .Or<TimeoutException>() // error timeout
                .Or<InvalidOperationException>() // connection error
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task ExecuteTransactionAsync(Func<Task> operation, int timeoutSeconds = 10)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // set timeout
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
                    var operationTask = _retryPolicy.ExecuteAsync(async () =>
                    {
                        await operation();
                    });

                    if (await Task.WhenAny(operationTask, timeoutTask) == timeoutTask)
                    {
                        throw new TimeoutException("Transaction took too long and was canceled.");
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
