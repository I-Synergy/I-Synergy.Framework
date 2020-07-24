using System;
using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Core.Helpers
{
    /// <summary>
    /// A loop that executes an action.
    /// </summary>
    public static class ActionTimer
    {
        /// <summary>
        /// Gets or sets a factory used to instanciate timers.
        /// </summary>
        /// <value>The factory function.</value>
        public static Func<ITimer> Create { get; set; } = () => new DelayTimer();
    }
}
