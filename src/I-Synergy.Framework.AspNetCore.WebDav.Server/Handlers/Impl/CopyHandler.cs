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
    /// The implementation of the <see cref="ICopyHandler"/> interface.
    /// </summary>
    public class CopyHandler : CopyMoveHandlerBase, ICopyHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CopyHandlerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyHandler"/> class.
        /// </summary>
        /// <param name="rootFileSystem">The root file system</param>
        /// <param name="host">The WebDAV server context</param>
        /// <param name="options">The options for the <c>COPY</c> handler</param>
        /// <param name="logger">The logger for this handler</param>
        /// <param name="serviceProvider">The service provider used to lazily query the <see cref="IRemoteCopyTargetActionsFactory"/> implementation</param>
        public CopyHandler(IFileSystem rootFileSystem, IWebDavContext host, IOptions<CopyHandlerOptions> options, ILogger<CopyHandler> logger, IServiceProvider serviceProvider)
            : base(rootFileSystem, host, logger)
        {
            _serviceProvider = serviceProvider;
            _options = options?.Value ?? new CopyHandlerOptions();
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; } = new[] { "COPY" };

        /// <inheritdoc />
        public Task<IWebDavResult> CopyAsync(string path, Uri destination, CancellationToken cancellationToken)
        {
            var doOverwrite = WebDavContext.RequestHeaders.Overwrite ?? _options.OverwriteAsDefault;
            var depth = WebDavContext.RequestHeaders.Depth ?? DepthHeader.Infinity;
            return ExecuteAsync(path, destination, depth, doOverwrite, _options.Mode, false, cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task<IRemoteTargetActions> CreateRemoteTargetActionsAsync(Uri destinationUrl, CancellationToken cancellationToken)
        {
            var remoteTargetActionsFactory = _serviceProvider.GetService<IRemoteCopyTargetActionsFactory>();
            if (remoteTargetActionsFactory != null)
            {
                var targetActions = await remoteTargetActionsFactory
                    .CreateCopyTargetActionsAsync(destinationUrl, cancellationToken);
                if (targetActions != null)
                    return targetActions;
            }

            return null;
        }

        /// <inheritdoc />
        protected override ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> CreateLocalTargetActions(RecursiveProcessingMode mode)
        {
            if (mode == RecursiveProcessingMode.PreferFastest)
                return new CopyInFileSystemTargetAction(WebDavContext.Dispatcher);
            return new CopyBetweenFileSystemsTargetAction(WebDavContext.Dispatcher);
        }
    }
}
