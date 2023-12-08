namespace ISynergy.Framework.UI.Models;

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
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return $"{Message}\r\n{Description}";
    }
}
