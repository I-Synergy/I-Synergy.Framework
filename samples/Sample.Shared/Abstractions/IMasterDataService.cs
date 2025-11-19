namespace Sample.Abstractions;

/// <summary>
/// Interface IMasterDataService
/// </summary>
public interface IMasterDataService
{
    /// <summary>
    /// Loads the master items asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task LoadMasterItemsAsync();

    /// <summary>
    /// Gets the currency code by identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    string? GetCurrencyCodeById(int id);
}
