using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Mvvm.Tests.Fixtures;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.Tests.StepDefinitions;

/// <summary>
/// Shared context for MVVM test scenarios.
/// Tracks state across different step definition classes.
/// </summary>
public class MvvmTestContext
{
    public TestViewModel? ViewModel { get; set; }
    public ObservableClass? ObservableObject { get; set; }
 public List<string> PropertyChangedEvents { get; set; } = new();
    public int PropertyChangeCount { get; set; }
    public CancellationTokenSource? CancellationTokenSource { get; set; }
    public Exception? CaughtException { get; set; }
    public bool CanExecuteChangedFired { get; set; }

    public void SubscribeToPropertyChanged()
    {
        if (ViewModel != null)
   {
      ViewModel.PropertyChanged += OnPropertyChanged;
 }

  if (ObservableObject != null)
     {
ObservableObject.PropertyChanged += OnPropertyChanged;
   }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.PropertyName))
 {
        PropertyChangedEvents.Add(e.PropertyName);
   PropertyChangeCount++;
  }
    }

    public void Cleanup()
    {
    if (ViewModel != null)
  {
        ViewModel.PropertyChanged -= OnPropertyChanged;
 }

        if (ObservableObject != null)
        {
ObservableObject.PropertyChanged -= OnPropertyChanged;
  }

        PropertyChangedEvents.Clear();
      PropertyChangeCount = 0;
        CancellationTokenSource?.Dispose();
    }
}
