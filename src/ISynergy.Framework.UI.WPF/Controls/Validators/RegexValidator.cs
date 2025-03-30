using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.UI.Extensions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Controls.Validators;

/// <summary>
/// TextBox extension for regex input validation.
/// </summary>
public static class RegexValidator
{
    private static Control _control;

    /// <summary>
    /// Regex validation mode.
    /// </summary>
    public enum ValidationMode
    {
        /// <summary>
        /// Update <see cref="IsValidProperty"/> with validation result at text changed.
        /// </summary>
        Normal,

        /// <summary>
        /// Update <see cref="IsValidProperty"/> with validation result and in case the textbox is not valid clear its value when the TextBox lose focus
        /// </summary>
        Forced,

        /// <summary>
        /// Update <see cref="IsValidProperty"/> with validation result at text changed and clear the newest character at input which is not valid
        /// </summary>
        Dynamic
    }

    /// <summary>
    /// Specify the type of validation required
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// The default validation that required property Regex to be setted
        /// </summary>
        Custom,

        /// <summary>
        /// Integer validation
        /// </summary>
        Integer,

        /// <summary>
        /// Decimal validation
        /// </summary>
        Decimal,

        /// <summary>
        /// Alphanumeric only validation
        /// </summary>
        Alphanumeric
    }

    /// <summary>
    /// Identifies the Regex attached dependency property.
    /// </summary>
    public static readonly DependencyProperty RegexProperty = DependencyProperty.RegisterAttached("Regex", typeof(string), typeof(RegexValidator), new PropertyMetadata(null, RegexPropertyOnChange));

    /// <summary>
    /// Identifies the IsValid attached dependency property.
    /// </summary>
    public static readonly DependencyProperty IsValidProperty = DependencyProperty.RegisterAttached("IsValid", typeof(bool), typeof(RegexValidator), new PropertyMetadata(false));

    /// <summary>
    /// Identifies the ValidationMode attached dependency property.
    /// </summary>
    public static readonly DependencyProperty ValidationModeProperty = DependencyProperty.RegisterAttached("ValidationMode", typeof(ValidationMode), typeof(RegexValidator), new PropertyMetadata(ValidationMode.Normal, RegexPropertyOnChange));

    /// <summary>
    /// Identifies the ValidationType attached dependency property.
    /// </summary>
    public static readonly DependencyProperty ValidationTypeProperty = DependencyProperty.RegisterAttached("ValidationType", typeof(ValidationType), typeof(RegexValidator), new PropertyMetadata(ValidationType.Custom, RegexPropertyOnChange));

    /// <summary>
    /// Gets the value of the TextBoxRegex.Regex XAML attached property from the specified TextBox.
    /// </summary>
    /// <param name="obj">TextBox to get Regex property from.</param>
    /// <returns>The regular expression assigned to the TextBox</returns>
    public static string GetRegex(DependencyObject obj)
    {
        return (string)obj.GetValue(RegexProperty);
    }

    /// <summary>
    /// Sets the value of the TextBoxRegex.Regex XAML attached property for a target TextBox.
    /// </summary>
    /// <param name="obj">The TextBox to set the regular expression on</param>
    /// <param name="value">Regex value</param>
    public static void SetRegex(DependencyObject obj, string value)
    {
        obj.SetValue(RegexProperty, value);
    }

    /// <summary>
    /// Gets the value of the TextBoxRegex.IsValid XAML attached property from the specified TextBox.
    /// </summary>
    /// <param name="obj">TextBox to be validated.</param>
    /// <returns>TextBox regular expression validation result</returns>
    public static bool GetIsValid(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsValidProperty);
    }

    /// <summary>
    /// Sets the value of the TextBoxRegex.IsValid XAML attached property for a target TextBox.
    /// </summary>
    /// <param name="obj">TextBox to be assigned the property</param>
    /// <param name="value">A value indicating if the Text is valid according to the Regex property.</param>
    public static void SetIsValid(DependencyObject obj, bool value)
    {
        obj.SetValue(IsValidProperty, value);
    }

    /// <summary>
    /// Gets the value of the TextBoxRegex.ValidationMode XAML attached property from the specified TextBox.
    /// </summary>
    /// <param name="obj">TextBox to get the <see cref="ValidationMode"/> from</param>
    /// <returns>TextBox <see cref="ValidationMode"/> value</returns>
    public static ValidationMode GetValidationMode(DependencyObject obj)
    {
        return (ValidationMode)obj.GetValue(ValidationModeProperty);
    }

    /// <summary>
    /// Sets the value of the TextBoxRegex.ValidationMode XAML attached property for a target TextBox.
    /// </summary>
    /// <param name="obj">TextBox to set the <see cref="ValidationMode"/> on.</param>
    /// <param name="value">TextBox <see cref="ValidationMode"/> value</param>
    public static void SetValidationMode(DependencyObject obj, ValidationMode value)
    {
        obj.SetValue(ValidationModeProperty, value);
    }

    /// <summary>
    /// Gets the value of the TextBoxRegex.ValidationType XAML attached property from the specified TextBox.
    /// </summary>
    /// <param name="obj">TextBox to get <see cref="ValidationType"/> from.</param>
    /// <returns>TextBox <see cref="ValidationType"/> Value</returns>
    public static ValidationType GetValidationType(DependencyObject obj)
    {
        return (ValidationType)obj.GetValue(ValidationTypeProperty);
    }

    /// <summary>
    /// Sets the value of the TextBoxRegex.ValidationType XAML attached property for a target TextBox.
    /// </summary>
    /// <param name="obj">TextBox to set the <see cref="ValidationType"/> on.</param>
    /// <param name="value">TextBox <see cref="ValidationType"/> value</param>
    public static void SetValidationType(DependencyObject obj, ValidationType value)
    {
        obj.SetValue(ValidationTypeProperty, value);
    }

    private static void RegexPropertyOnChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Control control && !control.IsLoaded)
            control.Loaded += Control_Loaded;
        else
            InitializeControl(sender);
    }

    private static void Control_Loaded(object sender, RoutedEventArgs e) =>
        InitializeControl(sender);

    private static void InitializeControl(object sender)
    {
        TextBox textBox = null;

        if (sender is TextBox parent)
        {
            _control = null;
            textBox = parent;
        }
        else if (sender is Control control)
        {
            _control = control;
            textBox = (sender as DependencyObject).FindChild<TextBox>();
        }

        if (textBox is null)
            return;

        ValidateTextBox(textBox, false);

        textBox.Loaded -= TextBox_Loaded;
        textBox.LostFocus -= TextBox_LostFocus;
        textBox.TextChanged -= TextBox_TextChanged;
        textBox.Loaded += TextBox_Loaded;
        textBox.LostFocus += TextBox_LostFocus;
        textBox.TextChanged += TextBox_TextChanged;
    }

    private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        ValidationMode validationMode = ValidationMode.Normal;

        if (_control is null)
            validationMode = (ValidationMode)textBox.GetValue(ValidationModeProperty);
        else
            validationMode = (ValidationMode)_control.GetValue(ValidationModeProperty);


        ValidateTextBox(textBox, validationMode == ValidationMode.Dynamic);
    }

    private static void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ValidateTextBox(textBox);
    }

    private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        ValidateTextBox(textBox);
    }

    private static void ValidateTextBox(TextBox textBox, bool force = true)
    {
        ValidationType validationType = ValidationType.Custom;

        if (_control is null)
            validationType = (ValidationType)textBox.GetValue(ValidationTypeProperty);
        else
            validationType = (ValidationType)_control.GetValue(ValidationTypeProperty);

        var regex = string.Empty;
        var regexMatch = false;

        switch (validationType)
        {
            default:
                if (_control is null)
                    regex = textBox.GetValue(RegexProperty) as string;
                else
                    regex = _control.GetValue(RegexProperty) as string;

                if (string.IsNullOrEmpty(regex))
                {
                    Debug.WriteLine("Regex property can't be null or empty when custom mode is selected");
                    return;
                }

                regexMatch = System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, regex, RegexOptions.None, TimeSpan.FromMilliseconds(100));
                break;
            case ValidationType.Decimal:
                regexMatch = textBox.Text.IsDecimal();
                break;
            case ValidationType.Integer:
                regexMatch = textBox.Text.IsInteger();
                break;
            case ValidationType.Alphanumeric:
                regexMatch = textBox.Text.IsAlphaNumeric();
                break;
        }

        if (!regexMatch && force)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                ValidationMode validationMode = ValidationMode.Normal;

                if (_control is null)
                    validationMode = (ValidationMode)textBox.GetValue(ValidationModeProperty);
                else
                    validationMode = (ValidationMode)_control.GetValue(ValidationModeProperty);

                if (validationMode == ValidationMode.Forced)
                {
                    textBox.Text = string.Empty;
                }
                else if (validationMode == ValidationMode.Dynamic)
                {
                    int selectionStart = textBox.SelectionStart == 0 ? textBox.SelectionStart : textBox.SelectionStart - 1;
                    textBox.Text = textBox.Text.Remove(selectionStart, 1);
                    textBox.SelectionStart = selectionStart;
                }
            }
        }

        if (_control is null)
            textBox.SetValue(IsValidProperty, regexMatch);
        else
            _control.SetValue(IsValidProperty, regexMatch);
    }
}
