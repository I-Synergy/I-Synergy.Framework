﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Parameter;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    /// <summary>
    /// Sync agent. It's the sync orchestrator
    /// Knows both the Sync Server provider and the Sync Client provider
    /// </summary>
    public class SyncAgent : IDisposable
    {
        private readonly IVersionService _versionService;

        private bool syncInProgress;
        private readonly object lockObj = new object();

        /// <summary>
        /// Gets or Sets the scope name, defining the tables involved in the sync
        /// </summary>
        public string ScopeName { get; private set; }

        /// <summary>
        /// Defines the state that a synchronization session is in.
        /// </summary>
        public SyncSessionState SessionState { get; set; } = SyncSessionState.Ready;

        /// <summary>
        /// Gets or Sets the local orchestrator
        /// </summary>
        public LocalOrchestrator LocalOrchestrator { get; set; }

        /// <summary>
        /// Get or Sets the remote orchestrator
        /// </summary>
        public RemoteOrchestrator RemoteOrchestrator { get; set; }

        /// <summary>
        /// Get or Sets the Sync parameters to pass to Remote provider for filtering rows
        /// </summary>
        public SyncParameters Parameters { get; private set; } = new SyncParameters();

        /// <summary>
        /// Occurs when sync is starting, ending
        /// </summary>
        public event EventHandler<SyncSessionState> SessionStateChanged = null;

        /// <summary>
        /// Gets the setup used for this sync
        /// </summary>
        public SyncSetup Setup => LocalOrchestrator?.Setup;

        /// <summary>
        /// Gets the options used on this sync process.
        /// </summary>
        public SyncOptions Options => LocalOrchestrator?.Options;

        public SyncSet Schema { get; private set; }

        /// <summary>
        /// Set interceptors on the LocalOrchestrator
        /// </summary>
        public void SetInterceptors(Interceptors interceptors) => LocalOrchestrator.On(interceptors);


        /// <summary>
        /// Shortcut to Apply changed failed if remote orchestrator supports it
        /// </summary>
        public void OnApplyChangesFailed(Action<ApplyChangesFailedArgs> action)
        {
            if (RemoteOrchestrator is null)
                throw new InvalidRemoteOrchestratorException();

            RemoteOrchestrator.OnApplyChangesFailed(action);
        }


        /// <summary>
        /// Lock sync to prevent multi call to sync at the same time
        /// </summary>
        private void LockSync()
        {
            lock (lockObj)
            {
                if (syncInProgress)
                    throw new AlreadyInProgressException();

                syncInProgress = true;
            }
        }

        /// <summary>
        /// Unlock sync to be able to launch a new sync
        /// </summary>
        private void UnlockSync()
        {
            // Enf sync from local provider
            lock (lockObj)
            {
                syncInProgress = false;
            }
        }

        private SyncAgent(IVersionService versionService, string scopeName)
        {
            _versionService = versionService;
            ScopeName = scopeName;
        }

        // 1
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="serverProvider">Local Provider connecting to your server database</param>
        /// <param name="tables">Tables list to synchronize</param>
        /// <param name="scopeName">Scope Name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, IProvider serverProvider, string[] tables, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, serverProvider, new SyncOptions(), new SyncSetup(tables), scopeName)
        {
        }

        // 2
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="serverProvider">Local Provider connecting to your server database</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, IProvider serverProvider, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, serverProvider, new SyncOptions(), new SyncSetup(), scopeName)
        {
        }

        // 3
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="serverProvider">Local Provider connecting to your server database</param>
        /// <param name="options">Sync Options defining options used by your local and remote provider</param>
        /// <param name="tables">tables list</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, IProvider serverProvider, SyncOptions options, string[] tables, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, serverProvider, options, new SyncSetup(tables), scopeName)
        {
        }

        // 3.5
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="serverProvider">Local Provider connecting to your server database</param>
        /// <param name="setup">Sync Setup containing your tables and columns list.</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, IProvider serverProvider, SyncSetup setup, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, serverProvider, new SyncOptions(), setup, scopeName)
        {
        }

        // 4
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="serverProvider">Local Provider connecting to your server database</param>
        /// <param name="options">Sync Options defining options used by your local and remote provider</param>
        /// <param name="setup">Sync Setup containing the definition of your tables, columns, filters and naming conventions.</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, IProvider serverProvider, SyncOptions options, SyncSetup setup, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, scopeName)
        {
            Argument.IsNotNull(versionService);
            Argument.IsNotNull(setup);
            Argument.IsNotNull(options);
            Argument.IsNotNull(clientProvider);
            Argument.IsNotNull(serverProvider);

            // Affect local and remote orchestrators
            LocalOrchestrator = new LocalOrchestrator(versionService, clientProvider, options, setup, scopeName);
            RemoteOrchestrator = new RemoteOrchestrator(versionService, serverProvider, options, setup, scopeName);

            EnsureOptionsAndSetupInstances();
        }

        // 5
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="remoteOrchestrator">Remote Orchestrator already configured with a SyncProvider</param>
        /// <param name="tables">tables list</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, RemoteOrchestrator remoteOrchestrator, string[] tables, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, remoteOrchestrator, new SyncOptions(), new SyncSetup(tables), scopeName)
        {
        }


        // 6
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="remoteOrchestrator">Remote Orchestrator already configured with a SyncProvider</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, RemoteOrchestrator remoteOrchestrator, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, remoteOrchestrator, new SyncOptions(), new SyncSetup(), scopeName)
        {
        }


        // 7
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="remoteOrchestrator">Remote Orchestrator already configured with a SyncProvider</param>
        /// <param name="options">Sync Options defining options used by your local provider (and remote provider if remoteOrchestrator is not a WebClientOrchestrator)</param>
        /// <param name="tables">tables list</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, RemoteOrchestrator remoteOrchestrator, SyncOptions options, string[] tables, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, remoteOrchestrator, options, new SyncSetup(tables), scopeName)
        {
        }

        // 8
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="remoteOrchestrator">Remote Orchestrator already configured with a SyncProvider</param>
        /// <param name="options">Sync Options defining options used by your local provider (and remote provider if remoteOrchestrator is not a WebClientOrchestrator)</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, RemoteOrchestrator remoteOrchestrator, SyncOptions options, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, remoteOrchestrator, options, new SyncSetup(), scopeName)
        {
        }

        // 8.5
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">Local Provider connecting to your client database</param>
        /// <param name="remoteOrchestrator">Remote Orchestrator already configured with a SyncProvider</param>
        /// <param name="setup"></param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, RemoteOrchestrator remoteOrchestrator, SyncSetup setup, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, clientProvider, remoteOrchestrator, new SyncOptions(), setup, scopeName)
        {
        }

        // 9
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="clientProvider">local provider to your client database</param>
        /// <param name="remoteOrchestrator">Remote Orchestrator already configured with a SyncProvider</param>
        /// <param name="options">Sync Options defining options used by your local provider (and remote provider if remoteOrchestrator is not a WebClientOrchestrator)</param>
        /// <param name="setup">Sync Setup containing the definition of your tables, columns, filters and naming conventions.</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, IProvider clientProvider, RemoteOrchestrator remoteOrchestrator, SyncOptions options, SyncSetup setup, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, scopeName)
        {
            if (clientProvider is null)
                throw new ArgumentNullException(nameof(clientProvider));
            if (remoteOrchestrator is null)
                throw new ArgumentNullException(nameof(remoteOrchestrator));
            if (options is null)
                throw new ArgumentNullException(nameof(options));
            if (setup is null)
                throw new ArgumentNullException(nameof(setup));

            // Override remote orchestrator options, setup and scope name
            remoteOrchestrator.Options = options;
            remoteOrchestrator.Setup = setup;
            remoteOrchestrator.ScopeName = scopeName;

            var localOrchestrator = new LocalOrchestrator(versionService, clientProvider, options, setup, scopeName);

            LocalOrchestrator = localOrchestrator;
            RemoteOrchestrator = remoteOrchestrator;

            EnsureOptionsAndSetupInstances();
        }

        // 10
        /// <summary>
        /// Creates a synchronization agent that will handle a full synchronization between a client and a server.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="localOrchestrator">Local Orchestrator already configured with a SyncProvider</param>
        /// <param name="remoteOrchestrator">Remote Orchestrator already configured with a SyncProvider</param>
        /// <param name="scopeName">scope name</param>
        public SyncAgent(IVersionService versionService, LocalOrchestrator localOrchestrator, RemoteOrchestrator remoteOrchestrator, string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, scopeName)
        {
            if (localOrchestrator is null)
                throw new ArgumentNullException(nameof(localOrchestrator));
            if (remoteOrchestrator is null)
                throw new ArgumentNullException(nameof(remoteOrchestrator));

            LocalOrchestrator = localOrchestrator;
            RemoteOrchestrator = remoteOrchestrator;

            EnsureOptionsAndSetupInstances();
        }



        /// <summary>
        /// Ensure Options and Setup instances are the same on local orchestrator and remote orchestrator
        /// </summary>
        private void EnsureOptionsAndSetupInstances()
        {
            // if we have a remote orchestrator with different options, raise an error
            if (RemoteOrchestrator.Options is not null && RemoteOrchestrator.Options != LocalOrchestrator.Options)
                throw new OptionsReferencesAreNotSameExecption();
            else if (RemoteOrchestrator.Options is null)
                RemoteOrchestrator.Options = LocalOrchestrator.Options;

            // if we have a remote orchestrator with different options, raise an error
            if (RemoteOrchestrator.Setup is not null && !RemoteOrchestrator.Setup.EqualsByProperties(LocalOrchestrator.Setup))
                throw new SetupReferencesAreNotSameExecption();
            else if (RemoteOrchestrator.Setup is null)
                RemoteOrchestrator.Setup = LocalOrchestrator.Setup;

        }

        /// <summary>
        /// Launch a normal synchronization without any IProgess or CancellationToken
        /// </summary>
        public Task<SyncResult> SynchronizeAsync() => SynchronizeAsync(SyncType.Normal, CancellationToken.None);

        /// <summary>
        /// Launch a normal synchronization without any IProgess or CancellationToken
        /// </summary>
        public Task<SyncResult> SynchronizeAsync(IProgress<ProgressArgs> progress) => SynchronizeAsync(SyncType.Normal, CancellationToken.None, progress);

        /// <summary>
        /// Launch a synchronization with a SyncType specified
        /// </summary>
        public Task<SyncResult> SynchronizeAsync(SyncType syncType, IProgress<ProgressArgs> progress = null) => SynchronizeAsync(syncType, CancellationToken.None, progress);

        /// <summary>
        /// Launch a synchronization with the specified mode
        /// </summary>
        public async Task<SyncResult> SynchronizeAsync(SyncType syncType, CancellationToken cancellationToken, IProgress<ProgressArgs> progress = null)
        {
            // checkpoints dates
            var startTime = DateTime.UtcNow;
            var completeTime = DateTime.UtcNow;

            // Create a logger
            var logger = Options.Logger ?? new LoggerFactory().CreateLogger(nameof(SyncAgent));

            // Lock sync to prevent multi call to sync at the same time
            LockSync();

            // Context, used to back and forth data between servers
            var context = new SyncContext(Guid.NewGuid(), ScopeName)
            {
                // if any parameters, set in context
                Parameters = Parameters,
                // set sync type (Normal, Reinitialize, ReinitializeWithUpload)
                SyncType = syncType
            };

            // Result, with sync results stats.
            var result = new SyncResult(context.SessionId)
            {
                // set start time
                StartTime = startTime,
                CompleteTime = completeTime,
            };

            SessionState = SyncSessionState.Synchronizing;
            SessionStateChanged?.Invoke(this, SessionState);
            await Task.Run(async () =>
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    ServerScopeInfo serverScopeInfo = null;

                    // Internal set the good reference. Don't use the SetContext method here
                    LocalOrchestrator._syncContext = context;
                    RemoteOrchestrator._syncContext = context;
                    LocalOrchestrator.StartTime = startTime;
                    RemoteOrchestrator.StartTime = startTime;

                    // Begin session
                    await LocalOrchestrator.BeginSessionAsync(cancellationToken, progress).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    // On local orchestrator, get scope info
                    var clientScopeInfo = await LocalOrchestrator.GetClientScopeAsync(default, default, cancellationToken, progress).ConfigureAwait(false);

                    // Register local scope id
                    context.ClientScopeId = clientScopeInfo.Id;

                    // if client is new or else schema does not exists
                    // We need to get it from server
                    if (clientScopeInfo.IsNewScope || clientScopeInfo.Schema is null)
                    {
                        // Ensure schema is defined on remote side
                        // This action will create schema on server if needed
                        // if schema already exists on server, then the server setup will be compared with this one
                        // if setup is different, it will be migrated.
                        // so serverScopeInfo.Setup MUST be equal to Setup
                        serverScopeInfo = await RemoteOrchestrator.EnsureSchemaAsync(default, default, cancellationToken, progress).ConfigureAwait(false);

                        // Affect local setup since the setup could potentially comes from Web server
                        // Affect local setup (equivalent to Setup)
                        if (!Setup.EqualsByProperties(serverScopeInfo.Setup) && !Setup.HasTables)
                            LocalOrchestrator.Setup = serverScopeInfo.Setup;

                        // Provision local database
                        var provision = SyncProvision.Table | SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;
                        await LocalOrchestrator.ProvisionAsync(serverScopeInfo.Schema, provision, false, clientScopeInfo, default, default, cancellationToken, progress).ConfigureAwait(false);

                        // Set schema for agent, just to let the opportunity to user to use it.
                        Schema = serverScopeInfo.Schema;
                    }
                    else
                    {
                        // Affect local setup since the setup could potentially comes from Web server
                        // Affect local setup (equivalent to Setup)
                        if (!Setup.EqualsByProperties(clientScopeInfo.Setup) && !Setup.HasTables)
                            LocalOrchestrator.Setup = clientScopeInfo.Setup;

                        // Do we need to upgrade ?
                        if (LocalOrchestrator.InternalNeedsToUpgrade(clientScopeInfo))
                        {
                            var newScope = await LocalOrchestrator.UpgradeAsync(clientScopeInfo, default, default, cancellationToken, progress).ConfigureAwait(false);
                            if (newScope is not null)
                                clientScopeInfo = newScope;
                        }

                        // on remote orchestrator get scope info as well
                        // if setup is different, it will be migrated.
                        // so serverScopeInfo.Setup MUST be equal to Setup
                        serverScopeInfo = await RemoteOrchestrator.GetServerScopeAsync(default, default, cancellationToken, progress).ConfigureAwait(false);

                        // compare local setup options with setup provided on SyncAgent constructor (check if pref / suf have changed)
                        var hasSameOptions = clientScopeInfo.Setup.HasSameOptions(Setup);

                        // compare local setup strucutre with remote structure
                        var hasSameStructure = clientScopeInfo.Setup.HasSameStructure(serverScopeInfo.Setup);

                        if (hasSameStructure)
                        {
                            // Set schema & setup
                            Schema = clientScopeInfo.Schema;

                            // Schema could be null if from web server 
                            if (serverScopeInfo.Schema is null)
                                serverScopeInfo.Schema = clientScopeInfo.Schema;
                        }
                        else
                        {
                            // Get full schema from server
                            serverScopeInfo = await RemoteOrchestrator.EnsureSchemaAsync(default, default, cancellationToken, progress).ConfigureAwait(false);

                            // Set the correct schema
                            Schema = serverScopeInfo.Schema;
                        }

                        // If one of the comparison is false, we make a migration
                        if (!hasSameOptions || !hasSameStructure)
                            clientScopeInfo = await LocalOrchestrator.MigrationAsync(clientScopeInfo.Setup, serverScopeInfo.Setup, serverScopeInfo.Schema, default, default, cancellationToken, progress).ConfigureAwait(false);

                        // Affect local setup (equivalent to Setup)
                        LocalOrchestrator.Setup = serverScopeInfo.Setup;
                    }

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    // Before call the changes from localorchestrator, check if we are outdated
                    if (serverScopeInfo is not null && context.SyncType != SyncType.Reinitialize && context.SyncType != SyncType.ReinitializeWithUpload)
                    {
                        var isOutDated = await LocalOrchestrator.IsOutDatedAsync(clientScopeInfo, serverScopeInfo).ConfigureAwait(false);

                        // if client does not change SyncType to Reinitialize / ReinitializeWithUpload on SyncInterceptor, we raise an error
                        // otherwise, we are outdated, but we can continue, because we have a new mode.
                        if (isOutDated)
                            Debug.WriteLine($"Client id outdated, but we change mode to {context.SyncType}");
                    }

                    context.ProgressPercentage = 0.1;

                    // On local orchestrator, get local changes
                    var clientChanges = await LocalOrchestrator.GetChangesAsync(clientScopeInfo, default, default, cancellationToken, progress).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    // Reinitialize timestamp is in Reinit Mode
                    if (context.SyncType == SyncType.Reinitialize || context.SyncType == SyncType.ReinitializeWithUpload)
                        clientScopeInfo.LastServerSyncTimestamp = null;

                    // Get if we need to get all rows from the datasource
                    var fromScratch = clientScopeInfo.IsNewScope || context.SyncType == SyncType.Reinitialize || context.SyncType == SyncType.ReinitializeWithUpload;

                    // IF is new and we have a snapshot directory, try to apply a snapshot
                    if (fromScratch)
                    {
                        // Get snapshot files
                        var serverSnapshotChanges = await RemoteOrchestrator.GetSnapshotAsync(Schema, default, default, cancellationToken, progress).ConfigureAwait(false);

                        // Apply snapshot
                        if (serverSnapshotChanges.ServerBatchInfo is not null)
                        {
                            (result.SnapshotChangesAppliedOnClient, clientScopeInfo) = await LocalOrchestrator.ApplySnapshotAsync(
                                clientScopeInfo, serverSnapshotChanges.ServerBatchInfo, clientChanges.ClientTimestamp, serverSnapshotChanges.RemoteClientTimestamp, serverSnapshotChanges.DatabaseChangesSelected, default, default, cancellationToken, progress).ConfigureAwait(false);
                        }
                    }

                    // Get if we have already applied a snapshot, so far we don't need to reset table even if we are i Reinitialize Mode
                    var snapshotApplied = result.SnapshotChangesAppliedOnClient is not null;

                    context.ProgressPercentage = 0.3;
                    // apply is 25%, get changes is 20%
                    var serverChanges = await RemoteOrchestrator.ApplyThenGetChangesAsync(clientScopeInfo, clientChanges.ClientBatchInfo, default, default, cancellationToken, progress).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    // Policy is always Server policy, so reverse this policy to get the client policy
                    var reverseConflictResolutionPolicy = serverChanges.ServerPolicy == ConflictResolutionPolicy.ServerWins ? ConflictResolutionPolicy.ClientWins : ConflictResolutionPolicy.ServerWins;

                    // apply is 25%
                    context.ProgressPercentage = 0.75;
                    var clientChangesApplied = await LocalOrchestrator.ApplyChangesAsync(
                        clientScopeInfo, Schema, serverChanges.ServerBatchInfo,
                        clientChanges.ClientTimestamp, serverChanges.RemoteClientTimestamp, reverseConflictResolutionPolicy, snapshotApplied,
                        serverChanges.ServerChangesSelected, default, default, cancellationToken, progress).ConfigureAwait(false);

                    completeTime = DateTime.UtcNow;
                    LocalOrchestrator.CompleteTime = completeTime;
                    RemoteOrchestrator.CompleteTime = completeTime;

                    result.CompleteTime = completeTime;

                    // All clients changes selected
                    result.ClientChangesSelected = clientChanges.ClientChangesSelected;
                    result.ServerChangesSelected = serverChanges.ServerChangesSelected;
                    result.ChangesAppliedOnClient = clientChangesApplied.ChangesApplied;
                    result.ChangesAppliedOnServer = serverChanges.ClientChangesApplied;

                    // Begin session
                    context.ProgressPercentage = 1;
                    await LocalOrchestrator.EndSessionAsync(cancellationToken, progress).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                }

                catch (SyncException se)
                {
                    Options.Logger.LogError(SyncEventsId.Exception, se, se.TypeName);
                    throw;
                }
                catch (Exception ex)
                {
                    Options.Logger.LogCritical(SyncEventsId.Exception, ex, ex.Message);
                    throw new SyncException(ex, SyncStage.None);
                }
                finally
                {
                    // End the current session
                    SessionState = SyncSessionState.Ready;
                    SessionStateChanged?.Invoke(this, SessionState);
                    // unlock sync since it's over
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    UnlockSync();
                }
            });

            return result;
        }

        #region IDisposable
        // Dispose() calls Dispose(true)
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        //~ObservableClass()
        //{
        //    // Finalizer calls Dispose(false)
        //    Dispose(false);
        //}

        // The bulk of the clean-up code is implemented in Dispose(bool)
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources if there are any.
        }
        #endregion
    }
}