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
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Mvvm
{
    /// <summary>
    /// Class ViewModelDialog.
    /// Implements the <see cref="ViewModel" />
    /// Implements the <see cref="IViewModelDialog{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="ViewModel" />
    /// <seealso cref="IViewModelDialog{TEntity}" />
    public abstract class ViewModelDialog<TEntity> : ViewModel, IViewModelDialog<TEntity>
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
        /// Gets or sets the IsNew property value.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        /// <value>The submit command.</value>
        public Command<TEntity> Submit_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelDialog{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="automaticValidation"></param>
        protected ViewModelDialog(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            bool automaticValidation = false) 
            : base(context, commonServices, loggerFactory, automaticValidation)
        {
            Validator = new Action<IObservableClass>(arg =>
            {
                if (arg is ViewModelDialog<TEntity> vm &&
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

            Submit_Command = new Command<TEntity>(async e => await SubmitAsync(e));
        }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void SetSelectedItem(TEntity entity)
        {
            SelectedItem = entity;
            IsNew = false;
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public virtual Task SubmitAsync(TEntity e)
        {
            OnSubmitted(new SubmitEventArgs<TEntity>(e));
            return CloseAsync();
        }
    }
}
