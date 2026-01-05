using System.Globalization;

namespace ISynergy.Framework.UI.Behaviors.Base;

/// <summary>
/// Base behavior for numeric entry input with locale-aware validation and formatting.
/// Provides common functionality for derived numeric behaviors.
/// </summary>
public abstract class NumericEntryBehaviorBase : Behavior<Entry>
{
    private bool _isUpdating;
    private string _lastValidText = string.Empty;
    private Entry? _attachedEntry;
    private bool _isFocused;

    public static readonly BindableProperty MinimumValueProperty = BindableProperty.Create(
        nameof(MinimumValue),
        typeof(decimal?),
        typeof(NumericEntryBehaviorBase),
        defaultValue: decimal.MinValue);

    public static readonly BindableProperty MaximumValueProperty = BindableProperty.Create(
        nameof(MaximumValue),
        typeof(decimal?),
        typeof(NumericEntryBehaviorBase),
        defaultValue: decimal.MaxValue);

    public static readonly BindableProperty AllowNegativeProperty = BindableProperty.Create(
        nameof(AllowNegative),
        typeof(bool),
        typeof(NumericEntryBehaviorBase),
        defaultValue: true);

    public static readonly BindableProperty CultureProperty = BindableProperty.Create(
        nameof(Culture),
        typeof(CultureInfo),
        typeof(NumericEntryBehaviorBase),
        defaultValue: CultureInfo.CurrentCulture,
        propertyChanged: OnCultureChanged);

