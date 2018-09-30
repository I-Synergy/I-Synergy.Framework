using System;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public interface IBaseViewModel : IDisposable
    {
        Task InitializeAsync();
        string Title { get; }
    }
}
