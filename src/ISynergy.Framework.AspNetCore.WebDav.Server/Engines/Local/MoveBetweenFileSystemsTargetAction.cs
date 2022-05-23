using System;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Local
{
    /// <summary>
    /// The <see cref="ITargetActions{TCollection,TDocument,TMissing}"/> implementation that moves between two file systems
    /// </summary>
    public class MoveBetweenFileSystemsTargetAction : ITargetActions<CollectionTarget, DocumentTarget, MissingTarget>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveBetweenFileSystemsTargetAction"/> class.
        /// </summary>
        /// <param name="dispatcher">The WebDAV dispatcher</param>
        public MoveBetweenFileSystemsTargetAction(IWebDavDispatcher dispatcher)
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
            var doc = await destination.Parent.Collection.CreateDocumentAsync(destination.Name, cancellationToken);
            var docTarget = new DocumentTarget(destination.Parent, destination.DestinationUrl, doc, this);
            await MoveAsync(source, doc, cancellationToken);
            await CopyETagAsync(source, doc, cancellationToken);
            await source.DeleteAsync(cancellationToken);

            return docTarget;
        }

        /// <inheritdoc />
        public async Task<ActionResult> ExecuteAsync(IDocument source, DocumentTarget destination, CancellationToken cancellationToken)
        {
            try
            {
                await MoveAsync(source, destination.Document, cancellationToken);
                await CopyETagAsync(source, destination.Document, cancellationToken);
                await source.DeleteAsync(cancellationToken);

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
        public async Task ExecuteAsync(ICollection source, CollectionTarget destination, CancellationToken cancellationToken)
        {
            await CopyETagAsync(source, destination.Collection, cancellationToken);
            await source.DeleteAsync(cancellationToken);
        }

        private static async Task MoveAsync(IDocument source, IDocument destination, CancellationToken cancellationToken)
        {
            using (var sourceStream = await source.OpenReadAsync(cancellationToken))
            {
                using (var destinationStream = await destination.CreateAsync(cancellationToken))
                {
                    await sourceStream.CopyToAsync(destinationStream, 65536, cancellationToken);
                }
            }
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
