using ISynergy.ViewModels.Base;
using System.Threading.Tasks;

namespace ISynergy.ViewModels
{
    public interface IShellViewModel : IViewModel
    {
        Task InitializeAsync(object frame);
    }
}