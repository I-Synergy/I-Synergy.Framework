namespace ISynergy.Framework.UI.Abstractions.Services;
public interface IStaticAssetService
{
    public Task<string?> GetAsync(string assetUrl, bool useCache = true);
}