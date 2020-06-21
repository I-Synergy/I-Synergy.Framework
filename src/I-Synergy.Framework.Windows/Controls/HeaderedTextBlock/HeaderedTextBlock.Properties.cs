﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Defines the properties for the <see cref="HeaderedTextBlock" /> control.
    /// </summary>
    public partial class HeaderedTextBlock
    {
        /// <summary>
        /// Defines the <see cref="HeaderTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            nameof(HeaderTemplate),
            typeof(DataTemplate),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null));

        /// <summary>
        /// Defines the <see cref="TextStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register(
            nameof(TextStyle),
            typeof(Style),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null));

        /// <summary>
        /// Defines the <see cref="Header" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null, (d, _) => { ((HeaderedTextBlock)d).UpdateVisibility(); }));

        /// <summary>
        /// Defines the <see cref="Text" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null, (d, _) => { ((HeaderedTextBlock)d).UpdateVisibility(); }));

        /// <summary>
        /// Defines the <see cref="Orientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(Orientation.Vertical, (d, e) => { ((HeaderedTextBlock)d).UpdateForOrientation((Orientation)e.NewValue); }));

        /// <summary>
        /// Defines the <see cref="HideTextIfEmpty" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HideTextIfEmptyProperty = DependencyProperty.Register(
            nameof(HideTextIfEmpty),
            typeof(bool),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(false, (d, _) => { ((HeaderedTextBlock)d).UpdateVisibility(); }));

        /// <summary>
        /// Gets or sets the header style.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text style.
        /// </summary>
        /// <value>The text style.</value>
        public Style TextStyle
        {
            get
            {
                return (Style)GetValue(TextStyleProperty);
            }

            set
            {
                SetValue(TextStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header
        {
            get
            {
                return (string)GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of inline text elements within a Windows.UI.Xaml.Controls.TextBlock.
        /// </summary>
        /// <value>The inlines.</value>
        public InlineCollectionWrapper Inlines { get; } = new InlineCollectionWrapper();

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Text TextBlock is hidden if its value is empty
        /// </summary>
        /// <value><c>true</c> if [hide text if empty]; otherwise, <c>false</c>.</value>
        public bool HideTextIfEmpty
        {
            get
            {
                return (bool)GetValue(HideTextIfEmptyProperty);
            }

            set
            {
                SetValue(HideTextIfEmptyProperty, value);
            }
        }
    }
}