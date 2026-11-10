using ISynergy.Framework.UI.Controls.Validators;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ISynergy.Framework.UI.WPF.Controls.Validators;

/// <summary>
/// Tests for <see cref="MaskValidator"/>. Every test body is executed on an STA thread because the
/// validator drives real WPF <see cref="TextBox"/> instances and routed events.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class MaskValidatorTests
{
    private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

    // ── Mask rendering ───────────────────────────────────────────────────────

    [TestMethod]
    public void Loaded_WithEmptyText_RendersMaskUsingDefaultPlaceholder() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999-999");

        Assert.AreEqual("___-___", textBox.Text);
        Assert.AreEqual(0, textBox.SelectionStart);
    });

    [TestMethod]
    public void Loaded_WithCustomPlaceholder_RendersMaskUsingCustomPlaceholder() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("aa-99", placeholder: "#");

        Assert.AreEqual("##-##", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithEscapedAlphaCharacter_KeepsEscapedCharacterLiteral() => StaTestRunner.Run(() =>
    {
        // "99\a9": the 'a' preceded by the escape character is a literal, the last '9' stays dynamic.
        var textBox = CreateMaskedTextBox("99\\a9");

        Assert.AreEqual("__a_", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithEscapedDigit_KeepsEscapedDigitLiteral() => StaTestRunner.Run(() =>
    {
        // "99\99": the '9' after the escape character is a literal, the other digits are dynamic.
        var textBox = CreateMaskedTextBox("99\\99");

        Assert.AreEqual("__9_", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithoutMask_LeavesTextUntouched() => StaTestRunner.Run(() =>
    {
        var textBox = new TextBox { Text = "abc" };

        InvokePrivateHandler("Textbox_Loaded", textBox);

        Assert.AreEqual("abc", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithEmptyPlaceholder_Throws() => Assert.ThrowsExactly<ArgumentNullException>(() => StaTestRunner.Run(() =>
    {
        var textBox = new TextBox();
        MaskValidator.SetMask(textBox, "99");
        MaskValidator.SetMaskPlaceholder(textBox, string.Empty);

        RaiseLoaded(textBox);
    }));

    // ── Initial text filtering ───────────────────────────────────────────────

    [TestMethod]
    public void Loaded_WithValidInitialText_CopiesEveryCharacter() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999", initialText: "123");

        Assert.AreEqual("123", textBox.Text);
        Assert.AreEqual(0, textBox.SelectionStart);
    });

    [TestMethod]
    public void Loaded_WithInvalidInitialText_ReplacesInvalidCharactersWithPlaceholder() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999", initialText: "1a3");

        Assert.AreEqual("1_3", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithInitialTextAndEscapedChar_KeepsEscapedCharacter() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("9-9", initialText: "12");

        // The mask literal '-' at index 1 is not overwritten by the initial value.
        Assert.AreEqual("1-_", textBox.Text);
    });

    // ── Custom masks ────────────────────────────────────────────────────────

    [TestMethod]
    public void Loaded_WithCustomMask_AcceptsCustomCharacters() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("xx", customMask: "x:[A-F]", initialText: "AF");

        Assert.AreEqual("AF", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithCustomMask_RejectsCharactersOutsideCustomPattern() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("xx", customMask: "x:[A-F]", initialText: "AG");

        Assert.AreEqual("A_", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithMultipleCustomMasks_AppliesEachRole() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("xy", customMask: "x:[A-F],y:[0-3]", initialText: "Z1");

        Assert.AreEqual("_1", textBox.Text);
    });

    [TestMethod]
    public void Loaded_WithCustomMaskWithoutSeparator_Throws() => Assert.Throws<ArgumentException>(() => StaTestRunner.Run(() =>
    {
        CreateMaskedTextBox("99", customMask: "invalid");
    }));

    [TestMethod]
    public void Loaded_WithCustomMaskInvalidKey_Throws() => Assert.Throws<ArgumentException>(() => StaTestRunner.Run(() =>
    {
        CreateMaskedTextBox("99", customMask: "AB:[0-9]");
    }));

    [TestMethod]
    public void Loaded_WithCustomMaskInvalidRegex_Throws() => Assert.Throws<ArgumentException>(() => StaTestRunner.Run(() =>
    {
        CreateMaskedTextBox("99", customMask: "x:[A-");
    }));

    // ── GotFocus ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void GotFocus_MovesCaretToFirstPlaceholder() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999", initialText: "12");
        Assert.AreEqual("12_", textBox.Text);

        textBox.SelectionStart = 0;
        InvokePrivateHandler("Textbox_GotFocus", textBox);

        Assert.AreEqual(2, textBox.SelectionStart);
    });

    [TestMethod]
    public void GotFocus_WithFullyFilledMask_KeepsCaretPosition() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("99", initialText: "12");

        textBox.SelectionStart = 0;
        InvokePrivateHandler("Textbox_GotFocus", textBox);

        Assert.AreEqual(0, textBox.SelectionStart);
    });

    [TestMethod]
    public void GotFocus_WithoutMask_DoesNothing() => StaTestRunner.Run(() =>
    {
        var textBox = new TextBox { Text = "abc" };

        InvokePrivateHandler("Textbox_GotFocus", textBox);

        Assert.AreEqual("abc", textBox.Text);
        Assert.AreEqual(0, textBox.SelectionStart);
    });

    // ── SelectionChanged ─────────────────────────────────────────────────────

    [TestMethod]
    public void SelectionChanged_StoresSelectionStateOnTextBox() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999");

        textBox.SelectionStart = 1;
        textBox.SelectionLength = 2;
        InvokePrivateHandler("Textbox_SelectionChanged", textBox);

        Assert.AreEqual(1, GetPrivateAttachedValue(textBox, "OldSelectionStartProperty"));
        Assert.AreEqual(2, GetPrivateAttachedValue(textBox, "OldSelectionLengthProperty"));
    });

    // ── PreviewKeyDown ───────────────────────────────────────────────────────

    [TestMethod]
    public void PreviewKeyDown_WithoutMaskState_ReturnsEarly() => StaTestRunner.Run(() =>
    {
        var textBox = new TextBox { Text = "abc" };

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("abc", textBox.Text);
    });

    [TestMethod]
    public void PreviewKeyDown_AtEndOfMask_TruncatesAdditionalInput() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999");

        textBox.Text = "1234";
        textBox.SelectionStart = 3;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("123", textBox.Text);
        Assert.AreEqual(3, textBox.SelectionStart);
    });

    [TestMethod]
    public void PreviewKeyDown_ProgrammaticTextChange_FiltersInvalidCharacters() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999");

        textBox.Text = "1a3";

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("1_3", textBox.Text);
        Assert.AreEqual("1_3", GetPrivateAttachedValue(textBox, "OldTextProperty"));
    });

    [TestMethod]
    public void PreviewKeyDown_ProgrammaticEmptyText_RestoresDisplayText() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999");

        textBox.Text = string.Empty;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("___", textBox.Text);
    });

    [TestMethod]
    public void PreviewKeyDown_Backspace_ResetsRemovedCharacterToPlaceholder() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999", initialText: "12");
        Assert.AreEqual("12_", textBox.Text);

        textBox.Text = "1_";
        textBox.SelectionStart = 1;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("1__", textBox.Text);
        Assert.AreEqual(1, textBox.SelectionStart);
    });

    [TestMethod]
    public void PreviewKeyDown_DynamicCharacterMatch_KeepsTypedCharacter() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("999");

        textBox.Text = "1__";
        textBox.SelectionStart = 1;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("1__", textBox.Text);
        Assert.AreEqual(2, textBox.SelectionStart);
    });

    [TestMethod]
    public void PreviewKeyDown_DynamicCharacterMismatchWithoutSelection_KeepsOldCharacter() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("aaa");

        textBox.Text = "1__";
        textBox.SelectionStart = 1;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("___", textBox.Text);
        Assert.AreEqual(1, textBox.SelectionStart);
    });

    [TestMethod]
    public void PreviewKeyDown_DynamicCharacterMismatchWithSelection_ResetsSelectionToPlaceholders() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("aaa");

        textBox.Text = "12_";
        textBox.SelectionStart = 1;
        textBox.SelectionLength = 2;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("___", textBox.Text);
        Assert.AreEqual(1, textBox.SelectionStart);
    });

    [TestMethod]
    public void PreviewKeyDown_FixedCharacter_KeepsMaskLiteral() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("9-9");

        textBox.Text = "1a_";
        textBox.SelectionStart = 1;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("_-_", textBox.Text);
        Assert.AreEqual(2, textBox.SelectionStart);
    });

    [TestMethod]
    public void PreviewKeyDown_DeleteOfSelectionOverFixedCharacter_KeepsMaskLiteral() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("9-9");

        textBox.Text = "__";
        textBox.SelectionStart = 1;
        textBox.SelectionLength = 1;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("_-_", textBox.Text);
        Assert.AreEqual(1, textBox.SelectionStart);
    });

    // ── Wrapping control scenario (_control branch) ───────────────────────────

    [TestMethod]
    public void GotFocus_WithWrappingControl_UsesControlState() => StaTestRunner.Run(() =>
    {
        var (_, textBox) = CreateWrappedMaskedTextBox("999", initialText: "12");
        Assert.AreEqual("12_", textBox.Text);

        textBox.SelectionStart = 0;
        InvokePrivateHandler("Textbox_GotFocus", textBox);

        Assert.AreEqual(2, textBox.SelectionStart);
    });

    [TestMethod]
    public void SelectionChanged_WithWrappingControl_StoresStateOnControl() => StaTestRunner.Run(() =>
    {
        var (container, textBox) = CreateWrappedMaskedTextBox("999");

        textBox.SelectionStart = 2;
        textBox.SelectionLength = 1;
        InvokePrivateHandler("Textbox_SelectionChanged", textBox);

        Assert.AreEqual(2, GetPrivateAttachedValue(container, "OldSelectionStartProperty"));
        Assert.AreEqual(1, GetPrivateAttachedValue(container, "OldSelectionLengthProperty"));
    });

    // ── Attached property plumbing ──────────────────────────────────────────

    [TestMethod]
    public void AttachedProperties_RoundTripValues() => StaTestRunner.Run(() =>
    {
        var textBox = new TextBox();

        Assert.AreEqual(string.Empty, MaskValidator.GetMask(textBox));
        Assert.AreEqual("_", MaskValidator.GetMaskPlaceholder(textBox));
        Assert.AreEqual(string.Empty, MaskValidator.GetCustomMask(textBox));

        MaskValidator.SetMask(textBox, "99");
        MaskValidator.SetMaskPlaceholder(textBox, "#");
        MaskValidator.SetCustomMask(textBox, "x:[A-F]");

        Assert.AreEqual("99", MaskValidator.GetMask(textBox));
        Assert.AreEqual("#", MaskValidator.GetMaskPlaceholder(textBox));
        Assert.AreEqual("x:[A-F]", MaskValidator.GetCustomMask(textBox));
    });

    // ── Lifecycle branches ──────────────────────────────────────────────────

    [TestMethod]
    public void Loaded_WithoutTextBoxDescendant_DoesNothing() => StaTestRunner.Run(() =>
    {
        var container = new ContentControl { Content = "no text box here" };

        container.Measure(new Size(200, 50));
        container.Arrange(new Rect(0, 0, 200, 50));
        container.UpdateLayout();

        MaskValidator.SetMask(container, "999");
        RaiseLoaded(container);

        Assert.AreEqual(string.Empty, MaskValidator.GetCustomMask(container));
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

            // The mask is applied through the already-loaded branch of MaskPropertyOnChange.
            MaskValidator.SetMask(textBox, "99-99");

            Assert.AreEqual("__-__", textBox.Text);
        }
        finally
        {
            window.Close();
        }
    });

    [TestMethod]
    public void PreviewKeyDown_NoFurtherDynamicCharacter_UsesCurrentIndex() => StaTestRunner.Run(() =>
    {
        var textBox = CreateMaskedTextBox("9-");

        textBox.Text = "1-";
        textBox.SelectionStart = 1;

        InvokePrivateHandler("TextBox_PreviewKeyDown", textBox);

        Assert.AreEqual("_-", textBox.Text);
        Assert.AreEqual(2, textBox.SelectionStart);
    });

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static TextBox CreateMaskedTextBox(
        string mask,
        string? placeholder = null,
        string? customMask = null,
        string? initialText = null)
    {
        var textBox = new TextBox();

        MaskValidator.SetMask(textBox, mask);

        if (placeholder is not null)
            MaskValidator.SetMaskPlaceholder(textBox, placeholder);

        if (customMask is not null)
            MaskValidator.SetCustomMask(textBox, customMask);

        if (initialText is not null)
            textBox.Text = initialText;

        RaiseLoaded(textBox);

        return textBox;
    }

    private static (ContentControl Container, TextBox TextBox) CreateWrappedMaskedTextBox(
        string mask,
        string? initialText = null)
    {
        var textBox = new TextBox();
        var container = new ContentControl { Content = textBox };

        // Applies the control template so the TextBox becomes a visual descendant.
        container.Measure(new Size(200, 50));
        container.Arrange(new Rect(0, 0, 200, 50));
        container.UpdateLayout();

        MaskValidator.SetMask(container, mask);

        if (initialText is not null)
            textBox.Text = initialText;

        RaiseLoaded(container);

        return (container, textBox);
    }

    private static void RaiseLoaded(FrameworkElement element) =>
        element.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, element));

    private static void InvokePrivateHandler(string methodName, TextBox textBox, EventArgs? eventArgs = null)
    {
        var method = typeof(MaskValidator).GetMethod(methodName, PrivateStatic)
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

    private static object? GetPrivateAttachedValue(DependencyObject target, string propertyFieldName)
    {
        var property = (DependencyProperty)typeof(MaskValidator)
            .GetField(propertyFieldName, PrivateStatic)!
            .GetValue(null)!;

        return target.GetValue(property);
    }
}
