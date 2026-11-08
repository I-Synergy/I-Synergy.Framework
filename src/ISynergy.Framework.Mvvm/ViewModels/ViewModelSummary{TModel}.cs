using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// View model that presents a summary collection of <typeparamref name="TModel"/>.
/// </summary>
/// <typeparam name="TModel">The type of the items.</typeparam>
/// <seealso cref="ViewModelCollection{TModel}" />
/// <seealso cref="IViewModelSummary{TModel}" />
public abstract class ViewModelSummary<TModel> : ViewModelCollection<TModel>, IViewModelSummary<TModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelSummary{TModel}"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger.</param>
    protected ViewModelSummary(
        ICommonServices commonServices,
        ILogger<ViewModelSummary<TModel>> logger)
        : base(commonServices, logger)
    {
        SubmitCommand = new AsyncRelayCommand<TModel>(e => SubmitAsync(e), e => e is not null);
    }

    /// <summary>
    /// Applies the query attributes to the view model.
    /// </summary>
    /// <param name="query">The query attributes.</param>
    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(GenericConstants.Parameter, out object? result) && result is TModel entity)
            SetSelectedItem(entity);
    }
}
