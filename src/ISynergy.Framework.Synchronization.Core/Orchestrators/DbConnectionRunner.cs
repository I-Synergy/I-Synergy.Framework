using ISynergy.Framework.Synchronization.Core.Enumerations;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Orchestrators
{
    //Make an extension method to allow calling the static method as in BaseOrchestrator

    public static class DbConnectionRunnerExtensions
    {
        public static async Task<DbConnectionRunner> GetConnectionAsync(this BaseOrchestrator orchestrator,
                                SyncStage syncStage = SyncStage.None,
                                DbConnection connection = default,
                                DbTransaction transaction = default,
                                CancellationToken cancellationToken = default,
                                IProgress<ProgressArgs> progress = default)
        {
            if (connection is null)
                connection = orchestrator.Provider.CreateConnection();

            var alreadyOpened = connection.State == ConnectionState.Open;
            var alreadyInTransaction = transaction is not null && transaction.Connection == connection;

            // Get context or create a new one
            var ctx = orchestrator.GetContext();
            ctx.SyncStage = syncStage;

            // Open connection
            if (!alreadyOpened)
                await orchestrator.OpenConnectionAsync(connection, cancellationToken, progress).ConfigureAwait(false);

            // Create a transaction
            if (!alreadyInTransaction)
            {
                transaction = connection.BeginTransaction(orchestrator.Provider.IsolationLevel);
                await orchestrator.InterceptAsync(new TransactionOpenedArgs(ctx, connection, transaction), progress, cancellationToken).ConfigureAwait(false);
            }

            return new DbConnectionRunner(orchestrator, connection, transaction, alreadyOpened, alreadyInTransaction, cancellationToken, progress);
        }
    }

    public sealed class DbConnectionRunner : IDisposable, IAsyncDisposable
    {
        public DbConnectionRunner(BaseOrchestrator orchestrator, DbConnection connection, DbTransaction transaction,
            bool alreadyOpened, bool alreadyInTransaction,
            CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = default)
        {
            Orchestrator = orchestrator;
            Connection = connection;
            Transaction = transaction;
            AlreadyOpened = alreadyOpened;
            AlreadyInTransaction = alreadyInTransaction;
            CancellationToken = cancellationToken;
            Progress = progress;
        }

        public BaseOrchestrator Orchestrator { get; set; }
        public DbConnection Connection { get; set; }
        public DbTransaction Transaction { get; set; }
        public bool AlreadyOpened { get; }
        public bool AlreadyInTransaction { get; }
        public CancellationToken CancellationToken { get; }
        public IProgress<ProgressArgs> Progress { get; }

        /// <summary>
        /// Commit the transaction and call an interceptor
        /// </summary>
        public async Task CommitAsync(bool autoClose = true)
        {
            await Orchestrator.InterceptAsync(
                new TransactionCommitArgs(Orchestrator.GetContext(), Connection, Transaction), Progress, CancellationToken).ConfigureAwait(false);

            if (!AlreadyInTransaction && Transaction is not null)
                Transaction.Commit();

            if (autoClose)
                await CloseAsync();
        }

        /// <summary>
        /// Commit the transaction and call an interceptor
        /// </summary>
        public async Task CloseAsync()
        {
            if (!AlreadyOpened && Connection is not null)
                await Orchestrator.CloseConnectionAsync(Connection, CancellationToken, Progress).ConfigureAwait(false);
        }

        public Task RollbackAsync() => Task.Run(() => Transaction.Rollback());


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue = false;

        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!AlreadyInTransaction && Transaction is not null)
                    {
                        Transaction.Dispose();
                        Transaction = null;
                    }

                    if (!AlreadyOpened && Connection is not null)
                    {
                        if (Connection.State == ConnectionState.Open)
                            Connection.Close();

                        Connection.Dispose();
                        Connection = null;
                    }
                }
                disposedValue = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!AlreadyInTransaction && Transaction is not null)
            {
                Transaction.Dispose();
                Transaction = null;
            }

            if (!AlreadyOpened && Connection is not null)
            {
                await Orchestrator.CloseConnectionAsync(Connection, CancellationToken, Progress).ConfigureAwait(false);
                Connection.Dispose();
                Connection = null;
            }

            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
