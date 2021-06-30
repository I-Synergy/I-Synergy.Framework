using System;
using System.Runtime.Serialization;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="SingularMatrixException"/> class.
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
        protected SingularMatrixException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }

    }
}
