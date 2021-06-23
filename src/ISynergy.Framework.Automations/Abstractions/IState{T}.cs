using System;

namespace ISynergy.Framework.Automations.Abstractions
{
    /// <summary>
    /// Public interface of a state.
    /// </summary>
    public interface IState<T> : IState
    {
        /// <summary>
        /// Gets or sets the From property value.
        /// </summary>
        public T From { get; set; }

        /// <summary>
        /// Gets or sets the To property value.
        /// </summary>
        public T To { get; set; }
    }
}
