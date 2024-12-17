using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Commands;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IShellViewModel
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="IViewModel" />
public interface IShellViewModel : IViewModel
{
    AsyncRelayCommand SettingsCommand { get; }
    AsyncRelayCommand SignInCommand { get; }
    ObservableCollection<NavigationItem> PrimaryItems { get; }
    ObservableCollection<NavigationItem> SecondaryItems { get; }
}
