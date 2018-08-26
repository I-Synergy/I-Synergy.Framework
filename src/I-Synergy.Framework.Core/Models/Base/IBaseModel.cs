using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ISynergy.Models.Base
{
    public interface IBaseModel
    {
        bool HasErrors { get; }

        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        event PropertyChangedEventHandler PropertyChanged;

        Dictionary<string, List<string>> GetAllErrors();
        IEnumerable GetErrors(string propertyName);
        void SetErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, IEnumerable<string> errors);
        bool ValidateProperties();
        bool ValidateProperty(string propertyName);
    }
}