using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

/// <summary>
/// Blade-owning page view model. Hosts <see cref="DetailViewModel"/> and, through it,
/// <see cref="DetailAViewModel"/> inside the <c>BladeView</c> of the details page.
/// </summary>
public class DetailsViewModel : ViewModelBladeView<TestItem>
{
    private readonly IBladeService _bladeService;
    private readonly BladeHostViewModel _bladeHost;

    public DetailsViewModel(
        ICommonServices commonServices,
        IBladeService bladeService,
        BladeHostViewModel bladeHost,
        ILogger<DetailsViewModel> logger)
        : base(commonServices, logger)
    {
        _bladeService = bladeService;
        _bladeHost = bladeHost;

        Title = "Blades";
    }

    public override Task AddAsync() => Task.CompletedTask;

    public override Task EditAsync(TestItem e)
    {
        Argument.IsNotNull(e);

        // Window-modal blades: open them against the shell-level host, so the pane covers the
        // whole window (sidebar and header included) instead of the page only.
        return _bladeService.OpenBladeAsync<DetailViewModel, TestItem>(_bladeHost, e);
    }

    public override Task RemoveAsync(TestItem e) => Task.CompletedTask;

    public override Task RetrieveItemsAsync(CancellationToken cancellationToken)
    {
        Items?.AddNewRange(new List<TestItem>
            {
                new TestItem { Id = 1, Description = "Test 1"},
                new TestItem { Id = 2, Description = "Test 2"},
                new TestItem { Id = 3, Description = "Test 3"},
                new TestItem { Id = 4, Description = "Test 4"},
                new TestItem { Id = 5, Description = "Test 5"}
            });
        return Task.CompletedTask;
    }

    public override Task SearchAsync(object e) => Task.CompletedTask;

    public override async Task SubmitAsync(TestItem e, bool validateUnderlayingProperties = true)
    {
        Argument.IsNotNull(e);

        await _bladeService.OpenBladeAsync<DetailViewModel, TestItem>(_bladeHost, e);
    }
}
