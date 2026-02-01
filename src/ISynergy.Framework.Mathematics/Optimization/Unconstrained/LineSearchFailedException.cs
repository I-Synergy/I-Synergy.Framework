namespace ISynergy.Framework.Mathematics.Optimization.Unconstrained;

/// <summary>
///     Line Search Failed Exception.
/// </summary>
/// <remarks>
///     This exception may be thrown by the <see cref="BroydenFletcherGoldfarbShanno">L-BFGS Optimizer</see>
///     when the line search routine used by the optimization method fails.
/// </remarks>
[Serializable]
public class LineSearchFailedException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LineSearchFailedException" /> class.
    /// </summary>
    public LineSearchFailedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LineSearchFailedException" /> class.
    /// </summary>
    /// <param name="info">The error code information of the line search routine.</param>
    /// <param name="message">Message providing some additional information.</param>
    public LineSearchFailedException(int info, string message)
        : base(message)
    {
        Information = info;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LineSearchFailedException" /> class.
    /// </summary>
    /// <param name="message">Message providing some additional information.</param>
    public LineSearchFailedException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LineSearchFailedException" /> class.
    /// </summary>
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public LineSearchFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    ///     Gets the error code information returned by the line search routine.
    /// </summary>
    /// <value>The error code information returned by the line search routine.</value>
    public int Information { get; }
}