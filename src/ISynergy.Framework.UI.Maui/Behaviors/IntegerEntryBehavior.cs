using ISynergy.Framework.Core.Formatters;
using ISynergy.Framework.UI.Behaviors.Base;

namespace ISynergy.Framework.UI.Behaviors;

/// <summary>
/// Behavior that restricts Entry input to integer values only.
/// No decimal places are allowed.
/// </summary>
public sealed class IntegerEntryBehavior : NumericEntryBehaviorBase
{
    private IntegerFormatter? _formatter;

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        _formatter = new IntegerFormatter(Culture);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        _formatter = null;
        base.OnDetachingFrom(entry);
    }

    protected override bool IsValueValid(decimal value)
    {
        if (!base.IsValueValid(value))
            return false;

        _formatter ??= new IntegerFormatter(Culture);
        return _formatter.IsInteger(value);
    }

    protected override string FormatValue(decimal value)
    {
        _formatter ??= new IntegerFormatter(Culture);
        return _formatter.FormatValue(value);
    }

    protected override string CleanInput(string input)
    {
        _formatter ??= new IntegerFormatter(Culture);

        // First normalize using base class
        var cleaned = base.CleanInput(input);

        // Then remove decimal part
        return _formatter.CleanInput(cleaned);
    }
}
