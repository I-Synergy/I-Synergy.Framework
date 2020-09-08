﻿using System;

namespace ISynergy.Framework.Core.Abstractions
{
    /// <summary>
    /// A repeated timer.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Start the timer and triggers the step action every elapsed interval, until false is returned.
        /// </summary>
        /// <param name="interval">The intervalle between steps.</param>
        /// <param name="step">An action executed every step. It should return false when finished.</param>
        void Start(TimeSpan interval, Func<bool> step);
    }
}
