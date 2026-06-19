using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Validation;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Text.Json;

namespace Sample.Services;

public class SettingsService : ISettingsService, IReloadable
{
    private readonly IMessengerService _messengerService;
    private readonly ILogger _logger;

    private LocalSettings? _localSettings;
    private RoamingSettings? _roamingSettings;
    private GlobalSettings? _globalSettings;

    private readonly HashSet<Type> _supportedTypes =
    [
        typeof(string),
        typeof(int),
        typeof(bool),
        typeof(long),
        typeof(double),
        typeof(float),
        typeof(DateTime),
        typeof(DateTimeOffset)
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    public SettingsService(IMessengerService messengerService, ILogger<SettingsService> logger)
    {
        _messengerService = messengerService;
        _logger = logger;
        _logger.LogTrace("SettingsService instance created with ID: {InstanceId}", Guid.NewGuid());

        _localSettings = new LocalSettings();
        _roamingSettings = new RoamingSettings();
        _globalSettings = new GlobalSettings();

        LoadLocalSettings();

        _messengerService.Register<UserSignedInMessage>(this, OnUserSignedIn, keepTargetAlive: true);
    }

    private async void OnUserSignedIn(UserSignedInMessage message) => await ReloadAsync();

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        LoadLocalSettings();
        await LoadGlobalSettingsAsync(cancellationToken);
    }

    public void LoadLocalSettings()
    {
        try
        {
            var properties = _localSettings!.GetProperties();

            foreach (var property in properties.EnsureNotNull())
            {
                var targetType = property.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(targetType);

                if (underlyingType != null)
                    targetType = underlyingType;

                if (!Preferences.Default.ContainsKey(property.Name))
                    continue;

                object? value;

                if (_supportedTypes.Contains(targetType))
                {
                    value = targetType switch
                    {
                        Type t when t == typeof(string) => Preferences.Default.Get(property.Name, default(string)),
                        Type t when t == typeof(int) => Preferences.Default.Get(property.Name, default(int)),
                        Type t when t == typeof(bool) => Preferences.Default.Get(property.Name, default(bool)),
                        Type t when t == typeof(long) => Preferences.Default.Get(property.Name, default(long)),
                        Type t when t == typeof(double) => Preferences.Default.Get(property.Name, default(double)),
                        Type t when t == typeof(float) => Preferences.Default.Get(property.Name, default(float)),
                        Type t when t == typeof(DateTime) => Preferences.Default.Get(property.Name, default(DateTime)),
                        Type t when t == typeof(DateTimeOffset) => Preferences.Default.Get(property.Name, default(DateTimeOffset)),
                        _ => null
                    };
                }
                else
                {
                    var json = Preferences.Default.Get(property.Name, default(string));
                    value = !string.IsNullOrEmpty(json)
                        ? JsonSerializer.Deserialize(json, targetType)
                        : null;
                }

                if (value is null)
                    continue;

                property.SetValue(_localSettings, value);
            }
        }
        catch
        {
            _localSettings = new LocalSettings();
        }
    }

    public bool SaveLocalSettings()
    {
        try
        {
            var properties = _localSettings!.GetProperties();

            foreach (var property in properties.EnsureNotNull())
            {
                var targetType = property.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(targetType);

                if (underlyingType != null)
                    targetType = underlyingType;

                var value = property.GetValue(_localSettings);

                if (value is null)
                    continue;

                if (_supportedTypes.Contains(targetType))
                {
                    switch (targetType)
                    {
                        case Type t when t == typeof(string):
                            Preferences.Default.Set(property.Name, (string)value);
                            break;
                        case Type t when t == typeof(int):
                            Preferences.Default.Set(property.Name, (int)value);
                            break;
                        case Type t when t == typeof(bool):
                            Preferences.Default.Set(property.Name, (bool)value);
                            break;
                        case Type t when t == typeof(long):
                            Preferences.Default.Set(property.Name, (long)value);
                            break;
                        case Type t when t == typeof(double):
                            Preferences.Default.Set(property.Name, (double)value);
                            break;
                        case Type t when t == typeof(float):
                            Preferences.Default.Set(property.Name, (float)value);
                            break;
                        case Type t when t == typeof(DateTime):
                            Preferences.Default.Set(property.Name, (DateTime)value);
                            break;
                        case Type t when t == typeof(DateTimeOffset):
                            Preferences.Default.Set(property.Name, (DateTimeOffset)value);
                            break;
                    }
                }
                else
                {
                    var json = JsonSerializer.Serialize(value, targetType);
                    Preferences.Default.Set(property.Name, json);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task LoadRoamingSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveRoamingSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> AddOrUpdateGlobalSettingsAsync(IGlobalSettings e, CancellationToken cancellationToken = default)
    {
        // Update settings
        return Task.FromResult(1);
    }

    public Task LoadGlobalSettingsAsync(CancellationToken cancellationToken = default)
    {
        // Load settings from database
        return Task.CompletedTask;
    }

    public T GetGlobalSetting<T>(string name, T defaultvalue) where T : IComparable<T> =>
        _globalSettings!.GetPropertyValue(name, defaultvalue);


    public void ClearSettings()
    {
        _globalSettings = null;
        _roamingSettings = null;
    }

    public LocalSettings LocalSettings => Argument.IsNotNull(_localSettings);
    public RoamingSettings RoamingSettings => Argument.IsNotNull(_roamingSettings);
    public GlobalSettings GlobalSettings => Argument.IsNotNull(_globalSettings);

    ILocalSettings ISettingsService.LocalSettings => Argument.IsNotNull(_localSettings);
    IRoamingSettings ISettingsService.RoamingSettings => Argument.IsNotNull(_roamingSettings);
    IGlobalSettings ISettingsService.GlobalSettings => Argument.IsNotNull(_globalSettings);
}