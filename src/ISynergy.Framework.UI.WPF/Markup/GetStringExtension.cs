using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System.Windows.Data;
using System.Windows.Markup;

namespace ISynergy.Framework.UI.Markup
{
    [MarkupExtensionReturnType(typeof(string))]
    public class GetStringExtension : MarkupExtension
    {
        [ConstructorArgument("key")]
        public string Key { get; protected set; }

        public GetStringExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(Key))
            {
                result = ServiceLocator.Default.GetInstance<ILanguageService>().GetString(Key);
            }
            else
            {
                result = $"[{Key}]";
            }

            var binding = new Binding()
            {
                Source = result,
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}
