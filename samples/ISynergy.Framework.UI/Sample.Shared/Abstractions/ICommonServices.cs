using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace Sample.Abstractions.Services
{
    /// <summary>
    /// Interface ICommonServices
    /// Implements the <see cref="IBaseCommonServices" />
    /// </summary>
    /// <seealso cref="IBaseCommonServices" />
    public interface ICommonServices : IBaseCommonServices
    {
        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        /// <value>The authentication service.</value>
        IBaseAuthenticationService AuthenticationService { get; }
        /// <summary>
        /// Gets the file service.
        /// </summary>
        /// <value>The file service.</value>
        IFileService FileService { get; }
    }
}
