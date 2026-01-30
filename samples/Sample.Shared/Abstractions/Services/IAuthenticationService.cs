namespace Sample.Abstractions.Services;

/// <summary>
/// Provides authentication services with event-driven state management.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Raised when authentication succeeds (user logged in).
    /// The authentication state has been updated in the context.
    /// </summary>
    event EventHandler<AuthenticationSuccessEventArgs>? AuthenticationSucceeded;

    /// <summary>
    /// Raised when authentication fails or user logs out.
    /// The context has been cleared.
    /// </summary>
    event EventHandler<EventArgs>? AuthenticationFailed;

    /// <summary>
    /// Determines if a user is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    Task AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default);
    Task AuthenticateWithRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task AuthenticateWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);
    Task SignOutAsync();
}

/// <summary>
/// Event arguments for authentication success.
/// </summary>
public class AuthenticationSuccessEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationSuccessEventArgs"/> class.
    /// </summary>
    /// <param name="shouldRemember">Indicates if the authentication should be remembered for auto-login.</param>
    public AuthenticationSuccessEventArgs(bool shouldRemember = false)
    {
        ShouldRemember = shouldRemember;
    }

    /// <summary>
    /// Gets a value indicating whether authentication should be remembered.
    /// </summary>
    public bool ShouldRemember { get; }
}
