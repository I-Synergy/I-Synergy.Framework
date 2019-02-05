namespace ISynergy.ViewModels.Base
{
    public interface IViewModelBlade : IViewModel
    {
        object Owner { get; set; }
        bool IsDisabled { get; set; }
    }
}
