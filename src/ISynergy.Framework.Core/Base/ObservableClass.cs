using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

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
        /// Automatic validation trigger.
        /// </summary>
        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public bool AutomaticValidationTrigger
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public Dictionary<string, IProperty> Properties { get; } = new Dictionary<string, IProperty>();

        /// <summary>
        /// Gets or sets the validator.
        /// </summary>
        /// <value>The validator.</value>
        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public Action<IObservableClass> Validator { set; get; }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public bool IsDirty
        {
            get { return Properties.Any(x => x.Value.IsDirty); }
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public bool IsValid => !HasErrors;

        /// <summary>
        /// Checks if both objects are the same based on common identity property.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is IObservableClass observable && observable.HasIdentityProperty())
                return observable.GetIdentityValue().Equals(this.GetIdentityValue());

            var currentPropertiesToCheck = this.GetType()
                .GetProperties()
                .Where(q =>
                    !q.Name.Equals(nameof(IObservableClass.AutomaticValidationTrigger)) &&
                    !q.Name.Equals(nameof(IObservableClass.Error)) &&
                    !q.Name.Equals(nameof(IObservableClass.Errors)) &&
                    !q.Name.Equals(nameof(IObservableClass.HasErrors)) &&
                    !q.Name.Equals("Item") &&
                    !q.Name.Equals(nameof(IObservableClass.IsDirty)) &&
                    !q.Name.Equals(nameof(IObservableClass.IsValid)) &&
                    !q.Name.Equals(nameof(IObservableClass.Properties)) &&
                    !q.Name.Equals(nameof(IObservableClass.Validator)));

            var propertiesToCheck = obj.GetType()
                .GetProperties()
                .Where(q =>
                    !q.Name.Equals(nameof(IObservableClass.AutomaticValidationTrigger)) &&
                    !q.Name.Equals(nameof(IObservableClass.Error)) &&
                    !q.Name.Equals(nameof(IObservableClass.Errors)) &&
                    !q.Name.Equals(nameof(IObservableClass.HasErrors)) &&
                    !q.Name.Equals("Item") &&
                    !q.Name.Equals(nameof(IObservableClass.IsDirty)) &&
                    !q.Name.Equals(nameof(IObservableClass.IsValid)) &&
                    !q.Name.Equals(nameof(IObservableClass.Properties)) &&
                    !q.Name.Equals(nameof(IObservableClass.Validator)));

            foreach (var property in currentPropertiesToCheck)
            {
                if (!propertiesToCheck.Any(a => a.Name.Equals(property.Name)))
                {
                    return false;
                }

                if (propertiesToCheck.Where(q => q.Name.Equals(property.Name)).SingleOrDefault() is PropertyInfo propertyInfo)
                {
                    if ((property.GetValue(this) is null && propertyInfo.GetValue(obj) is not null) ||
                        (property.GetValue(this) is not null && propertyInfo.GetValue(obj) is null))
                    {
                        return false;
                    }
                    else if (property.GetValue(this) is not null && propertyInfo.GetValue(obj) is not null && !property.GetValue(this).Equals(propertyInfo.GetValue(obj)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            if (this.GetIdentityValue() is not null)
                return this.GetIdentityValue().GetHashCode();

            return new HashCode().ToHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableClass" /> class.
        /// </summary>
        /// <param name="automaticValidationTrigger">The validation.</param>
        protected ObservableClass(bool automaticValidationTrigger = false)
        {
            AutomaticValidationTrigger = automaticValidationTrigger;
            ErrorsChanged += ObservableClass_ErrorsChanged;
        }

        private void ObservableClass_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(Errors));
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>T.</returns>
        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>(propertyName));

            if (Properties[propertyName] is IProperty<T> property)
                return property.Value;

            return default;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>(propertyName));

            if (Properties[propertyName] is IProperty<T> property)
            {
                var previous = property.Value;

                if (!property.IsOriginalSet || !Equals(value, previous) || typeof(T).IsNullableType() && value is null)
                {
                    property.Value = value;

                    OnPropertyChanged(propertyName);

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
        protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>(propertyName));

            if (Properties[propertyName] is IProperty<T> property)
            {
                var previous = field;

                if (!property.IsOriginalSet || !Equals(value, previous) || typeof(T).IsNullableType() && value is null)
                {
                    field = value;

                    OnPropertyChanged(propertyName);

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
            OnErrorsChanged(nameof(Errors));

            return IsValid;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Validate(bool validateUnderlayingProperties = true)
        {
            ClearErrors();

            if (validateUnderlayingProperties)
            {
                foreach (var property in this.GetType().GetProperties().Where(q => q.PropertyType.GetInterfaces().Contains(typeof(IObservableClass))))
                {
                    if (property.GetValue(this, null) is ObservableClass observable &&
                        !observable.Validate(validateUnderlayingProperties))
                    {
                        Errors.AddRange(observable.Errors);
                        observable.Errors.Clear();
                    }
                }
            }
            
            Validator?.Invoke(this);

            OnPropertyChanged(nameof(IsValid));
            OnErrorsChanged(nameof(Errors));

            return IsValid;
        }

        /// <summary>
        /// Reverts this instance.
        /// </summary>
        public void Revert()
        {
            foreach (var property in Properties)
                property.Value.ResetChanges();

            if (AutomaticValidationTrigger)
                Validate();
        }

        /// <summary>
        /// Marks as clean.
        /// </summary>
        public void MarkAsClean()
        {
            foreach (var property in Properties)
                property.Value.MarkAsClean();

            if (AutomaticValidationTrigger)
                Validate();
        }

        #region IDataErrorInfo
        public void AddValidationError(string propertyName, string errorMessage) =>
            Errors.Add(new KeyValuePair<string, string>(propertyName, errorMessage));

        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public ObservableCollection<KeyValuePair<string, string>> Errors { get; } = new ObservableCollection<KeyValuePair<string, string>>();

        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public string this[string propertyName]
        {
            get
            {
                if (Errors.Any(a => a.Key.Equals(propertyName)))
                    return Errors
                        .FirstOrDefault(q => q.Key.Equals(propertyName))
                        .Value ?? propertyName;
                return string.Empty;
            }
        }
        #endregion

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public virtual void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (Errors.Any(a => a.Key.Equals(propertyName)))
                {
                    return Errors
                        .Where(q => q.Key.Equals(propertyName))
                        .Select(s => s.Value)
                        .ToList();
                }
                else
                {
                    return Enumerable.Empty<string>();
                }
            }
            else
            {
                return Errors.SelectMany(m => m.Value.ToList()).ToList();
            }
        }

        [JsonIgnore]
        [DataTableIgnore]
        [XmlIgnore]
        [Display(AutoGenerateField = false)]
        public bool HasErrors
        {
            get => Errors.Any(a => a.Value.Any());
        }
        #endregion

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
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                ErrorsChanged -= ObservableClass_ErrorsChanged;
            }

            // free native resources if there are any.
        }
        #endregion
    }
}
