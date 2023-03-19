using DymoSDK.Implementations;
using DymoSDK.Interfaces;
using ISynergy.Framework.Printer.Label.Abstractions.Services;

namespace ISynergy.Framework.Printer.Label.Dymo.Services
{
    /// <summary>
    /// Label printer class.
    /// </summary>
    internal class LabelPrinterService : ILabelPrinterService
    {
        private readonly IDymoLabel _label;
        private readonly IEnumerable<IPrinter> _printers;
        private readonly List<string> _twinTurboRolls;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabelPrinterService()
        {
            DymoSDK.App.Init();

            _label = DymoLabel.Instance;
            _printers = DymoPrinter.Instance.GetPrinters();
            _twinTurboRolls = new List<string>() { "Auto", "Left", "Right" };
        }

        /// <summary>
        /// Prints the dymo label.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="copies"></param>
        /// <exception cref="NotImplementedException"></exception>
        public Task PrintLabelAsync(string content, int copies = 1)
        {
            if (_printers.Any() && _printers.First() is IPrinter printer)
            {
                // Send to print.
                if (printer.Name.Contains("Twin Turbo"))
                    DymoPrinter.Instance.PrintLabel(_label, printer.Name, copies, rollSelected: 0);
                else
                    DymoPrinter.Instance.PrintLabel(_label, printer.Name, copies);
            }
            else
            {
                throw new NotSupportedException("There is no Dymo label printer installed.");
            }

            return Task.CompletedTask;
        }
    }
}
