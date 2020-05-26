using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Commands;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface IShellViewModel : IViewModel
    {
        Task InitializeAsync(object parameter);
        Task ProcessAuthenticationRequestAsync();
        RelayCommand Settings_Command { get; set; }
    }
}
