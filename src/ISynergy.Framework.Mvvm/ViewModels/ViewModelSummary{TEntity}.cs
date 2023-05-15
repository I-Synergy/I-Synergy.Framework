using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    public abstract class ViewModelSummary<TEntity> : ViewModel, IViewModelSummary<TEntity>
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
        /// Gets or sets the Items property value.
        /// </summary>
        /// <value>The items.</value>
        public ObservableCollection<TEntity> Items
        {
            get => GetValue<ObservableCollection<TEntity>>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
        /// <value>The selected item.</value>
        public TEntity SelectedItem
        {
            get => GetValue<TEntity>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the IsNew property value.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the submit command.
        /// </summary>
        /// <value>The submit command.</value>
        public AsyncRelayCommand<TEntity> SubmitCommand { get; set; }

        /// <summary>
        /// Gets a value indicating whether [refresh on initialization].
        /// </summary>
        /// <value><c>true</c> if [refresh on initialization]; otherwise, <c>false</c>.</value>
        public bool RefreshOnInitialization { get; }

        /// <summary>
        /// Gets or sets the add command.
        /// </summary>
        /// <value>The add command.</value>
        public AsyncRelayCommand AddCommand { get; set; }
        /// <summary>
        /// Gets or sets the edit command.
        /// </summary>
        /// <value>The edit command.</value>
        public AsyncRelayCommand<TEntity> EditCommand { get; set; }
        /// <summary>
        /// Gets or sets the delete command.
        /// </summary>
        /// <value>The delete command.</value>
        public AsyncRelayCommand<TEntity> DeleteCommand { get; set; }
        /// <summary>
        /// Gets or sets the refresh command.
        /// </summary>
        /// <value>The refresh command.</value>
        public AsyncRelayCommand RefreshCommand { get; set; }
        /// <summary>
        /// Gets or sets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public AsyncRelayCommand<object> SearchCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelSummary{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="refreshOnInitialization">if set to <c>true</c> [refresh on initialization].</param>
        /// <param name="automaticValidation"></param>
        protected ViewModelSummary(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger,
            bool refreshOnInitialization = true,
            bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation)
        {
            RefreshOnInitialization = refreshOnInitialization;

            Validator = new Action<IObservableClass>(arg =>
            {
                if (arg is ViewModelSummary<TEntity> vm &&
                    vm.SelectedItem is IObservableClass selectedItem)
                {
                }
            });

            Items = new ObservableCollection<TEntity>();

            AddCommand = new AsyncRelayCommand(async () => await AddAsync());
            EditCommand = new AsyncRelayCommand<TEntity>(async (e) => await EditAsync(e.Clone()));
            DeleteCommand = new AsyncRelayCommand<TEntity>(async (e) => await DeleteAsync(e));
            RefreshCommand = new AsyncRelayCommand(async () => await RefreshAsync());
            SearchCommand = new AsyncRelayCommand<object>(async (e) => await SearchAsync(e));
            SubmitCommand = new AsyncRelayCommand<TEntity>(async (e) => await SubmitAsync(e));
        }

        /// <summary>
        /// initialize as an asynchronous operation.
        /// </summary>
        /// <returns>Task.</returns>
        public override async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                if (RefreshOnInitialization)
                    IsInitialized = await RefreshAsync();
            }
        }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="e">The e.</param>
        public virtual Task SetSelectedItemAsync(TEntity e)
        {
            SelectedItem = e;
            IsNew = false;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public abstract Task AddAsync();
        /// <summary>
        /// Edits the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public abstract Task EditAsync(TEntity e);

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="e">The e.</param>
        public async Task DeleteAsync(TEntity e)
        {
            string item;
            if (e.GetType().GetProperty("Description").GetValue(e) is string value)
            {
                item = value;
            }
            else
            {
                item = BaseCommonServices.LanguageService.GetString("ThisItem");
            }

            if (await BaseCommonServices.DialogService.ShowMessageAsync(
                                string.Format(BaseCommonServices.LanguageService.GetString("WarningItemRemove"), item),
                                BaseCommonServices.LanguageService.GetString("Delete"),
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await RemoveAsync(e);
                await RefreshAsync();
            }
        }

        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public abstract Task RemoveAsync(TEntity e);
        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public abstract Task SearchAsync(object e);

        /// <summary>
        /// Refreshes the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> RefreshAsync() => GetItemsAsync();

        /// <summary>
        /// The get items cancellation token
        /// </summary>
        private CancellationTokenSource GetItemsCancellationToken = new CancellationTokenSource();

        /// <summary>
        /// get items as an asynchronous operation.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> GetItemsAsync()
        {
            var result = false;

            var cancellationToken = new CancellationTokenSource();

            try
            {
                BaseCommonServices.BusyService.StartBusy();

                var items = await RetrieveItemsAsync(cancellationToken.Token);

                Items.AddNewRange(items);
                result = true;
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Logger.LogTrace("User canceled request.");
            }
            catch (OperationCanceledException)
            {
                Logger.LogTrace("Request timed out.");
            }
            finally
            {
                BaseCommonServices.BusyService.EndBusy();
            }

            return result;
        }

        /// <summary>
        /// Retrieves the items asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
        public abstract Task<List<TEntity>> RetrieveItemsAsync(CancellationToken cancellationToken);

        public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(GenericConstants.Parameter, out object result) && result is TEntity entity)
                SetSelectedItemAsync(entity);
        }

        /// <summary>
        /// Submits the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public virtual Task SubmitAsync(TEntity e)
        {
            if (Validate())
                OnSubmitted(new SubmitEventArgs<TEntity>(e));

            return Task.CompletedTask;
        }
    }
}
