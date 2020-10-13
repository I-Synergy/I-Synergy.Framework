using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class LanguageService.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetString(string key)
        {
            return key;
        }
    }
}
