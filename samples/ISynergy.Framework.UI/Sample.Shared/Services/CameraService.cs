using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;

namespace Sample.Services
{
    /// <summary>
    /// Class CameraService.
    /// </summary>
    public class CameraService : ICameraService
    {
        /// <summary>
        /// Takes the picture asynchronous.
        /// </summary>
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>Task&lt;FileResult&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<FileResult> TakePictureAsync(ulong maxfilesize)
        {
            throw new NotImplementedException();
        }
    }
}
