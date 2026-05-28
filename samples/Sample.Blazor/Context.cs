using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;

namespace Sample;

/// <summary>
/// Class Context. This class cannot be inherited.
/// Implements the <see cref="ObservableValidatedClass" />
/// Implements the <see cref="IContext" />
/// </summary>
/// <seealso cref="ObservableValidatedClass" />
/// <seealso cref="IContext" />
public sealed class Context : IContext
{
    /// <summary>
    /// Gets or sets the current profile.
    /// </summary>
    /// <value>The current profile.</value>
    public IProfile? Profile { get; set; }

    /// <summary>
    /// Gets the current time zone.
    /// </summary>
    /// <value>The current time zone.</value>
    public TimeZoneInfo? TimeZone
    {
        get
        {
            if (Profile is not null)
                return TimeZoneInfo.FindSystemTimeZoneById(Profile.TimeZoneId);

            return TimeZoneInfo.Local;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is authenticated.
    /// </summary>
    /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
    public bool IsAuthenticated
    {
        get
        {
            if (Profile is not null)
                return true;

            return false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is user administrator.
    /// </summary>
    /// <value><c>true</c> if this instance is user administrator; otherwise, <c>false</c>.</value>
    public bool IsUserAdministrator
    {
        get
        {
            if (Profile is not null)
                return Profile.IsInRole(nameof(RoleNames.Administrator));

            return false;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is offline.
    /// </summary>
    /// <value><c>true</c> if this instance is offline; otherwise, <c>false</c>.</value>
    public bool IsOffline { get; set; }

    /// <summary>
    /// Gets or sets the custom properties.
    /// </summary>
    public Dictionary<string, object> Properties { get; } = new();
}
