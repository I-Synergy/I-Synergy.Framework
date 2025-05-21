using ISynergy.Framework.Storage.Abstractions.Services;

namespace Sample.Storage.Azure;

/// <summary>
/// Initializes a new instance of the <see cref="Startup"/> class.
/// </summary>
/// <param name="storageService">The Storage service.</param>
public class Startup(IStorageService storageService)
{
    private readonly string _connectionString = "https://aka.ms/bloburl";
    private readonly string _container = Guid.Parse("ECEB4346-97AD-4919-9248-3EA1012FCA47").ToString();

    /// <summary>
    /// run as an asynchronous operation.
    /// </summary>
    public async Task RunAsync()
    {
        Console.WriteLine("Azure implementation started...");

        string fileName = Randomize();

        Console.WriteLine("Upload started...");
        Uri url = await UploadAsync(fileName).ConfigureAwait(false);
        Console.WriteLine($"Upload completed to {url}.");

        Console.WriteLine("Download started...");
        byte[] fileBytes = await DownloadAsync(fileName).ConfigureAwait(false);

        if (fileBytes is not null)
        {
            Console.WriteLine($"Download completed with {fileBytes.Length} bits.");

            Console.WriteLine("Update started...");
            url = await UpdateAsync(fileBytes, fileName).ConfigureAwait(false);

            Console.WriteLine($"Update completed of {url}.");
        }

        Console.WriteLine("Removal started...");

        if (await RemoveAsync(fileName))
        {
            Console.WriteLine("File successfully removed.");
        }
        else
        {
            Console.WriteLine("File removal failed.");
        }

        Console.ReadLine();
    }

    public async Task<Uri> UploadAsync(string fileName)
    {
        string path = CreateTempFile();

        // Open the file and upload its data
        byte[] file = await File.ReadAllBytesAsync(path);

        return await storageService.UploadFileAsync(_connectionString, _container, file, "text/plain", fileName, "");
    }

    public Task<byte[]> DownloadAsync(string fileName)
    {
        return storageService.DownloadFileAsync(_connectionString, _container, fileName, "");
    }

    public Task<Uri> UpdateAsync(byte[] fileBytes, string fileName)
    {
        return storageService.UpdateFileAsync(_connectionString, _container, fileBytes, "text/plain", fileName, "");
    }

    public Task<bool> RemoveAsync(string fileName)
    {
        return storageService.RemoveFileAsync(_connectionString, _container, fileName, "");
    }

    /// <summary>
    /// Get a random name so we won't have any conflicts when creating
    /// resources.
    /// </summary>
    /// <param name="prefix">Optional prefix for the random name.</param>
    /// <returns>A random name.</returns>
    public string Randomize(string prefix = "sample") =>
        $"{prefix}-{Guid.NewGuid()}";

    /// <summary>
    /// Lorem Ipsum sample file content
    /// </summary>
    protected const string SampleFileContent = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras dolor purus, interdum in turpis ut, ultrices ornare augue. Donec mollis varius sem, et mattis ex gravida eget. Duis nibh magna, ultrices a nisi quis, pretium tristique ligula. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Vestibulum in dui arcu. Nunc at orci volutpat, elementum magna eget, pellentesque sem. Etiam id placerat nibh. Vestibulum varius at elit ut mattis.  Suspendisse ipsum sem, placerat id blandit ac, cursus eget purus. Vestibulum pretium ante eu augue aliquam, ultrices fermentum nibh condimentum. Pellentesque pulvinar feugiat augue vel accumsan. Nulla imperdiet viverra nibh quis rhoncus. Nunc tincidunt sollicitudin urna, eu efficitur elit gravida ut. Quisque eget urna convallis, commodo diam eu, pretium erat. Nullam quis magna a dolor ullamcorper malesuada. Donec bibendum sem lectus, sit amet faucibus nisi sodales eget. Integer lobortis lacus et volutpat dignissim. Suspendisse cras amet.";

    /// <summary>
    /// Create a temporary path for creating files.
    /// </summary>
    /// <param name="extension">An optional file extension.</param>
    /// <returns>A temporary path for creating files.</returns>
    public string CreateTempPath(string extension = ".txt") =>
        Path.ChangeExtension(Path.GetRandomFileName(), extension);

    /// <summary>
    /// Create a temporary file on disk.
    /// </summary>
    /// <param name="content">Optional content for the file.</param>
    /// <returns>Path to the temporary file.</returns>
    public string CreateTempFile(string content = SampleFileContent)
    {
        string path = CreateTempPath();
        File.WriteAllText(path, content);
        return path;
    }
}
