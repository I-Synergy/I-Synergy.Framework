using System;
using ISynergy.Framework.UI.Services.Base;
using Sample.Abstractions.Services;

namespace Sample.Services
{
    /// <summary>
    /// Class SettingsService.
    /// </summary>
    public class SettingsService : BaseSettingsService, ISettingsService
    {
        /// <summary>
        /// Gets a value indicating whether this instance is first run.
        /// </summary>
        /// <value><c>true</c> if this instance is first run; otherwise, <c>false</c>.</value>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsFirstRun => throw new NotImplementedException();

        /// <summary>
        /// Gets the default currency identifier.
        /// </summary>
        /// <value>The default currency identifier.</value>
        /// <exception cref="NotImplementedException"></exception>
        public int DefaultCurrencyId => throw new NotImplementedException();
    }
}
