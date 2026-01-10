using ISynergy.Framework.Core.Formatters;
using ISynergy.Framework.UI.Behaviors.Base;

namespace ISynergy.Framework.UI.Behaviors;

/// <summary>
/// Behavior that restricts Entry input to currency values with culture-specific formatting.
/// </summary>
public sealed class CurrencyEntryBehavior : NumericEntryBehaviorBase
{
    private Entry? _attachedEntry;
    private CurrencyFormatter? _formatter;

    public static readonly BindableProperty DecimalPlacesProperty = BindableProperty.Create(
        nameof(DecimalPlaces),
        typeof(int),
        typeof(CurrencyEntryBehavior),
        defaultValue: 2,
        validateValue: (_, value) => value is int places && places >= 0 && places <= 10,
        propertyChanged: OnDecimalPlacesChanged);

    /// <summary>
    /// Gets or sets the number of decimal places to display and allow for the currency.
    /// </summary>
    public int DecimalPlaces
    {
        get => (int)GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        _attachedEntry = entry;
        UpdateFormatter();
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        _attachedEntry = null;
        _formatter = null;
        base.OnDetachingFrom(entry);
    }

    protected override bool IsValueValid(decimal value)
    {
        // Don't validate decimal places here - we'll round during formatting
        // Just validate the base constraints (min/max/negative)
        return base.IsValueValid(value);
    }

    protected override string FormatValue(decimal value)
    {
        _formatter ??= new CurrencyFormatter(Culture, DecimalPlaces);
        
        // Round the value to the specified decimal places before formatting
        var rounded = Math.Round(value, DecimalPlaces, MidpointRounding.AwayFromZero);
        
        return _formatter.FormatValue(rounded);
    }

    protected override string CleanInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        _formatter ??= new CurrencyFormatter(Culture, DecimalPlaces);

        // Currency formatter handles everything including decimal separator normalization
        return _formatter.CleanInput(input);
    }

    protected override void OnCulturePropertyChanged()
    {
        base.OnCulturePropertyChanged();
        
        // Recreate the formatter with the new culture
        UpdateFormatter();
        
        // Reformat the current text if we have an attached entry
        if (_attachedEntry is not null && !string.IsNullOrWhiteSpace(_attachedEntry.Text))
        {
            if (TryParseInput(_attachedEntry.Text, out var value) && IsValueValid(value))
            {
                var formatted = FormatValue(value);
                _attachedEntry.Text = formatted;
            }
        }
    }

    private void UpdateFormatter()
    {
        _formatter = new CurrencyFormatter(Culture, DecimalPlaces);
    }

    private static void OnDecimalPlacesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CurrencyEntryBehavior behavior)
            return;

        behavior.UpdateFormatter();

        if (behavior._attachedEntry is null)
            return;

        // Reformat the current text with the new decimal places
        var currentText = behavior._attachedEntry.Text ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(currentText) && behavior.TryParseInput(currentText, out var value))
        {
            var formatted = behavior.FormatValue(value);
            behavior._attachedEntry.Text = formatted;
        }
    }
}
