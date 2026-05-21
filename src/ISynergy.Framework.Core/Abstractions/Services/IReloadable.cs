namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Implemented by singleton services that must reload per-user state
/// when a new session begins.
/// </summary>
public interface IReloadable
{
    /// <summary>Called after <see cref="Messages.UserSignedInMessage"/> is published.</summary>
    Task ReloadAsync(CancellationToken cancellationToken = default);
}
