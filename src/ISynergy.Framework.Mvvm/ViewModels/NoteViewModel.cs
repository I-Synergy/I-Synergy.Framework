using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Events;

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
    /// The target property
    /// </summary>
    private readonly string _targetProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="NoteViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="note">The note.</param>
    [PreferredConstructor]
    public NoteViewModel(
        ICommonServices commonServices,
        string note)
        : base(commonServices)
    {
        SelectedItem = note;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoteViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="note">The note.</param>
    /// <param name="targetProperty">The target property.</param>
    public NoteViewModel(
        ICommonServices commonServices,
        string note,
        string targetProperty)
        : this(commonServices, note)
    {
        _targetProperty = targetProperty;
    }

    /// <summary>
    /// Called when [submitted].
    /// </summary>
    /// <param name="e">The e.</param>
    protected override void OnSubmitted(SubmitEventArgs<string> e)
    {
        if (!string.IsNullOrEmpty(_targetProperty))
        {
            base.OnSubmitted(new SubmitEventArgs<string>(e.Owner, e.Result, _targetProperty));
        }
        else
        {
            base.OnSubmitted(e);
        }
    }
}
