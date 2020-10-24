using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class LanguageService.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        /// <summary>
        /// The managers
        /// </summary>
        private readonly List<ResourceManager> _managers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageService"/> class.
        /// </summary>
        /// <param name="defaultResourceManager">The default resource manager.</param>
        public LanguageService(ResourceManager defaultResourceManager)
        {
            Argument.IsNotNull(nameof(defaultResourceManager), defaultResourceManager);

            _managers = new List<ResourceManager>
            {
                defaultResourceManager
            };
        }

        /// <summary>
        /// Adds the resource manager.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        public void AddResourceManager(ResourceManager resourceManager) =>
            _managers.Add(resourceManager);

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetString(string key)
        {
            foreach (var manager in _managers)
            {
                string result = manager.GetString(key, CultureInfo.CurrentCulture);
                if(!string.IsNullOrEmpty(result))
                    return result;
            }

            return $"[{key}]";
        }
    }
}
