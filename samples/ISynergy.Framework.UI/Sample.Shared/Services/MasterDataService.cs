using Sample.Abstractions.Services;

namespace Sample.Services
{
    /// <summary>
    /// Class MasterDataService.
    /// Implements the <see cref="IMasterDataService" />
    /// </summary>
    /// <seealso cref="IMasterDataService" />
    public class MasterDataService : IMasterDataService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataService"/> class.
        /// </summary>
        /// <param name="restService">The rest service.</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="settingsService">The settings service.</param>
        public MasterDataService()
        {
        }
    }
}
