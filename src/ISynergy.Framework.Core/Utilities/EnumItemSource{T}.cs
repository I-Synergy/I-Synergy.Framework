using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.Utilities
{
    /// <summary>
    /// Class EnumItemSource.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumItemSource<T>
        where T : struct, IConvertible
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; }
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; }
        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>The display.</value>
        public string Display { get; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumItemSource{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public EnumItemSource(string key, T value)
        {
            Argument.IsNotEnum(nameof(value), value);

            Key = key;
            Value = value;
            Display = ServiceLocator.Default.GetInstance<ILanguageService>().GetString(value.ToString());
        }
    }
}
