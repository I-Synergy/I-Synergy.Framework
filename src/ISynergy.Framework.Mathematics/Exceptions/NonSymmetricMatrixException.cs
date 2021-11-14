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
        /// <summary>
        /// Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        /// 
        protected NonSymmetricMatrixException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }

    }
}
