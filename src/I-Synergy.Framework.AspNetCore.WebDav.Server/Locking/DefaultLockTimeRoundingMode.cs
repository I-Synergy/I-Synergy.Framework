namespace ISynergy.Framework.AspNetCore.WebDav.Server.Locking
{
    /// <summary>
    /// The rounding modes for the default rounding mode implementation
    /// </summary>
    public enum DefaultLockTimeRoundingMode
    {
        /// <summary>
        /// Round to the next second
        /// </summary>
        OneSecond,

        /// <summary>
        /// Round to the next 100 milliseconds
        /// </summary>
        OneHundredMilliseconds,
    }
}
