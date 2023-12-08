namespace ISynergy.Framework.Mathematics.Exceptions;

/// <summary>
///   Non-Positive Definite Matrix Exception.
/// </summary>
/// 
/// <remarks><para>The non-positive definite matrix exception is thrown in cases where a method 
/// expects a matrix to have only positive eigenvalues, such when dealing with covariance matrices.</para>
/// </remarks>
/// 
[Serializable]
public class NonPositiveDefiniteMatrixException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonPositiveDefiniteMatrixException"/> class.
    /// </summary>
    public NonPositiveDefiniteMatrixException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonPositiveDefiniteMatrixException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public NonPositiveDefiniteMatrixException(string message) :
        base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonPositiveDefiniteMatrixException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// 
    public NonPositiveDefiniteMatrixException(string message, Exception innerException) :
        base(message, innerException)
    { }
}
