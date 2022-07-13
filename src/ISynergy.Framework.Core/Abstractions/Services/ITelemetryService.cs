using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Abstractions.Services
{
    /// <summary>
    /// Interface ITelemetryService
    /// </summary>
    public interface ITelemetryService
    {
        /// <summary>
        /// Tracks the exception asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        void TrackException(Exception e, string message);
        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        void TrackEvent(string e);
        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="prop">The property.</param>
        /// <returns>Task.</returns>
        void TrackEvent(string e, Dictionary<string, string> prop);
        /// <summary>
        /// Tracks the page view asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        void TrackPageView(string e);
        /// <summary>
        /// Flushes this instance.
        /// </summary>
        void Flush();
    }
}
