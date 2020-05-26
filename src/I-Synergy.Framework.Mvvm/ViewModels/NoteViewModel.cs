using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Core.Attributes;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    public class NoteViewModel : ViewModelDialog<string>
    {
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("Note");
            }
        }

        private readonly string _targetProperty;

        [PreferredConstructor]
        public NoteViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            string note)
            : base(context, commonServices, loggerFactory)
        {
            SelectedItem = note;
        }

        public NoteViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            string note,
            string targetProperty)
            : this(context, commonServices, loggerFactory, note)
        {
            _targetProperty = targetProperty;
        }

        protected override void OnSubmitted(SubmitEventArgs<string> e)
        {
            if(!string.IsNullOrEmpty(_targetProperty))
            {
                base.OnSubmitted(new SubmitEventArgs<string>(e.Owner, e.Result, _targetProperty));
            }
            else
            {
                base.OnSubmitted(e);
            }
        }
    }
}
