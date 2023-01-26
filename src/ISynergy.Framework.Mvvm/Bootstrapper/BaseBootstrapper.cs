using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.Bootstrapper
{
    /// <summary>
    /// Base bootstrapper.
    /// </summary>
    public abstract class BaseBootstrapper : IBootstrap
    {
        /// <summary>
        /// Context.
        /// </summary>
        protected readonly IContext _context;

        /// <summary>
        /// Common services.
        /// </summary>
        protected readonly IBaseCommonServices _commonServices;

        /// <summary>
        /// Logger.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="commonServices"></param>
        /// <param name="logger"></param>
        protected BaseBootstrapper(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger)
        {
            _context = context;
            _commonServices = commonServices;
            _logger = logger;
        }

        /// <summary>
        /// Bootstrapper method.
        /// </summary>
        public abstract void Bootstrap();
    }
}
