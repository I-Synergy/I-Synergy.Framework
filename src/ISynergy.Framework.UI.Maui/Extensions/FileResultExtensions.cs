namespace ISynergy.Framework.UI.Extensions;

public static class FileResultExtensions
{
    /// <summary>
    /// Convert Microsoft.Maui.Storage.FileResult to ISynergy.Framework.Mvvm.Models.FileResult
    /// </summary>
    /// <param name="fileResult"></param>
    /// <returns></returns>
    public static ISynergy.Framework.Core.Models.Results.FileResult? ToFileResult(this Microsoft.Maui.Storage.FileResult fileResult)
    {
        if (fileResult is not null)
        {
            return new ISynergy.Framework.Core.Models.Results.FileResult(
                fileResult.FullPath,
                () => fileResult.OpenReadAsync().GetAwaiter().GetResult());
        }

        return null;
    }
}
