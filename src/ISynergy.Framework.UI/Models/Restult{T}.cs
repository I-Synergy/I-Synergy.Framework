using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.UI.Models
{
    /// <summary>
    /// Class Result.
    /// Implements the <see cref="Result" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Result" />
    public class Result<T> : Result
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; private set; }

        /// <summary>
        /// Oks the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Result&lt;T&gt;.</returns>
        public static Result<T> Ok(T result = default)
        {
            return Ok(null, null, result);
        }
        /// <summary>
        /// Oks the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="description">The description.</param>
        /// <param name="result">The result.</param>
        /// <returns>Result&lt;T&gt;.</returns>
        public static Result<T> Ok(string message = null, string description = null, T result = default)
        {
            return new Result<T> { IsOk = true, Message = message, Description = description, Value = result };
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="description">The description.</param>
        /// <param name="result">The result.</param>
        /// <returns>Result&lt;T&gt;.</returns>
        public static Result<T> Error(string message = null, string description = null, T result = default)
        {
            return new Result<T> { IsOk = false, Message = message, Description = description, Value = result };
        }
        /// <summary>
        /// Errors the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="result">The result.</param>
        /// <returns>Result&lt;T&gt;.</returns>
        public static Result<T> Error(Exception ex, T result = default)
        {
            return new Result<T> { IsOk = false, Message = ex.Message, Description = ex.ToString(), Exception = ex, Value = result };
        }
    }
}
