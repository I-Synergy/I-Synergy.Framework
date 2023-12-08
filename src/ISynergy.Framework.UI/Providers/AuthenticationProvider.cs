using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.UI.Abstractions.Providers;
using System.Windows.Input;

namespace ISynergy.Framework.UI.Providers;

/// <summary>
/// Class AuthenticationProvider.
/// Implements the <see cref="IAuthenticationProvider" />
/// </summary>
/// <seealso cref="IAuthenticationProvider" />
public class AuthenticationProvider : IAuthenticationProvider
{
    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>The context.</value>
    public IContext Context { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationProvider"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public AuthenticationProvider(IContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Determines whether this instance [can command be executed] the specified relay command.
    /// </summary>
    /// <param name="Command">The relay command.</param>
    /// <param name="commandParameter">The command parameter.</param>
    /// <returns><c>true</c> if this instance [can command be executed] the specified relay command; otherwise, <c>false</c>.</returns>
    public bool CanCommandBeExecuted(ICommand Command, object commandParameter)
    {
        return true;
    }

    /// <summary>
    /// Determines whether [has access to UI element] [the specified element].
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="authorizationTag">The authorization tag.</param>
    /// <returns><c>true</c> if [has access to UI element] [the specified element]; otherwise, <c>false</c>.</returns>
    public bool HasAccessToUIElement(object element, object tag, string authorizationTag)
    {
        if (authorizationTag is not null && Context.Profile is not null)
        {
            var roles = authorizationTag.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (Context.Profile.Roles.Intersect(roles).Any()) return true;
        }

        return false;
    }
}
