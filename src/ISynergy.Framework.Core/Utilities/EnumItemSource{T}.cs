using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.Utilities
{
    public class EnumItemSource<T>
        where T : struct, IConvertible
    {
        public Type Type { get; }
        public string Key { get; }
        public string Display { get; }
        public T Value { get; }

        public EnumItemSource(string key, T value)
        {
            Argument.IsNotEnum(nameof(value), value);

            Key = key;
            Value = value;
            Display = ServiceLocator.Default.GetInstance<ILanguageService>().GetString(value.ToString());
        }
    }
}
