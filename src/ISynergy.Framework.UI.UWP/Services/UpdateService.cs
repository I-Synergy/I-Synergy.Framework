using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Windows.Services.Store;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class UpdateService.
/// </summary>
internal class UpdateService : IUpdateService
{
    private readonly ILogger _logger;
    private readonly IDialogService _dialogService;

    private StoreContext? _storeContext = null;
    private IReadOnlyList<StorePackageUpdate>? updates = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateService" /> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="logger"></param>
    public UpdateService(
        IDialogService dialogService,
        ILogger<UpdateService> logger)
    {
        _dialogService = dialogService;

        _logger = logger;
        _logger.LogTrace($"UpdateService instance created with ID: {Guid.NewGuid()}");


    }

    /// <summary>
    /// check for update as an asynchronous operation.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public async Task<bool> CheckForUpdateAsync()
    {
        var result = false;

        try
        {
            if (_storeContext is null)
            {
#if WINDOWS
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Application.MainWindow);

                _storeContext = StoreContext.GetDefault();

                WinRT.Interop.InitializeWithWindow.Initialize(_storeContext, hwnd);

                updates = await _storeContext.GetAppAndOptionalStorePackageUpdatesAsync();

                if (updates.Count > 0)
                {
                    result = true;
                }
#endif
            }
        }
        catch (Exception)
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    /// download and install update as an asynchronous operation.
    /// </summary>
    public async Task DownloadAndInstallUpdateAsync()
    {
        if (updates is not null && updates is not null && updates.Count > 0)
        {
            // Download the packages.
            if (_storeContext is not null && _storeContext.CanSilentlyDownloadStorePackageUpdates)
            {
                await DownloadAndInstallPackageUpdatesSilentlyAsync(updates);
            }
            else
            {
                await DownloadAndInstallPackageUpdatesAsync(updates);
            }
        }
    }

    /// <summary>
    /// install package updates as an asynchronous operation.
    /// </summary>
    /// <param name="updates">The updates.</param>
    private async Task DownloadAndInstallPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
    {
        if (_storeContext is not null)
        {
            var installOperation = _storeContext.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

            // The package updates were already downloaded separately, so this method skips the download
            // operatation and only installs the updates; no download progress notifications are provided.
            var result = await installOperation.AsTask();

            switch (result.OverallState)
            {
                case StorePackageUpdateState.Completed:
                    break;
                default:
                    // Get the failed updates.
                    var failedUpdates = result.StorePackageUpdateStatuses.Where(
                        status => status.PackageUpdateState != StorePackageUpdateState.Completed);

                    // See if any failed updates were mandatory
                    if (updates.Any(u => u.Mandatory && failedUpdates.Any(failed => failed.PackageFamilyName == u.Package.Id.FamilyName)))
                    {
                        // At least one of the updates is mandatory, so tell the user.
                        await HandleMandatoryPackageErrorAsync();
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// install package updates silently as an asynchronous operation.
    /// </summary>
    /// <param name="updates">The updates.</param>
    private async Task DownloadAndInstallPackageUpdatesSilentlyAsync(IEnumerable<StorePackageUpdate> updates)
    {
        if (_storeContext is not null)
        {
            // Start the silent installation of the packages. Because the packages have already
            // been downloaded in the previous method, the following line of code just installs
            // the downloaded packages.
            var downloadResult = await _storeContext.TrySilentDownloadAndInstallStorePackageUpdatesAsync(updates);

            switch (downloadResult.OverallState)
            {
                // If the user cancelled the installation or you can't perform the installation  
                // for some other reason, try again later. The RetryInstallLater method is not  
                // implemented in this example, you should implement it as needed for your own app.
                case StorePackageUpdateState.Completed:
                    break;
                default:
                    // Get the failed updates.
                    var failedUpdates = downloadResult.StorePackageUpdateStatuses.Where(
                        status => status.PackageUpdateState != StorePackageUpdateState.Completed);

                    // See if any failed updates were mandatory
                    if (updates.Any(u => u.Mandatory && failedUpdates.Any(failed => failed.PackageFamilyName == u.Package.Id.FamilyName)))
                    {
                        // At least one of the updates is mandatory, so tell the user.
                        await HandleMandatoryPackageErrorAsync();
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Helper method for handling the scenario where a mandatory package update fails to
    /// download or install. Add code to this method to perform whatever actions you want
    /// to take, such as notifying the user and disabling features in your app.
    /// </summary>
    /// <returns>Task.</returns>
    private Task HandleMandatoryPackageErrorAsync()
    {
        return _dialogService.ShowErrorAsync(
            LanguageService.Default.GetString("WarningMandatoryUpdateFailed"));
    }
}