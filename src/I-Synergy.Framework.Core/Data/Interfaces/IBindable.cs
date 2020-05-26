using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Data
{
    public interface IBindable : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName]string propertyName = null);
    }
}
