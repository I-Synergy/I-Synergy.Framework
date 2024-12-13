using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace ISynergy.Framework.Core.Abstractions;

/// <summary>
/// Interface IContext
/// </summary>
public interface IContext : IObservableClass
{
    /// <summary>
    /// Gets or sets the current profile.
    /// </summary>
    /// <value>The current profile.</value>
    IProfile Profile { get; set; }
    /// <summary>
    /// Gets the current time zone.
    /// </summary>
    /// <value>The current time zone.</value>
    TimeZoneInfo TimeZone { get; }
    /// <summary>
    /// Gets or sets the number format.
    /// </summary>
    /// <value>The number format.</value>
    NumberFormatInfo NumberFormat { get; set; }
    /// <summary>
    /// Gets or sets the environment.
    /// </summary>
    /// <value>The environment.</value>
    SoftwareEnvironments Environment { get; set; }
    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    /// <value>The currency symbol.</value>
    string CurrencySymbol { get; set; }
    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    /// <value>The currency code.</value>
    string CurrencyCode { get; set; }
    /// <summary>
    /// Gets a value indicating whether this instance is authenticated.
    /// </summary>
    /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
    bool IsAuthenticated { get; }
}
