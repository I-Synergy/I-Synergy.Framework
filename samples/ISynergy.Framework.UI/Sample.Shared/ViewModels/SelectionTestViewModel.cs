using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class SelectionTestViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Converters"); } }

        public Command Select_Command { get; set; }

        public ObservableCollection<TestItem> TestItems { get; set; }

        public SelectionTestViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            TestItems = new ObservableCollection<TestItem>()
            {

            };

            Select_Command = new Command(async () => await SelectAsync());
        }

        private Task SelectAsync()
        {
            throw new NotImplementedException();
        }
    }
}
