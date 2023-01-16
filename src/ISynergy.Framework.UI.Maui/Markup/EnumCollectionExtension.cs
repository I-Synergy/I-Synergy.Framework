using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Markup
{
    [Bindable(BindableSupport.Yes)]
    [ContentProperty(nameof(EnumType))]
    public class EnumCollectionExtension : IMarkupExtension<List<KeyValuePair<int, string>>>
    {
        public Type EnumType { get; set; }

        public EnumCollectionExtension() 
            : base()
        { 
        }

        public EnumCollectionExtension(Type enumType)
            : this()
        {
            EnumType = enumType;
        }

        public List<KeyValuePair<int, string>> ProvideValue(IServiceProvider serviceProvider)
        {
            Argument.IsNotNull(EnumType);

            var list = new List<KeyValuePair<int, string>>();

            if (EnumType.IsEnum)
            {
                foreach (Enum item in Enum.GetValues(EnumType))
                {
                    list.Add(new KeyValuePair<int, string>(System.Convert.ToInt32(item), GetDescription(item)));
                }
            }

            return list;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            (this as IMarkupExtension<List<KeyValuePair<int, string>>>).ProvideValue(serviceProvider);

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentNullException">value</exception>
        private static string GetDescription(Enum value)
        {
            Argument.IsNotNull(value);
            return ServiceLocator.Default.GetInstance<ILanguageService>().GetString(value.ToString());
        }
    }
}
