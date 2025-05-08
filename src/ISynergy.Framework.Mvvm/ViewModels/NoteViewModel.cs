using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class NoteViewModel.
/// Implements the <see cref="ViewModelDialog{String}" />
/// </summary>
/// <seealso cref="ViewModelDialog{String}" />
public class NoteViewModel : ViewModelDialog<string>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title
    {
        get
        {
            return LanguageService.Default.GetString("Note");
        }
    }

    /// <summary>
    /// Gets or sets the TargetProperty property value.
    /// </summary>
    public string? TargetProperty
    {
        get => GetValue<string?>();
        set => SetValue(value);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="NoteViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    public NoteViewModel(
        ICommonServices commonServices,
        ILogger<NoteViewModel> logger)
        : base(commonServices, logger)
    {
    }

    public virtual void SetNote(string note, string? targetProperty)
    {
        SetSelectedItem(note);
        TargetProperty = targetProperty;
    }

    /// <summary>
    /// Called when [submitted].
    /// </summary>
    /// <param name="e">The e.</param>
    protected override void OnSubmitted(SubmitEventArgs<string> e)
    {
        Argument.IsNotNull(e);

        if (!string.IsNullOrEmpty(TargetProperty))
        {
            base.OnSubmitted(new SubmitEventArgs<string>(e.Owner!, e.Result, TargetProperty));
        }
        else
        {
            base.OnSubmitted(e);
        }
    }
}
