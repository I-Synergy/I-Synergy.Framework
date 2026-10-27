using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

/// <summary>
/// Blade-owning page view model. Hosts <see cref="DetailViewModel"/> and, through it,
/// <see cref="DetailAViewModel"/> inside the <c>BladePane</c> of the details page.
/// </summary>
public class DetailsViewModel : ViewModelBladeView<TestItem>
{
    private readonly IBladeService _bladeService;

    public DetailsViewModel(
        ICommonServices commonServices,
        IBladeService bladeService,
        ILogger<DetailsViewModel> logger)
        : base(commonServices, logger)
    {
        _bladeService = bladeService;

        Title = "Blades";
    }

    public override Task AddAsync()
    {
        throw new NotImplementedException();
    }

    public override Task EditAsync(TestItem e)
    {
        Argument.IsNotNull(e);

        return _bladeService.OpenBladeAsync<DetailViewModel, TestItem>(this, e);
    }

    public override Task RemoveAsync(TestItem e)
    {
        throw new NotImplementedException();
    }

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

    public override Task SearchAsync(object e)
    {
        throw new NotImplementedException();
    }

    public override async Task SubmitAsync(TestItem e, bool validateUnderlayingProperties = true)
    {
        Argument.IsNotNull(e);

        await _bladeService.OpenBladeAsync<DetailViewModel, TestItem>(this, e);
    }
}
