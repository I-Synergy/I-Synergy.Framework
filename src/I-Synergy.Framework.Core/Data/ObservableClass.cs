using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ISynergy.Data
{
    public abstract class ObservableClass : ObservableObject, IDisposable
    {
        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (_propertyBackingDictionary.TryGetValue(propertyName, out object value)) return (T)value;

            return default;
        }

        protected bool SetValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (EqualityComparer<T>.Default.Equals(newValue, GetValue<T>(propertyName))) return false;

            _propertyBackingDictionary[propertyName] = newValue;

            RaisePropertyChanged(propertyName);

            if (!string.IsNullOrEmpty(propertyName)) ValidateProperty(propertyName);

            return true;
        }

        private bool ValidateProperty(string propertyName)
        {
            Argument.IsNotNullOrEmpty(propertyName, propertyName);

            var propertyInfo = GetType().GetRuntimeProperty(propertyName);

            if (propertyInfo is null)
                throw new ArgumentException("Invalid property name", propertyName);

            var propertyErrors = new List<string>();

            return TryValidateProperty(propertyInfo, propertyErrors);
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

        public void Dispose()
        {
            PropertyChanged -= PropertyChangedHandler;
        }
    }
}
