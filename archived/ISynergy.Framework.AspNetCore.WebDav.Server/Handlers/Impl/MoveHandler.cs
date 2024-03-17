using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Engines;
using ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Local;
using ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Remote;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers.Impl
{
    /// <summary>
    /// The implementation of the <see cref="IMoveHandler"/> interface.
    /// </summary>
    public class MoveHandler : CopyMoveHandlerBase, IMoveHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MoveHandlerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveHandler"/> class.
        /// </summary>
        /// <param name="rootFileSystem">The root file system</param>
        /// <param name="host">The WebDAV server context</param>
        /// <param name="options">The options for the <c>MOVE</c> handler</param>
        /// <param name="logger">The logger for this handler</param>
        /// <param name="serviceProvider">The service provider used to lazily query the <see cref="IRemoteMoveTargetActionsFactory"/> implementation</param>
        public MoveHandler(IFileSystem rootFileSystem, IWebDavContext host, IOptions<MoveHandlerOptions> options, ILogger<MoveHandler> logger, IServiceProvider serviceProvider)
            : base(rootFileSystem, host, logger)
        {
            _serviceProvider = serviceProvider;
            _options = options?.Value ?? new MoveHandlerOptions();
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; } = new[] { "MOVE" };

        /// <inheritdoc />
        public Task<IWebDavResult> MoveAsync(string sourcePath, Uri destination, CancellationToken cancellationToken)
        {
            var doOverwrite = WebDavContext.RequestHeaders.Overwrite ?? _options.OverwriteAsDefault;
            return ExecuteAsync(sourcePath, destination, DepthHeader.Infinity, doOverwrite, _options.Mode, true, cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task<IRemoteTargetActions> CreateRemoteTargetActionsAsync(Uri destinationUrl, CancellationToken cancellationToken)
        {
            var remoteTargetActionsFactory = _serviceProvider.GetService<IRemoteMoveTargetActionsFactory>();
            if (remoteTargetActionsFactory != null)
            {
                var targetActions = await remoteTargetActionsFactory
                    .CreateMoveTargetActionsAsync(destinationUrl, cancellationToken);
                if (targetActions != null)
                    return targetActions;
            }

            return null;
        }

        /// <inheritdoc />
        protected override ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> CreateLocalTargetActions(RecursiveProcessingMode mode)
        {
            if (mode == RecursiveProcessingMode.PreferFastest)
                return new MoveInFileSystemTargetAction(WebDavContext.Dispatcher, Logger);
            return new MoveBetweenFileSystemsTargetAction(WebDavContext.Dispatcher);
        }
    }
}
