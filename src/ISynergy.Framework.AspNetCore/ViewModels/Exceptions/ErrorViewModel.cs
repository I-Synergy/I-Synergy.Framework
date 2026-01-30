using ISynergy.Framework.Core.Attributes;

namespace ISynergy.Framework.AspNetCore.ViewModels.Exceptions;

/// <summary>
/// Class ErrorViewModel.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>The error.</value>
    [Description("Error")]
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error description.
    /// </summary>
    /// <value>The error description.</value>
    [Description("Description")]
    public string ErrorDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request identifier.
    /// </summary>
    /// <value>The request identifier.</value>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether [show request identifier].
    /// </summary>
    /// <value><c>true</c> if [show request identifier]; otherwise, <c>false</c>.</value>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
