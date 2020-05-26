using ISynergy.Framework.Core.Data;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Behaviors
{
    [TypeConstraint(typeof(FrameworkElement))]
    public class ValidationBehavior : DependencyObject, IBehavior
    {
        public DependencyObject AssociatedObject { get; private set; }
        FrameworkElement Control => AssociatedObject as FrameworkElement;

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            Control.DataContextChanged += Control_DataContextChanged;

            if (Control.DataContext != null)
            {
                Setup();
            }
        }

        public void Detach() => TearDown();

        private void Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) => Setup();

        private void Setup()
        {
            TearDown();
            GetProperty().PropertyChanged += Property_PropertyChanged;
            ExecuteActions();
        }

        private void TearDown()
        {
            if(GetProperty(false) != null)
            {
                GetProperty(false).PropertyChanged -= Property_PropertyChanged;
            }

            if(Control != null)
            {
                Control.DataContextChanged -= Control_DataContextChanged;
            }
        }

        private void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ExecuteActions();
        }

        private void ExecuteActions()
        {
            if (GetProperty().IsValid)
            {
                Interaction.ExecuteActions(AssociatedObject, WhenValidActions, null);
            }
            else
            {
                Interaction.ExecuteActions(AssociatedObject, WhenInvalidActions, null);
            }
        }

        IProperty _property;
        IProperty GetProperty(bool throwException = true)
        {
            if (_property != null)
                return _property;
            var context = (AssociatedObject as FrameworkElement)?.DataContext;
            if (context == null)
                if (throwException) throw new NullReferenceException("AssociatedObject is not valid");
                else return null;
            if (string.IsNullOrEmpty(PropertyName))
                if (throwException) throw new NullReferenceException("PropertyName is not set");
                else return null;
            if (!(context is IObservableClass model))
                if (throwException) throw new NullReferenceException("DataContext is not IModel");
                else return null;
            if (!model.Properties.ContainsKey(PropertyName))
                if (throwException) throw new KeyNotFoundException("PropertyName is not found");
                else return null;
            try
            {
                return _property = model.Properties[PropertyName];
            }
            catch
            {
                if (throwException) throw;
                else return null;
            }
        }

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string),
                typeof(ValidationBehavior), new PropertyMetadata(string.Empty, PropertyNameChanged));

        private static void PropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (!string.IsNullOrEmpty(e.OldValue?.ToString() ?? string.Empty))
                {
                    throw new InvalidOperationException("PropertyName cannot be changed once set.");
                }
            }
        }

        public ActionCollection WhenValidActions
        {
            get
            {
                if (!(GetValue(WhenValidActionsProperty) is ActionCollection actions))
                {
                    SetValue(WhenValidActionsProperty, actions = new ActionCollection());
                }
                return actions;
            }
        }
        public static readonly DependencyProperty WhenValidActionsProperty =
            DependencyProperty.Register(nameof(WhenValidActions), typeof(ActionCollection),
                typeof(ValidationBehavior), new PropertyMetadata(null));

        public ActionCollection WhenInvalidActions
        {
            get
            {
                if (!(GetValue(WhenInvalidActionsProperty) is ActionCollection actions))
                {
                    SetValue(WhenInvalidActionsProperty, actions = new ActionCollection());
                }
                return actions;
            }
        }
        public static readonly DependencyProperty WhenInvalidActionsProperty =
            DependencyProperty.Register(nameof(WhenInvalidActions), typeof(ActionCollection),
                typeof(ValidationBehavior), new PropertyMetadata(null));
    }
}
