using ISynergy.Framework.Geography.Global;

namespace ISynergy.Framework.Geography.Tests
{
    /// <summary>
    /// Some constants used throughout the Unit Tests.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// That's where my home is.
        /// </summary>
        public static readonly GlobalCoordinates MyHome =
            new GlobalCoordinates(49.8459444, 8.7993944);

        /// <summary>
        /// My office
        /// </summary>
        public static readonly GlobalCoordinates MyOffice =
            new GlobalCoordinates(50.2160806, 8.6152611);
    }
}