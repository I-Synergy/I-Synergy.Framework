using Flurl.Http;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Models.Base;
using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Events;
using ISynergy.Enumerations;

namespace ISynergy.ViewModels.Base
{
    public abstract class ViewModelBladeView<TEntity> : BaseNavigationViewModel<TEntity>, IViewModelBladeView<TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// Gets or sets the Blades property value.
        /// </summary>
        public ObservableCollection<IView> Blades
        {
            get { return GetValue<ObservableCollection<IView>>(); }
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
        /// Gets or sets the Items property value.
        /// </summary>
        public ObservableCollection<TEntity> Items
        {
            get { return GetValue<ObservableCollection<TEntity>>(); }
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
        /// Gets or sets the IsPaneEnabled property value.
        /// </summary>
        public bool IsPaneEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        
        public RelayCommand Add_Command { get; set; }
        public RelayCommand<TEntity> Edit_Command { get; set; }
        public RelayCommand<TEntity> Delete_Command { get; set; }
        public RelayCommand Refresh_Command { get; set; }
        public RelayCommand<object> Search_Command { get; set; }
        public RelayCommand<TEntity> Submit_Command { get; set; }

        public ViewModelBladeView(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            Blades = new ObservableCollection<IView>();

            Add_Command = new RelayCommand(async () => await AddAsync());
            Edit_Command = new RelayCommand<TEntity>(async (e) => await EditAsync(e));
            Delete_Command = new RelayCommand<TEntity>(async (e) => await DeleteAsync(e));
            Refresh_Command = new RelayCommand(async () => await RefreshAsync());
            Search_Command = new RelayCommand<object>(async (e) => await SearchAsync(e));

            Messenger.Default.Register<AddBladeMessage>(this, async (e) => await AddBladeAsync(e));
        }

        protected async Task AddBladeAsync(AddBladeMessage message)
        {
            if(message.Viewmodel != null)
            {
                if(message.Viewmodel.Owner is null)
                {
                    message.Viewmodel.Owner = message.Sender;
                }

                if (message.Viewmodel.Owner == this)
                {
                    await OpenBladeAsync(this, message.Viewmodel);
                }
            }
        }

        protected virtual async Task<bool> ValidateInputAsync()
        {
            if (SelectedItem is IBaseModel)
            {
                IBaseModel item = SelectedItem as IBaseModel;

                item.ValidateProperties();
                Errors = FlattenErrors();

                if (item.HasErrors)
                {
                    await BaseService.DialogService.ShowErrorAsync(BaseService.LanguageService.GetString("Warning_Validation_Failed"));
                    return false;
                }
            }

            return true;
        }

        protected abstract Task AddAsync();

        protected abstract Task EditAsync(TEntity e);

        protected virtual async Task DeleteAsync(TEntity e)
        {
            string item;

            if (e.GetType().GetProperty("Description") != null)
            {
                item = e.GetType().GetProperty("Description").GetValue(e) as string;
            }
            else
            {
                item = BaseService.LanguageService.GetString("Generic_This_Item");
            }

            if (await BaseService.DialogService.ShowAsync(
                                string.Format(BaseService.LanguageService.GetString("Warning_Item_Remove"), item),
                                BaseService.LanguageService.GetString("Generic_Operation_Delete"),
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    await RemoveAsync(e);
                    await RefreshAsync();
                }
                catch (FlurlHttpException ex)
                {
                    if (ex.Call.Exception.Message.Contains(ExceptionConstants.Error_547))
                    {
                        await BaseService.DialogService.ShowErrorAsync(BaseService.LanguageService.GetString("Warning_No_Remove_547_Error"));
                    }
                }
            }
        }

        protected abstract Task RemoveAsync(TEntity e);

        private Task SearchAsync(object e)
        {
            throw new NotImplementedException();
        }

        protected Task OpenBladeAsync(IViewModelBladeView<TEntity> owner, IViewModelBlade viewmodel)
        {
            if(owner == this)
            {
                viewmodel.Owner = this;

                var view = BaseService.NavigationService.GetNavigationBlade(viewmodel.GetType().FullName, viewmodel);

                if (!Blades.Any(a => a.GetType().FullName.Equals(view.GetType().FullName)))
                {
                    foreach (var blade in Blades)
                    {
                        ((IViewModelBlade)blade.DataContext).IsDisabled = true;
                    }

                    Blades.Add(view);
                }

                IsPaneEnabled = true;
            }

            return Task.CompletedTask;
        }

        protected Task<bool> RemoveBladeAsync(IViewModelBlade viewmodel)
        {
            if (Blades != null)
            {
                if (Blades.Remove(Blades.Where(q => q.DataContext == viewmodel && ((IViewModelBlade)q.DataContext).Owner == this).FirstOrDefault()))
                {
                    if (Blades.Count < 1)
                    {
                        IsPaneEnabled = false;
                    }
                    else
                    {
                        ((IViewModelBlade)Blades.Last().DataContext).IsDisabled = false;
                    }

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        public override async Task OnCancellationAsync(OnCancellationMessage e)
        {
            IViewModelBlade viewmodel = e.Sender as IViewModelBlade;

            if (!e.Handled && viewmodel != null && viewmodel.Owner == this)
            {
                if (await RemoveBladeAsync(viewmodel))
                {
                    IsCancelled = true;
                    e.Handled = true;
                }
            }
        }

        public override async Task OnSubmittanceAsync(OnSubmittanceMessage e)
        {
            IViewModelBlade viewmodel = e.Sender as IViewModelBlade;

            if (!e.Handled && viewmodel != null && viewmodel.Owner == this)
            {
                if (await RemoveBladeAsync(viewmodel))
                {
                    await RefreshAsync();
                    e.Handled = true;
                }
            }
        }

        protected virtual Task<bool> RefreshAsync() => GetItemsAsync();

        protected async Task<bool> GetItemsAsync()
        {
            bool result = false;

            try
            {
                await BaseService.BusyService.StartBusyAsync();

                Items = new ObservableCollection<TEntity>(await RetrieveItemsAsync() ?? new List<TEntity>());
                result = true;
            }
            finally
            {
                await BaseService.BusyService.EndBusyAsync();
            }

            return result;
        }

        protected abstract Task<List<TEntity>> RetrieveItemsAsync();
    }
}
