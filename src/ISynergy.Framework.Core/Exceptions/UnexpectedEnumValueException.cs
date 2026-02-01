namespace ISynergy.Framework.Core.Exceptions;

/// <summary>
/// Class UnexpectedEnumValueException.
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
[Serializable]
public class UnexpectedEnumValueException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedEnumValueException"/> class.
    /// </summary>
    public UnexpectedEnumValueException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedEnumValueException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnexpectedEnumValueException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedEnumValueException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public UnexpectedEnumValueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedEnumValueException"/> class.
    /// </summary>
    /// <param name="enumClass">The enum class.</param>
    /// <param name="value">The value.</param>
    public UnexpectedEnumValueException(Type enumClass, object value)
        : base($"The value({enumClass}) of Enum type '{value}' was unexpected.")
    {
    }
}
