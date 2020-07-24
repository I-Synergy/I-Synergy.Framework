using System;
using System.Collections.Generic;
using ISynergy.Framework.Core.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Class ControlWrapper. This class cannot be inherited.
    /// Implements the <see cref="Windows.UI.Xaml.Controls.ContentControl" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.ContentControl" />
    public sealed class ControlWrapper : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlWrapper"/> class.
        /// </summary>
        public ControlWrapper()
        {
            DefaultStyleKey = typeof(ControlWrapper);
            DataContextChanged += (s, e) => Property = GetProperty(e.NewValue);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>IProperty.</returns>
        /// <exception cref="NullReferenceException">PropertyName not set</exception>
        /// <exception cref="NullReferenceException">DataContext not IModel</exception>
        /// <exception cref="KeyNotFoundException">PropertyName not found</exception>
        IProperty GetProperty(object context)
        {
            if (context is null)
                return null;
            if (string.IsNullOrEmpty(PropertyName))
                throw new NullReferenceException("PropertyName not set");

            if (!(context is IObservableClass model))
                throw new NullReferenceException("DataContext not IModel");

            if (!model.Properties.ContainsKey(PropertyName))
                throw new KeyNotFoundException("PropertyName not found");

            return model.Properties[PropertyName];
        }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public IProperty Property
        {
            get { return (IProperty)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        /// <summary>
        /// The property property
        /// </summary>
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register(nameof(Property), typeof(IProperty),
                typeof(ControlWrapper), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        /// <summary>
        /// The property name property
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string),
                typeof(ControlWrapper), new PropertyMetadata(string.Empty));
    }
}
