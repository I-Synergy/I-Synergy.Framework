using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// View model that presents a collection of <typeparamref name="TModel"/> in a blade view.
/// </summary>
/// <typeparam name="TModel">The type of the items.</typeparam>
/// <seealso cref="ViewModelCollection{TModel}" />
/// <seealso cref="IViewModelBladeView{TModel}" />
public abstract class ViewModelBladeView<TModel> : ViewModelCollection<TModel>, IViewModelBladeView<TModel>, IViewModelBlade<TModel>
{
    /// <summary>
    /// Gets or sets the owner.
    /// </summary>
    public IViewModelBladeView Owner
    {
        get => GetValue<IViewModelBladeView>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is disabled.
    /// </summary>
    public bool IsDisabled
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the blades.
    /// </summary>
    [IgnoreValidation]
    public ObservableCollection<IView>? Blades
    {
        get => GetValue<ObservableCollection<IView>?>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the pane is visible.
    /// </summary>
    public bool IsPaneVisible
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBladeView{TModel}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger.</param>
    protected ViewModelBladeView(
        ICommonServices commonServices,
        ILogger<ViewModelBladeView<TModel>> logger)
        : base(commonServices, logger)
    {
        Blades = new ObservableCollection<IView>();

        // CanExecute checks both parameter and property to handle MAUI Button visual state correctly
        SubmitCommand = new AsyncRelayCommand<TModel>(
            execute: e => SubmitAsync(e ?? SelectedItem!),
            canExecute: e => (e ?? SelectedItem) is not null);
    }

    /// <inheritdoc />
    public override void Cleanup(bool isClosing = true)
    {
        try
        {
            IsInCleanup = true;
            Blades = null;
            base.Cleanup(isClosing);
        }
        finally
        {
            IsInCleanup = false;
        }
    }
}
