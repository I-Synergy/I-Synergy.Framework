namespace ISynergy.Framework.AspNetCore.Options
{
    /// <summary>
    /// Class EnviromentalOptions.
    /// </summary>
    public class EnviromentalOptions
    {
        /// <summary>
        /// Enum Environments
        /// </summary>
        public enum Environments
        {
            /// <summary>
            /// The development
            /// </summary>
            development,
            /// <summary>
            /// The test
            /// </summary>
            test,
            /// <summary>
            /// The release
            /// </summary>
            release
        }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>The environment.</value>
        public Environments Environment { get; set; }
    }
}
