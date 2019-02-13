using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ISynergy
{
    public interface IBindable : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName]string propertyName = null);
    }
}
