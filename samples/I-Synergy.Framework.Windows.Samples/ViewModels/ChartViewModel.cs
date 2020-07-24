using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Windows.Samples.ViewModels
{
    public class ChartViewModel : ViewModelNavigation<object>
    {
        public override string Title { get { return "Chart examples"; } }

        public ChartViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
        }
    }
}
