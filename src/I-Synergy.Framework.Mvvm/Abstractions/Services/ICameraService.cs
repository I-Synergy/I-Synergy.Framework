using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface ICameraService
    {
        Task<FileResult> TakePictureAsync(ulong maxfilesize);
    }
}
