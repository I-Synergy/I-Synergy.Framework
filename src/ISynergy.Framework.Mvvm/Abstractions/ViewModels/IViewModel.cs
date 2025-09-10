using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModel
/// Implements the <see cref="IObservableValidatedClass" />
/// Implements the <see cref="ICleanup" />
/// </summary>
/// <seealso cref="IObservableValidatedClass" />
/// <seealso cref="ICleanup" />
public interface IViewModel : IObservableValidatedClass, ICleanup
{
    AsyncRelayCommand CancelCommand { get; }
    bool CanClose { get; set; }
    AsyncRelayCommand CloseCommand { get; }
    ICommonServices CommonServices { get; }
    bool IsCancelled { get; }
    bool IsInitialized { get; set; }
    bool IsRefreshing { get; set; }
    object Parameter { get; set; }
    string Title { get; set; }

    event EventHandler? Cancelled;
    event EventHandler? Closed;

    Task CancelAsync();
    Task CloseAsync();
    string GetEnumDescription(Enum value);
    Task InitializeAsync();
    void OnCancelled(EventArgs e);
    void OnClosed(EventArgs e);
    void OnNavigatedFrom();
    void OnNavigatedTo();
    void OnPropertyChanged(object? sender, PropertyChangedEventArgs e);
}
