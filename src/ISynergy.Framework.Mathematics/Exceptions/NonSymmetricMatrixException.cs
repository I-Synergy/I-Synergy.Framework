
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Exceptions;

/// <summary>
///   Non-Symmetric Matrix Exception.
/// </summary>
/// 
/// <remarks><para>The not symmetric matrix exception is thrown in cases where a method 
/// expects a matrix to be symmetric but it is not.</para>
/// </remarks>
/// 
[Serializable]
public class NonSymmetricMatrixException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
    /// </summary>
    public NonSymmetricMatrixException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public NonSymmetricMatrixException(string message) :
        base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// 
    public NonSymmetricMatrixException(string message, Exception innerException) :
        base(message, innerException)
    { }

#pragma warning disable SYSLIB0051
    /// <summary>
    ///   Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    protected NonSymmetricMatrixException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
}
