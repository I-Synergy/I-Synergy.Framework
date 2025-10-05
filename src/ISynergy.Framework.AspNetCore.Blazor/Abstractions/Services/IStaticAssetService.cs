namespace ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;
public interface IStaticAssetService
{
    public Task<string?> GetAsync(string assetUrl, bool useCache = true);
}