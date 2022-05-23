using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model.Headers;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem
{
    /// <summary>
    /// Is implemented when a <see cref="IDocument"/> or <see cref="ICollection"/> allows native <see cref="EntityTag"/> support.
    /// </summary>
    public interface IEntityTagEntry
    {
        /// <summary>
        /// Gets the <see cref="EntityTag"/> for a <see cref="IDocument"/> or <see cref="ICollection"/>
        /// </summary>
        EntityTag ETag { get; }

        /// <summary>
        /// Enforces the update of an <see cref="EntityTag"/>
        /// </summary>
        /// <remarks>
        /// This is usually called when the <see cref="IEntry"/> properties were changed.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The new <see cref="EntityTag"/></returns>
        Task<EntityTag> UpdateETagAsync(CancellationToken cancellationToken);
    }
}
