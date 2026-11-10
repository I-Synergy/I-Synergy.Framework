using ISynergy.Framework.UI.Controls.Validators;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ISynergy.Framework.UI.WPF.Controls.Validators;

/// <summary>
/// Tests for <see cref="RegexValidator"/>. Every test body is executed on an STA thread because the
/// validator drives real WPF <see cref="TextBox"/> instances.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class RegexValidatorTests
{
    private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
    private const string DigitsOnly = "^[0-9]+$";

    // ── Custom regex validation ──────────────────────────────────────────────

    [TestMethod]
    public void Loaded_WithMatchingText_SetsIsValidTrue() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(DigitsOnly, initialText: "123");

        Assert.IsTrue(RegexValidator.GetIsValid(textBox));
    });

    [TestMethod]
    public void Loaded_WithNonMatchingText_SetsIsValidFalseAndKeepsText() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(DigitsOnly, initialText: "abc");

        Assert.IsFalse(RegexValidator.GetIsValid(textBox));
        Assert.AreEqual("abc", textBox.Text);
    });

    [TestMethod]
    public void TextChanged_WithNonMatchingText_SetsIsValidFalse() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(DigitsOnly, initialText: "123");
        Assert.IsTrue(RegexValidator.GetIsValid(textBox));

        textBox.Text = "12a";

        Assert.IsFalse(RegexValidator.GetIsValid(textBox));
    });

    [TestMethod]
    public void TextChanged_WithMatchingText_SetsIsValidTrue() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(DigitsOnly, initialText: "abc");
        Assert.IsFalse(RegexValidator.GetIsValid(textBox));

        textBox.Text = "456";

        Assert.IsTrue(RegexValidator.GetIsValid(textBox));
    });

    // ── Validation modes ────────────────────────────────────────────────────

    [TestMethod]
    public void ForcedMode_LostFocus_ClearsInvalidText() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(
            DigitsOnly,
            mode: RegexValidator.ValidationMode.Forced,
            initialText: "abc");

        InvokePrivateHandler("TextBox_LostFocus", textBox);

        Assert.AreEqual(string.Empty, textBox.Text);
        Assert.IsFalse(RegexValidator.GetIsValid(textBox));
    });

    [TestMethod]
    public void ForcedMode_LostFocus_KeepsValidText() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(
            DigitsOnly,
            mode: RegexValidator.ValidationMode.Forced,
            initialText: "123");

        InvokePrivateHandler("TextBox_LostFocus", textBox);

        Assert.AreEqual("123", textBox.Text);
        Assert.IsTrue(RegexValidator.GetIsValid(textBox));
    });

    [TestMethod]
    public void DynamicMode_TextChanged_RemovesInvalidCharacters() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(DigitsOnly, mode: RegexValidator.ValidationMode.Dynamic);

        textBox.Text = "12a";

        // Every invalid character is peeled off one at a time until nothing is left.
        Assert.AreEqual(string.Empty, textBox.Text);
        Assert.IsFalse(RegexValidator.GetIsValid(textBox));
    });

    [TestMethod]
    public void NormalMode_TextChanged_KeepsInvalidText() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(
            DigitsOnly,
            mode: RegexValidator.ValidationMode.Normal,
            initialText: "abc");

        Assert.AreEqual("abc", textBox.Text);
        Assert.IsFalse(RegexValidator.GetIsValid(textBox));
    });

    // ── Validation types ────────────────────────────────────────────────────

    [TestMethod]
    public void IntegerValidation_ValidatesText() => StaTestRunner.Run(() =>
    {
        var valid = CreateRegexTextBox(type: RegexValidator.ValidationType.Integer, initialText: "123");
        var invalid = CreateRegexTextBox(type: RegexValidator.ValidationType.Integer, initialText: "12a");

        Assert.IsTrue(RegexValidator.GetIsValid(valid));
        Assert.IsFalse(RegexValidator.GetIsValid(invalid));
    });

    [TestMethod]
    public void DecimalValidation_ValidatesText() => StaTestRunner.Run(() =>
    {
        var valid = CreateRegexTextBox(type: RegexValidator.ValidationType.Decimal, initialText: "12.5");
        var invalid = CreateRegexTextBox(type: RegexValidator.ValidationType.Decimal, initialText: "abc");

        Assert.IsTrue(RegexValidator.GetIsValid(valid));
        Assert.IsFalse(RegexValidator.GetIsValid(invalid));
    });

    [TestMethod]
    public void AlphanumericValidation_ValidatesText() => StaTestRunner.Run(() =>
    {
        var valid = CreateRegexTextBox(type: RegexValidator.ValidationType.Alphanumeric, initialText: "abc123");
        var invalid = CreateRegexTextBox(type: RegexValidator.ValidationType.Alphanumeric, initialText: "abc-123");

        Assert.IsTrue(RegexValidator.GetIsValid(valid));
        Assert.IsFalse(RegexValidator.GetIsValid(invalid));
    });

    // ── Custom mode without a regex ─────────────────────────────────────────

    [TestMethod]
    public void CustomModeWithoutRegex_LeavesValidationUntouched() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(mode: RegexValidator.ValidationMode.Forced, initialText: "abc");

        Assert.IsFalse(RegexValidator.GetIsValid(textBox), "IsValid must keep its default value when no regex is configured");

        InvokePrivateHandler("TextBox_LostFocus", textBox);

        Assert.AreEqual("abc", textBox.Text, "Text must not be cleared when validation cannot be performed");
        Assert.IsFalse(RegexValidator.GetIsValid(textBox));
    });

    // ── Attached property plumbing ──────────────────────────────────────────

    [TestMethod]
    public void AttachedProperties_RoundTripValues() => StaTestRunner.Run(() =>
    {
        var textBox = new TextBox();

        Assert.IsNull(RegexValidator.GetRegex(textBox));
        Assert.IsFalse(RegexValidator.GetIsValid(textBox));
        Assert.AreEqual(RegexValidator.ValidationMode.Normal, RegexValidator.GetValidationMode(textBox));
        Assert.AreEqual(RegexValidator.ValidationType.Custom, RegexValidator.GetValidationType(textBox));

        RegexValidator.SetRegex(textBox, DigitsOnly);
        RegexValidator.SetIsValid(textBox, true);
        RegexValidator.SetValidationMode(textBox, RegexValidator.ValidationMode.Dynamic);
        RegexValidator.SetValidationType(textBox, RegexValidator.ValidationType.Integer);

        Assert.AreEqual(DigitsOnly, RegexValidator.GetRegex(textBox));
        Assert.IsTrue(RegexValidator.GetIsValid(textBox));
        Assert.AreEqual(RegexValidator.ValidationMode.Dynamic, RegexValidator.GetValidationMode(textBox));
        Assert.AreEqual(RegexValidator.ValidationType.Integer, RegexValidator.GetValidationType(textBox));
    });

    // ── Wrapping control scenario (_control branch) ─────────────────────────

    [TestMethod]
    public void WrappingControl_UsesControlValidationState() => StaTestRunner.Run(() =>
    {
        var textBox = new TextBox { Text = "abc" };
        var container = new ContentControl { Content = textBox };

        container.Measure(new Size(200, 50));
        container.Arrange(new Rect(0, 0, 200, 50));
        container.UpdateLayout();

        RegexValidator.SetRegex(container, DigitsOnly);
        RegexValidator.SetValidationMode(container, RegexValidator.ValidationMode.Forced);
        container.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, container));

        Assert.AreEqual(RegexValidator.ValidationMode.Forced, RegexValidator.GetValidationMode(container));
        Assert.IsFalse(RegexValidator.GetIsValid(container));

        InvokePrivateHandler("TextBox_LostFocus", textBox);

        Assert.AreEqual(string.Empty, textBox.Text, "The validation mode must be read from the wrapping control");
    });

    // ── Helpers ─────────────────────────────────────────────────────────────

    // ── Lifecycle branches ──────────────────────────────────────────────────

    [TestMethod]
    public void Loaded_WithoutTextBoxDescendant_DoesNothing() => StaTestRunner.Run(() =>
    {
        var container = new ContentControl { Content = "no text box here" };

        container.Measure(new Size(200, 50));
        container.Arrange(new Rect(0, 0, 200, 50));
        container.UpdateLayout();

        RegexValidator.SetRegex(container, DigitsOnly);
        container.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, container));

        Assert.IsFalse(RegexValidator.GetIsValid(container));
    });

    [TestMethod]
    public void Loaded_WhenControlIsAlreadyLoaded_InitializesImmediately() => StaTestRunner.Run(() =>
    {
        var window = new Window { Width = 300, Height = 200 };
        var textBox = new TextBox();
        window.Content = textBox;

        try
        {
            window.Show();
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.Loaded);
            Assert.IsTrue(textBox.IsLoaded);

            // Assigned while the control is loaded: validation runs immediately.
            RegexValidator.SetRegex(textBox, DigitsOnly);

            Assert.IsFalse(RegexValidator.GetIsValid(textBox));

            textBox.Text = "123";

            Assert.IsTrue(RegexValidator.GetIsValid(textBox));
        }
        finally
        {
            window.Close();
        }
    });

    [TestMethod]
    public void LoadedEvent_RevalidatesText() => StaTestRunner.Run(() =>
    {
        var textBox = CreateRegexTextBox(DigitsOnly, initialText: "123");

        RegexValidator.SetIsValid(textBox, false);
        textBox.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, textBox));

        Assert.IsTrue(RegexValidator.GetIsValid(textBox));
    });

    private static TextBox CreateRegexTextBox(
        string? regex = null,
        RegexValidator.ValidationMode mode = RegexValidator.ValidationMode.Normal,
        RegexValidator.ValidationType type = RegexValidator.ValidationType.Custom,
        string? initialText = null)
    {
        var textBox = new TextBox();

        if (regex is not null)
            RegexValidator.SetRegex(textBox, regex);

        if (mode != RegexValidator.ValidationMode.Normal)
            RegexValidator.SetValidationMode(textBox, mode);

        if (type != RegexValidator.ValidationType.Custom)
            RegexValidator.SetValidationType(textBox, type);

        if (initialText is not null)
            textBox.Text = initialText;

        textBox.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, textBox));

        return textBox;
    }

    private static void InvokePrivateHandler(string methodName, TextBox textBox, EventArgs? eventArgs = null)
    {
        var method = typeof(RegexValidator).GetMethod(methodName, PrivateStatic)
            ?? throw new InvalidOperationException($"Handler '{methodName}' was not found.");

        try
        {
            method.Invoke(null, [textBox, eventArgs!]);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
        }
    }
}
