namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface IViewModelBlade : IViewModel
    {
        IViewModelBladeView Owner { get; set; }
        bool IsDisabled { get; set; }
    }
}
