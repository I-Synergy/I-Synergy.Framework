using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ISynergy
{
    public abstract class ObservableClass : IObservableClass, INotifyPropertyChanged
    {
        public ObservableClass()
        {
            Properties.CollectionChanged += (s, e) =>
            {
                //if (e.Action.Equals(NotifyCollectionChangedAction.Add))
                //    Properties[e.Key].ValueChanged += (sender, args) =>
                //    {
                //        OnPropertyChanged(e.Key);
                //        OnPropertyChanged(nameof(IsDirty));
                //        OnPropertyChanged(nameof(IsValid));
                //    };
            };
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());
            return (Properties[propertyName] as IProperty<T>).Value;
        }

        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null, bool validateAfter = true)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());
            var property = (Properties[propertyName] as IProperty<T>);
            var previous = property.Value;
            if (!property.IsOriginalSet || !Equals(value, previous))
            {
                property.Value = value;
                Validate(validateAfter);
            }
        }

        public bool Validate(bool validateAfter = true)
        {
            if (validateAfter)
            {
                foreach (var property in Properties)
                {
                    property.Value.Errors.Clear();
                }

                Validator?.Invoke(this);
                Errors.Clear();

                foreach (var error in Properties.Values.SelectMany(x => x.Errors))
                {
                    Errors.Add(error);
                }

                OnPropertyChanged(nameof(IsValid));
            }
            return IsValid;
        }

        public void Revert()
        {
            foreach (var property in Properties)
            {
                property.Value.Revert();
            }

            Validate();
        }

        public void MarkAsClean()
        {
            foreach (var property in Properties)
            {
                property.Value.MarkAsClean();
            }

            Validate();
        }

        [JsonIgnore]
        public ObservableConcurrentDictionary<string, IProperty> Properties { get; }
            = new ObservableConcurrentDictionary<string, IProperty>();

        [JsonIgnore]
        public ObservableCollection<string> Errors { get; }
            = new ObservableCollection<string>();

        [JsonIgnore]
        public Action<IObservableClass> Validator { set; get; }

        [JsonIgnore]
        public bool IsValid => !Errors.Any();

        [JsonIgnore]
        public bool IsDirty => Properties.Any(x => x.Value.IsDirty);

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
}
