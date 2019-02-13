using ISynergy.Mvvm;
using System.Threading.Tasks;

namespace ISynergy
{
    public interface IWindow
    {
        IViewModel DataContext { get; set; }
        Task<bool?> ShowAsync<TEntity>() where TEntity : class, new();
        void Close();
    }
}