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

        private const string DefaultLength = "m";

        /// <summary>
        /// Default length unit for measurements.
        /// </summary>
        public string LengthUnit
        {
            get
            {
                var setting = Sample.Properties.Settings.Default[nameof(LengthUnit)];

                if (setting is null)
                {
                    setting = DefaultLength;
                    LengthUnit = setting.ToString();
                }

                return (string)setting;
            }
            set
            {
                Sample.Properties.Settings.Default[nameof(LengthUnit)] = value;
                Sample.Properties.Settings.Default.Save();
            }
        }
    }
}
