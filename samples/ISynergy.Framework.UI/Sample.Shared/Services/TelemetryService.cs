using ISynergy.Framework.Core.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Services
{
    /// <summary>
    /// See https://www.meziantou.net/use-application-insights-in-a-desktop-application.htm
    /// </summary>
    public class TelemetryService : ITelemetryService
    {
        /// <summary>
        /// Flushes this instance.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task TrackEventAsync(string e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="prop">The property.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task TrackEventAsync(string e, Dictionary<string, string> prop)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tracks the exception asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task TrackExceptionAsync(Exception e, string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tracks the page view asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task TrackPageViewAsync(string e)
        {
            throw new NotImplementedException();
        }
    }
}
