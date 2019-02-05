using ISynergy.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ISynergy.Services
{
    public class ValidationService : IValidationService
    {
        private ErrorsContainer<string> _errorsContainer;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        public ValidationService()
        {
            _errorsContainer = new ErrorsContainer<string>(pn => RaiseErrorsChanged(pn));
        }
        
        public IEnumerable GetErrors(string propertyName) =>
            _errorsContainer.GetErrors(propertyName);

        public void SetErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, IEnumerable<string> errors) =>
            _errorsContainer.SetErrors(propertyExpression, errors);

        public bool HasErrors { get { return _errorsContainer.HasErrors; } }

        public Dictionary<string, List<string>> GetAllErrors() => _errorsContainer.GetAllErrors();

        public List<string> GetErrorList()
        {
            List<string> errors = new List<string>();
            Dictionary<string, List<string>> allErrors = _errorsContainer.GetAllErrors();

            foreach (string propertyName in allErrors.Keys)
            {
                foreach (string errorString in allErrors[propertyName].EnsureNotNull())
                {
                    errors.Add(propertyName + ": " + errorString);
                }
            }
            return errors;
        }

        private void RaiseErrorsChanged(string propertyName) =>
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));

        public bool ValidateProperty(string propertyName)
        {
            Argument.IsNotNullOrEmpty(propertyName, propertyName);

            var propertyInfo = GetType().GetRuntimeProperty(propertyName);

            if (propertyInfo is null)
                throw new ArgumentException("Invalid property name", propertyName);

            var propertyErrors = new List<string>();

            bool isValid = TryValidateProperty(propertyInfo, propertyErrors);

            _errorsContainer.SetErrors(propertyInfo.Name, propertyErrors);

            return isValid;
        }

        public bool ValidateProperties() => ValidateProperties(GetType());

        public bool ValidateProperties(Type property)
        {
            var propertiesWithChangedErrors = new List<string>();

            // Get all the properties decorated with the ValidationAttribute attribute.
            var propertiesToValidate = property.GetType().GetRuntimeProperties().Where(c => c.GetCustomAttributes(typeof(ValidationAttribute)).Any());

            foreach (PropertyInfo propertyInfo in propertiesToValidate.EnsureNotNull())
            {
                var propertyErrors = new List<string>();
                TryValidateProperty(propertyInfo, propertyErrors);

                // If the errors have changed, save the property name to notify the update at the end of this method.
                _errorsContainer.SetErrors(propertyInfo.Name, propertyErrors);
            }

            return _errorsContainer.HasErrors;
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
