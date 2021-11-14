namespace ISynergy.Framework.Payment.Base
{
    /// <summary>
    /// Class ClientBase.
    /// </summary>
    public abstract class ClientBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBase" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected ClientBase(ILogger<ClientBase> logger)
        {
            _logger = logger;
        }
    }
}
