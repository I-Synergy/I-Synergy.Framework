using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

/// <summary>
/// Class ForgotPasswordViewModel.
/// Implements the <see cref="ViewModelDialog{Boolean}" />
/// Implements the <see cref="IForgotPasswordViewModel" />
/// </summary>
/// <seealso cref="ViewModelDialog{Boolean}" />
/// <seealso cref="IForgotPasswordViewModel" />
public class ForgotPasswordViewModel : ViewModelDialog<bool>, IForgotPasswordViewModel
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return LanguageService.Default.GetString("Password_Forgot"); } }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordViewModel" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="authenticationService"></param>
    /// <param name="logger">The logger factory.</param>
    public ForgotPasswordViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        IAuthenticationService authenticationService,
        ILogger logger)
        : base(context, commonServices, logger)
    {
        Validator = new Action<IObservableClass>(arg =>
        {
            if (string.IsNullOrEmpty(EmailAddress) || !NetworkUtility.IsValidEMail(EmailAddress))
            {
                AddValidationError(nameof(EmailAddress), "E-mail address is invalid.");
            }
        });

        EmailAddress = "";
    }

    /// <summary>
    /// Gets or sets the EmailAddress property value.
    /// </summary>
    /// <value>The email address.</value>
    public string EmailAddress
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Resets the password asynchronous.
    /// </summary>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public Task<bool> ResetPasswordAsync()
    {
        return Task.FromResult(false);
    }

    /// <summary>
    /// submit as an asynchronous operation.
    /// </summary>
    /// <param name="e">if set to <c>true</c> [e].</param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task SubmitAsync(bool e, bool validateUnderlayingProperties = true)
    {
        bool result = false;

        if (Validate())
        {
            try
            {
                _commonServices.BusyService.StartBusy();
                result = true;
            }
            finally
            {
                _commonServices.BusyService.StopBusy();
            }
        }

        await base.SubmitAsync(result, validateUnderlayingProperties);
    }
}
