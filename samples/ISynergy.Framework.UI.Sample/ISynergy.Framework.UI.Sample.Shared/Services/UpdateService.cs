#if NETFX_CORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Sample.Abstractions.Services;
using Windows.Services.Store;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class UpdateService.
    /// </summary>
    public class UpdateService : IUpdateService
    {
        /// <summary>
        /// The context
        /// </summary>
        private StoreContext context = null;
        /// <summary>
        /// The updates
        /// </summary>
        private IReadOnlyList<StorePackageUpdate> updates = null;

        /// <summary>
        /// Gets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        public IDialogService DialogService { get; }
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        public ILanguageService LanguageService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public UpdateService(
            ILanguageService languageService,
            IDialogService dialogService)
        {
            LanguageService = languageService;
            DialogService = dialogService;
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
                updates = null;

                if (context is null)
                {
                    context = StoreContext.GetDefault();
                }

                updates = await context.GetAppAndOptionalStorePackageUpdatesAsync();

                if (updates.Count > 0)
                {
                    result = true;
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
            if(updates != null && updates.Count > 0)
            {
                // Download the packages.
                var downloaded = await DownloadPackageUpdatesAsync(updates);

                if (downloaded)
                {
                    // Install the packages.
                    await InstallPackageUpdatesAsync(updates);
                }
            }
        }

        // Helper method for downloading package updates.
        /// <summary>
        /// download package updates as an asynchronous operation.
        /// </summary>
        /// <param name="updates">The updates.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> DownloadPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
        {
            var downloadedSuccessfully = false;

            var downloadOperation =
                context.RequestDownloadStorePackageUpdatesAsync(updates);

            // The Progress async method is called one time for each step in the download process for each
            // package in this request.
            downloadOperation.Progress = (asyncInfo, progress) =>
            {
            };

            var result = await downloadOperation.AsTask();

            switch (result.OverallState)
            {
                case StorePackageUpdateState.Completed:
                    downloadedSuccessfully = true;
                    break;
                default:
                    // Get the failed updates.
                    var failedUpdates = result.StorePackageUpdateStatuses.Where(
                        status => status.PackageUpdateState != StorePackageUpdateState.Completed);

                    // See if any failed updates were mandatory
                    if (updates.Any(u => u.Mandatory && failedUpdates.Any(
                        failed => failed.PackageFamilyName == u.Package.Id.FamilyName)))
                    {
                        // At least one of the updates is mandatory. Perform whatever actions you
                        // want to take for your app: for example, notify the user and disable
                        // features in your app.
                        await HandleMandatoryPackageErrorAsync();
                    }
                    break;
            }

            return downloadedSuccessfully;
        }

        // Helper method for installing package updates.
        /// <summary>
        /// install package updates as an asynchronous operation.
        /// </summary>
        /// <param name="updates">The updates.</param>
        private async Task InstallPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
        {
            var installOperation =
                context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

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

        // Helper method for handling the scenario where a mandatory package update fails to
        // download or install. Add code to this method to perform whatever actions you want
        // to take, such as notifying the user and disabling features in your app.
        /// <summary>
        /// Handles the mandatory package error asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task HandleMandatoryPackageErrorAsync()
        {
            return DialogService.ShowErrorAsync(
                        LanguageService.GetString("Warning_MandatoryUpdateFailed"));
        }
    }
}
#endif
