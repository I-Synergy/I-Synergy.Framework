using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Sample.ViewModels;

public class EditableComboViewModel : ViewModelNavigation<object>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return "Editable ComboBox Test"; } }

    /// <summary>
    /// Gets or sets the MaskExample property value.
    /// </summary>
    public string MaskExpression
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the RegexExample property value.
    /// </summary>
    public string RegexExpression
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the TextSample property value.
    /// </summary>
    public string TextSample
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the RegexSample property value.
    /// </summary>
    public string RegexSample
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableComboViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    public EditableComboViewModel(ICommonServices commonServices, ILogger<EditableComboViewModel> logger)
        : base(commonServices, logger)
    {
        MaskExpression = "9999aa";
        RegexExpression = "0000>LL"; //1111Ab 0000Xx

        this.Validator = new Action<IObservableClass>(_ =>
        {
            Regex regex = RegexUtility.MaskToRegexConverter(RegexExpression);

            if (string.IsNullOrEmpty(RegexSample) || !regex.IsMatch(RegexSample))
            {
                AddValidationError(nameof(RegexSample), "TextSample is fout.");
            }
        });
    }

    public override Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        if (Validate())
            return base.SubmitAsync(e, validateUnderlayingProperties);

        return Task.CompletedTask;
    }
}
