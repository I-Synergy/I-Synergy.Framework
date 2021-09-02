using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Automations.Abstractions
{
    /// <summary>
    /// Base state interface
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets or sets the StateId property value.
        /// </summary>
        Guid StateId { get; }

        /// <summary>
        /// You can use For to have the trigger only fire if the state holds for some time.
        /// </summary>
        TimeSpan For { get; set; }
    }
}
