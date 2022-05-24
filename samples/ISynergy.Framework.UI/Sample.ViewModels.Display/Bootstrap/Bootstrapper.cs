using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Bootstrapper;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels.Display
{
    /// <summary>
    /// Bootstrapper.
    /// </summary>
    public class Bootstrapper : BaseBootstrapper
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="commonServices"></param>
        /// <param name="logger"></param>
        public Bootstrapper(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger) : base(context, commonServices, logger) { }

        /// <summary>
        /// Bootstrap method.
        /// </summary>
        public override void Bootstrap()
        {
        }
    }
}
