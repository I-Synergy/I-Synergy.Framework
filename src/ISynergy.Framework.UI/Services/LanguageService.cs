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
        public LanguageService()
        {
            _managers = new List<ResourceManager>
            {
                new ResourceManager(typeof(ISynergy.Framework.Core.Properties.Resources)),
                new ResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources)),
                new ResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources))
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
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return $"[{key}]";
        }
    }
}
