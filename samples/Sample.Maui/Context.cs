using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Sample;

/// <summary>
/// Class Context. This class cannot be inherited.
/// Implements the <see cref="ObservableClass" />
/// Implements the <see cref="IContext" />
/// </summary>
/// <seealso cref="ObservableClass" />
/// <seealso cref="IContext" />
public class Context : ObservableClass, IContext
{
    private readonly ConfigurationOptions _configurationOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="Context" /> class.
    /// </summary>
    /// <param name="configurationOptions"></param>
    /// <param name="serviceScopeFactory"></param>
    public Context(
        IOptions<ConfigurationOptions> configurationOptions,
        IServiceScopeFactory serviceScopeFactory)
    {
        _configurationOptions = configurationOptions.Value;

        CurrencyCode = "EURO";
        CurrencySymbol = "€";
        Environment = SoftwareEnvironments.Production;
        ScopedServices = serviceScopeFactory.CreateScope();
    }

    /// <summary>
    /// Gets the service scopes.
    /// </summary>
    public IServiceScope ScopedServices
    {
        get => GetValue<IServiceScope>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the current profile.
    /// </summary>
    /// <value>The current profile.</value>
    public IProfile Profile
    {
        get { return GetValue<IProfile>(); }
        set
        {
            SetValue(value);
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }

    /// <summary>
    /// Gets the current time zone.
    /// </summary>
    /// <value>The current time zone.</value>
    public TimeZoneInfo TimeZone
    {
        get
        {
            if (Profile != null)
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
        get { return GetValue<SoftwareEnvironments>(); }
        set
        {
            SetValue(value);
            ApplyEnvironment(value);
        }
    }

    /// <summary>
    /// Applies the environment.
    /// </summary>
    /// <param name="value">The value.</param>
    private void ApplyEnvironment(SoftwareEnvironments value)
    {
        InfoService.Default.SetTitle(value);

        switch (value)
        {
            case SoftwareEnvironments.Local:
            case SoftwareEnvironments.Test:
            case SoftwareEnvironments.Production:
            default:
                _configurationOptions.ServiceEndpoint = @"https://localhost:5000/api";
                _configurationOptions.SignalREndpoint = @"https://localhost:5000/monitor/";
                _configurationOptions.AuthenticationEndpoint = @"https://localhost:5000/connect/token";
                _configurationOptions.AccountEndpoint = @"https://localhost:5000/account";
                _configurationOptions.WebEndpoint = @"https://localhost:5001";
                break;
        }
    }

    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    /// <value>The currency symbol.</value>
    public string CurrencySymbol
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    /// <value>The currency code.</value>
    public string CurrencyCode
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is authenticated.
    /// </summary>
    /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
    public bool IsAuthenticated
    {
        get
        {
            if (Profile != null)
                return Profile.IsAuthenticated();

            return false;
        }
    }

    /// <summary>
    /// Gets or sets the NumberFormat property value.
    /// </summary>
    /// <value>The number format.</value>
    public NumberFormatInfo NumberFormat
    {
        get { return GetValue<NumberFormatInfo>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Culture property value.
    /// </summary>
    /// <value>The culture.</value>
    public CultureInfo Culture
    {
        get { return GetValue<CultureInfo>(); }
        set { SetValue(value); }
    }
}
