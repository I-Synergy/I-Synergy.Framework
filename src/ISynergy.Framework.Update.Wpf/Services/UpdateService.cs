using Flurl;
using Flurl.Http;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Update.Options;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Update.Services
{
    internal partial class UpdateService
    {
        private readonly UpdateOptions _updateOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService" /> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="options"></param>
        public UpdateService(
            ILanguageService languageService,
            IDialogService dialogService,
            IOptions<UpdateOptions> options)
            : this (languageService, dialogService)
        {
            _updateOptions = options.Value;
        }

        /// <summary>
        /// Checks for update asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> CheckForUpdateAsync()
        {
            if (await CheckForInternet())
            {
                return await CheckVersionAsync(_updateOptions.ApplicationId);
            }

            return false;
        }

        /// <summary>
        /// Downloads the and install update asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task DownloadAndInstallUpdateAsync()
        {
            await GetUpdateAsync(_updateOptions.ApplicationId);

            if (System.IO.File.Exists(_updateOptions.UpdateFilePath))
            {
                System.Diagnostics.Process.Start(_updateOptions.UpdateFilePath);
            }
            else
            {
                await ServiceLocator.Default.GetInstance<IDialogService>().ShowInformationAsync(
                    ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Update_Cannot_find_file"));
            }
        }

        protected async Task<bool> CheckForInternet()
        {
            if (await NetworkUtility.IsInternetConnectionAvailable() && NetworkUtility.IsUrlReachable(_updateOptions.UpdateUrl))
            {
                return true;
            }

            throw new Exception(string.Format("{0}" + System.Environment.NewLine + "{1}",
                        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Generic_No_internet_connection_available"),
                        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Update_Cannot_install_update")));
        }

        protected async Task<bool> CheckVersionAsync(string applicationId)
        {
            string version = await new Url(_updateOptions.UpdateUrl)
                        .AppendPathSegments("software", "version")
                        .SetQueryParam("application", applicationId)
                        .GetStringAsync();

            if ((version != ""))
            {
                var currentVersion = ServiceLocator.Default.GetInstance<IInfoService>().ProductVersion;
                int intCurrentMajor = currentVersion.Major;
                int intCurrentMinor = currentVersion.Minor;
                int intCurrentBuild = currentVersion.Build;
                int intCurrentRevision = currentVersion.Revision;

                string[] intUpdate = version.Split('.');
                int intUpdateMajor = int.Parse(intUpdate[0]);
                int intUpdateMinor = int.Parse(intUpdate[1]);
                int intUpdateBuild = int.Parse(intUpdate[2]);
                int intUpdateRevision = int.Parse(intUpdate[3]);

                if ((intUpdateMajor > intCurrentMajor))
                {
                    return true;
                }
                else if ((intUpdateMajor == intCurrentMajor))
                {
                    if ((intUpdateMinor > intCurrentMinor))
                    {
                        return true;
                    }
                    else if ((intUpdateMinor == intCurrentMinor))
                    {
                        if ((intUpdateBuild > intCurrentBuild))
                        {
                            return true;
                        }
                        else if ((intUpdateBuild == intCurrentBuild))
                        {
                            if ((intUpdateRevision != intCurrentRevision))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception(ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Error_Could_not_retrieve_version_information"));
            }
        }

        protected async Task GetUpdateAsync(string applicationId)
        {
            try
            {
                System.Net.WebClient UpdateWebClient = new System.Net.WebClient();

                await UpdateWebClient.DownloadFileTaskAsync(
                    new Uri(_updateOptions.UpdateUrl + $"software/download?application={applicationId}"),
                    _updateOptions.UpdateFilePath);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}" + System.Environment.NewLine + "{1}",
                        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Generic_No_internet_connection_available"),
                        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Update_Cannot_install_update")));
            }
        }
    }
}
