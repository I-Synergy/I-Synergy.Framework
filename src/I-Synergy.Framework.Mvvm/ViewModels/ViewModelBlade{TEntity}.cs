using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Core.Utilities;

namespace ISynergy.Framework.Mvvm
{
    public abstract class ViewModelBlade<TEntity> : ViewModel, IViewModelBlade
    {
        public event EventHandler<SubmitEventArgs<TEntity>> Submitted;
        protected virtual void OnSubmitted(SubmitEventArgs<TEntity> e) => Submitted?.Invoke(this, e);

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
        public TEntity SelectedItem
        {
            get { return GetValue<TEntity>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Owner property value.
        /// </summary>
        public IViewModelBladeView Owner
        {
            get { return GetValue<IViewModelBladeView>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsNew property value.
        /// </summary>
        public bool IsNew
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsDisabled property value.
        /// </summary>
        public bool IsDisabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public RelayCommand Submit_Command { get; }

        protected ViewModelBlade(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory) 
            : base(context, commonServices, loggerFactory)
        {
            Validator = new Action<IObservableClass>(arg =>
            {
                if (arg is ViewModelBlade<TEntity> vm &&
                    vm.SelectedItem is IObservableClass selectedItem)
                {
                    if (!selectedItem.Validate())
                    {
                        foreach (var error in selectedItem.Errors)
                        {
                            vm.Properties[nameof(vm.SelectedItem)].Errors.Add(error);
                        }
                    }
                }
            });

            SelectedItem = TypeActivator.CreateInstance<TEntity>();
            IsNew = true;
            Submit_Command = new RelayCommand(async () => await SubmitAsync(SelectedItem));
        }

        public virtual Task SubmitAsync(TEntity e)
        {
            OnSubmitted(new SubmitEventArgs<TEntity>(e));
            return CloseAsync();
        }
    }
}
