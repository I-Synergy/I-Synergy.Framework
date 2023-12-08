using System.Text.Json;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Class ErrorDetail.
/// </summary>
public class ErrorDetail
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDetail" /> class.
    /// </summary>
    public ErrorDetail()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDetail" /> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="message">The message.</param>
    /// <param name="type">The type.</param>
    public ErrorDetail(int statusCode, string message, string type)
    {
        StatusCode = statusCode;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    /// <value>The status code.</value>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; set; }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
