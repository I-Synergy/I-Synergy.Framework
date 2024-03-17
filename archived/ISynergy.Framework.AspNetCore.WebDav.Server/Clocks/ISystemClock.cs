using System;

namespace ISynergy.Framework.AspNetCore.WebDav.Server
{
    /// <summary>
    /// Interface for querying the system clock
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Gets the <see cref="DateTime.UtcNow"/>
        /// </summary>
        DateTime UtcNow { get; }
    }
}
