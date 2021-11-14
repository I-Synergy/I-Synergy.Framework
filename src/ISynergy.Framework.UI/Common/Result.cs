namespace ISynergy.Framework.UI.Common
{
    /// <summary>
    /// Class Result.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is ok.
        /// </summary>
        /// <value><c>true</c> if this instance is ok; otherwise, <c>false</c>.</value>
        public bool IsOk { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Oks this instance.
        /// </summary>
        /// <returns>Result.</returns>
        public static Result Ok()
        {
            return Ok(null, null);
        }
        /// <summary>
        /// Oks the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="description">The description.</param>
        /// <returns>Result.</returns>
        public static Result Ok(string message, string description = null)
        {
            return new Result { IsOk = true, Message = message, Description = description };
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="description">The description.</param>
        /// <returns>Result.</returns>
        public static Result Error(string message, string description = null)
        {
            return new Result { IsOk = false, Message = message, Description = description };
        }
        /// <summary>
        /// Errors the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>Result.</returns>
        public static Result Error(Exception ex)
        {
            return new Result { IsOk = false, Message = ex.Message, Description = ex.ToString(), Exception = ex };
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Message}\r\n{Description}";
        }
    }

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
