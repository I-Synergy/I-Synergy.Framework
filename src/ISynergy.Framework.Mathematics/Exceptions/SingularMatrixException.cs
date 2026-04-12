
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Exceptions;

/// <summary>
///   Singular Matrix Exception.
/// </summary>
/// 
/// <remarks><para>The singular matrix exception is thrown in cases where a method which
/// performs matrix inversions has encountered a non-invertible matrix during the process.</para>
/// </remarks>
/// 
[Serializable]
public class SingularMatrixException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SingularMatrixException"/> class.
    /// </summary>
    public SingularMatrixException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingularMatrixException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public SingularMatrixException(string message) :
        base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingularMatrixException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// 
    public SingularMatrixException(string message, Exception innerException) :
        base(message, innerException)
    { }

#pragma warning disable SYSLIB0051
    /// <summary>
    ///   Initializes a new instance of the <see cref="SingularMatrixException"/> class.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    protected SingularMatrixException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
}
