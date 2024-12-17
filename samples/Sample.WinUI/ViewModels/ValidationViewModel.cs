using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Sample.ViewModels;

/// <summary>
/// Validation sample viewmodel.
/// </summary>
public class ValidationViewModel : ViewModelNavigation<object>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return _commonServices.LanguageService.GetString("Validation"); } }

    /// <summary>
    /// Gets or sets the Test property value.
    /// </summary>
    public string Test
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Regex property value.
    /// </summary>
    public string Regex
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsRegexCheck property value.
    /// </summary>
    public bool IsRegexCheck
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsLengthCheck property value.
    /// </summary>
    public bool IsLengthCheck
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsNullCheck property value.
    /// </summary>
    public bool IsNullCheck
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="commonServices"></param>
    /// <param name="logger"></param>
    public ValidationViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger)
        : base(context, commonServices, logger)
    {
        IsNullCheck = true;
        Regex = @"\d\d\d\d[A-Z]";
        Description = commonServices.LanguageService.GetString("ValidationDescription");

        Validator = new Action<IObservableClass>(_ =>
        {
            if (string.IsNullOrEmpty(Test))
            {
                AddValidationError(nameof(Test), $"Value of [{nameof(Test)}] cannot be null or empty.");
            }

            if (IsLengthCheck && !string.IsNullOrEmpty(Test) && Test.Length < 4)
            {
                AddValidationError(nameof(Test), $"Value of [{nameof(Test)}] should be equal or larger then 4 characters.");
            }

            if (IsRegexCheck && !string.IsNullOrEmpty(Test))
            {
                if (string.IsNullOrEmpty(Regex))
                {
                    AddValidationError(nameof(Test), $"Value of [{nameof(Test)}] should be a valid regex expression.");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(Test, Regex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                {
                    AddValidationError(nameof(Test), $"Value of [{nameof(Test)}] does not match the regular expression.");
                }
            }
        });
    }

    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        Argument.IsNotNullOrEmpty(Test);

        if (Validate())
        {
            await _commonServices.DialogService.ShowInformationAsync($"Validation succeeded.");
        }
    }
}
