using ISynergy.Framework.Mvvm.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Core.Utilities;

namespace ISynergy.Framework.Mvvm
{
    public abstract class ViewModelBladeView<TEntity> : ViewModel, IViewModelBladeView
    {
        public event EventHandler<SubmitEventArgs<TEntity>> Submitted;
        protected virtual void OnSubmitted(SubmitEventArgs<TEntity> e) => Submitted?.Invoke(this, e);

        /// <summary>
        /// Gets or sets the Blades property value.
        /// </summary>
        public ObservableCollection<IView> Blades
        {
            get { return GetValue<ObservableCollection<IView>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Items property value.
        /// </summary>
        public ObservableCollection<TEntity> Items
        {
            get { return GetValue<ObservableCollection<TEntity>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsPaneEnabled property value.
        /// </summary>
        public bool IsPaneEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
        public TEntity SelectedItem
        {
            get { return GetValue<TEntity>(); }
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

        public RelayCommand<TEntity> Submit_Command { get; set; }

        public bool RefreshOnInitialization { get; }

        public RelayCommand Add_Command { get; set; }
        public RelayCommand<TEntity> Edit_Command { get; set; }
        public RelayCommand<TEntity> Delete_Command { get; set; }
        public RelayCommand Refresh_Command { get; set; }
        public RelayCommand<object> Search_Command { get; set; }

        protected ViewModelBladeView(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            bool refreshOnInitialization = true) 
            : base(context, commonServices, loggerFactory)
        {
            RefreshOnInitialization = refreshOnInitialization;

            Validator = new Action<IObservableClass>(arg =>
            {
                if (arg is ViewModelBladeView<TEntity> vm &&
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

            Items = new ObservableCollection<TEntity>();
            Blades = new ObservableCollection<IView>();

            Add_Command = new RelayCommand(async () => await AddAsync());
            Edit_Command = new RelayCommand<TEntity>(async (e) => await EditAsync(e.Clone()));
            Delete_Command = new RelayCommand<TEntity>(async (e) => await DeleteAsync(e));
            Refresh_Command = new RelayCommand(async () => await RefreshAsync());
            Search_Command = new RelayCommand<object>(async (e) => await SearchAsync(e));
            Submit_Command = new RelayCommand<TEntity>(async (e) => await SubmitAsync(e));
        }

        public override async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                await base.InitializeAsync();

                if(RefreshOnInitialization)
                    IsInitialized = await RefreshAsync();
            }
        }

        public virtual void SetSelectedItem(TEntity entity)
        {
            SelectedItem = entity;
            IsNew = false;
        }

        public abstract Task AddAsync();
        public abstract Task EditAsync(TEntity e);

        public async Task DeleteAsync(TEntity e)
        {
            string item;
            if (e.GetType().GetProperty("Description").GetValue(e) is string value)
            {
                item = value;
            }
            else
            {
                item = BaseCommonServices.LanguageService.GetString("This_Item");
            }

            if (await BaseCommonServices.DialogService.ShowAsync(
                                string.Format(BaseCommonServices.LanguageService.GetString("Warning_Item_Remove"), item),
                                BaseCommonServices.LanguageService.GetString("Operation_Delete"),
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await RemoveAsync(e);
                await RefreshAsync();
            }
        }

        public abstract Task RemoveAsync(TEntity e);
        public abstract Task SearchAsync(object e);

        public virtual Task<bool> RefreshAsync() => GetItemsAsync();

        private CancellationTokenSource GetItemsCancellationToken = new CancellationTokenSource();

        public async Task<bool> GetItemsAsync()
        {
            var result = false;

            GetItemsCancellationToken.Cancel();
            GetItemsCancellationToken = new CancellationTokenSource();

            try
            {
                await BaseCommonServices.BusyService.StartBusyAsync();

                Items.AddNewRange(await RetrieveItemsAsync(GetItemsCancellationToken.Token));
                result = true;

                GetItemsCancellationToken.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException) { }
            catch (OperationCanceledException) { }
            finally
            {
                await BaseCommonServices.BusyService.EndBusyAsync();
            }

            return result;
        }

        public abstract Task<List<TEntity>> RetrieveItemsAsync(CancellationToken cancellationToken);

        public virtual Task SubmitAsync(TEntity e)
        {
            OnSubmitted(new SubmitEventArgs<TEntity>(e));
            return Task.CompletedTask;
        }
    }
}
