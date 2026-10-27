using ISynergy.Framework.Core.Events;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// A registered recipient action together with its optional messaging-channel token.
/// </summary>
internal sealed class WeakActionAndToken
{
    /// <summary>
    /// Gets or sets the weak action that will be invoked when a matching message is sent.
    /// </summary>
    public WeakAction? Action { get; set; }

    /// <summary>
    /// Gets or sets the optional channel token.
    /// </summary>
    public object? Token { get; set; }
}
