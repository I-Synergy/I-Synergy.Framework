using ISynergy.Framework.Mvvm.Events;
using System;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface IViewModelDialog<TEntity> : IViewModelSelectedItem<TEntity>
    {
        event EventHandler<SubmitEventArgs<TEntity>> Submitted;
    }
}
