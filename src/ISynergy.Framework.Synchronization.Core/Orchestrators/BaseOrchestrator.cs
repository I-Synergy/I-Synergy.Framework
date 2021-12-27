using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Parameter;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Orchestrators
{
    public abstract partial class BaseOrchestrator
    {
        // Collection of Interceptors
        private Interceptors _interceptors = new Interceptors();
        internal SyncContext _syncContext;
        protected readonly IVersionService _versionService;

        /// <summary>
        /// Gets or Sets orchestrator side
        /// </summary>
        public abstract SyncSide Side { get; }

        /// <summary>
        /// Gets or Sets the provider used by this local orchestrator
        /// </summary>
        public IProvider Provider { get; set; }

        /// <summary>
        /// Gets the options used by this local orchestrator
        /// </summary>
        public virtual SyncOptions Options { get; internal set; }

        /// <summary>
        /// Gets the Setup used by this local orchestrator
        /// </summary>
        public virtual SyncSetup Setup { get; set; }

        /// <summary>
        /// Gets the scope name used by this local orchestrator
        /// </summary>
        public virtual string ScopeName { get; internal protected set; }

        /// <summary>
        /// Gets or Sets the start time for this orchestrator
        /// </summary>
        public virtual DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or Sets the end time for this orchestrator
        /// </summary>
        public virtual DateTime? CompleteTime { get; set; }

        /// <summary>
        /// Gets or Sets the logger used by this orchestrator
        /// </summary>
        public virtual ILogger Logger { get; set; }


        /// <summary>
        /// Create a local orchestrator, used to orchestrates the whole sync on the client side
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="provider"></param>
        /// <param name="options"></param>
        /// <param name="setup"></param>
        /// <param name="scopeName"></param>
        public BaseOrchestrator(
            IVersionService versionService,
            IProvider provider, 
            SyncOptions options, 
            SyncSetup setup, 
            string scopeName = SyncOptions.DefaultScopeName)
        {
            Argument.IsNotNull(versionService);
            Argument.IsNotNull(scopeName);
            Argument.IsNotNull(provider);
            Argument.IsNotNull(options);
            Argument.IsNotNull(setup);

            _versionService = versionService;

            ScopeName = scopeName;
            Provider = provider;
            Options = options;
            Setup = setup;

            Provider.Orchestrator = this;
            Logger = options.Logger;
        }

        /// <summary>
        /// Set an interceptor to get info on the current sync process
        /// </summary>
        [DebuggerStepThrough]
        internal void On<T>(Action<T> interceptorAction) where T : ProgressArgs =>
            _interceptors.GetInterceptor<T>().Set(interceptorAction);

        /// <summary>
        /// Set an interceptor to get info on the current sync process
        /// </summary>
        [DebuggerStepThrough]
        internal void On<T>(Func<T, Task> interceptorAction) where T : ProgressArgs =>
            _interceptors.GetInterceptor<T>().Set(interceptorAction);

        /// <summary>
        /// Set a collection of interceptors
        /// </summary>
        [DebuggerStepThrough]
        internal void On(Interceptors interceptors) => _interceptors = interceptors;

        /// <summary>
        /// Returns the Task associated with given type of BaseArgs 
        /// Because we are not doing anything else than just returning a task, no need to use async / await. Just return the Task itself
        /// </summary>
        internal async Task<T> InterceptAsync<T>(T args, IProgress<ProgressArgs> progress = default, CancellationToken cancellationToken = default) where T : ProgressArgs
        {
            if (_interceptors is null)
                return args;

            var interceptor = _interceptors.GetInterceptor<T>();

            // Check logger, because we make some reflection here
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                //for example, getting DatabaseChangesSelectingArgs and transform to DatabaseChangesSelecting
                var argsTypeName = args.GetType().Name.Replace("Args", "");

                Logger.LogDebug(new EventId(args.EventId, argsTypeName), null, args) ;
            }

            await interceptor.RunAsync(args, cancellationToken).ConfigureAwait(false);

            if (progress != default)
                ReportProgress(args.Context, progress, args, args.Connection, args.Transaction);

            return args;
        }

        /// <summary>
        /// Affect an interceptor
        /// </summary>
        [DebuggerStepThrough]
        internal void SetInterceptor<T>(Action<T> action) where T : ProgressArgs => On(action);

        /// <summary>
        /// Affect an interceptor
        /// </summary>
        [DebuggerStepThrough]
        internal void SetInterceptor<T>(Func<T, Task> action) where T : ProgressArgs => On(action);

        /// <summary>
        /// Gets a boolean returning true if an interceptor of type T, exists
        /// </summary>
        [DebuggerStepThrough]
        internal bool ContainsInterceptor<T>() where T : ProgressArgs => _interceptors.Contains<T>();

        /// <summary>
        /// Try to report progress
        /// </summary>
        internal void ReportProgress(SyncContext context, IProgress<ProgressArgs> progress, ProgressArgs args, DbConnection connection = null, DbTransaction transaction = null)
        {
            // Check logger, because we make some reflection here
            if (Logger.IsEnabled(LogLevel.Information))
            {
                var argsTypeName = args.GetType().Name.Replace("Args", ""); ;
                if (Logger.IsEnabled(LogLevel.Debug))
                    Logger.LogDebug(new EventId(args.EventId, argsTypeName),null, args.Context);
                else
                    Logger.LogInformation(new EventId(args.EventId, argsTypeName), null, args);
            }

            if (progress is null)
                return;

            if (connection is null && args.Connection is not null)
                connection = args.Connection;

            if (transaction is null && args.Transaction is not null)
                transaction = args.Transaction;

            if (args.Connection is null || args.Connection != connection)
                args.Connection = connection;

            if (args.Transaction is null || args.Transaction != transaction)
                args.Transaction = transaction;

            if (Options.ProgressLevel <= args.ProgressLevel)
                progress.Report(args);
        }

        /// <summary>
        /// Open a connection
        /// </summary>
        //[DebuggerStepThrough]
        internal async Task OpenConnectionAsync(DbConnection connection, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            // Make an interceptor when retrying to connect
            var onRetry = new Func<Exception, int, TimeSpan, object, Task>((ex, cpt, ts, arg) =>
                InterceptAsync(new ReConnectArgs(GetContext(), connection, ex, cpt, ts), progress, cancellationToken));

            // Defining my retry policy
            var policy = SyncPolicy.WaitAndRetry(
                                3,
                                retryAttempt => TimeSpan.FromMilliseconds(500 * retryAttempt),
                                (ex, arg) => Provider.ShouldRetryOn(ex),
                                onRetry);

            // Execute my OpenAsync in my policy context
            await policy.ExecuteAsync(ct => connection.OpenAsync(ct), cancellationToken);

            // Let provider knows a connection is opened
            Provider.OnConnectionOpened(connection);

            await InterceptAsync(new ConnectionOpenedArgs(GetContext(), connection), progress, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Close a connection
        /// </summary>
        internal async Task CloseConnectionAsync(DbConnection connection, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            if (connection is not null && connection.State == ConnectionState.Closed)
                return;

            bool isClosedHere = false;

            if (connection is not null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                isClosedHere = true;
            }

            if (!cancellationToken.IsCancellationRequested)
                await InterceptAsync(new ConnectionClosedArgs(GetContext(), connection), progress, cancellationToken).ConfigureAwait(false);

            // Let provider knows a connection is closed
            Provider.OnConnectionClosed(connection);

            if (isClosedHere && connection is not null)
                connection.Dispose();
        }


        [DebuggerStepThrough]
        internal SyncException GetSyncError(Exception exception)
        {
            var syncException = new SyncException(exception, GetContext().SyncStage);

            // try to let the provider enrich the exception
            Provider.EnsureSyncException(syncException);
            syncException.Side = Side;

            Logger.LogError(SyncEventsId.Exception, syncException, syncException.Message);

            return syncException;
        }

        /// <summary>
        /// Get the provider sync adapter
        /// </summary>
        public DbSyncAdapter GetSyncAdapter(SyncTable tableDescription, SyncSetup setup)
        {
            var (tableName, trackingTableName) = Provider.GetParsers(tableDescription, setup);
            return Provider.GetSyncAdapter(tableDescription, tableName, trackingTableName, setup);
        }

        /// <summary>
        /// Get the provider table builder
        /// </summary>
        public DbTableBuilder GetTableBuilder(SyncTable tableDescription, SyncSetup setup)
        {
            var (tableName, trackingTableName) = Provider.GetParsers(tableDescription, setup);
            return Provider.GetTableBuilder(tableDescription, tableName, trackingTableName, setup);
        }


        /// <summary>
        /// Get a provider scope builder by scope table name
        /// </summary>
        public DbScopeBuilder GetScopeBuilder(string scopeInfoTableName)
        {
            return Provider.GetScopeBuilder(scopeInfoTableName);
        }
        /// <summary>
        /// Sets the current context
        /// </summary>
        internal virtual void SetContext(SyncContext context) => _syncContext = context;

        /// <summary>
        /// Gets the current context
        /// </summary>
        [DebuggerStepThrough]
        public virtual SyncContext GetContext()
        {
            if (_syncContext is not null)
                return _syncContext;

            _syncContext = new SyncContext(Guid.NewGuid(), ScopeName); ;

            return _syncContext;
        }


        /// <summary>
        /// Check if the orchestrator database is outdated
        /// </summary>
        /// <param name="clientScopeInfo"></param>
        /// <param name="serverScopeInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual async Task<bool> IsOutDatedAsync(ScopeInfo clientScopeInfo, ServerScopeInfo serverScopeInfo, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

            bool isOutdated = false;

            // Get context or create a new one
            var ctx = GetContext();

            // if we have a new client, obviously the last server sync is < to server stored last clean up (means OutDated !)
            // so far we return directly false
            if (clientScopeInfo.IsNewScope)
                return false;

            // Check if the provider is not outdated
            // We can have negative value where we want to compare anyway
            if (clientScopeInfo.LastServerSyncTimestamp != 0 || serverScopeInfo.LastCleanupTimestamp != 0)
                isOutdated = clientScopeInfo.LastServerSyncTimestamp < serverScopeInfo.LastCleanupTimestamp;

            // Get a chance to make the sync even if it's outdated
            if (isOutdated)
            {
                var outdatedArgs = new OutdatedArgs(ctx, clientScopeInfo, serverScopeInfo);

                // Interceptor
                await InterceptAsync(outdatedArgs, progress, cancellationToken).ConfigureAwait(false);

                if (outdatedArgs.Action != OutdatedAction.Rollback)
                    ctx.SyncType = outdatedArgs.Action == OutdatedAction.Reinitialize ? SyncType.Reinitialize : SyncType.ReinitializeWithUpload;

                if (outdatedArgs.Action == OutdatedAction.Rollback)
                    throw new OutOfDateException(clientScopeInfo.LastServerSyncTimestamp, serverScopeInfo.LastCleanupTimestamp);
            }

            return isOutdated;
        }

        /// <summary>
        /// Get hello from database
        /// </summary>
        public virtual async Task<(SyncContext SyncContext, string DatabaseName, string Version)> GetHelloAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = default)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.None, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var databaseBuilder = Provider.GetDatabaseBuilder();
                var hello = await databaseBuilder.GetHelloAsync(runner.Connection, runner.Transaction);
                await runner.CommitAsync().ConfigureAwait(false);
                return (GetContext(), hello.DatabaseName, hello.Version);
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }



        public async Task<T> RunInTransactionAsync2<T>(SyncStage stage = SyncStage.None, Func<SyncContext, DbConnection, DbTransaction, Task<T>> actionTask = null,
              DbConnection connection = null, DbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

            // Get context or create a new one
            var ctx = GetContext();

            T result = default;

            using var c = Provider.CreateConnection();

            try
            {
                await c.OpenAsync();

                using (var t = c.BeginTransaction(Provider.IsolationLevel))
                {
                    if (actionTask is not null)
                        result = await actionTask(ctx, c, t);

                    t.Commit();
                }
                c.Close();
                c.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Run an actin inside a connection / transaction
        /// </summary>
        public async Task<T> RunInTransactionAsync<T>(SyncStage stage = SyncStage.None, Func<SyncContext, DbConnection, DbTransaction, Task<T>> actionTask = null,
              DbConnection connection = null, DbTransaction transaction = null, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = default)
        {
            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

            // Get context or create a new one
            var ctx = GetContext();

            T result = default;

            if (connection is null)
                connection = Provider.CreateConnection();

            var alreadyOpened = connection.State == ConnectionState.Open;
            var alreadyInTransaction = transaction is not null && transaction.Connection == connection;

            try
            {
                if (stage != SyncStage.None)
                    ctx.SyncStage = stage;

                // Open connection
                if (!alreadyOpened)
                    await OpenConnectionAsync(connection, cancellationToken, progress).ConfigureAwait(false);

                // Create a transaction
                if (!alreadyInTransaction)
                {
                    transaction = connection.BeginTransaction(Provider.IsolationLevel);
                    await InterceptAsync(new TransactionOpenedArgs(ctx, connection, transaction), progress, cancellationToken).ConfigureAwait(false);
                }

                if (actionTask is not null)
                    result = await actionTask(ctx, connection, transaction);

                if (!alreadyInTransaction)
                {
                    await InterceptAsync(new TransactionCommitArgs(ctx, connection, transaction), progress, cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                    transaction.Dispose();
                }

                if (!alreadyOpened)
                    await CloseConnectionAsync(connection, cancellationToken, progress).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
            finally
            {
                if (!alreadyOpened)
                    await CloseConnectionAsync(connection, cancellationToken, progress).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get a snapshot root directory name and folder directory name
        /// </summary>
        public virtual Task<(string DirectoryRoot, string DirectoryName)> GetSnapshotDirectoryAsync(SyncParameters syncParameters = null, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Get context or create a new one
            var ctx = GetContext();

            // check parameters
            // If context has no parameters specified, and user specifies a parameter collection we switch them
            if ((ctx.Parameters is null || ctx.Parameters.Count <= 0) && syncParameters is not null && syncParameters.Count > 0)
                ctx.Parameters = syncParameters;

            return InternalGetSnapshotDirectoryAsync(ctx, cancellationToken, progress);
        }

        /// <summary>
        /// Internal routine to clean tmp folders. MUST be compare also with Options.CleanFolder
        /// </summary>
        internal virtual async Task<bool> InternalCanCleanFolderAsync(SyncContext context, BatchInfo batchInfo,
                             CancellationToken cancellationToken, IProgress<ProgressArgs> progress = null)
        {
            var batchInfoDirectoryFullPath = new DirectoryInfo(batchInfo.GetDirectoryFullPath());

            var (snapshotRootDirectory, snapshotNameDirectory) = await GetSnapshotDirectoryAsync();

            // if we don't have any snapshot configuration, we are sure that the current batchinfo is actually stored into a temp folder
            if (string.IsNullOrEmpty(snapshotRootDirectory))
                return true;

            var snapInfo = Path.Combine(snapshotRootDirectory, snapshotNameDirectory);
            var snapshotDirectoryFullPath = new DirectoryInfo(snapInfo);

            // check if the batch dir IS NOT the snapshot directory
            var canCleanFolder = batchInfoDirectoryFullPath.FullName != snapshotDirectoryFullPath.FullName;

            return canCleanFolder;
        }

        /// <summary>
        /// Internal routine to get the snapshot root directory and batch directory name
        /// </summary>
        internal virtual Task<(string DirectoryRoot, string DirectoryName)> InternalGetSnapshotDirectoryAsync(SyncContext context,
                             CancellationToken cancellationToken, IProgress<ProgressArgs> progress = null)
        {

            if (string.IsNullOrEmpty(Options.SnapshotsDirectory))
                return Task.FromResult<(string, string)>((default, default));

            // cleansing scope name
            var directoryScopeName = new string(context.ScopeName.Where(char.IsLetterOrDigit).ToArray());

            var directoryFullPath = Path.Combine(Options.SnapshotsDirectory, directoryScopeName);

            var sb = new StringBuilder();
            var underscore = "";

            if (context.Parameters is not null)
            {
                foreach (var p in context.Parameters.OrderBy(p => p.Name))
                {
                    var cleanValue = new string(p.Value.ToString().Where(char.IsLetterOrDigit).ToArray());
                    var cleanName = new string(p.Name.Where(char.IsLetterOrDigit).ToArray());

                    sb.Append($"{underscore}{cleanName}_{cleanValue}");
                    underscore = "_";
                }
            }

            var directoryName = sb.ToString();
            directoryName = string.IsNullOrEmpty(directoryName) ? "ALL" : directoryName;

            return Task.FromResult((directoryFullPath, directoryName));
        }
    }
}
