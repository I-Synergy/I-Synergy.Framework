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
        if (!base.IsValueValid(value))
            return false;

        _formatter ??= new CurrencyFormatter(Culture, DecimalPlaces);
        return _formatter.HasValidDecimalPlaces(value);
    }

    protected override string FormatValue(decimal value)
    {
        _formatter ??= new CurrencyFormatter(Culture, DecimalPlaces);
        return _formatter.FormatValue(value);
    }

    protected override string CleanInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        _formatter ??= new CurrencyFormatter(Culture, DecimalPlaces);

        // Currency formatter handles everything including decimal separator normalization
        return _formatter.CleanInput(input);
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

        var formatted = behavior.FormatValue(behavior.Value);
        behavior._attachedEntry.Text = formatted;
    }
}
