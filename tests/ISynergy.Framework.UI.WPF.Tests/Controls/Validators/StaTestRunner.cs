using ISynergy.Framework.UI.Controls.Validators;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace ISynergy.Framework.UI.WPF.Controls.Validators;

/// <summary>
/// Runs a test body on a dedicated STA thread, which is required to create and drive
/// WPF objects such as <see cref="System.Windows.Controls.TextBox"/>.
/// </summary>
internal static class StaTestRunner
{
    public static void Run(Action action)
    {
        Exception? capturedException = null;

        var thread = new Thread(() =>
        {
            try
            {
                ResetValidatorState();
                action();
            }
            catch (Exception exception)
            {
                capturedException = exception;
            }
            finally
            {
                ResetValidatorState();
            }
        })
        {
            IsBackground = true
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        if (!thread.Join(TimeSpan.FromSeconds(30)))
            throw new TimeoutException("The STA test thread did not complete within 30 seconds.");

        if (capturedException is not null)
            ExceptionDispatchInfo.Capture(capturedException).Throw();
    }

    /// <summary>
    /// Clears the static state (the wrapping control cache) of the validators so tests do not
    /// observe state leaked from a previous test.
    /// </summary>
    private static void ResetValidatorState()
    {
        ResetControlField(typeof(MaskValidator));
        ResetControlField(typeof(RegexValidator));
    }

    private static void ResetControlField(Type validatorType) =>
        validatorType
            .GetField("_control", BindingFlags.NonPublic | BindingFlags.Static)?
            .SetValue(null, null);
}
