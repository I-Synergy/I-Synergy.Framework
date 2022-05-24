namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Class SettingsStorageExtensions.
    /// </summary>
    public static partial class SettingsStorageExtensions
    {
        /// <summary>
        /// The file extension
        /// </summary>
        private const string _jsonExtension = ".json";

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        private static string GetJsonFileName(string name) => $"{name}.{_jsonExtension}";
    }
}
