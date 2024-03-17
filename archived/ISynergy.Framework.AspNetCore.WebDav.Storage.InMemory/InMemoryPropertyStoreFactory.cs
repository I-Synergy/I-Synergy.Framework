using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.AspNetCore.WebDav.Storage.InMemory
{
    /// <summary>
    /// The factory for the <see cref="InMemoryPropertyStore"/>
    /// </summary>
    public class InMemoryPropertyStoreFactory : IPropertyStoreFactory
    {
        private readonly ILogger<InMemoryPropertyStore> _logger;

        private readonly IDeadPropertyFactory _deadPropertyFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryPropertyStoreFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger for the property store factory</param>
        /// <param name="deadPropertyFactory">The factory for dead properties</param>
        public InMemoryPropertyStoreFactory(ILogger<InMemoryPropertyStore> logger, IDeadPropertyFactory deadPropertyFactory)
        {
            _logger = logger;
            _deadPropertyFactory = deadPropertyFactory;
        }

        /// <inheritdoc />
        public IPropertyStore Create(IFileSystem fileSystem)
        {
            return new InMemoryPropertyStore(_deadPropertyFactory, _logger);
        }
    }
}
