using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IScreenSaverService
    {
        Task InitializeScreenSaverAsync(Uri logo);
    }
}
