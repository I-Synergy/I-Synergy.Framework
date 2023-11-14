namespace ISynergy.Framework.Mathematics.Exceptions
{
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
    }
}
