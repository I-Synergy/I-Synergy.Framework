using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using Sample.Constants;

namespace Sample;

/// <summary>
/// Class Context. This class cannot be inherited.
/// Implements the <see cref="ObservableClass" />
/// Implements the <see cref="IContext" />
/// </summary>
/// <seealso cref="ObservableClass" />
/// <seealso cref="IContext" />
public class Context : IContext
{
    private SoftwareEnvironments _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="Context" /> class.
    /// </summary>
    public Context()
    {
        Environment = SoftwareEnvironments.Production;
    }

    /// <summary>
    /// Gets or sets the current profile.
    /// </summary>
    /// <value>The current profile.</value>
    public IProfile Profile { get; set; }

    /// <summary>
    /// Gets the current time zone.
    /// </summary>
    /// <value>The current time zone.</value>
    public TimeZoneInfo TimeZone
    {
        get
        {
            if (Profile is not null)
                return TimeZoneInfo.FindSystemTimeZoneById(Profile.TimeZoneId);

            return TimeZoneInfo.Local;
        }
    }

    /// <summary>
    /// Gets or sets the environment.
    /// </summary>
    /// <value>The environment.</value>
    public SoftwareEnvironments Environment
    {
        get => _environment;
        set
        {
            _environment = value;

            switch (_environment)
            {
                case SoftwareEnvironments.Local:
                case SoftwareEnvironments.Test:
                default:
                    if (Properties.ContainsKey(Endpoints.ApiEndpoint))
                        Properties[Endpoints.ApiEndpoint] = @"https://localhost:5000";
                    else
                        Properties.Add(Endpoints.ApiEndpoint, @"https://localhost:5000");

                    if (Properties.ContainsKey(Endpoints.IdentityEndpoint))
                        Properties[Endpoints.IdentityEndpoint] = @"https://localhost:5000";
                    else
                        Properties.Add(Endpoints.IdentityEndpoint, @"https://localhost:5000");

                    break;
            }
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
                return Profile.IsAuthenticated();

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
