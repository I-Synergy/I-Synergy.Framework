using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Contract for authentication lifecycle management.
/// Implementations fire <see cref="Messages.UserSignedInMessage"/> and
/// <see cref="Messages.UserSignedOutMessage"/> via <see cref="IMessengerService"/>
/// so the rest of the application can react without coupling to a specific provider.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>Whether a user is currently signed in.</summary>
    bool IsAuthenticated { get; }

    /// <summary>The signed-in user's profile, or <c>null</c> when not authenticated.</summary>
    IProfile? CurrentProfile { get; }

    /// <summary>
    /// Attempts to restore a previously saved session (e.g. from stored tokens).
    /// Returns <c>true</c> when a valid session was found and restored.
    /// </summary>
    Task<bool> TryRestoreSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates the sign-in flow.
    /// For browser-based providers (OIDC/Keycloak) this opens the system browser.
    /// For credential-based implementations the shell typically navigates to a
    /// sign-in view instead of calling this directly.
    /// </summary>
    Task SignInAsync(CancellationToken cancellationToken = default);

    /// <summary>Signs out the current user and clears any stored session data.</summary>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}
