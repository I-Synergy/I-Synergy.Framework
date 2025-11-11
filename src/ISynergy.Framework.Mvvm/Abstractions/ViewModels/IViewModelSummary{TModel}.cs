using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModelSummary
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="IViewModel" />
public interface IViewModelSummary<TModel> : IViewModelSelectedItem<TModel>
{
    /// <summary>
    /// Occurs when [submitted].
    /// </summary>
    event EventHandler<SubmitEventArgs<TModel>>? Submitted;

    /// <summary>
    /// Gets or sets the Items property value.
    /// </summary>
    /// <value>The items.</value>
    ObservableCollection<TModel> Items { get; set; }

    /// <summary>
    /// Gets a value indicating whether [refresh on initialization].
    /// </summary>
    /// <value><c>true</c> if [refresh on initialization]; otherwise, <c>false</c>.</value>
    bool DisableRefreshOnInitialization { get; set; }

    /// <summary>
    /// Gets or sets the add command.
    /// </summary>
    /// <value>The add command.</value>
    AsyncRelayCommand AddCommand { get; }

    /// <summary>
    /// Gets or sets the edit command.
    /// </summary>
    /// <value>The edit command.</value>
    AsyncRelayCommand<TModel> EditCommand { get; }

    /// <summary>
    /// Gets or sets the delete command.
    /// </summary>
    /// <value>The delete command.</value>
    AsyncRelayCommand<TModel> DeleteCommand { get; }

    /// <summary>
    /// Gets or sets the refresh command.
    /// </summary>
    /// <value>The refresh command.</value>
    AsyncRelayCommand RefreshCommand { get; }

    /// <summary>
    /// Gets or sets the search command.
    /// </summary>
    /// <value>The search command.</value>
    AsyncRelayCommand<object> SearchCommand { get; }

    /// <summary>
    /// Adds the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task AddAsync();

    /// <summary>
    /// Edits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    Task EditAsync(TModel e);

    /// <summary>
    /// Removes the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    Task RemoveAsync(TModel e);

    /// <summary>
    /// Searches the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    Task SearchAsync(object e);

    /// <summary>
    /// Refreshes the asynchronous.
    /// </summary>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the items asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
    Task RetrieveItemsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Applies the specified query parameters to the current instance.
    /// </summary>
    /// <param name="query">A dictionary containing query parameter names and their corresponding values to apply. Cannot be null.</param>
    void ApplyQueryAttributes(IDictionary<string, object> query);

    /// <summary>
    /// Submits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="validateUnderlayingProperties"></param>
    /// <returns>Task.</returns>
    Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true);
}
