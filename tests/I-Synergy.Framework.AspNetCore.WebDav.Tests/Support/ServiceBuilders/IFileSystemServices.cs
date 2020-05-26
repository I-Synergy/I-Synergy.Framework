using System;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders
{
    public interface IFileSystemServices
    {
        IServiceProvider ServiceProvider { get; }
    }
}
