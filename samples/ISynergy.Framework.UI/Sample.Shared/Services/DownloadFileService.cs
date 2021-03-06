﻿using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Windows.System;

namespace Sample.Services
{
    /// <summary>
    /// Class DownloadFileService.
    /// </summary>
    public class DownloadFileService : IDownloadFileService
    {
        /// <summary>
        /// The file service
        /// </summary>
        protected readonly IFileService FileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileService" /> class.
        /// </summary>
        /// <param name="fileservice">The fileservice.</param>
        public DownloadFileService(IFileService fileservice)
        {
            FileService = fileservice;
        }

        /// <summary>
        /// download file as an asynchronous operation.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="file">The file.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task DownloadFileAsync(string filename, byte[] file)
        {
            if(await FileService.SaveFileAsync(filename, file) is FileResult result)
            {
                await Launcher.LaunchUriAsync(new Uri(result.FilePath));
            };
        }
    }
}