    public static readonly BindableProperty SelectAllOnFocusProperty = BindableProperty.Create(
        nameof(SelectAllOnFocus),
        typeof(bool),
        typeof(NumericEntryBehaviorBase),
        defaultValue: true);

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(decimal),
        typeof(NumericEntryBehaviorBase),
        defaultValue: 0m,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: OnValueChanged);

    /// <summary>
    /// Gets or sets the minimum allowed value. Null for no minimum.
    /// </summary>
    public decimal? MinimumValue
    {
        get => (decimal?)GetValue(MinimumValueProperty);
        set => SetValue(MinimumValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum allowed value. Null for no maximum.
    /// </summary>
    public decimal? MaximumValue
    {
        get => (decimal?)GetValue(MaximumValueProperty);
        set => SetValue(MaximumValueProperty, value);
    }

    /// <summary>
    /// Gets or sets whether negative values are allowed.
    /// </summary>
    public bool AllowNegative
    {
        get => (bool)GetValue(AllowNegativeProperty);
        set => SetValue(AllowNegativeProperty, value);
    }

    /// <summary>
    /// Gets or sets the culture used for formatting. Defaults to current culture.
    /// </summary>
    public CultureInfo Culture
    {
        get => (CultureInfo)GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to select all text when the entry receives focus.
    /// </summary>
    public bool SelectAllOnFocus
    {
        get => (bool)GetValue(SelectAllOnFocusProperty);
        set => SetValue(SelectAllOnFocusProperty, value);
    }

    /// <summary>
    /// Gets or sets the numeric value. Use this for two-way binding in MVVM scenarios.
    /// </summary>
    public decimal Value
    {
        get => (decimal)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets the decimal separator for the current culture.
    /// </summary>
    protected string DecimalSeparator => Culture.NumberFormat.NumberDecimalSeparator;

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);

        _attachedEntry = entry;

        entry.Keyboard = Keyboard.Numeric;
        entry.TextChanged += OnTextChanged;
        entry.Focused += OnFocused;
        entry.Unfocused += OnUnfocused;

        UpdateEntryText(entry, Value);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged;
        entry.Focused -= OnFocused;
        entry.Unfocused -= OnUnfocused;

        _attachedEntry = null;

        base.OnDetachingFrom(entry);
    }

    private void OnFocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        _isFocused = true;

        if (!SelectAllOnFocus)
            return;

        entry.Dispatcher.Dispatch(() =>
        {
            if (!string.IsNullOrEmpty(entry.Text))
            {
                entry.CursorPosition = 0;
                entry.SelectionLength = entry.Text.Length;
            }
        });
    }

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        _isFocused = false;

        // If only a minus sign remains, reset to zero
        var trimmedText = entry.Text?.Trim() ?? string.Empty;
        if (trimmedText == "-" || trimmedText == Culture.NumberFormat.NegativeSign)
        {
            UpdateValue(0m);
        }

        UpdateEntryText(entry, Value);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdating || sender is not Entry entry)
            return;

        var newText = e.NewTextValue ?? string.Empty;

        if (string.IsNullOrWhiteSpace(newText))
        {
            _lastValidText = string.Empty;
            UpdateValue(0m);
            return;
        }

        // Allow intermediate input for negative sign or partial decimal
        if (IsIntermediateInput(newText))
        {
            _lastValidText = newText;
            return; // Don't update value yet, wait for more input
        }

        if (TryParseInput(newText, out var parsedValue))
        {
            if (IsValueValid(parsedValue))
            {
                _lastValidText = newText;
                UpdateValue(parsedValue);
            }
            else
            {
                RevertToLastValidText(entry);
            }
        }
        else
        {
            RevertToLastValidText(entry);
        }
    }

    /// <summary>
    /// Checks if the input is in an intermediate state (not yet complete but valid so far).
    /// </summary>
    private bool IsIntermediateInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Try cleaning the input first to handle currency symbols and group separators
        var cleaned = CleanInput(input);

        // Allow negative sign as intermediate input (while typing "-1")
        if (AllowNegative && (cleaned == "-" || cleaned == Culture.NumberFormat.NegativeSign))
            return true;

        // Ends with decimal separator (e.g., "5." or "-5." or "€ 5.")
        if (cleaned.EndsWith(Culture.NumberFormat.NumberDecimalSeparator))
            return true;

        return false;
    }

    /// <summary>
    /// Attempts to parse the input text into a decimal value.
    /// </summary>
    protected virtual bool TryParseInput(string input, out decimal value)
    {
        value = 0;

        var cleanedInput = CleanInput(input);

        if (string.IsNullOrWhiteSpace(cleanedInput))
            return false;

        var hasNegative = cleanedInput.StartsWith("-") ||
                         cleanedInput.StartsWith(Culture.NumberFormat.NegativeSign);

        if (hasNegative && !AllowNegative)
            return false;

        // Removed AllowThousands since we already cleaned group separators
        return decimal.TryParse(
            cleanedInput,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            Culture,
            out value);
    }

    /// <summary>
    /// Cleans the input text by normalizing decimal separators and removing unwanted characters.
    /// </summary>
    protected virtual string CleanInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove group separators FIRST before normalizing decimal separators
        var cleaned = input.Replace(Culture.NumberFormat.NumberGroupSeparator, string.Empty);
        cleaned = cleaned.Trim();

        // Now normalize decimal separators
        var decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
        var groupSeparator = Culture.NumberFormat.NumberGroupSeparator;
        var alternativeSeparators = new[] { ".", ",", "·", "٫" }
            .Where(s => s != decimalSeparator && s != groupSeparator)
            .ToArray();

        foreach (var separator in alternativeSeparators)
        {
            cleaned = cleaned.Replace(separator, decimalSeparator);
        }

        return cleaned;
    }

    /// <summary>
    /// Validates whether the parsed value meets all constraints.
    /// Derived classes can override to add additional validation rules.
    /// </summary>
    protected virtual bool IsValueValid(decimal value)
    {
        if (MinimumValue.HasValue && value < MinimumValue.Value)
            return false;

        if (MaximumValue.HasValue && value > MaximumValue.Value)
            return false;

        if (!AllowNegative && value < 0)
            return false;

        return true;
    }

    /// <summary>
    /// Formats the numeric value for display in the entry.
    /// Derived classes must implement this to provide type-specific formatting.
    /// </summary>
    protected abstract string FormatValue(decimal value);

    private void RevertToLastValidText(Entry entry)
    {
        _isUpdating = true;
        try
        {
            var currentCursorPosition = entry.CursorPosition;
            entry.Text = _lastValidText;
            entry.CursorPosition = Math.Min(currentCursorPosition, _lastValidText.Length);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void UpdateValue(decimal value)
    {
        if (Value == value)
            return;

        _isUpdating = true;
        try
        {
            Value = value;
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void UpdateEntryText(Entry entry, decimal value)
    {
        _isUpdating = true;
        try
        {
            var formatted = FormatValue(value);
            entry.Text = formatted;
            _lastValidText = formatted;
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not NumericEntryBehaviorBase behavior || behavior._isUpdating)
            return;

        if (behavior._attachedEntry is null)
            return;

        // Don't reformat while user is actively typing
        if (behavior._isFocused)
            return;

        if (newValue is decimal value)
        {
            behavior.UpdateEntryText(behavior._attachedEntry, value);
        }
        else
        {
            behavior._isUpdating = true;
            try
            {
                behavior._attachedEntry.Text = string.Empty;
                behavior._lastValidText = string.Empty;
            }
            finally
            {
                behavior._isUpdating = false;
            }
        }
    }

    private static void OnCultureChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not NumericEntryBehaviorBase behavior)
            return;

        if (behavior._attachedEntry is null)
            return;

        // Don't reformat while user is actively typing
        if (behavior._isFocused)
            return;

        behavior.UpdateEntryText(behavior._attachedEntry, behavior.Value);
    }
}