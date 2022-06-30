using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ISynergy.Framework.UI.Markup
{
    /// <summary>
    /// Class LanguageResource.
    /// Implements the <see cref="MarkupExtension" />
    /// </summary>
    /// <seealso cref="MarkupExtension" />
    public partial class LanguageResource : MarkupExtension
    {
        [ConstructorArgument("key")]
        public string ResourceKey { get; protected set; }

        public LanguageResource(string key)
        {
            ResourceKey = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding()
            {
                Source = GetResource(),
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }

        private object GetResource()
        {
            var resource = "[Empty]";

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return ResourceKey;
            }
            else if (!string.IsNullOrEmpty(ResourceKey) && ServiceLocator.Default.GetInstance<ILanguageService>() is ILanguageService languageService)
            {
                return languageService.GetString(ResourceKey);
            }

            return resource;
        }
    }
}
