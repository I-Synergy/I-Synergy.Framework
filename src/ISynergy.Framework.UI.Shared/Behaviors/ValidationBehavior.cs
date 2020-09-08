using ISynergy.Framework.Core.Data;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace ISynergy.Framework.UI.Behaviors
{
    /// <summary>
    /// Class ValidationBehavior.
    /// Implements the <see cref="DependencyObject" />
    /// Implements the <see cref="IBehavior" />
    /// </summary>
    /// <seealso cref="DependencyObject" />
    /// <seealso cref="IBehavior" />
    [TypeConstraint(typeof(FrameworkElement))]
    public partial class ValidationBehavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> is attached.
        /// </summary>
        /// <value>The associated object.</value>
        public DependencyObject AssociatedObject { get; private set; }
        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <value>The control.</value>
        FrameworkElement Control => AssociatedObject as FrameworkElement;

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            Control.DataContextChanged += Control_DataContextChanged;

            if (Control.DataContext != null)
            {
                Setup();
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach() => TearDown();

        /// <summary>
        /// Controls the data context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DataContextChangedEventArgs"/> instance containing the event data.</param>
        private void Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) => Setup();

        /// <summary>
        /// Setups this instance.
        /// </summary>
        private void Setup()
        {
            TearDown();
            GetProperty().PropertyChanged += Property_PropertyChanged;
            ExecuteActions();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
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

        /// <summary>
        /// Handles the PropertyChanged event of the Property control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ExecuteActions();
        }

        /// <summary>
        /// Executes the actions.
        /// </summary>
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

        /// <summary>
        /// The property
        /// </summary>
        IProperty _property;
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns>IProperty.</returns>
        /// <exception cref="NullReferenceException">AssociatedObject is not valid</exception>
        /// <exception cref="NullReferenceException">PropertyName is not set</exception>
        /// <exception cref="NullReferenceException">DataContext is not IModel</exception>
        /// <exception cref="KeyNotFoundException">PropertyName is not found</exception>
        IProperty GetProperty(bool throwException = true)
        {
            if (_property != null)
                return _property;
            var context = (AssociatedObject as FrameworkElement)?.DataContext;
            if (context is null)
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
                typeof(ValidationBehavior), new PropertyMetadata(string.Empty, PropertyNameChanged));

        /// <summary>
        /// Properties the name changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">PropertyName cannot be changed once set.</exception>
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

        /// <summary>
        /// Gets the when valid actions.
        /// </summary>
        /// <value>The when valid actions.</value>
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
        /// <summary>
        /// The when valid actions property
        /// </summary>
        public static readonly DependencyProperty WhenValidActionsProperty =
            DependencyProperty.Register(nameof(WhenValidActions), typeof(ActionCollection),
                typeof(ValidationBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Gets the when invalid actions.
        /// </summary>
        /// <value>The when invalid actions.</value>
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
        /// <summary>
        /// The when invalid actions property
        /// </summary>
        public static readonly DependencyProperty WhenInvalidActionsProperty =
            DependencyProperty.Register(nameof(WhenInvalidActions), typeof(ActionCollection),
                typeof(ValidationBehavior), new PropertyMetadata(null));
    }
}
