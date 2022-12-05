using ISynergy.Framework.Core.Abstractions.Base;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Abstractions.Services.Base
{
    /// <summary>
    /// Interface IBaseApplicationSettingsService
    /// </summary>
    public interface IBaseApplicationSettingsService
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        IBaseApplicationSettings Settings { get; }
        /// <summary>
        /// Loads the settings.
        /// </summary>
        Task LoadSettingsAsync();
        /// <summary>
        /// Saves the settings.
        /// </summary>
        Task SaveSettingsAsync();
    }
}
