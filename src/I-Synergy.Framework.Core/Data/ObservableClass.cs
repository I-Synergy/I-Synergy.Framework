using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Data
{
    public abstract class ObservableClass : IObservableClass, INotifyPropertyChanged
    {
        [JsonIgnore]
        private ValidationTriggers ValidationTrigger { get; }

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
        public bool IsDirty
        {
            get { return Properties.Any(x => x.Value.IsDirty); }
        }

        /// <summary>
        /// Checks if both objects are the same based on common identity property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IObservableClass observable && observable.HasIdentityProperty())
                return observable.GetIdentityValue().Equals(this.GetIdentityValue());

            return base.Equals(obj);
        }

        protected ObservableClass(ValidationTriggers validation = ValidationTriggers.Manual)
        {
            ValidationTrigger = validation;
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());

            if (Properties[propertyName] is IProperty<T> property)
            {
                return property.Value;
            }

            return default;
        }

        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());

            if (Properties[propertyName] is IProperty<T> property)
            {
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
        }

        public bool ClearErrors()
        {
            Errors.Clear();

            OnPropertyChanged(nameof(IsValid));

            return IsValid;
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
