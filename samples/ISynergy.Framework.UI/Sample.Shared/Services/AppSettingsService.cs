using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using Sample.Models;
using System;
using System.Threading.Tasks;

namespace Sample.Services
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
