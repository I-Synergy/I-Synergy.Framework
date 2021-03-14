using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur during WeakEventManager.HandleEvent execution.
    /// </summary>
    public class InvalidHandleEventException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidHandleEventException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="targetParameterCountException">Target parameter count exception.</param>
        public InvalidHandleEventException(string message, TargetParameterCountException targetParameterCountException) : base(message, targetParameterCountException)
        {
        }
    }
}
