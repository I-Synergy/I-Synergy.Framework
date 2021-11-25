using ISynergy.Framework.Core.Abstractions.Options;

namespace ISynergy.Framework.UI.Options
{
    /// <summary>
    /// Base configuration options
    /// </summary>
    public class ConfigurationOptions : IConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>The application title.</value>
        public string Application { get; }
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; }
        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        public string ClientSecret { get; }
        /// <summary>
        /// Gets or sets the service endpoint.
        /// </summary>
        /// <value>The service endpoint.</value>
        public string ServiceEndpoint { get; set; }
        /// <summary>
        /// Gets or sets the signal r endpoint.
        /// </summary>
        /// <value>The signal r endpoint.</value>
        public string SignalREndpoint { get; set; }
        /// <summary>
        /// Gets or sets the authentication endpoint.
        /// </summary>
        /// <value>The authentication endpoint.</value>
        public string AuthenticationEndpoint { get; set; }
        /// <summary>
        /// Gets or sets the account endpoint.
        /// </summary>
        /// <value>The account endpoint.</value>
        public string AccountEndpoint { get; set; }
        /// <summary>
        /// Gets or sets the web endpoint.
        /// </summary>
        /// <value>The web endpoint.</value>
        public string WebEndpoint { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigurationOptions()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public ConfigurationOptions(string application, string clientId, string clientSecret)
            : this()
        {
            Application = application;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}
