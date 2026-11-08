namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Selection ViewModel that also exposes a free-text query. Used by the dialog variants whose
/// views bind <c>Query</c> and <c>RefreshCommand</c>.
/// </summary>
public interface ISearchableSelectionViewModel : ISelectionViewModel
{
    /// <summary>
    /// Gets or sets the query text used to filter the items.
    /// </summary>
    string Query { get; set; }
}
