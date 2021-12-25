using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Extensions
{
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
}
