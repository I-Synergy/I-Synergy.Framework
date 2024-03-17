using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Local
{
    /// <summary>
    /// The <see cref="ITargetActions{TCollection,TDocument,TMissing}"/> implementation that copies only entries within the same file system
    /// </summary>
    public class CopyInFileSystemTargetAction : ITargetActions<CollectionTarget, DocumentTarget, MissingTarget>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyInFileSystemTargetAction"/> class.
        /// </summary>
        /// <param name="dispatcher">The WebDAV dispatcher</param>
        public CopyInFileSystemTargetAction(IWebDavDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        /// <inheritdoc />
        public IWebDavDispatcher Dispatcher { get; }

        /// <inheritdoc />
        public RecursiveTargetBehaviour ExistingTargetBehaviour { get; } = RecursiveTargetBehaviour.Overwrite;

        /// <inheritdoc />
        public async Task<DocumentTarget> ExecuteAsync(IDocument source, MissingTarget destination, CancellationToken cancellationToken)
        {
            var doc = await source.CopyToAsync(destination.Parent.Collection, destination.Name, cancellationToken);
            return new DocumentTarget(destination.Parent, destination.DestinationUrl, doc, this);
        }

        /// <inheritdoc />
        public async Task<ActionResult> ExecuteAsync(IDocument source, DocumentTarget destination, CancellationToken cancellationToken)
        {
            try
            {
                Debug.Assert(destination.Parent != null, "destination.Parent != null");
                await source.CopyToAsync(destination.Parent.Collection, destination.Name, cancellationToken);
                return new ActionResult(ActionStatus.Overwritten, destination);
            }
            catch (Exception ex)
            {
                return new ActionResult(ActionStatus.OverwriteFailed, destination)
                {
                    Exception = ex,
                };
            }
        }

        /// <inheritdoc />
        public Task ExecuteAsync(ICollection source, CollectionTarget destination, CancellationToken cancellationToken)
        {
            return CopyETagAsync(source, destination.Collection, cancellationToken);
        }

        private static async Task CopyETagAsync(IEntry source, IEntry dest, CancellationToken cancellationToken)
        {
            if (dest is IEntityTagEntry)
                return;

            var sourcePropStore = source.FileSystem.PropertyStore;
            var destPropStore = dest.FileSystem.PropertyStore;
            if (sourcePropStore != null && destPropStore != null)
            {
                var etag = await sourcePropStore.GetETagAsync(source, cancellationToken);
                await destPropStore.SetAsync(dest, etag.ToXml(), cancellationToken);
            }
        }
    }
}
