namespace Sample.Options
{
    /// <summary>
    /// Class AppCenterOptions.
    /// </summary>
    public class AppCenterOptions
    {
        /// <summary>
        /// Gets or sets the instrumentation key.
        /// </summary>
        /// <value>The instrumentation key.</value>
        public string InstrumentationKey { get; }

        /// <summary>
        /// Default constructur.
        /// </summary>
        /// <param name="instrumentationKey"></param>
        public AppCenterOptions(string instrumentationKey)
        {
            InstrumentationKey = instrumentationKey;
        }
    }
}
