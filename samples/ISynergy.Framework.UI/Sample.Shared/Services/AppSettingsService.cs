using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using Sample.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sample.Services
{
    public class AppSettingsService : IBaseApplicationSettingsService
    {
        private const string _fileName = "settings.json";

        /// <summary>
        /// The global settings folder.
        /// </summary>
        private readonly string _settingsFolder;

        /// <summary>
        /// Settings used globally.
        /// </summary>
        private ApplicationSetting _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoamingSettingsService"/> class.
        /// </summary>
        public AppSettingsService()
        {
            _settings = new ApplicationSetting();
            _settingsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "I-Synergy Framework UI Sample",
                "Settings");
        }

        /// <summary>
        /// Loads the settings json file.
        /// </summary>
        public async Task LoadSettingsAsync()
        {
            try
            {
                var file = Path.Combine(_settingsFolder, _fileName);

                if (!File.Exists(file))
                {
                    await SaveSettingsAsync();
                }

                var json = File.ReadAllText(file);
                _settings = JsonSerializer.Deserialize<ApplicationSetting>(json);
            }
            catch (FileNotFoundException)
            {
                _settings = new ApplicationSetting();
            }
        }

        /// <summary>
        /// Saves all changes to the json file.
        /// </summary>
        public Task SaveSettingsAsync()
        {
            if (!Directory.Exists(_settingsFolder))
                Directory.CreateDirectory(_settingsFolder);

            var file = Path.Combine(_settingsFolder, _fileName);
            var json = JsonSerializer.Serialize(_settings);
            File.WriteAllText(file, json);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Settings used globally.
        /// </summary>
        public IBaseApplicationSettings Settings => _settings;
    }
}
