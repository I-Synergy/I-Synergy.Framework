using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions
{
    public interface IWindow
    {
        IViewModel DataContext { get; set; }
        Task<bool> ShowAsync<TEntity>();
        void Close();
    }
}
