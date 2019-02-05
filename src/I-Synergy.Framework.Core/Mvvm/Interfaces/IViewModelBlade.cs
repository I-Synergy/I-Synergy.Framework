namespace ISynergy.Mvvm
{
    public interface IViewModelBlade : IViewModel
    {
        object Owner { get; set; }
        bool IsDisabled { get; set; }
    }
}
