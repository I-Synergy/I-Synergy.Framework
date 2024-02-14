using ISynergy.Framework.Core.Validation;
using System.Text.Json;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Class SettingsStorageExtensions.
/// </summary>
public static class SettingsStorageExtensions
{
    /// <summary>
    /// The file extension
    /// </summary>
    private const string _jsonExtension = ".json";

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    private static string GetJsonFileName(string name) => $"{name}.{_jsonExtension}";

    /// <summary>
    /// save as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="folder">The folder.</param>
    /// <param name="name">The name.</param>
    /// <param name="content">The content.</param>
    public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
    {
        var file = await folder.CreateFileAsync(GetJsonFileName(name), CreationCollisionOption.ReplaceExisting);
        var fileContent = JsonSerializer.Serialize(content);

        await FileIO.WriteTextAsync(file, fileContent);
    }

    /// <summary>
    /// read as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="folder">The folder.</param>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    public static async Task<T> ReadAsync<T>(this StorageFolder folder, string name)
    {
        if (!File.Exists(Path.Combine(folder.Path, GetJsonFileName(name))))
        {
            return default;
        }

        var file = await folder.GetFileAsync(GetJsonFileName(name));
        var fileContent = await FileIO.ReadTextAsync(file);

        return JsonSerializer.Deserialize<T>(fileContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    /// <summary>
    /// save as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="settings">The settings.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static Task SaveAsync<T>(this ApplicationDataContainer settings, string key, T value)
    {
        settings.SaveString(key, JsonSerializer.Serialize(value));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Saves the string.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SaveString(this ApplicationDataContainer settings, string key, string value)
    {
        settings.Values[key] = value;
    }

    /// <summary>
    /// read as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="settings">The settings.</param>
    /// <param name="key">The key.</param>
    /// <returns>T.</returns>
    public static Task<T> ReadAsync<T>(this ApplicationDataContainer settings, string key)
    {
        if (settings.Values.TryGetValue(key, out var obj))
        {
            return Task.FromResult(JsonSerializer.Deserialize<T>((string)obj, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }));
        }

        return default;
    }

    /// <summary>
    /// save file as an asynchronous operation.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="content">The content.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="options">The options.</param>
    /// <returns>StorageFile.</returns>
    /// <exception cref="ArgumentNullException">content</exception>
    /// <exception cref="ArgumentException">ExceptionSettingsStorageExtensionsFileNameIsNullOrEmpty".GetLocalized() - fileName</exception>
    public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
    {
        Argument.IsNotNull(content);
        Argument.IsNotNullOrEmpty(fileName);

        var storageFile = await folder.CreateFileAsync(fileName, options);
        await FileIO.WriteBytesAsync(storageFile, content);
        return storageFile;
    }

    /// <summary>
    /// read file as an asynchronous operation.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>System.Byte[].</returns>
    public static async Task<byte[]> ReadFileAsync(this StorageFolder folder, string fileName)
    {
        var item = await folder.TryGetItemAsync(fileName).AsTask();

        if ((item is not null) && item.IsOfType(StorageItemTypes.File))
        {
            var storageFile = await folder.GetFileAsync(fileName);
            var content = await storageFile.ReadBytesAsync();
            return content;
        }

        return null;
    }

    /// <summary>
    /// read bytes as an asynchronous operation.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>System.Byte[].</returns>
    public static async Task<byte[]> ReadBytesAsync(this StorageFile file)
    {
        if (file is not null)
        {
            using IRandomAccessStream stream = await file.OpenReadAsync();
            using var reader = new DataReader(stream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)stream.Size);
            var bytes = new byte[stream.Size];
            reader.ReadBytes(bytes);
            return bytes;
        }

        return null;
    }
}
