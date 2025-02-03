using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;

namespace Sample.ViewModels;

public class Test2ViewModel(
    ICommonServices commonServices,
    bool automaticValidation = false) : ViewModelDialog<object>(commonServices, automaticValidation)
{
}
