using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.ViewModels;

/// <summary>
/// Shell-level owner of the blade pane. It carries no items of its own: a UI head hosts a
/// pane bound to <see cref="ViewModelBladeView{TModel}.Blades"/> and feeds it through the
/// blade service, which is what makes the pane independent of the current page.
/// </summary>
/// <seealso cref="ViewModelBladeView{TModel}" />
public sealed class BladeHostViewModel : ViewModelBladeView<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BladeHostViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger.</param>
    public BladeHostViewModel(
        ICommonServices commonServices,
        ILogger<BladeHostViewModel> logger)
        : base(commonServices, logger)
    {
    }

    /// <summary>
    /// Adds a new item asynchronously.
    /// </summary>
    /// <returns>A completed task; the host never owns items.</returns>
    public override Task AddAsync() => Task.CompletedTask;

    /// <summary>
    /// Edits the specified item asynchronously.
    /// </summary>
    /// <param name="e">The item to edit.</param>
    /// <returns>A completed task; the host never owns items.</returns>
    public override Task EditAsync(object e) => Task.CompletedTask;

    /// <summary>
    /// Removes the specified item asynchronously.
    /// </summary>
    /// <param name="e">The item to remove.</param>
    /// <returns>A completed task; the host never owns items.</returns>
    public override Task RemoveAsync(object e) => Task.CompletedTask;

    /// <summary>
    /// Searches for items asynchronously.
    /// </summary>
    /// <param name="e">The search criteria.</param>
    /// <returns>A completed task; the host has nothing to search.</returns>
    public override Task SearchAsync(object e) => Task.CompletedTask;
}
