using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Background.Tasks
{
    /// <summary>
    /// Some work to be done.
    /// If you are relying on the library to instantiate the job, make sure you implement a parameterless constructor
    /// (else you will be getting a System.MissingMethodException).
    /// </summary>
    public interface IJob
    {
        Task ExecuteAsync();
    }
}
