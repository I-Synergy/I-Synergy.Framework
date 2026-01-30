using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using Microsoft.UI.Xaml;

namespace ISynergy.Framework.UI.Triggers;

/// <summary>
/// Class ObjectDataTrigger.
/// Implements the <see cref="StateTriggerBase" />
/// </summary>
/// <seealso cref="StateTriggerBase" />
public class ObjectDataTrigger : StateTriggerBase
{
    #region DataValue
    /// <summary>
    /// Gets or sets the data value.
    /// </summary>
    /// <value>The data value.</value>
    public object DataValue
    {
        get => (object)GetValue(DataValueProperty);
        set => SetValue(DataValueProperty, value);
    }

    /// <summary>
    /// The data value property
    /// </summary>
    public static readonly DependencyProperty DataValueProperty =
        DependencyProperty.Register(nameof(DataValue), typeof(object), typeof(ObjectDataTrigger), new PropertyMetadata(null, DataValueChanged));

    /// <summary>
    /// Datas the value changed.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void DataValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        TriggerStateCheck(
            target,
            (string)target.GetValue(OperatorProperty),
            e.NewValue,
            target.GetValue(TriggerValueProperty));
    }
    #endregion

    #region TriggerValue
    /// <summary>
    /// Gets or sets the trigger value.
    /// </summary>
    /// <value>The trigger value.</value>
    public object TriggerValue
    {
        get => (object)GetValue(TriggerValueProperty);
        set => SetValue(TriggerValueProperty, value);
    }

    /// <summary>
    /// The trigger value property
    /// </summary>
    public static readonly DependencyProperty TriggerValueProperty =
        DependencyProperty.Register(nameof(TriggerValue), typeof(object), typeof(ObjectDataTrigger), new PropertyMetadata(null, TriggerValueChanged));

    /// <summary>
    /// Triggers the value changed.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void TriggerValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        TriggerStateCheck(
            target,
            (string)target.GetValue(OperatorProperty),
            target.GetValue(DataValueProperty),
            e.NewValue);
    }
    #endregion

    #region Operator
    /// <summary>
    /// Gets or sets the operator.
    /// </summary>
    /// <value>The operator.</value>
    public string Operator
    {
        get => (string)GetValue(OperatorProperty);
        set => SetValue(OperatorProperty, value);
    }

    /// <summary>
    /// The operator property
    /// </summary>
    public static readonly DependencyProperty OperatorProperty =
        DependencyProperty.Register(nameof(Operator), typeof(string), typeof(ObjectDataTrigger), new PropertyMetadata("=="));
    #endregion

    /// <summary>
    /// Triggers the state check.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="dataValue">The data value.</param>
    /// <param name="triggerValue">The trigger value.</param>
    private static void TriggerStateCheck(DependencyObject target, string operation, object dataValue, object triggerValue)
    {
        Argument.IsNotNull(target);
        Argument.IsNotNullOrEmpty(operation);

        if (!(target is ObjectDataTrigger trigger) || dataValue is null) return;
        trigger.SetActive(CompareUtility.Compare(operation, dataValue, triggerValue));
    }
}
