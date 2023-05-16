using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels
{
    public class ValidationViewModel : ViewModelNavigation<TestItem>
    {
        public RelayCommand CreateInstanceCommand { get; set; }
        public RelayCommand<TestItem> ValidateCommand { get; set; }

        public ValidationViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            Validator = new Action<IObservableClass>(_ =>
            {
                if (SelectedItem is null)
                    AddValidationError(nameof(SelectedItem), "SelectedItem cannot be null!");
            });

            CreateInstanceCommand = new RelayCommand(() => SelectedItem = new TestItem { Id = -1, Description = "Hi" });
            ValidateCommand = new RelayCommand<TestItem>(ValidateTest);
        }

        private void ValidateTest(TestItem e)
        {
            Argument.IsNotNull(SelectedItem);
            Argument.IsNotNull(SelectedItem.Description);

            if (Validate())
            {
                var task = new Task(() => BaseCommonServices.DialogService.ShowInformationAsync($"Validation succeeded."));
                task.RunSynchronously();
            }
        }

        public override async Task SubmitAsync(TestItem e)
        {
            Argument.IsNotNull(SelectedItem);
            Argument.IsNotNull(SelectedItem.Description);

            if (Validate())
            {
                await BaseCommonServices.DialogService.ShowInformationAsync($"Validation succeeded.");
            }
        }
    }
}
