using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Core.Data
{
    public interface IProperty : IBindable
    {
        event EventHandler ValueChanged;

        void ResetChanges();

        void MarkAsClean();

        ObservableCollection<string> Errors { get; }

        [JsonIgnore]
        bool IsValid { get; }

        bool IsDirty { get; }

        bool IsOriginalSet { get; }
    }
}
