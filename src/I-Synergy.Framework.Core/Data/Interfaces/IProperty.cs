using ISynergy.Attributes;
using System;
using System.Collections.ObjectModel;

namespace ISynergy
{
    public interface IProperty : IBindable
    {
        event EventHandler ValueChanged;

        void ResetChanges();

        void MarkAsClean();

        ObservableCollection<string> Errors { get; }

        [IgnoreProperty]
        bool IsValid { get; }

        bool IsDirty { get; }

        bool IsOriginalSet { get; }
    }
}
