namespace ISynergy.Framework.Mathematics.Exceptions
{
    /// <summary>
    ///   Dimension Mismatch Exception.
    /// </summary>
    ///
    /// <remarks><para>The dimension mismatch exception is thrown in cases where a method expects 
    /// a matrix or array object having specific or compatible dimensions, such as the inner matrix
    /// dimensions in matrix multiplication.</para>
    /// </remarks>
    ///
    [Serializable]
    public class DimensionMismatchException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionMismatchException"/> class.
        /// </summary>
        public DimensionMismatchException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionMismatchException"/> class.
        /// </summary>
        /// 
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>
        /// 
        public DimensionMismatchException(string paramName) :
            base(paramName, "Array dimensions must match.")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionMismatchException"/> class.
        /// </summary>
        /// 
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public DimensionMismatchException(string paramName, string message) :
            base(message, paramName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionMismatchException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public DimensionMismatchException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
