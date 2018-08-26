using System.Threading.Tasks;

namespace ISynergy
{
    public interface IWindow
    {
        object DataContext { get; set; }
        object Owner { get; set; }
        Task<bool?> ShowAsync<TEntity>() where TEntity : class, new();
        void Close();
    }
}