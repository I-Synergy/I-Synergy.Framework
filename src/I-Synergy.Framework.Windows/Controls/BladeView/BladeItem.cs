using System;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// The Blade is used as a child in the BladeView
    /// </summary>
    [TemplatePart(Name = "CloseButton", Type = typeof(Button))]
    [TemplatePart(Name = "EnlargeButton", Type = typeof(Button))]
    public partial class BladeItem : Expander
    {
        /// <summary>
        /// The close button
        /// </summary>
        private Button _closeButton;
        /// <summary>
        /// The enlarge button
        /// </summary>
        private Button _enlargeButton;
        /// <summary>
        /// The normal mode width
        /// </summary>
        private double _normalModeWidth;
        /// <summary>
        /// The loaded
        /// </summary>
        private bool _loaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BladeItem" /> class.
        /// </summary>
        public BladeItem()
        {
            DefaultStyleKey = typeof(BladeItem);

            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Override default OnApplyTemplate to capture child controls
        /// </summary>
        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            _loaded = true;
            base.OnApplyTemplate();

            _closeButton = GetTemplateChild("CloseButton") as Button;
            _enlargeButton = GetTemplateChild("EnlargeButton") as Button;

            if (_closeButton == null)
            {
                return;
            }

            _closeButton.Click -= CloseButton_Click;
            _closeButton.Click += CloseButton_Click;

            if (_enlargeButton == null)
            {
                return;
            }

            _enlargeButton.Click -= EnlargeButton_Click;
            _enlargeButton.Click += EnlargeButton_Click;
        }

        /// <inheritdoc/>
        protected override void OnExpanded(EventArgs args)
        {
            base.OnExpanded(args);
            if (_loaded)
            {
                Width = _normalModeWidth;
                VisualStateManager.GoToState(this, "Expanded", true);
                var name = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Expanded");
                if (_enlargeButton != null)
                {
                    AutomationProperties.SetName(_enlargeButton, name);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnCollapsed(EventArgs args)
        {
            base.OnCollapsed(args);
            if (_loaded)
            {
                Width = double.NaN;
                VisualStateManager.GoToState(this, "Collapsed", true);
                var name = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Collapsed");
                if (_enlargeButton != null)
                {
                    AutomationProperties.SetName(_enlargeButton, name);
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:SizeChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="sizeChangedEventArgs">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (IsExpanded)
            {
                _normalModeWidth = Width;
            }
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        /// <summary>
        /// Handles the Click event of the EnlargeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EnlargeButton_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }
    }
}
