using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace ISynergy.Services
{
    public interface IValidationService
    {
        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        bool HasErrors { get; }

        IEnumerable GetErrors(string propertyName);
        void SetErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, IEnumerable<string> errors);
        Dictionary<string, List<string>> GetAllErrors();
        List<string> GetErrorList();
        bool ValidateProperties();
        bool ValidateProperties(Type property);
        bool ValidateProperty(string propertyName);
    }
}
