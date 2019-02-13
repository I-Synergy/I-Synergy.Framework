using GalaSoft.MvvmLight;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ISynergy.Data
{
    public abstract class ObservableClass : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (_propertyBackingDictionary.TryGetValue(propertyName, out object value))
                return (T)value;

            return default;
        }

        protected bool SetValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (EqualityComparer<T>.Default.Equals(newValue, GetValue<T>(propertyName))) return false;

            _propertyBackingDictionary[propertyName] = newValue;

            OnPropertyChanged(propertyName);

            if (!string.IsNullOrEmpty(propertyName))
                ValidateProperty(propertyName);

            return true;
        }

        private bool ValidateProperty(string propertyName)
        {
            Argument.IsNotNullOrEmpty(propertyName, propertyName);

            var propertyInfo = GetType().GetRuntimeProperty(propertyName);

            if (propertyInfo is null)
                throw new ArgumentException("Invalid property name", propertyName);

            if (Attribute.IsDefined(propertyInfo, typeof(ValidationAttribute)))
            {
                var propertyErrors = new List<string>();
                return TryValidateProperty(propertyInfo, propertyErrors);
            }

            return true;
        }

        private bool TryValidateProperty(PropertyInfo propertyInfo, List<string> propertyErrors)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = propertyInfo.Name };
            object propertyValue = propertyInfo.GetValue(this);

            // Validate the property
            bool isValid = Validator.TryValidateProperty(propertyValue, context, results);

            if (results.Any())
            {
                propertyErrors.AddRange(results.Select(c => c.ErrorMessage));
            }

            return isValid;
        }
    }
}
