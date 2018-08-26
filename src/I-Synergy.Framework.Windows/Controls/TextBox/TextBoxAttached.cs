﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Controls
{
    public static class TextBoxAttached
    {
        public static bool GetAutoSelectable(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSelectableProperty);
        }

        public static void SetAutoSelectable(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSelectableProperty, value);
        }

        public static readonly DependencyProperty AutoSelectableProperty =
            DependencyProperty.RegisterAttached(
                "AutoSelectable",
                typeof(bool),
                typeof(TextBoxAttached),
                new PropertyMetadata(false, AutoFocusableChangedHandler));

        private static void AutoFocusableChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue == true)
                {
                    (d as TextBox).GotFocus += OnGotFocusHandler;
                }
                else
                {
                    (d as TextBox).GotFocus -= OnGotFocusHandler;
                }
            }
        }

        private static void OnGotFocusHandler(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
