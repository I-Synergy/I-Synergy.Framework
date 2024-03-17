using System;

namespace ISynergy.Framework.AspNetCore.WebDav.Server
{
    /// <summary>
    /// The default implementation of <see cref="ISystemClock"/>
    /// </summary>
    public class SystemClock : ISystemClock
    {
        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
