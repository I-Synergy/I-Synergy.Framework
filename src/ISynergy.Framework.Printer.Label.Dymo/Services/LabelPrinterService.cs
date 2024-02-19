using DymoSDK.Implementations;
using DymoSDK.Interfaces;
using ISynergy.Framework.Printer.Label.Abstractions.Services;

namespace ISynergy.Framework.Printer.Label.Dymo.Services;

/// <summary>
/// Label printer class.
/// </summary>
internal class LabelPrinterService : ILabelPrinterService
{
    private readonly IDymoLabel _label;
    private readonly List<string> _twinTurboRolls;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public LabelPrinterService()
    {
        DymoSDK.App.Init();

        _label = DymoLabel.Instance;
        _twinTurboRolls = ["Auto", "Left", "Right"];
    }

    /// <summary>
    /// Prints the dymo label.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="copies"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task PrintLabelAsync(string content, int copies = 1)
    {
        var printers = await DymoPrinter.Instance.GetPrinters();
        if (printers.Any() && printers.First() is { } printer)
        {
            // Send to print.
            if (printer.Name.Contains("Twin Turbo"))
                await DymoPrinter.Instance.PrintLabel(_label, printer.Name, copies, rollSelected: 0);
            else
                await DymoPrinter.Instance.PrintLabel(_label, printer.Name, copies);
        }
        else
        {
            throw new NotSupportedException("There is no Dymo label printer installed.");
        }
    }
}
