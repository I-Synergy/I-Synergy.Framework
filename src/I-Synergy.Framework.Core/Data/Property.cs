using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Data
{
#nullable disable
    public class Property<T> : IProperty<T>, INotifyPropertyChanged
    {

        public event EventHandler ValueChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsOriginalSet = false;
        private bool _IsDirty = true;
        private T _Value = default;
        private T _OriginalValue = default;

        public Property()
        {
            Errors = new ObservableCollection<string>();
            Errors.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsValid));
        }

        public Property(T value)
            : this()
        {
            Value = value;
        }

        [JsonIgnore]
        public ObservableCollection<string> Errors { get; }

        [JsonIgnore]
        public bool IsValid => !Errors.Any();

        [JsonIgnore]
        public bool IsDirty
        {
            get { return _IsDirty; }
            private set { Set(ref _IsDirty, value); }
        }

        public T Value
        {
            get { return _Value; }
            set
            {
                if (!IsOriginalSet)
                {
                    OriginalValue = value;
                }

                Set(ref _Value, value);
                IsDirty = true;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsOriginalSet
        {
            get { return _IsOriginalSet; }
            private set { Set(ref _IsOriginalSet, value); }
        }

        public T OriginalValue
        {
            get { return _OriginalValue; }
            set
            {
                IsOriginalSet = true;
                Set(ref _OriginalValue, value);
            }
        }

        private bool Set<V>(ref V storage, V value, [CallerMemberName]string callerMemberName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(callerMemberName);
            return true;
        }

        public void ResetChanges()
        {
            Value = OriginalValue;
            IsDirty = false;
        }

        public void MarkAsClean()
        {
            OriginalValue = Value;
            IsDirty = false;
        }

        public override string ToString() => Value.ToString();

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
