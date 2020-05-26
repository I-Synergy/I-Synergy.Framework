using System;
using System.Collections.ObjectModel;
using ISynergy.Framework.Core.Collections;

namespace ISynergy.Framework.Core.Data
{
    public interface IObservableClass : IBindable, IDisposable
    {
        bool Validate();
        void Revert();
        void MarkAsClean();
        ObservableConcurrentDictionary<string, IProperty> Properties { get; }
        ObservableCollection<string> Errors { get; }
        Action<IObservableClass> Validator { set; get; }
        bool IsValid { get; }
        bool IsDirty { get; }
    }
}
