using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ISynergy
{
    public interface IBindable : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName]string propertyName = null);
    }
}
