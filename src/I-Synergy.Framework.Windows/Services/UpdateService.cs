using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Services.Store;
using Windows.UI.Xaml;

namespace ISynergy.Services
{
    public class UpdateService : IUpdateService
    {
        private StoreContext context = null;
        private IReadOnlyList<StorePackageUpdate> updates = null;

        public async Task<bool> CheckForUpdateAsync()
        {
            updates = null;

            if (context == null)
            {
                context = StoreContext.GetDefault();
            }

            updates = await context.GetAppAndOptionalStorePackageUpdatesAsync();

            if (updates.Count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task DownloadAndInstallUpdateAsync()
        {
            if(updates != null && updates.Count > 0)
            {
                // Download the packages.
                bool downloaded = await DownloadPackageUpdatesAsync(updates);

                if (downloaded)
                {
                    // Install the packages.
                    await InstallPackageUpdatesAsync(updates);
                }
            }
        }

        // Helper method for downloading package updates.
        private async Task<bool> DownloadPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
        {
            bool downloadedSuccessfully = false;

            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
                this.context.RequestDownloadStorePackageUpdatesAsync(updates);

            // The Progress async method is called one time for each step in the download process for each
            // package in this request.
            downloadOperation.Progress = (asyncInfo, progress) =>
            {
            };

            StorePackageUpdateResult result = await downloadOperation.AsTask();

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
        private async Task InstallPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
        {
            IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> installOperation =
                this.context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

            // The package updates were already downloaded separately, so this method skips the download
            // operatation and only installs the updates; no download progress notifications are provided.
            StorePackageUpdateResult result = await installOperation.AsTask();

            switch (result.OverallState)
            {
                case StorePackageUpdateState.Completed:
                    Application.Current.Exit();
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
        private Task HandleMandatoryPackageErrorAsync()
        {
            return ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                        ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_MandatoryUpdateFailed"));
        }
    }
}
