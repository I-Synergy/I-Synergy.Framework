using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Models;

namespace Sample.Abstractions;

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
    IAuthenticationService AuthenticationService { get; }
    /// <summary>
    /// Gets the file service.
    /// </summary>
    /// <value>The file service.</value>
    IFileService<FileResult> FileService { get; }
}

