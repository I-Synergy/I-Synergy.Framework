using System;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Controls;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.UI.Controls;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class BladeItem.
    /// Implements the <see cref="Expander" />
    /// </summary>
    /// <seealso cref="Expander" />
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

#if NETFX_CORE || (NET5_0 && WINDOWS)
        /// <summary>
        /// On expanded method.
        /// </summary>
        /// <param name="args"></param>
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

        /// <summary>
        /// On collapsed method.
        /// </summary>
        /// <param name="args"></param>
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
#endif

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

            if (_closeButton is null)
            {
                return;
            }

            _closeButton.Click -= CloseButton_Click;
            _closeButton.Click += CloseButton_Click;

            if (_enlargeButton is null)
            {
                return;
            }

            _enlargeButton.Click -= EnlargeButton_Click;
            _enlargeButton.Click += EnlargeButton_Click;
        }

        /// <summary>
        /// Handles the <see cref="E:SizeChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="sizeChangedEventArgs">The <see cref="SizeChangedEventArgs" /> instance containing the event data.</param>
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
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        /// <summary>
        /// Handles the Click event of the EnlargeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void EnlargeButton_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }
    }
}
