using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ISynergy.Background.Tasks
{
    /// <summary>
    /// A job factory.
    /// </summary>
    public interface IJobFactory
    {
        /// <summary>
        /// Instantiate a job of the given type.
        /// </summary>
        /// <typeparam name="T">Type of the job to instantiate</typeparam>
        /// <returns>The instantiated job</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The 'T' requirement is on purpose.")]
        IJob GetJobInstance<T>() where T : IJob;
    }
}
