using System;

namespace ISynergy.Framework.IO.Events
{
    /// <summary>
    /// Class PathAvailablitiyEventArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class PathAvailablitiyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating whether [path is available].
        /// </summary>
        /// <value><c>true</c> if [path is available]; otherwise, <c>false</c>.</value>
        public bool PathIsAvailable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathAvailablitiyEventArgs"/> class.
        /// </summary>
        /// <param name="available">if set to <c>true</c> [available].</param>
        public PathAvailablitiyEventArgs(bool available)
        {
            PathIsAvailable = available;
        }
    }
}
