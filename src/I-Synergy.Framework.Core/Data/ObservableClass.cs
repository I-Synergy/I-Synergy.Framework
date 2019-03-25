using ISynergy.Attributes;
using ISynergy.Enumerations;
using ISynergy.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ISynergy
{
    public abstract class ObservableClass : IObservableClass, INotifyPropertyChanged
    {
        [IgnoreProperty]
        private ValidationTriggers ValidationTrigger { get; set; }

        [IgnoreProperty]
        public ObservableConcurrentDictionary<string, IProperty> Properties { get; }
            = new ObservableConcurrentDictionary<string, IProperty>();

        [IgnoreProperty]
        public ObservableCollection<string> Errors { get; }
            = new ObservableCollection<string>();

        [IgnoreProperty]
        public Action<IObservableClass> Validator { set; get; }

        [IgnoreProperty]
        public bool IsValid => !Errors.Any();

        [IgnoreProperty]
        public bool IsDirty
        {
            get { return Properties.Any(x => x.Value.IsDirty); }
        }

        public void SetValidationTrigger(ValidationTriggers validation)
        {
            ValidationTrigger = validation;
        }

        protected ObservableClass()
        {
            var propertyInfos = this.GetType()
                .GetProperties()
                .Where(p => !Attribute.IsDefined(p, typeof(IgnorePropertyAttribute)))
                .ToList();

            foreach (var propertyInfo in propertyInfos)
            {
                //add to property dictionary
                var propValue = propertyInfo.GetValue(this);
                var propType = propertyInfo.PropertyType;

                var property = (IProperty)typeof(Property<>)
                    .MakeGenericType(propType)
                    .GetConstructor(new[] { propType })
                    .Invoke(new[] { propValue });

                Properties.Add(propertyInfo.Name, property);
            }
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());

            return (Properties[propertyName] as IProperty<T>).Value;
        }

        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());

            var property = Properties[propertyName] as IProperty<T>;
            var previous = property.Value;

            if (!property.IsOriginalSet || !Equals(value, previous) || typeof(T).IsNullableType())
            {
                property.Value = value;
                OnPropertyChanged(propertyName);

                if (ValidationTrigger == ValidationTriggers.ChangedProperty)
                {
                    Validate();
                }
            }
        }

        public bool Validate()
        {
            foreach (var property in Properties)
            {
                property.Value.Errors.Clear();
            }

            Validator?.Invoke(this);

            foreach (var error in Errors.ToList())
            {
                if (!Properties.Values.SelectMany(x => x.Errors).Contains(error))
                {
                    Errors.Remove(error);
                }
            }

            foreach (var error in Properties.Values.SelectMany(x => x.Errors).ToList())
            {
                if (!Errors.Contains(error))
                {
                    Errors.Add(error);
                }
            }

            OnPropertyChanged(nameof(IsValid));

            return IsValid;
        }

        public void Revert()
        {
            foreach (var property in Properties)
            {
                property.Value.ResetChanges();
            }

            if (ValidationTrigger == ValidationTriggers.ChangedProperty)
            {
                Validate();
            }
        }

        public void MarkAsClean()
        {
            foreach (var property in Properties)
            {
                property.Value.MarkAsClean();
            }

            if (ValidationTrigger == ValidationTriggers.ChangedProperty)
            {
                Validate();
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDisposable
        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        //~ObservableClass()
        //{
        //    // Finalizer calls Dispose(false)
        //    Dispose(false);
        //}

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources if there are any.
        }
        #endregion
    }
}