using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Models.Accounts;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface ILoginViewModel
    /// Implements the <see cref="IViewModelNavigation{Boolean}" />
    /// </summary>
    /// <seealso cref="IViewModelNavigation{Boolean}" />
    public interface ILoginViewModel : IViewModelNavigation<bool>
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic login].
        /// </summary>
        /// <value><c>true</c> if [automatic login]; otherwise, <c>false</c>.</value>
        bool AutoLogin { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [login visible].
        /// </summary>
        /// <value><c>true</c> if [login visible]; otherwise, <c>false</c>.</value>
        bool LoginVisible { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        string Password { get; set; }
        /// <summary>
        /// Gets or sets the registration mail.
        /// </summary>
        /// <value>The registration mail.</value>
        string Registration_Mail { get; set; }
        /// <summary>
        /// Gets or sets the registration modules.
        /// </summary>
        /// <value>The registration modules.</value>
        ObservableCollection<Module> Registration_Modules { get; set; }
        /// <summary>
        /// Gets or sets the name of the registration.
        /// </summary>
        /// <value>The name of the registration.</value>
        string Registration_Name { get; set; }
        /// <summary>
        /// Gets or sets the registration password.
        /// </summary>
        /// <value>The registration password.</value>
        string Registration_Password { get; set; }
        /// <summary>
        /// Gets or sets the registration password check.
        /// </summary>
        /// <value>The registration password check.</value>
        string Registration_Password_Check { get; set; }
        /// <summary>
        /// Gets or sets the registration time zone.
        /// </summary>
        /// <value>The registration time zone.</value>
        string Registration_TimeZone { get; set; }
        /// <summary>
        /// Gets or sets the time zones.
        /// </summary>
        /// <value>The time zones.</value>
        ObservableCollection<TimeZoneInfo> TimeZones { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        string Username { get; set; }
        /// <summary>
        /// Gets or sets the usernames.
        /// </summary>
        /// <value>The usernames.</value>
        ObservableCollection<string> Usernames { get; set; }

        /// <summary>
        /// Checks the automatic login asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task CheckAutoLoginAsync();
        /// <summary>
        /// Forgots the password asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task ForgotPasswordAsync();
        /// <summary>
        /// Registers the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> RegisterAsync();
    }
}
