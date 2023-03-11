using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels
{
    public class DialogsViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Dialogs"); } }

        /// <summary>
        /// Show Yes/No dialog.
        /// </summary>
        public AsyncRelayCommand ShowDialogYesNo { get; set; }

        /// <summary>
        /// Show Ok dialog.
        /// </summary>
        public AsyncRelayCommand ShowDialogOk { get; set; }

        /// <summary>
        /// Show Ok/Cancel dialog.
        /// </summary>
        public AsyncRelayCommand ShowDialogOkCancel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        public DialogsViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            ShowDialogYesNo = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.YesNo));
            ShowDialogOk = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.OK));
            ShowDialogOkCancel = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.OKCancel));
        }

        private async Task ShowDialogAsync(MessageBoxButton buttons)
        {
            if (await BaseCommonServices.DialogService.ShowMessageAsync(
                                $"Testing {buttons} Dialog",
                                "Test",
                                buttons) is MessageBoxResult result)
            {
                await BaseCommonServices.DialogService.ShowInformationAsync($"{result} selected.", "Result...");
            };
        }
    }
}
