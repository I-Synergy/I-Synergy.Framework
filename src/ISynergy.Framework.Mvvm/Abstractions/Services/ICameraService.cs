using ISynergy.Framework.Mvvm.Models;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface ICameraService
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// Takes the picture asynchronous.
        /// </summary>
        /// <returns>Task&lt;FileResult&gt;.</returns>
        Task<FileResult> TakePictureAsync();
    }
}
