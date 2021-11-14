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
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>Task&lt;FileResult&gt;.</returns>
        Task<FileResult> TakePictureAsync(ulong maxfilesize);
    }
}
