using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using ISynergy.Framework.UI.Extensions;
using System.Text;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.UI.Controls.Validators
{
    /// <summary>
    /// TextBox extension for Masked input validation.
    /// </summary>
    public static class MaskValidator
    {
        private static Control _control;

        private const string DefaultPlaceHolder = "_";
        private const char EscapeChar = '\\';

        private static readonly KeyValuePair<char, string> AlphaCharacterRepresentation = new KeyValuePair<char, string>('a', "[A-Za-z]");
        private static readonly KeyValuePair<char, string> NumericCharacterRepresentation = new KeyValuePair<char, string>('9', "[0-9]");
        private static readonly KeyValuePair<char, string> AlphaNumericRepresentation = new KeyValuePair<char, string>('*', "[A-Za-z0-9]");

        /// <summary>
        /// Represents a mask/format for the textBox that the user must follow
        /// </summary>
        public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask", typeof(string), typeof(MaskValidator), new PropertyMetadata(string.Empty, MaskPropertyOnChange));

        /// <summary>
        /// Represents the mask place holder which represents the variable character that the user can edit in the textBox
        /// </summary>
        public static readonly DependencyProperty MaskPlaceholderProperty = DependencyProperty.RegisterAttached("MaskPlaceholder", typeof(string), typeof(MaskValidator), new PropertyMetadata(DefaultPlaceHolder, MaskPropertyOnChange));

        /// <summary>
        /// Represents the custom mask that the user can create to add his own variable characters based on regex expression
        /// </summary>
        public static readonly DependencyProperty CustomMaskProperty = DependencyProperty.RegisterAttached("CustomMask", typeof(string), typeof(MaskValidator), new PropertyMetadata(string.Empty, MaskPropertyOnChange));

        private static readonly DependencyProperty RepresentationDictionaryProperty = DependencyProperty.RegisterAttached("RepresentationDictionary", typeof(Dictionary<char, string>), typeof(MaskValidator), new PropertyMetadata(null));
        private static readonly DependencyProperty OldTextProperty = DependencyProperty.RegisterAttached("OldText", typeof(string), typeof(MaskValidator), new PropertyMetadata(null));
        private static readonly DependencyProperty DefaultDisplayTextProperty = DependencyProperty.RegisterAttached("DefaultDisplayText", typeof(string), typeof(MaskValidator), new PropertyMetadata(null));
        private static readonly DependencyProperty OldSelectionLengthProperty = DependencyProperty.RegisterAttached("OldSelectionLength", typeof(int), typeof(MaskValidator), new PropertyMetadata(0));
        private static readonly DependencyProperty OldSelectionStartProperty = DependencyProperty.RegisterAttached("OldSelectionStart", typeof(int), typeof(MaskValidator), new PropertyMetadata(0));
        private static readonly DependencyProperty EscapedMaskProperty = DependencyProperty.RegisterAttached("EscapedMask", typeof(string), typeof(MaskValidator), new PropertyMetadata(null));
        private static readonly DependencyProperty EscapedCharacterIndicesProperty = DependencyProperty.RegisterAttached("MaskEscapedCharacters", typeof(List<int>), typeof(MaskValidator), new PropertyMetadata(null));

        /// <summary>
        /// Gets mask value
        /// </summary>
        /// <param name="obj">control</param>
        /// <returns>mask value</returns>
        public static string GetMask(DependencyObject obj)
        {
            return (string)obj.GetValue(MaskProperty);
        }

        /// <summary>
        /// Sets textBox mask property which represents mask/format for the textBox that the user must follow
        /// </summary>
        /// <param name="obj">Control</param>
        /// <param name="value">Mask Value</param>
        public static void SetMask(DependencyObject obj, string value)
        {
            obj.SetValue(MaskProperty, value);
        }

        /// <summary>
        /// Gets placeholder value
        /// </summary>
        /// <param name="obj">control</param>
        /// <returns>placeholder value</returns>
        public static string GetMaskPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(MaskPlaceholderProperty);
        }

        /// <summary>
        /// Sets placeholder property which represents the variable character that the user can edit in the textBox
        /// </summary>
        /// <param name="obj">Control</param>
        /// <param name="value">placeholder Value</param>
        public static void SetMaskPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(MaskPlaceholderProperty, value);
        }

        /// <summary>
        /// Gets CustomMask value
        /// </summary>
        /// <param name="obj">control</param>
        /// <returns>CustomMask value</returns>
        public static string GetCustomMask(DependencyObject obj)
        {
            return (string)obj.GetValue(CustomMaskProperty);
        }

        /// <summary>
        /// Sets CustomMask property which represents the custom mask that the user can create to add his own variable characters based on certain regex expression
        /// </summary>
        /// <param name="obj">Control</param>
        /// <param name="value">CustomMask Value</param>
        public static void SetCustomMask(DependencyObject obj, string value)
        {
            obj.SetValue(CustomMaskProperty, value);
        }

        private static void MaskPropertyOnChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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

            textBox.SelectionChanged -= Textbox_SelectionChanged;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.Loaded -= Textbox_Loaded;
            textBox.GotFocus -= Textbox_GotFocus;
            textBox.Loaded += Textbox_Loaded;

            Textbox_Loaded(textBox, null);
        }

        private static void Textbox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;

            // In case no value is provided, use it as normal textBox
            var mask = string.Empty;

            if (_control is null)
                mask = textBox.GetValue(MaskProperty).ToString();
            else
                mask = _control.GetValue(MaskProperty).ToString();

            if (string.IsNullOrEmpty(mask))
                return;

            var placeHolderValue = string.Empty;

            if (_control is null)
                placeHolderValue = textBox.GetValue(MaskPlaceholderProperty).ToString();
            else
                placeHolderValue = _control.GetValue(MaskPlaceholderProperty).ToString();

            Argument.IsNotNullOrEmpty(placeHolderValue);

            var escapedChars = new List<int>();

            var builder = new StringBuilder(mask);
            for (int i = 0; i < builder.Length - 1; i++)
            {
                if (builder[i] == EscapeChar)
                {
                    escapedChars.Add(i);
                    builder.Remove(i, 1);
                }
            }

            var escapedMask = builder.ToString();

            if (_control is null)
            {
                textBox.SetValue(EscapedCharacterIndicesProperty, escapedChars);
                textBox.SetValue(EscapedMaskProperty, escapedMask);
            }
            else
            {
                _control.SetValue(EscapedCharacterIndicesProperty, escapedChars);
                _control.SetValue(EscapedMaskProperty, escapedMask);
            }

            var placeHolder = placeHolderValue[0];

            var representationDictionary = new Dictionary<char, string>();
            representationDictionary.Add(AlphaCharacterRepresentation.Key, AlphaCharacterRepresentation.Value);
            representationDictionary.Add(NumericCharacterRepresentation.Key, NumericCharacterRepresentation.Value);
            representationDictionary.Add(AlphaNumericRepresentation.Key, AlphaNumericRepresentation.Value);

            var customDictionaryValue = string.Empty;

            if (_control is null)
                customDictionaryValue = textBox.GetValue(CustomMaskProperty).ToString();
            else
                customDictionaryValue = textBox.GetValue(CustomMaskProperty).ToString();

            if (!string.IsNullOrWhiteSpace(customDictionaryValue))
            {
                var customRoles = customDictionaryValue.Split(',');
                foreach (var role in customRoles)
                {
                    var roleValues = role.Split(':');
                    if (roleValues.Length != 2)
                    {
                        throw new ArgumentException("Invalid CustomMask property");
                    }

                    var keyValue = roleValues[0];
                    var value = roleValues[1];
                    char key;

                    // an exception should be throw if the regex is not valid
                    System.Text.RegularExpressions.Regex.Match(string.Empty, value);
                    if (!char.TryParse(keyValue, out key))
                    {
                        throw new ArgumentException("Invalid CustomMask property, please validate the mask key");
                    }

                    representationDictionary.Add(key, value);
                }
            }

            if (_control is null)
                textBox.SetValue(RepresentationDictionaryProperty, representationDictionary);
            else
                _control.SetValue(RepresentationDictionaryProperty, representationDictionary);


            var displayTextBuilder = new StringBuilder(escapedMask);

            for (int i = 0; i < displayTextBuilder.Length; i++)
            {
                if (escapedChars.Contains(i))
                {
                    continue;
                }

                foreach (var key in representationDictionary.Keys)
                {
                    if (displayTextBuilder[i] == key)
                    {
                        displayTextBuilder[i] = placeHolder;
                    }
                }
            }

            var displayText = displayTextBuilder.ToString();

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = displayText;
            }
            else
            {
                var textboxInitialValue = textBox.Text;
                textBox.Text = displayText;

                int oldSelectionStart;

                if (_control is null)
                    oldSelectionStart = (int)textBox.GetValue(OldSelectionStartProperty);
                else
                    oldSelectionStart = (int)_control.GetValue(OldSelectionStartProperty);


                SetTextBoxValue(textboxInitialValue, textBox, escapedMask, escapedChars, representationDictionary, placeHolder, oldSelectionStart);
            }


            textBox.SelectionChanged += Textbox_SelectionChanged;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.GotFocus += Textbox_GotFocus;

            if (_control is null)
            {
                textBox.SetValue(OldTextProperty, textBox.Text);
                textBox.SetValue(DefaultDisplayTextProperty, displayText);
            }
            else
            {
                _control.SetValue(OldTextProperty, textBox.Text);
                _control.SetValue(DefaultDisplayTextProperty, displayText);
            }

            textBox.SelectionStart = 0;
        }

        private static void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;

            var mask = string.Empty;
            var placeHolderValue = string.Empty;
            Dictionary<char, string> representationDictionary = null;

            if (_control is null)
            {
                mask = textBox.GetValue(MaskProperty) as string;
                placeHolderValue = textBox.GetValue(MaskPlaceholderProperty) as string;
                representationDictionary = textBox.GetValue(RepresentationDictionaryProperty) as Dictionary<char, string>;
            }
            else
            {
                mask = _control.GetValue(MaskProperty) as string;
                placeHolderValue = _control.GetValue(MaskPlaceholderProperty) as string;
                representationDictionary = _control.GetValue(RepresentationDictionaryProperty) as Dictionary<char, string>;
            }

            if (string.IsNullOrWhiteSpace(mask) ||
                representationDictionary == null ||
                string.IsNullOrEmpty(placeHolderValue))
            {
                return;
            }

            var placeHolder = placeHolderValue[0];

            // if the textBox got focus and the textBox is empty (contains only mask) set the textBox cursor at the beginning to simulate normal TextBox behavior if it is empty.
            // if the textBox has value set the cursor to the first empty mask character
            var textboxText = textBox.Text;
            for (int i = 0; i < textboxText.Length; i++)
            {
                if (placeHolder == textboxText[i])
                {
                    textBox.SelectionStart = i;
                    break;
                }
            }
        }

        private static void SetTextBoxValue(
            string newValue,
            TextBox textBox,
            string mask,
            List<int> escapedChars,
            Dictionary<char, string> representationDictionary,
            char placeholder,
            int oldSelectionStart)
        {
            var maxLength = (newValue.Length + oldSelectionStart) < mask.Length ? (newValue.Length + oldSelectionStart) : mask.Length;
            var textArray = textBox.Text.ToCharArray();

            for (int i = oldSelectionStart; i < maxLength; i++)
            {
                var maskChar = mask[i];
                var selectedChar = newValue[i - oldSelectionStart];

                // If dynamic character a,9,* or custom
                if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(i))
                {
                    var pattern = representationDictionary[maskChar];
                    if (System.Text.RegularExpressions.Regex.IsMatch(selectedChar.ToString(), pattern))
                    {
                        textArray[i] = selectedChar;
                    }
                    else
                    {
                        textArray[i] = placeholder;
                    }
                }
            }

            textBox.Text = new string(textArray);
            textBox.SelectionStart = maxLength;
        }

        private static void Textbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;

            if (_control is null)
            {
                textBox.SetValue(OldSelectionStartProperty, textBox.SelectionStart);
                textBox.SetValue(OldSelectionLengthProperty, textBox.SelectionLength);
            }
            else
            {
                _control.SetValue(OldSelectionStartProperty, textBox.SelectionStart);
                _control.SetValue(OldSelectionLengthProperty, textBox.SelectionLength);
            }
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;

            var escapedMask = string.Empty;
            List<int> escapedChars = null;
            Dictionary<char, string> representationDictionary = null;
            var placeHolderValue = string.Empty;
            var oldText = string.Empty;
            int oldSelectionStart;
            int oldSelectionLength;

            if (_control is null)
            {
                escapedMask = textBox.GetValue(EscapedMaskProperty) as string;
                escapedChars = textBox.GetValue(EscapedCharacterIndicesProperty) as List<int>;
                representationDictionary = textBox.GetValue(RepresentationDictionaryProperty) as Dictionary<char, string>;
                placeHolderValue = textBox.GetValue(MaskPlaceholderProperty) as string;
                oldText = textBox.GetValue(OldTextProperty) as string;
                oldSelectionStart = (int)textBox.GetValue(OldSelectionStartProperty);
                oldSelectionLength = (int)textBox.GetValue(OldSelectionLengthProperty);
            }
            else
            {
                escapedMask = _control.GetValue(EscapedMaskProperty) as string;
                escapedChars = _control.GetValue(EscapedCharacterIndicesProperty) as List<int>;
                representationDictionary = _control.GetValue(RepresentationDictionaryProperty) as Dictionary<char, string>;
                placeHolderValue = _control.GetValue(MaskPlaceholderProperty) as string;
                oldText = _control.GetValue(OldTextProperty) as string;
                oldSelectionStart = (int)_control.GetValue(OldSelectionStartProperty);
                oldSelectionLength = (int)_control.GetValue(OldSelectionLengthProperty);
            }

            if (string.IsNullOrWhiteSpace(escapedMask) || representationDictionary == null || string.IsNullOrEmpty(placeHolderValue) || oldText == null)
                return;

            var placeHolder = placeHolderValue[0];
            var isDeleteOrBackspace = false;
            var deleteBackspaceIndex = 0;

            // Delete or backspace is triggered
            // if the new length is less than or equal the old text - the old selection length then a delete or backspace is triggered with or without selection and no characters is added
            if (textBox.Text.Length < oldText.Length
                && textBox.Text.Length <= oldText.Length - oldSelectionLength)
            {
                isDeleteOrBackspace = true;
                if (oldSelectionLength == 0)
                {
                    // backspace else delete
                    if (oldSelectionStart != textBox.SelectionStart)
                    {
                        deleteBackspaceIndex++;
                    }
                }
            }

            // case adding data at the end of the textBox
            if (oldSelectionStart >= oldText.Length && !isDeleteOrBackspace)
            {
                textBox.Text = textBox.Text.Substring(0, oldText.Length);
                if (oldText.Length >= 0)
                {
                    textBox.SelectionStart = oldText.Length;
                }

                return;
            }

            var textArray = oldText.ToCharArray();

            // detect if backspace or delete is triggered to handle the right removed character
            var newSelectionIndex = oldSelectionStart - deleteBackspaceIndex;

            // check if single selection
            var isSingleSelection = oldSelectionLength != 0 && oldSelectionLength != 1;

            // for handling single key click add +1 to match length for selection = 1
            var singleOrMultiSelectionIndex = oldSelectionLength == 0 ? oldSelectionLength + 1 : oldSelectionLength;

            // Case change due to Text property is assigned a value (Ex Textbox.Text="value")
            if (textBox.SelectionStart == 0 && !textBox.IsFocused)
            {
                var displayText = string.Empty;

                if (_control is null)
                    displayText = textBox.GetValue(DefaultDisplayTextProperty) as string ?? string.Empty;
                else
                    displayText = _control.GetValue(DefaultDisplayTextProperty) as string ?? string.Empty;

                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = displayText;
                    return;
                }
                else
                {
                    var textboxInitialValue = textBox.Text;
                    textBox.Text = displayText;
                    SetTextBoxValue(textboxInitialValue, textBox, escapedMask, escapedChars, representationDictionary, placeHolderValue[0], 0);

                    if (_control is null)
                        textBox.SetValue(OldTextProperty, textBox.Text);
                    else
                        _control.SetValue(OldTextProperty, textBox.Text);

                    return;
                }
            }

            if (!isDeleteOrBackspace)
            {
                // In case the change happened due to user input
                var selectedChar = textBox.SelectionStart > 0 ?
                                    textBox.Text[textBox.SelectionStart - 1] :
                                    placeHolder;

                var maskChar = escapedMask[newSelectionIndex];

                // If dynamic character a,9,* or custom
                if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(newSelectionIndex))
                {
                    var pattern = representationDictionary[maskChar];
                    if (System.Text.RegularExpressions.Regex.IsMatch(selectedChar.ToString(), pattern))
                    {
                        textArray[newSelectionIndex - 1] = selectedChar;

                        // updating text box new index
                        newSelectionIndex++;
                    }

                    // character doesn't match the pattern get the old character
                    else
                    {
                        // if single press don't change
                        if (oldSelectionLength == 0)
                        {
                            textArray[newSelectionIndex] = oldText[newSelectionIndex];
                        }

                        // if change in selection reset to default place holder instead of keeping the old valid to be clear for the user
                        else
                        {
                            textArray[newSelectionIndex] = placeHolder;
                        }
                    }
                }

                // if fixed character
                else
                {
                    textArray[newSelectionIndex] = oldText[newSelectionIndex];

                    // updating text box new index
                    newSelectionIndex++;
                }
            }

            if (isSingleSelection || isDeleteOrBackspace)
            {
                for (int i = newSelectionIndex;
                    i < (oldSelectionStart - deleteBackspaceIndex + singleOrMultiSelectionIndex);
                    i++)
                {
                    var maskChar = escapedMask[i];

                    // If dynamic character a,9,* or custom
                    if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(i))
                    {
                        textArray[i] = placeHolder;
                    }

                    // if fixed character
                    else
                    {
                        textArray[i] = oldText[i];
                    }
                }
            }

            textBox.Text = new string(textArray);

            if (_control is null)
                textBox.SetValue(OldTextProperty, textBox.Text);
            else
                _control.SetValue(OldTextProperty, textBox.Text);

            textBox.SelectionStart = isDeleteOrBackspace ? newSelectionIndex : GetSelectionStart(escapedMask, escapedChars, newSelectionIndex, representationDictionary);
        }

        private static int GetSelectionStart(string mask, List<int> escapedChars, int selectionIndex, Dictionary<char, string> representationDictionary)
        {
            for (int i = selectionIndex; i < mask.Length; i++)
            {
                var maskChar = mask[i];

                // If dynamic character a,9,* or custom
                if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(i))
                {
                    return i;
                }
            }

            return selectionIndex;
        }
    }
}
