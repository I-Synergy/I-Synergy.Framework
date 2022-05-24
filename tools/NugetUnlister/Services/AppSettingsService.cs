using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace NugetUnlister.Services
{
    public class AppSettingsService : IBaseApplicationSettingsService
    {
        public IBaseApplicationSettings Settings { get; set; }

        public AppSettingsService()
        {
            Settings = new ApplicationSetting();
        }

        public Task LoadSettingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveSettingsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
