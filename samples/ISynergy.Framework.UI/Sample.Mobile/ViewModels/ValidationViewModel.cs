﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels
{
    public class ValidationViewModel : ViewModelNavigation<TestItem>
    {
        public RelayCommand CreateInstance_Command { get; set; }

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

            CreateInstance_Command = new RelayCommand(() => SelectedItem = new TestItem { Id = -1, Description = "Hi" });
        }

        public override async Task SubmitAsync(TestItem e)
        {
            if (Validate())
            {
                await BaseCommonServices.DialogService.ShowInformationAsync($"Validation succeeded.");
            }
        }
    }
}