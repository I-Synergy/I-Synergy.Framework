using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;
public class DetailsViewModel : ViewModelSummary<TestItem>
{
    private readonly IBaseCommonServices _commonServices;

    public DetailsViewModel(
        IContext context, 
        IBaseCommonServices commonServices, 
        ILogger logger, 
        bool refreshOnInitialization = true, 
        bool automaticValidation = false) 
        : base(context, commonServices, logger, refreshOnInitialization, automaticValidation)
    {
        _commonServices = commonServices;
    }

    public override Task AddAsync()
    {
        throw new NotImplementedException();
    }

    public override Task EditAsync(TestItem e)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveAsync(TestItem e)
    {
        throw new NotImplementedException();
    }

    public override Task<List<TestItem>> RetrieveItemsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(
            new List<TestItem>
            {
                new TestItem { Id = 1, Description = "Test 1"},
                new TestItem { Id = 2, Description = "Test 2"},
                new TestItem { Id = 3, Description = "Test 3"},
                new TestItem { Id = 4, Description = "Test 4"},
                new TestItem { Id = 5, Description = "Test 5"}
            });
    }

    public override Task SearchAsync(object e)
    {
        throw new NotImplementedException();
    }

    public override async Task SubmitAsync(TestItem e, bool validateUnderlayingProperties = true)
    {
        Argument.IsNotNull(e);

        var detailsVm = new DetailViewModel(Context, _commonServices, Logger, e);
        await _commonServices.NavigationService.NavigateAsync(detailsVm);
    }
}
