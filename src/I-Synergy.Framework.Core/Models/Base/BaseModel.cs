using ISynergy.Extensions;
using Newtonsoft.Json;
using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ISynergy.Models.Base
{
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class BaseModel : ViewModelBase, INotifyPropertyChanged, INotifyDataErrorInfo, IBaseModel
    {
        /// <summary>
        /// Gets or sets the Memo property value.
        /// </summary>
        public string Memo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        
        /// <summary>
        /// Gets or sets the InputFirst property value.
        /// </summary>
        public string InputFirst
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the InputLast property value.
        /// </summary>
        public string InputLast
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

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

        private ErrorsContainer<string> _errorsContainer;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        public IEnumerable GetErrors(string propertyName)
        {
            return ErrorsContainer.GetErrors(propertyName);
        }

        public void SetErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, IEnumerable<string> errors)
        {
            ErrorsContainer.SetErrors(propertyExpression, errors);
        }

        public virtual bool HasErrors
        {
            get { return ErrorsContainer.HasErrors; }
        }

        public Dictionary<string, List<string>> GetAllErrors()
        {
            return ErrorsContainer.GetAllErrors();
        }

        protected ErrorsContainer<string> ErrorsContainer
        {
            get
            {
                if (_errorsContainer is null)
                {
                    _errorsContainer =
                        new ErrorsContainer<string>(pn => this.RaiseErrorsChanged(pn));
                }

                return _errorsContainer;
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
        }

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged(this, e);
        }

        public bool ValidateProperty(string propertyName)
        {
            Argument.IsNotNullOrEmpty(propertyName, propertyName);

            var propertyInfo = this.GetType().GetRuntimeProperty(propertyName);

            if (propertyInfo is null)
                throw new ArgumentException("Invalid property name", propertyName);

            var propertyErrors = new List<string>();

            bool isValid = TryValidateProperty(propertyInfo, propertyErrors);

            ErrorsContainer.SetErrors(propertyInfo.Name, propertyErrors);

            return isValid;
        }

        public bool ValidateProperties()
        {
            var propertiesWithChangedErrors = new List<string>();

            // Get all the properties decorated with the ValidationAttribute attribute.
            var propertiesToValidate = this.GetType().GetRuntimeProperties().Where(c => c.GetCustomAttributes(typeof(ValidationAttribute)).Any());

            foreach (PropertyInfo propertyInfo in propertiesToValidate.EnsureNotNull())
            {
                var propertyErrors = new List<string>();
                TryValidateProperty(propertyInfo, propertyErrors);

                // If the errors have changed, save the property name to notify the update at the end of this method.
                ErrorsContainer.SetErrors(propertyInfo.Name, propertyErrors);
            }

            return ErrorsContainer.HasErrors;
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