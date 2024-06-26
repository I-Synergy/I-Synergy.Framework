﻿namespace Sample.Abstractions;

/// <summary>
/// Interface ISettingsService
/// </summary>
public interface ISettingsService<TSettings>
    where TSettings : class
{
    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <value>The settings.</value>
    TSettings Settings { get; }
    /// <summary>
    /// Updates the settings.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken"></param>
    Task<int> AddOrUpdateSettingsAsync(TSettings e, CancellationToken cancellationToken = default);
    /// <summary>
    /// Loads the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task LoadSettingsAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets the setting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    /// <returns>T.</returns>
    T GetSetting<T>(string name, T defaultvalue) where T : IComparable<T>;
}
