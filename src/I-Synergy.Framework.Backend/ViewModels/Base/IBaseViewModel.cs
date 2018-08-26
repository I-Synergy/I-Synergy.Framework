using ISynergy.Interfaces;
using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.ViewModels
{
    public interface IBaseViewModel : IDisposable
    {
        IFactoryService _factory { get; }

        Task InitializeAsync();
        string Title { get; }
    }
}
