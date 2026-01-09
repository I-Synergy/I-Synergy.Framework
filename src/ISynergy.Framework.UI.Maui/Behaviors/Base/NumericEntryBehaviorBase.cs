using System.Globalization;

namespace ISynergy.Framework.UI.Behaviors.Base;

/// <summary>
/// Base behavior for numeric entry input with locale-aware validation and formatting.
/// Provides common functionality for derived numeric behaviors.
/// Works with Entry.Text binding through value converters.
/// </summary>
public abstract class NumericEntryBehaviorBase : Behavior<Entry>
{
    private bool _isUpdating;
    private string _lastValidText = string.Empty;

    public static readonly BindableProperty MinimumValueProperty = BindableProperty.Create(
        nameof(MinimumValue),
        typeof(decimal?),
        typeof(NumericEntryBehaviorBase),
        defaultValue: null);

    public static readonly BindableProperty MaximumValueProperty = BindableProperty.Create(
        nameof(MaximumValue),
        typeof(decimal?),
        typeof(NumericEntryBehaviorBase),
        defaultValue: null);

    public static readonly BindableProperty AllowNegativeProperty = BindableProperty.Create(
        nameof(AllowNegative),
        typeof(bool),
        typeof(NumericEntryBehaviorBase),
        defaultValue: true);

    public static readonly BindableProperty CultureProperty = BindableProperty.Create(
        nameof(Culture),
        typeof(CultureInfo),
        typeof(NumericEntryBehaviorBase),
        defaultValue: CultureInfo.CurrentCulture);

    public static readonly BindableProperty SelectAllOnFocusProperty = BindableProperty.Create(
        nameof(SelectAllOnFocus),
        typeof(bool),
        typeof(NumericEntryBehaviorBase),
        defaultValue: true);

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
    /// Gets the decimal separator for the current culture.
    /// </summary>
    protected string DecimalSeparator => Culture.NumberFormat.NumberDecimalSeparator;

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);

        entry.Keyboard = Keyboard.Numeric;
        entry.TextChanged += OnTextChanged;
        entry.Focused += OnFocused;
        entry.Unfocused += OnUnfocused;

        // Format the initial text if present
        // This handles the case where the binding sets the initial value
        entry.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(10), () =>
        {
            if (!string.IsNullOrWhiteSpace(entry.Text))
            {
                if (TryParseInput(entry.Text, out var initialValue) && IsValueValid(initialValue))
                {
                    _isUpdating = true;
                    try
                    {
                        var formatted = FormatValue(initialValue);
                        entry.Text = formatted;
                        _lastValidText = formatted;
                    }
                    finally
                    {
                        _isUpdating = false;
                    }
                }
            }
        });
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged;
        entry.Focused -= OnFocused;
        entry.Unfocused -= OnUnfocused;

        base.OnDetachingFrom(entry);
    }

    private void OnFocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Entry entry)
            return;

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

        // Format the current text when losing focus
        var currentText = entry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(currentText) ||
            currentText == "-" ||
            currentText == Culture.NumberFormat.NegativeSign)
        {
            // Clear invalid input
            _isUpdating = true;
            try
            {
                entry.Text = string.Empty;
                _lastValidText = string.Empty;
            }
            finally
            {
                _isUpdating = false;
            }
            return;
        }

        // Parse and reformat the value
        if (TryParseInput(currentText, out var value) && IsValueValid(value))
        {
            _isUpdating = true;
            try
            {
                // Format will handle rounding
                var formatted = FormatValue(value);
                entry.Text = formatted;
                _lastValidText = formatted;
            }
            finally
            {
                _isUpdating = false;
            }
        }
        else
        {
            // Invalid input - clear or revert to last valid
            _isUpdating = true;
            try
            {
                entry.Text = string.IsNullOrEmpty(_lastValidText) ? string.Empty : _lastValidText;
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Do nothing during typing - let the user type freely
        // The binding will update the ViewModel property automatically
        // Formatting and validation will happen on blur (OnUnfocused)
        return;
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
}