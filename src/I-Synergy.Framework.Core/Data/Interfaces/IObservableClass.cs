using ISynergy.Enumerations;
using System;
using System.Collections.ObjectModel;

namespace ISynergy
{
    public interface IObservableClass : IBindable, IDisposable
    {
        bool Validate();
        void Revert();
        void MarkAsClean();
        ObservableConcurrentDictionary<string, IProperty> Properties { get; }
        ObservableCollection<string> Errors { get; }
        Action<IObservableClass> Validator { set; get; }
        void SetValidationTrigger(ValidationTriggers validation);
        bool IsValid { get; }
        bool IsDirty { get; }
    }
}
