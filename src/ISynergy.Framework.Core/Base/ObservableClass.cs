using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Messaging;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Base
{
    /// <summary>
    /// Class ObservableClass.
    /// Implements the <see cref="IObservableClass" />
    /// </summary>
    /// <seealso cref="IObservableClass" />
    public abstract class ObservableClass : IObservableClass
    {
        /// <summary>
        /// Gets the validation trigger.
        /// </summary>
        /// <value>The validation trigger.</value>
        [JsonIgnore]
        [DataTableIgnore]
        public bool AutomaticValidationTrigger
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [JsonIgnore]
        [DataTableIgnore]
        public ObservableConcurrentDictionary<string, IProperty> Properties { get; }
            = new ObservableConcurrentDictionary<string, IProperty>();

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        [JsonIgnore]
        [DataTableIgnore]
        public ObservableCollection<string> Errors { get; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the validator.
        /// </summary>
        /// <value>The validator.</value>
        [JsonIgnore]
        [DataTableIgnore]
        public Action<IObservableClass> Validator { set; get; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        [DataTableIgnore]
        public bool IsValid => !Errors.Any();

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        [DataTableIgnore]
        public bool IsDirty
        {
            get { return Properties.Any(x => x.Value.IsDirty); }
        }

        /// <summary>
        /// Checks if both objects are the same based on common identity property.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is IObservableClass observable && observable.HasIdentityProperty())
                return observable.GetIdentityValue().Equals(this.GetIdentityValue());

            return false;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            if (this.GetIdentityValue() is not null)
                return this.GetIdentityValue().GetHashCode();
            return GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableClass" /> class.
        /// </summary>
        /// <param name="automaticValidation">The validation.</param>
        protected ObservableClass(bool automaticValidation = false)
        {
            AutomaticValidationTrigger = automaticValidation;
            Validator = new Action<IObservableClass>(_ =>
            {
                foreach (var item in GetType().GetProperties())
                {
                    var value = item.GetValue(this);

                    if (Attribute.IsDefined(item, typeof(RequiredAttribute)) && value is null)
                    {
                        if (!Properties.ContainsKey(item.Name))
                            Properties.Add(item.Name, new Property<object>(item.Name, value));

                        Properties[item.Name].Errors.Add(string.Format(Core.Properties.Resources.WarningMandatoryProperty, $"[{item.Name}]"));
                    }
                }
            });
        }

        private IMessageService _messengerInstance;

        /// <summary>
        /// Gets or sets an instance of a <see cref="IMessageService" /> used to
        /// broadcast messages to other objects. If null, this class will
        /// attempt to broadcast using the Messenger's default instance.
        /// </summary>
        protected IMessageService MessengerInstance
        {
            get
            {
                return _messengerInstance ?? MessageService.Default;
            }
            set
            {
                _messengerInstance = value;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>T.</returns>
        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>(propertyName));

            if (Properties[propertyName] is IProperty<T> property)
            {
                return property.Value;
            }

            return default;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="broadcast"></param>
        protected void SetValue<T>(T value, bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>(propertyName));

            if (Properties[propertyName] is IProperty<T> property)
            {
                var previous = property.Value;

                if (!property.IsOriginalSet || !Equals(value, previous) || typeof(T).IsNullableType() && value is null)
                {
                    property.Value = value;
                    OnPropertyChanged(propertyName);

                    if (broadcast)
                        Broadcast(previous, value, propertyName);

                    if (AutomaticValidationTrigger)
                        Validate();
                }
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="broadcast"></param>
        protected void SetValue<T>(ref T field, T value, bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>(propertyName));

            if (Properties[propertyName] is IProperty<T> property)
            {
                var previous = field;

                if (!property.IsOriginalSet || !Equals(value, previous) || typeof(T).IsNullableType() && value is null)
                {
                    field = value;
                    OnPropertyChanged(propertyName);

                    if (broadcast)
                        Broadcast(previous, value, propertyName);

                    if (AutomaticValidationTrigger)
                        Validate();
                }
            }
        }

        /// <summary>
        /// Clears the errors.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ClearErrors()
        {
            Errors.Clear();

            OnPropertyChanged(nameof(IsValid));

            return IsValid;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Reverts this instance.
        /// </summary>
        public void Revert()
        {
            foreach (var property in Properties)
            {
                property.Value.ResetChanges();
            }

            if (AutomaticValidationTrigger)
                Validate();
        }

        /// <summary>
        /// Marks as clean.
        /// </summary>
        public void MarkAsClean()
        {
            foreach (var property in Properties)
            {
                property.Value.MarkAsClean();
            }

            MessengerInstance.Unregister(this);

            if (AutomaticValidationTrigger)
                Validate();
        }

        /// <summary>
        /// Broadcasts a PropertyChangedMessage using either the instance of
        /// the Messenger that was passed to this class (if available) 
        /// or the Messenger's default instance.
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="oldValue">The value of the property before it
        /// changed.</param>
        /// <param name="newValue">The value of the property after it
        /// changed.</param>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        protected virtual void Broadcast<T>(T oldValue, T newValue, string propertyName)
        {
            var message = new PropertyChangedMessage<T>(this, oldValue, newValue, propertyName);
            MessengerInstance.Send(message);
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDisposable
        // Dispose() calls Dispose(true)
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
