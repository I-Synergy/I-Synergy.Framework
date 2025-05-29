namespace ISynergy.Framework.UI.Extensions;

public static class FileResultExtensions
{
    /// <summary>
    /// Convert Microsoft.Maui.Storage.FileResult to ISynergy.Framework.Mvvm.Models.FileResult
    /// </summary>
    /// <param name="fileResult"></param>
    /// <returns></returns>
    public static ISynergy.Framework.Mvvm.Models.FileResult ToFileResult(this Microsoft.Maui.Storage.FileResult fileResult)
    {
        if (fileResult is not null)
        {
            return new ISynergy.Framework.Mvvm.Models.FileResult(
                fileResult.FullPath,
                fileResult.FileName,
                () => fileResult.OpenReadAsync().GetAwaiter().GetResult());
        }

        return null;
    }
}
