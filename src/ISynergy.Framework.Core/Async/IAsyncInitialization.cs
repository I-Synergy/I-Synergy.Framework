﻿using System.Threading.Tasks;

namespace ISynergy.Framework.Core
{
    /// <summary>
    /// Marks a type as requiring asynchronous initialization and provides the result of that initialization.
    /// </summary>
    public interface IAsyncInitialization
    {
        /// <summary>
        /// The result of the asynchronous initialization of this instance.
        /// </summary>
        /// <value>The initialization.</value>
        Task Initialization { get; }
    }
}
