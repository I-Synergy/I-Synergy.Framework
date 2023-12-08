namespace ISynergy.Framework.Core.Exceptions;

/// <summary>
/// Class EntityNotFoundException.
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
[Serializable]
public class EntityNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    public EntityNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
