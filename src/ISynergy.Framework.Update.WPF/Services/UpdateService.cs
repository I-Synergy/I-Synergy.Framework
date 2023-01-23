using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Update.Options;
using ISynergy.Framework.Update.Abstractions.Services;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net.Http;

namespace ISynergy.Framework.Update.Services
{
    /// <summary>
    /// Class UpdateService.
    /// </summary>
    internal class UpdateService : IUpdateService
    {
        /// <summary>
        /// Gets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Gets the Info service.
        /// </summary>
        private readonly IInfoService _infoService;

        /// <summary>
        /// Options for update service.
        /// </summary>
        private readonly UpdateOptions _updateOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService" /> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="infoService"></param>
        /// <param name="options"></param>
        public UpdateService(
            ILanguageService languageService,
            IDialogService dialogService,
            IInfoService infoService,
            IOptions<UpdateOptions> options)
        {
            Argument.IsNotNull(options);

            _languageService = languageService;
            _dialogService = dialogService;
            _infoService = infoService;
            _updateOptions = options.Value;
        }

        /// <summary>
        /// check for update as an asynchronous operation.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> CheckForUpdateAsync()
        {
            if (await NetworkUtility.IsInternetConnectionAvailable())
                return await CheckVersionAsync(_updateOptions.ApplicationId);

            return false;
        }

        /// <summary>
        /// download and install update as an asynchronous operation.
        /// </summary>
        public async Task DownloadAndInstallUpdateAsync()
        {
            await GetUpdateAsync(_updateOptions.ApplicationId);

            var updatePath = Path.Combine(Path.GetTempPath(), _updateOptions.Filename);

            if (File.Exists(updatePath))
            {
                System.Diagnostics.Process.Start(updatePath);
            }
            else
            {
                await _dialogService.ShowInformationAsync(_languageService.GetString("CannotFindUpdateFile"));
            }
        }

        private async Task<bool> CheckVersionAsync(int applicationId)
        {
            var version = string.Empty;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _updateOptions.VersionEndpoint);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    version = await response.Content.ReadAsStringAsync();
            }

            if (!string.IsNullOrEmpty(version))
            {
                int intCurrentMajor = _infoService.ProductVersion.Major;
                int intCurrentMinor = _infoService.ProductVersion.Minor;
                int intCurrentBuild = _infoService.ProductVersion.Build;
                int intCurrentRevision = _infoService.ProductVersion.Revision;

                string[] intUpdate = version.Split('.');
                int intUpdateMajor = int.Parse(intUpdate[0]);
                int intUpdateMinor = int.Parse(intUpdate[1]);
                int intUpdateBuild = int.Parse(intUpdate[2]);
                int intUpdateRevision = int.Parse(intUpdate[3]);

                if ((intUpdateMajor > intCurrentMajor) ||
                    ((intUpdateMajor == intCurrentMajor) && (intUpdateMinor > intCurrentMinor)) ||
                    ((intUpdateMajor == intCurrentMajor) && (intUpdateMinor == intCurrentMinor) && (intUpdateBuild > intCurrentBuild)) ||
                    ((intUpdateMajor == intCurrentMajor) && (intUpdateMinor == intCurrentMinor) && (intUpdateBuild == intCurrentBuild) && (intUpdateRevision != intCurrentRevision)))
                {
                    return true;
                }

                return false;
            }
            else
            {
                throw new Exception(_languageService.GetString("CouldNotRetrieveVersionInformation"));
            }
        }

        protected async Task GetUpdateAsync(int applicationId)
        {
            try
            {
                if (await NetworkUtility.IsInternetConnectionAvailable())
                {
                    var updatePath = Path.Combine(Path.GetTempPath(), _updateOptions.Filename);

                    using (var client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, _updateOptions.DownloadEndpoint);
                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            var stream = await response.Content.ReadAsStreamAsync();
                            var fileInfo = new FileInfo(updatePath);

                            using (var fileStream = fileInfo.OpenWrite())
                                await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format("{0}" + System.Environment.NewLine + "{1}",
                        _languageService.GetString("NoInternetConnectionAvailable"),
                        _languageService.GetString("CannotInstallUpdate")));
                }
            }
            catch (Exception)
            {
                throw;
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
                        _languageService.GetString("MandatoryUpdateFailed"));
        }
    }
}