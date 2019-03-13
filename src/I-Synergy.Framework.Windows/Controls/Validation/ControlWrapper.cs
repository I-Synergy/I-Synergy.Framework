using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Controls
{
    public sealed class ControlWrapper : ContentControl
    {
        public ControlWrapper()
        {
            DefaultStyleKey = typeof(ControlWrapper);
            DataContextChanged += (s, e) => Property = GetProperty(e.NewValue);
        }

        IProperty GetProperty(object context)
        {
            if (context == null)
                return null;
            if (string.IsNullOrEmpty(PropertyName))
                throw new NullReferenceException("PropertyName not set");

            if (!(context is IObservableClass model))
                throw new NullReferenceException("DataContext not IModel");

            if (!model.Properties.ContainsKey(PropertyName))
                throw new KeyNotFoundException("PropertyName not found");

            return model.Properties[PropertyName];
        }

        public IProperty Property
        {
            get { return (IProperty)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register(nameof(Property), typeof(IProperty),
                typeof(ControlWrapper), new PropertyMetadata(null));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string),
                typeof(ControlWrapper), new PropertyMetadata(string.Empty));
    }
}
