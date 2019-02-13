﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ISynergy
{
    public class Property<T> : IProperty<T>, INotifyPropertyChanged
    {
        public Property()
        {
            Errors.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsValid));
        }

        public event EventHandler ValueChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Revert() => Value = OriginalValue;

        public void MarkAsClean() => OriginalValue = Value;

        public override string ToString() => Value?.ToString();

        [JsonIgnore]
        public ObservableCollection<string> Errors { get; } = new ObservableCollection<string>();

        [JsonIgnore]
        public bool IsValid => !Errors.Any();

        [JsonIgnore]
        public bool IsDirty
        {
            get
            {
                if (Value == null)
                    return OriginalValue != null;
                return !Value.Equals(OriginalValue);
            }
        }

        T _Value = default;

        public T Value
        {
            get { return _Value; }
            set
            {
                if (!IsOriginalSet)
                    OriginalValue = value;
                Set(ref _Value, value);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        bool _IsOriginalSet = false;

        public bool IsOriginalSet
        {
            get { return _IsOriginalSet; }
            private set { Set(ref _IsOriginalSet, value); }
        }

        T _OriginalValue = default;

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
            OnPropertyChanged(nameof(IsDirty));
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
