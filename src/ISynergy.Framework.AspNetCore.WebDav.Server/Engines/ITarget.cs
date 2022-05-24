using System;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines
{
    /// <summary>
    /// The target for a copy or move operation
    /// </summary>
    public interface ITarget
    {
        /// <summary>
        /// Gets the name of the target
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the destination URL of the target
        /// </summary>
        Uri DestinationUrl { get; }
    }
}
