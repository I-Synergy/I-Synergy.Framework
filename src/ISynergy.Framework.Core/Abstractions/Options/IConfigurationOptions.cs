namespace ISynergy.Framework.Core.Abstractions.Options
{
    /// <summary>
    /// Configuration options interface.
    /// </summary>
    public interface IConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        string ClientId { get; }
        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        string ClientSecret { get; }
        /// <summary>
        /// Gets or sets the service endpoint.
        /// </summary>
        /// <value>The service endpoint.</value>
        string ServiceEndpoint { get; set; }
        /// <summary>
        /// Gets or sets the signal r endpoint.
        /// </summary>
        /// <value>The signal r endpoint.</value>
        string SignalREndpoint { get; set; }
        /// <summary>
        /// Gets or sets the authentication endpoint.
        /// </summary>
        /// <value>The authentication endpoint.</value>
        string AuthenticationEndpoint { get; set; }
    }
}
