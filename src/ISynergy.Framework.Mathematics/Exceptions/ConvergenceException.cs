namespace ISynergy.Framework.Mathematics.Exceptions;

/// <summary>
///   Algorithm Convergence Exception.
/// </summary>
/// 
/// <remarks><para>The algorithm convergence exception is thrown in cases where a iterative
/// algorithm could not converge to a finite solution.</para>
/// </remarks>
/// 
[Serializable]
public class ConvergenceException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConvergenceException"/> class.
    /// </summary>
    /// 
    public ConvergenceException() { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConvergenceException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public ConvergenceException(string message) :
        base(message)
    { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConvergenceException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// 
    public ConvergenceException(string message, Exception innerException) :
        base(message, innerException)
    { }
}
