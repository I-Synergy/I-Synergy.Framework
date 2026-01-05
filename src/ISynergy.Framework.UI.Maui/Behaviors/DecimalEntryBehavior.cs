using ISynergy.Framework.Core.Formatters;
using ISynergy.Framework.UI.Behaviors.Base;

namespace ISynergy.Framework.UI.Behaviors;

/// <summary>
/// Behavior that restricts Entry input to decimal values with configurable decimal places.
/// </summary>
public sealed class DecimalEntryBehavior : NumericEntryBehaviorBase
{
    private Entry? _attachedEntry;
    private DecimalFormatter? _formatter;

    public static readonly BindableProperty DecimalPlacesProperty = BindableProperty.Create(
        nameof(DecimalPlaces),
        typeof(int),
        typeof(DecimalEntryBehavior),
        defaultValue: 2,
        validateValue: (_, value) => value is int places && places >= 0 && places <= 10,
        propertyChanged: OnDecimalPlacesChanged);

    /// <summary>
    /// Gets or sets the number of decimal places to display and allow.
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

        _formatter ??= new DecimalFormatter(Culture, DecimalPlaces);
        return _formatter.HasValidDecimalPlaces(value);
    }

    protected override string FormatValue(decimal value)
    {
        _formatter ??= new DecimalFormatter(Culture, DecimalPlaces);
        return _formatter.FormatValue(value);
    }

    private void UpdateFormatter()
    {
        _formatter = new DecimalFormatter(Culture, DecimalPlaces);
    }

    protected override string CleanInput(string input)
    {
        _formatter ??= new DecimalFormatter(Culture, DecimalPlaces);

        // First normalize using base class
        var cleaned = base.CleanInput(input);

        // Then remove decimal part
        return _formatter.CleanInput(cleaned);
    }

    private static void OnDecimalPlacesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not DecimalEntryBehavior behavior)
            return;

        behavior.UpdateFormatter();

        if (behavior._attachedEntry is null)
            return;

        var formatted = behavior.FormatValue(behavior.Value);
        behavior._attachedEntry.Text = formatted;
    }
}
