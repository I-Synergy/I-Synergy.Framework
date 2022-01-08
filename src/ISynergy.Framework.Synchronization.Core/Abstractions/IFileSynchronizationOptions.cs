
using System;

namespace ISynergy.Framework.Synchronization.Core.Abstractions
{
    public interface IFileSynchronizationOptions
    {
        TimeSpan CheckHostInterval { get; set; }
        string Host { get; set; }
        string SynchronizationDownloadParameter { get; set; }
        string SynchronizationDownloadRoute { get; set; }
        string SynchronizationFolderPath { get; set; }
        string SynchronizationFolderRoute { get; set; }
        string SynchronizationListRoute { get; set; }
    }
}