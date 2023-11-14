namespace ISynergy.Framework.Mathematics.Exceptions
{
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
    }
}
