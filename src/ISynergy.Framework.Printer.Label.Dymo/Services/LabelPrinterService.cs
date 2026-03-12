using DymoSDK.Implementations;
using DymoSDK.Interfaces;
using ISynergy.Framework.Printer.Label.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Printer.Label.Dymo.Services;

/// <summary>
/// Label printer class.
/// </summary>
/// <remarks>
/// <para>
/// <strong>AOT/Trimming notice:</strong> DymoSDK uses COM interop and reflection for printer enumeration and
/// label access. This service is Windows-only and is not compatible with Native AOT publishing. Applications
/// targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> cannot use this library. For non-AOT Windows
/// applications, suppress <c>IL2026</c> and <c>IL3050</c> warnings at the
/// <c>AddPrinterLabelDymoIntegration</c> call site.
/// </para>
/// </remarks>
[RequiresUnreferencedCode("DymoSDK uses COM interop and reflection for printer access. Not compatible with AOT publishing.")]
[RequiresDynamicCode("DymoSDK requires dynamic code generation for COM/native interop.")]
internal class LabelPrinterService : ILabelPrinterService
{
    private readonly ILogger _logger;
    private readonly IDymoLabel _label;
    private readonly List<string> _twinTurboRolls;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public LabelPrinterService(ILogger<LabelPrinterService> logger)
    {
        _logger = logger;

        DymoSDK.App.Init();

        _label = DymoLabel.LabelSharedInstance;
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
        var printers = DymoPrinter.Instance.GetPrinters();
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
