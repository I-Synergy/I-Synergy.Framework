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
        /// <param name="maxFileSize">Maximum filesize, default 1Mb (1 * 1024 * 1024)</param>
        /// <returns>Task&lt;FileResult&gt;.</returns>
        Task<FileResult> TakePictureAsync(long maxFileSize = 1 * 1024 * 1024);
    }
}
