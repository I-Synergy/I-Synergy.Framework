using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.UI.Extensions;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ISynergy.Framework.UI.Controls.Validators;

/// <summary>
/// TextBox extension for Masked input validation.
/// </summary>
public static class MaskValidator
{
    private static Control? _control;

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
        TextBox? textBox = null;

        if (sender is TextBox parent)
        {
            _control = null;
            textBox = parent;
        }
        else if (sender is Control control)
        {
            _control = control;
            textBox = (sender as DependencyObject)?.FindDescendant<TextBox>();
        }

        if (textBox is null)
            return;

        textBox.SelectionChanged -= Textbox_SelectionChanged;
        textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
        textBox.Loaded -= Textbox_Loaded;
        textBox.GotFocus -= Textbox_GotFocus;
        textBox.Loaded += Textbox_Loaded;

        Textbox_Loaded(textBox, new RoutedEventArgs()); // NOSONAR
    }

    private static void Textbox_Loaded(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;

        // In case no value is provided, use it as normal textBox
        var mask = GetRawValue(textBox, MaskProperty).ToString();

        if (string.IsNullOrEmpty(mask))
            return;

        var placeHolderValue = GetRawValue(textBox, MaskPlaceholderProperty).ToString();

        Argument.IsNotNullOrEmpty(placeHolderValue);

        var escapedChars = new List<int>();
        var escapedMask = RemoveEscapeChars(mask, escapedChars);

        SetRawValue(textBox, EscapedCharacterIndicesProperty, escapedChars);
        SetRawValue(textBox, EscapedMaskProperty, escapedMask);

        var placeHolder = placeHolderValue![0];

        var representationDictionary = BuildRepresentationDictionary(textBox.GetValue(CustomMaskProperty).ToString());

        SetRawValue(textBox, RepresentationDictionaryProperty, representationDictionary);

        var displayText = RenderDisplayText(escapedMask, escapedChars, representationDictionary, placeHolder);

        if (string.IsNullOrEmpty(textBox.Text))
        {
            textBox.Text = displayText;
        }
        else
        {
            var textboxInitialValue = textBox.Text;
            textBox.Text = displayText;

            var oldSelectionStart = (int)GetRawValue(textBox, OldSelectionStartProperty);

            SetTextBoxValue(textboxInitialValue, textBox, escapedMask, escapedChars, representationDictionary, placeHolder, oldSelectionStart);
        }

        textBox.SelectionChanged += Textbox_SelectionChanged;
        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        textBox.GotFocus += Textbox_GotFocus;

        SetRawValue(textBox, OldTextProperty, textBox.Text);
        SetRawValue(textBox, DefaultDisplayTextProperty, displayText);

        textBox.SelectionStart = 0;
    }

    private static void Textbox_GotFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var mask = string.Empty;
        var placeHolderValue = string.Empty;
        Dictionary<char, string>? representationDictionary = null;

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
        List<int>? escapedChars,
        Dictionary<char, string> representationDictionary,
        char placeholder,
        int oldSelectionStart)
    {
        var maxLength = newValue.Length + oldSelectionStart < mask.Length ? newValue.Length + oldSelectionStart : mask.Length;
        var textArray = textBox.Text.ToCharArray();

        for (int i = oldSelectionStart; i < maxLength; i++)
        {
            var maskChar = mask[i];
            var selectedChar = newValue[i - oldSelectionStart];

            // If dynamic character a,9,* or custom
            if (representationDictionary.TryGetValue(maskChar, out string? pattern) && escapedChars is not null && !escapedChars.Contains(i))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(selectedChar.ToString(), pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
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

        if (!TryReadPreviewState(textBox, out var state))
            return;

        var (isDeleteOrBackspace, deleteBackspaceIndex) = DetectDeleteOrBackspace(textBox, state);

        // case adding data at the end of the textBox
        if (state.OldSelectionStart >= state.OldText.Length && !isDeleteOrBackspace)
        {
            textBox.Text = textBox.Text.Substring(0, state.OldText.Length);
            textBox.SelectionStart = state.OldText.Length;
            return;
        }

        var textArray = state.OldText.ToCharArray();

        // detect if backspace or delete is triggered to handle the right removed character
        var newSelectionIndex = state.OldSelectionStart - deleteBackspaceIndex;

        // check if single selection
        var isSingleSelection = state.OldSelectionLength != 0 && state.OldSelectionLength != 1;

        // for handling single key click add +1 to match length for selection = 1
        var singleOrMultiSelectionIndex = state.OldSelectionLength == 0 ? state.OldSelectionLength + 1 : state.OldSelectionLength;

        // Case change due to Text property is assigned a value (Ex Textbox.Text="value")
        if (textBox.SelectionStart == 0 && !textBox.IsFocused)
        {
            HandleProgrammaticTextChange(textBox, state);
            return;
        }

        if (!isDeleteOrBackspace)
        {
            newSelectionIndex = ApplyKeyInput(textBox, state, newSelectionIndex, textArray);
        }

        if (isSingleSelection || isDeleteOrBackspace)
        {
            ResetSelectionRange(state, newSelectionIndex, deleteBackspaceIndex, singleOrMultiSelectionIndex, textArray);
        }

        textBox.Text = new string(textArray);

        SetRawValue(textBox, OldTextProperty, textBox.Text);

        textBox.SelectionStart = isDeleteOrBackspace
            ? newSelectionIndex
            : GetSelectionStart(state.EscapedMask, state.EscapedChars, newSelectionIndex, state.RepresentationDictionary);
    }

    private static int GetSelectionStart(string mask, List<int>? escapedChars, int selectionIndex, Dictionary<char, string> representationDictionary)
    {
        for (int i = selectionIndex; i < mask.Length; i++)
        {
            var maskChar = mask[i];

            // If dynamic character a,9,* or custom
            if (representationDictionary.ContainsKey(maskChar) && escapedChars is not null && !escapedChars.Contains(i))
            {
                return i;
            }
        }

        return selectionIndex;
    }
    /// <summary>
    /// Resolves the effective <see cref="DependencyObject"/> that carries the attached mask state:
    /// the wrapping control when present, otherwise the text box itself.
    /// </summary>
    private static DependencyObject ResolveTarget(TextBox textBox) => _control is null ? textBox : _control;

    private static object GetRawValue(TextBox textBox, DependencyProperty property) =>
        ResolveTarget(textBox).GetValue(property);

    private static void SetRawValue(TextBox textBox, DependencyProperty property, object value) =>
        ResolveTarget(textBox).SetValue(property, value);

    /// <summary>
    /// Copies the mask into a buffer, recording the indices of characters escaped with <see cref="EscapeChar"/>
    /// and removing the escape characters themselves.
    /// </summary>
    private static string RemoveEscapeChars(string mask, List<int> escapedChars)
    {
        var builder = new StringBuilder(mask);

        for (int i = 0; i < builder.Length - 1; i++)
        {
            if (builder[i] == EscapeChar)
            {
                escapedChars.Add(i);
                builder.Remove(i, 1);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Builds the character representation dictionary (a, 9, * plus any custom roles) used to
    /// interpret dynamic mask characters.
    /// </summary>
    private static Dictionary<char, string> BuildRepresentationDictionary(string? customDictionaryValue)
    {
        var representationDictionary = new Dictionary<char, string>
        {
            { AlphaCharacterRepresentation.Key, AlphaCharacterRepresentation.Value },
            { NumericCharacterRepresentation.Key, NumericCharacterRepresentation.Value },
            { AlphaNumericRepresentation.Key, AlphaNumericRepresentation.Value }
        };

        if (string.IsNullOrWhiteSpace(customDictionaryValue))
            return representationDictionary;

        var customRoles = customDictionaryValue.Split(',');
        foreach (var role in customRoles.EnsureNotNull())
        {
            var roleValues = role.Split(':');
            if (roleValues.Length != 2)
            {
                throw new ArgumentException("Invalid CustomMask property");
            }

            var keyValue = roleValues[0];
            var value = roleValues[1];

            // an exception should be thrown if the regex is not valid
            Regex.Match(string.Empty, value, RegexOptions.None, TimeSpan.FromMilliseconds(100));
            if (!char.TryParse(keyValue, out var key))
            {
                throw new ArgumentException("Invalid CustomMask property, please validate the mask key");
            }

            representationDictionary.Add(key, value);
        }

        return representationDictionary;
    }

    /// <summary>
    /// Renders the initial display text, replacing dynamic characters with the mask placeholder.
    /// </summary>
    private static string RenderDisplayText(
        string escapedMask,
        List<int> escapedChars,
        Dictionary<char, string> representationDictionary,
        char placeHolder)
    {
        var displayTextBuilder = new StringBuilder(escapedMask);

        for (int i = 0; i < displayTextBuilder.Length; i++)
        {
            if (escapedChars.Contains(i))
            {
                continue;
            }

            foreach (var key in representationDictionary.Keys.EnsureNotNull())
            {
                if (displayTextBuilder[i] == key)
                {
                    displayTextBuilder[i] = placeHolder;
                }
            }
        }

        return displayTextBuilder.ToString();
    }

    /// <summary>
    /// Handles the case where the Text property was assigned a value programmatically
    /// (cursor at index 0 and the control is not focused).
    /// </summary>
    private static void HandleProgrammaticTextChange(TextBox textBox, in PreviewState state)
    {
        var displayText = GetRawValue(textBox, DefaultDisplayTextProperty) as string ?? string.Empty;

        if (string.IsNullOrEmpty(textBox.Text))
        {
            textBox.Text = displayText;
            return;
        }

        var textboxInitialValue = textBox.Text;
        textBox.Text = displayText;
        SetTextBoxValue(textboxInitialValue, textBox, state.EscapedMask, state.EscapedChars, state.RepresentationDictionary, state.PlaceHolderValue[0], 0);

        SetRawValue(textBox, OldTextProperty, textBox.Text);
    }

    /// <summary>
    /// Determines whether the current key press is a delete or backspace, and how far the deletion
    /// index should be shifted back.
    /// </summary>
    private static (bool isDeleteOrBackspace, int deleteBackspaceIndex) DetectDeleteOrBackspace(
        TextBox textBox,
        in PreviewState state)
    {
        // Delete or backspace is triggered
        // if the new length is less than or equal the old text - the old selection length then a delete or backspace is triggered with or without selection and no characters is added
        if (textBox.Text.Length < state.OldText.Length
            && textBox.Text.Length <= state.OldText.Length - state.OldSelectionLength)
        {
            // backspace else delete
            var deleteBackspaceIndex = state.OldSelectionLength == 0 && state.OldSelectionStart != textBox.SelectionStart ? 1 : 0;
            return (true, deleteBackspaceIndex);
        }

        return (false, 0);
    }

    /// <summary>
    /// Applies a single key input at the current selection index, returning the updated index.
    /// </summary>
    private static int ApplyKeyInput(TextBox textBox, in PreviewState state, int newSelectionIndex, char[] textArray)
    {
        // In case the change happened due to user input
        var selectedChar = textBox.SelectionStart > 0
            ? textBox.Text[textBox.SelectionStart - 1]
            : state.PlaceHolder;

        var maskChar = state.EscapedMask[newSelectionIndex];

        // If dynamic character a,9,* or custom
        if (state.RepresentationDictionary.TryGetValue(maskChar, out string? pattern) && state.EscapedChars is not null && !state.EscapedChars.Contains(newSelectionIndex))
        {
            if (Regex.IsMatch(selectedChar.ToString(), pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            {
                textArray[newSelectionIndex - 1] = selectedChar;

                // updating text box new index
                return newSelectionIndex + 1;
            }

            // character doesn't match the pattern get the old character
            // if single press don't change
            if (state.OldSelectionLength == 0)
            {
                textArray[newSelectionIndex] = state.OldText[newSelectionIndex];
            }

            // if change in selection reset to default place holder instead of keeping the old valid to be clear for the user
            else
            {
                textArray[newSelectionIndex] = state.PlaceHolder;
            }

            return newSelectionIndex;
        }

        // if fixed character
        textArray[newSelectionIndex] = state.OldText[newSelectionIndex];

        // updating text box new index
        return newSelectionIndex + 1;
    }

    /// <summary>
    /// Resets the selected range to placeholders for dynamic characters and to the old text for fixed ones.
    /// </summary>
    private static void ResetSelectionRange(
        in PreviewState state,
        int newSelectionIndex,
        int deleteBackspaceIndex,
        int singleOrMultiSelectionIndex,
        char[] textArray)
    {
        for (int i = newSelectionIndex; i < state.OldSelectionStart - deleteBackspaceIndex + singleOrMultiSelectionIndex; i++)
        {
            var maskChar = state.EscapedMask[i];

            // If dynamic character a,9,* or custom
            if (state.RepresentationDictionary.ContainsKey(maskChar) && state.EscapedChars is not null && !state.EscapedChars.Contains(i))
            {
                textArray[i] = state.PlaceHolder;
            }

            // if fixed character
            else
            {
                textArray[i] = state.OldText[i];
            }
        }
    }

    /// <summary>
    /// Reads the attached mask state into a <see cref="PreviewState"/>; returns <c>false</c> when the
    /// required mask values are not set up.
    /// </summary>
    private static bool TryReadPreviewState(TextBox textBox, out PreviewState state)
    {
        var escapedMask = GetRawValue(textBox, EscapedMaskProperty) as string;
        var escapedChars = GetRawValue(textBox, EscapedCharacterIndicesProperty) as List<int>;
        var representationDictionary = GetRawValue(textBox, RepresentationDictionaryProperty) as Dictionary<char, string>;
        var placeHolderValue = GetRawValue(textBox, MaskPlaceholderProperty) as string;
        var oldText = GetRawValue(textBox, OldTextProperty) as string;
        var oldSelectionStart = (int)GetRawValue(textBox, OldSelectionStartProperty);
        var oldSelectionLength = (int)GetRawValue(textBox, OldSelectionLengthProperty);

        if (string.IsNullOrWhiteSpace(escapedMask) || representationDictionary == null || string.IsNullOrEmpty(placeHolderValue) || oldText == null)
        {
            state = default;
            return false;
        }

        state = new PreviewState(escapedMask, escapedChars, representationDictionary, placeHolderValue, oldText, oldSelectionStart, oldSelectionLength);
        return true;
    }

    /// <summary>
    /// Immutable snapshot of the attached mask state used while processing a key press.
    /// </summary>
    private readonly struct PreviewState
    {
        public string EscapedMask { get; }
        public List<int>? EscapedChars { get; }
        public Dictionary<char, string> RepresentationDictionary { get; }
        public string PlaceHolderValue { get; }
        public string OldText { get; }
        public int OldSelectionStart { get; }
        public int OldSelectionLength { get; }
        public char PlaceHolder => PlaceHolderValue[0];

        public PreviewState(
            string escapedMask,
            List<int>? escapedChars,
            Dictionary<char, string> representationDictionary,
            string placeHolderValue,
            string oldText,
            int oldSelectionStart,
            int oldSelectionLength)
        {
            EscapedMask = escapedMask;
            EscapedChars = escapedChars;
            RepresentationDictionary = representationDictionary;
            PlaceHolderValue = placeHolderValue;
            OldText = oldText;
            OldSelectionStart = oldSelectionStart;
            OldSelectionLength = oldSelectionLength;
        }
    }
}
