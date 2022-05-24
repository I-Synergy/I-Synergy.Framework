using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    /// <summary>
    /// Class ViewModelBlade.
    /// Implements the <see cref="ViewModel" />
    /// Implements the <see cref="IViewModelBlade" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="ViewModel" />
    /// <seealso cref="IViewModelBlade" />
    public abstract class ViewModelBlade<TEntity> : ViewModel, IViewModelBlade
    {
        /// <summary>
        /// Occurs when [submitted].
        /// </summary>
        public event EventHandler<SubmitEventArgs<TEntity>> Submitted;
        /// <summary>
        /// Called when [submitted].
        /// </summary>
        /// <param name="e">The e.</param>
        protected virtual void OnSubmitted(SubmitEventArgs<TEntity> e) => Submitted?.Invoke(this, e);

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
        /// <value>The selected item.</value>
        public TEntity SelectedItem
        {
            get { return GetValue<TEntity>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Owner property value.
        /// </summary>
        /// <value>The owner.</value>
        public IViewModelBladeView Owner
        {
            get { return GetValue<IViewModelBladeView>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsNew property value.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsDisabled property value.
        /// </summary>
        /// <value><c>true</c> if this instance is disabled; otherwise, <c>false</c>.</value>
        public bool IsDisabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        /// <value>The submit command.</value>
        public Command Submit_Command { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBlade{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="automaticValidation"></param>
        protected ViewModelBlade(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger,
            bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation)
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
            Submit_Command = new Command(async () => await SubmitAsync(SelectedItem));
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public virtual Task SubmitAsync(TEntity e)
        {
            if (Validate())
            {
                OnSubmitted(new SubmitEventArgs<TEntity>(e));
                return CloseAsync();
            }

            return Task.CompletedTask;
        }
    }
}
