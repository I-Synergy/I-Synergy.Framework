using ISynergy.Framework.Core.Enumerations;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ISynergy.Framework.Core.Abstractions
{
    /// <summary>
    /// Interface IContext
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets or sets the profiles.
        /// </summary>
        /// <value>The profiles.</value>
        ObservableCollection<IProfile> Profiles { get; set; }
        /// <summary>
        /// Gets or sets the current profile.
        /// </summary>
        /// <value>The current profile.</value>
        IProfile CurrentProfile { get; set; }
        /// <summary>
        /// Gets the current time zone.
        /// </summary>
        /// <value>The current time zone.</value>
        TimeZoneInfo CurrentTimeZone { get; }
        /// <summary>
        /// Gets or sets the number format.
        /// </summary>
        /// <value>The number format.</value>
        NumberFormatInfo NumberFormat { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }
        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>The environment.</value>
        SoftwareEnvironments Environment { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [normal screen].
        /// </summary>
        /// <value><c>true</c> if [normal screen]; otherwise, <c>false</c>.</value>
        bool NormalScreen { get; set; }
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
        /// <summary>
        /// Gets a value indicating whether this instance is user administrator.
        /// </summary>
        /// <value><c>true</c> if this instance is user administrator; otherwise, <c>false</c>.</value>
        bool IsUserAdministrator { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is offline.
        /// </summary>
        /// <value><c>true</c> if this instance is offline; otherwise, <c>false</c>.</value>
        bool IsOffline { get; set; }
        /// <summary>
        /// Gets or sets the view models.
        /// </summary>
        /// <value>The view models.</value>
        List<Type> ViewModels { get; set; }
    }
}
