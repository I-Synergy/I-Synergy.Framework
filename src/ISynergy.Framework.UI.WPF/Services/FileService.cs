using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Base class for file services.
/// </summary>
public class FileService : IFileService<FileResult>
{
    /// <summary>
    /// The dialog service.
    /// </summary>
    private readonly IDialogService _dialogService;

    /// <summary>
    /// The language service.
    /// </summary>
    private readonly ILanguageService _languageService;

    public string Filter { get; set; }
    public bool AddExtension { get; set; }
    public bool CheckFileExists { get; set; }
    public bool CheckPathExists { get; set; }
    public int FilterIndex { get; set; }
    public string InitialDirectory { get; set; }
    public string Title { get; set; }
    public bool ValidateNames { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService" /> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="languageService">The language service.</param>
    public FileService(IDialogService dialogService, ILanguageService languageService)
    {
        _dialogService = dialogService;
        _languageService = languageService;

        AddExtension = true;
        CheckFileExists = false;
        CheckPathExists = true;
        FilterIndex = 1;
        ValidateNames = true;
    }

    public Task<List<FileResult>> BrowseFileAsync(string filefilter, bool multiple = false, long maxFileSize = 1048576)
    {
        var result = new List<FileResult>();

        Filter = filefilter;

        var fileDialog = new OpenFileDialog();

        ConfigureFileDialog(fileDialog);

        fileDialog.Multiselect = multiple;

        bool dialogResult = fileDialog.ShowDialog() ?? false;

        if (dialogResult)
        {
            foreach (var file in fileDialog.FileNames)
            {
                result.Add(new FileResult(
                    file,
                    Path.GetFileName(file),
                    () => File.OpenRead(file)
                ));
            }
        }

        return Task.FromResult(result);
    }

    public async Task<byte[]> BrowseImageAsync(string[] filter, long maxFileSize = 1048576)
    {
        if (await BrowseFileAsync(string.Join(";", filter), false, maxFileSize) is List<FileResult> result)
            return result.First().File;

        return null;
    }

    public async Task OpenFileAsync(string fileToOpen)
    {
        await Task.Run(() =>
        {
            Process docProcess = new Process();
            docProcess.StartInfo.FileName = fileToOpen;
            docProcess.StartInfo.UseShellExecute = true;
            docProcess.Start();
        });
    }

    /// <summary>
    /// Saves file to folder async
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="filename"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public Task<FileResult> SaveFileAsync(string folder, string filename, byte[] file)
    {
        var extension = Path.GetExtension(filename);
        var fileDialog = new SaveFileDialog();

        ConfigureFileDialog(fileDialog);

        fileDialog.OverwritePrompt = true;
        fileDialog.AddExtension = false;
        fileDialog.Filter = $"{extension}|{extension}";
        fileDialog.DefaultExt = extension;
        fileDialog.InitialDirectory = folder;
        fileDialog.FileName = filename;

        if (fileDialog.ShowDialog() ?? false)
        {
            return Task.FromResult(
                new FileResult(
                    fileDialog.FileName,
                    Path.GetFileName(fileDialog.FileName),
                    () => File.OpenRead(fileDialog.FileName)));
        }

        return Task.FromResult<FileResult>(null);
    }

    /// <summary>
    /// Configures the file dialog.
    /// </summary>
    /// <param name="fileDialog">The file dialog.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="fileDialog"/> is <c>null</c>.</exception>
    private void ConfigureFileDialog(FileDialog fileDialog)
    {
        Argument.IsNotNull(fileDialog);

        fileDialog.Filter = Filter;
        fileDialog.AddExtension = AddExtension;
        fileDialog.CheckFileExists = CheckFileExists;
        fileDialog.CheckPathExists = CheckPathExists;
        fileDialog.FilterIndex = FilterIndex;
        fileDialog.InitialDirectory = InitialDirectory.ToEndingWithDirectorySeparator();
        fileDialog.Title = Title;
        fileDialog.ValidateNames = ValidateNames;
    }
}
