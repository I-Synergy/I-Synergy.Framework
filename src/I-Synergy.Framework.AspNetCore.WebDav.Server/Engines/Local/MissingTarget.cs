using System;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Local
{
    /// <summary>
    /// The missing local file system target
    /// </summary>
    public class MissingTarget : IMissingTarget<CollectionTarget, DocumentTarget, MissingTarget>
    {
        private readonly ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> _targetActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTarget"/> class.
        /// </summary>
        /// <param name="destinationUrl">The destination URL for this entry</param>
        /// <param name="name">The name of the missing target</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        public MissingTarget(Uri destinationUrl, string name, CollectionTarget parent, ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions)
        {
            _targetActions = targetActions;
            DestinationUrl = destinationUrl;
            Name = name;
            Parent = parent;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Gets the parent collection
        /// </summary>
        public CollectionTarget Parent { get; }

        /// <inheritdoc />
        public Uri DestinationUrl { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="MissingTarget"/> class.
        /// </summary>
        /// <param name="destinationUrl">The destination URL for this entry</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="name">The name of the missing target</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        /// <returns>The newly created missing target object</returns>
        public static MissingTarget NewInstance(
            Uri destinationUrl,
            ICollection parent,
            string name,
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions)
        {
            var collUrl = destinationUrl.GetCollectionUri();
            var collTarget = new CollectionTarget(collUrl, null, parent, false, targetActions);
            var target = new MissingTarget(destinationUrl, name, collTarget, targetActions);
            return target;
        }

        /// <inheritdoc />
        public async Task<CollectionTarget> CreateCollectionAsync(CancellationToken cancellationToken)
        {
            var coll = await Parent.Collection.CreateCollectionAsync(Name, cancellationToken);
            return new CollectionTarget(DestinationUrl, Parent, coll, true, _targetActions);
        }
    }
}
