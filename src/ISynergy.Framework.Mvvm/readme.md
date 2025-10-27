# I-Synergy Framework MVVM

A comprehensive MVVM (Model-View-ViewModel) framework for building modern .NET 10.0 applications. This package provides a complete set of base classes, commands, and services for implementing the MVVM pattern with built-in support for validation, navigation, dialogs, and asynchronous operations.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.Mvvm.svg)](https://www.nuget.org/packages/I-Synergy.Framework.Mvvm/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Base ViewModel classes** with lifecycle management and validation
- **Async command support** with cancellation, timeout, and execution tracking
- **Dialog service** for showing messages, errors, and custom dialogs
- **Navigation service** for managing application navigation
- **Specialized ViewModels** for common scenarios (Dialog, Blade, Selection, Wizard)
- **Built-in validation** using DataAnnotations
- **Automatic busy state management** integration
- **Clean separation** of UI and business logic
- **Fully testable** ViewModels and commands
- **Command cancellation** and concurrent execution control
- **Event aggregation** through messenger service integration

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.Mvvm
```

## Quick Start

### 1. Create a Basic ViewModel

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;

public class CustomerViewModel : ViewModel
{
    private readonly ICustomerService _customerService;

    public string Name
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string Email
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand LoadCustomersCommand { get; }

    public CustomerViewModel(
        ICommonServices commonServices,
        ICustomerService customerService,
        ILogger<CustomerViewModel> logger)
        : base(commonServices, logger)
    {
        _customerService = customerService;

        Title = "Customer Management";

        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        LoadCustomersCommand = new AsyncRelayCommand(LoadCustomersAsync);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await LoadCustomersCommand.ExecuteAsync(null);
        IsInitialized = true;
    }

    private bool CanSave() => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email);

    private async Task SaveAsync()
    {
        try
        {
            CommonServices.BusyService.StartBusy("Saving customer...");

            var customer = new Customer
            {
                Name = Name,
                Email = Email
            };

            await _customerService.SaveAsync(customer);

            await CommonServices.DialogService.ShowInformationAsync(
                "Customer saved successfully",
                "Success");
        }
        catch (Exception ex)
        {
            await CommonServices.DialogService.ShowErrorAsync(ex, "Error");
        }
        finally
        {
            CommonServices.BusyService.StopBusy();
        }
    }

    private async Task LoadCustomersAsync()
    {
        // Load customers logic
    }
}
```

### 2. Using Async Commands

```csharp
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;

public class DataViewModel : ViewModel
{
    public AsyncRelayCommand<string> SearchCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand LongRunningCommand { get; }

    public DataViewModel(ICommonServices commonServices, ILogger<DataViewModel> logger)
        : base(commonServices, logger)
    {
        // Basic async command
        SearchCommand = new AsyncRelayCommand<string>(SearchAsync);

        // Command with CanExecute
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, CanRefresh);

        // Command with timeout (30 seconds)
        LongRunningCommand = new AsyncRelayCommand(
            LongRunningOperationAsync,
            TimeSpan.FromSeconds(30));

        // Command with options
        var downloadCommand = new AsyncRelayCommand(
            DownloadAsync,
            AsyncRelayCommandOptions.AllowConcurrentExecutions);
    }

    private async Task SearchAsync(string query, CancellationToken cancellationToken)
    {
        // Search with cancellation support
        var results = await _service.SearchAsync(query, cancellationToken);
        // Update UI
    }

    private bool CanRefresh() => IsInitialized && !string.IsNullOrEmpty(SomeProperty);

    private async Task RefreshAsync()
    {
        await Task.Delay(1000);
        RefreshCommand.NotifyCanExecuteChanged();
    }

    private async Task LongRunningOperationAsync(CancellationToken cancellationToken)
    {
        // Long running operation with cancellation
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(100, cancellationToken);
        }
    }

    private async Task DownloadAsync()
    {
        // Can be called multiple times concurrently
        await _service.DownloadFileAsync();
    }
}
```

### 3. Dialog ViewModel

```csharp
using ISynergy.Framework.Mvvm.ViewModels;

public class EditCustomerViewModel : ViewModelDialog<Customer>
{
    public string Name
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public AsyncRelayCommand SubmitCommand { get; }

    public EditCustomerViewModel(
        ICommonServices commonServices,
        ILogger<EditCustomerViewModel> logger)
        : base(commonServices, logger)
    {
        Title = "Edit Customer";
        SubmitCommand = new AsyncRelayCommand(SubmitAsync);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Load data from SelectedItem
        if (SelectedItem is not null)
        {
            Name = SelectedItem.Name;
        }

        IsInitialized = true;
    }

    private async Task SubmitAsync()
    {
        if (!IsValid)
        {
            await CommonServices.DialogService.ShowWarningAsync(
                "Please fix validation errors",
                "Validation");
            return;
        }

        // Update the selected item
        SelectedItem.Name = Name;

        // Close the dialog with the result
        OnSubmit(SelectedItem);
        await CloseAsync();
    }
}

// Usage in another ViewModel
public async Task EditCustomerAsync(Customer customer)
{
    await CommonServices.DialogService.ShowDialogAsync<EditCustomerWindow, EditCustomerViewModel, Customer>(customer);
}
```

### 4. Navigation ViewModel

```csharp
public class ProductListViewModel : ViewModelNavigation<Product>
{
    private readonly IProductService _productService;

    public ObservableCollection<Product> Products { get; } = new();

    public AsyncRelayCommand<Product> SelectProductCommand { get; }
    public AsyncRelayCommand AddProductCommand { get; }

    public ProductListViewModel(
        ICommonServices commonServices,
        IProductService productService,
        ILogger<ProductListViewModel> logger)
        : base(commonServices, logger)
    {
        _productService = productService;

        Title = "Products";

        SelectProductCommand = new AsyncRelayCommand<Product>(SelectProductAsync);
        AddProductCommand = new AsyncRelayCommand(AddProductAsync);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var products = await _productService.GetAllAsync();
        Products.Clear();
        foreach (var product in products)
        {
            Products.Add(product);
        }

        IsInitialized = true;
    }

    private async Task SelectProductAsync(Product product)
    {
        // Navigate to detail view
        await CommonServices.NavigationService.NavigateToAsync<ProductDetailViewModel>(product);
    }

    private async Task AddProductAsync()
    {
        var newProduct = await CommonServices.DialogService
            .ShowDialogAsync<AddProductWindow, AddProductViewModel, Product>();

        if (newProduct is not null)
        {
            Products.Add(newProduct);
        }
    }
}
```

## Architecture

### ViewModel Hierarchy

```
ViewModel (base)
├── ViewModelDialog<TModel>          # For dialog windows with submit/cancel
├── ViewModelNavigation<TModel>      # For navigation views with selection
├── ViewModelBlade<TModel>           # For blade/panel patterns
├── ViewModelBladeView<TModel>       # For blade child views
├── ViewModelSummary<TModel>         # For summary/read-only views
└── ViewModelWizard<TModel>          # For multi-step wizards
```

### Command Types

```
IRelayCommand
├── RelayCommand                     # Synchronous command without parameter
├── RelayCommand<T>                  # Synchronous command with parameter
├── AsyncRelayCommand                # Async command without parameter
├── AsyncRelayCommand<T>             # Async command with parameter
└── CancelCommand                    # Command for cancellation
```

## Core Services

### Dialog Service

```csharp
public class MyViewModel : ViewModel
{
    public async Task ShowMessagesAsync()
    {
        // Show information
        await CommonServices.DialogService.ShowInformationAsync(
            "Operation completed successfully",
            "Success");

        // Show warning
        await CommonServices.DialogService.ShowWarningAsync(
            "This action cannot be undone",
            "Warning");

        // Show error
        await CommonServices.DialogService.ShowErrorAsync(
            new Exception("Something went wrong"),
            "Error");

        // Show confirmation
        var result = await CommonServices.DialogService.ShowMessageAsync(
            "Are you sure you want to delete this item?",
            "Confirm",
            MessageBoxButtons.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            // Delete item
        }

        // Show custom dialog
        var customer = await CommonServices.DialogService
            .ShowDialogAsync<CustomerDialog, CustomerViewModel, Customer>();
    }
}
```

### Navigation Service

```csharp
public class NavigationExample : ViewModel
{
    public async Task NavigationExamplesAsync()
    {
        // Navigate to view
        await CommonServices.NavigationService.NavigateToAsync<ProductListViewModel>();

        // Navigate with parameter
        await CommonServices.NavigationService.NavigateToAsync<ProductDetailViewModel>(product);

        // Navigate back
        await CommonServices.NavigationService.GoBackAsync();

        // Check if can go back
        bool canGoBack = CommonServices.NavigationService.CanGoBack;
    }
}
```

### Busy Service

```csharp
public async Task LoadDataAsync()
{
    try
    {
        CommonServices.BusyService.StartBusy("Loading data...");

        var data = await _service.LoadDataAsync();

        CommonServices.BusyService.UpdateMessage("Processing data...");
        ProcessData(data);
    }
    finally
    {
        CommonServices.BusyService.StopBusy();
    }
}
```

## Advanced Features

### Command Cancellation

```csharp
public class CancellationExample : ViewModel
{
    public AsyncRelayCommand LongOperationCommand { get; }

    public CancellationExample(ICommonServices commonServices, ILogger<CancellationExample> logger)
        : base(commonServices, logger)
    {
        LongOperationCommand = new AsyncRelayCommand(LongOperationAsync);
    }

    private async Task LongOperationAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < 100; i++)
        {
            // Check cancellation
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(100, cancellationToken);

            // Update progress
            CommonServices.BusyService.UpdateMessage($"Processing {i + 1}/100...");
        }
    }

    public void CancelOperation()
    {
        // Cancel the running command
        LongOperationCommand.Cancel();
    }

    protected override void OnNavigatedFrom()
    {
        // Automatically cancel running commands when navigating away
        base.OnNavigatedFrom();
    }
}
```

### Command Options

```csharp
// Allow concurrent executions (default: false)
var command1 = new AsyncRelayCommand(
    ExecuteAsync,
    AsyncRelayCommandOptions.AllowConcurrentExecutions);

// Flow exceptions to TaskScheduler (for global error handling)
var command2 = new AsyncRelayCommand(
    ExecuteAsync,
    AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

// Combine options
var command3 = new AsyncRelayCommand(
    ExecuteAsync,
    AsyncRelayCommandOptions.AllowConcurrentExecutions |
    AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
```

### ViewModel Validation

```csharp
using System.ComponentModel.DataAnnotations;

public class ValidatedViewModel : ViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
    public string Name
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int Age
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    private async Task SaveAsync()
    {
        // Validate before saving
        if (!IsValid)
        {
            var errors = GetErrors().Select(e => e.ErrorMessage);
            await CommonServices.DialogService.ShowWarningAsync(
                string.Join(Environment.NewLine, errors),
                "Validation Errors");
            return;
        }

        // Save data
    }
}
```

### Wizard ViewModel

```csharp
public class CustomerWizardViewModel : ViewModelDialogWizard<Customer>
{
    public AsyncRelayCommand NextCommand { get; }
    public AsyncRelayCommand PreviousCommand { get; }

    public int CurrentStep
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public CustomerWizardViewModel(
        ICommonServices commonServices,
        ILogger<CustomerWizardViewModel> logger)
        : base(commonServices, logger)
    {
        Title = "New Customer Wizard";
        CurrentStep = 1;

        NextCommand = new AsyncRelayCommand(NextStepAsync, CanGoNext);
        PreviousCommand = new AsyncRelayCommand(PreviousStepAsync, CanGoPrevious);
    }

    private bool CanGoNext() => CurrentStep < 3 && IsValid;
    private bool CanGoPrevious() => CurrentStep > 1;

    private async Task NextStepAsync()
    {
        CurrentStep++;
        NextCommand.NotifyCanExecuteChanged();
        PreviousCommand.NotifyCanExecuteChanged();
    }

    private async Task PreviousStepAsync()
    {
        CurrentStep--;
        NextCommand.NotifyCanExecuteChanged();
        PreviousCommand.NotifyCanExecuteChanged();
    }
}
```

## Best Practices

> [!TIP]
> Use **AsyncRelayCommand** for all asynchronous operations to get automatic busy state management and cancellation support.

> [!IMPORTANT]
> Always call **base.InitializeAsync()** when overriding InitializeAsync() in derived ViewModels.

> [!NOTE]
> Commands automatically handle exceptions when using the default options. Set **FlowExceptionsToTaskScheduler** to handle exceptions globally.

### ViewModel Lifecycle

- Override `InitializeAsync()` for async initialization
- Override `Cleanup()` for resource cleanup
- Override `OnNavigatedFrom()` to cancel running commands
- Override `OnNavigatedTo()` to reset command states
- Call `MarkAsClean()` after saving changes
- Use `IsDirty` to track unsaved changes

### Command Usage

- Use `AsyncRelayCommand` for async operations
- Use `RelayCommand` for simple synchronous operations
- Pass `CancellationToken` to support cancellation
- Call `NotifyCanExecuteChanged()` when CanExecute state changes
- Use timeout constructors for long-running operations
- Set `AllowConcurrentExecutions` for independent operations

### Dialog Patterns

- Inherit from `ViewModelDialog<T>` for CRUD dialogs
- Call `OnSubmit(result)` to return data
- Call `OnCancelled()` for cancel operations
- Check `IsValid` before submitting
- Use `SelectedItem` for editing existing items

### Navigation Patterns

- Inherit from `ViewModelNavigation<T>` for list views
- Use `NavigationService` for view transitions
- Pass data through navigation parameters
- Clean up resources in `OnNavigatedFrom()`
- Restore state in `OnNavigatedTo()`

## Testing

The MVVM framework is designed for testability:

```csharp
[Fact]
public async Task SaveCommand_ValidData_SavesCustomer()
{
    // Arrange
    var commonServices = CreateMockCommonServices();
    var customerService = new Mock<ICustomerService>();
    var logger = Mock.Of<ILogger<CustomerViewModel>>();

    var viewModel = new CustomerViewModel(commonServices.Object, customerService.Object, logger);
    viewModel.Name = "John Doe";
    viewModel.Email = "john@example.com";

    // Act
    await viewModel.SaveCommand.ExecuteAsync(null);

    // Assert
    customerService.Verify(s => s.SaveAsync(It.Is<Customer>(c =>
        c.Name == "John Doe" &&
        c.Email == "john@example.com")), Times.Once);
}

[Fact]
public void SaveCommand_InvalidData_CannotExecute()
{
    // Arrange
    var viewModel = new CustomerViewModel(/*...*/);
    viewModel.Name = ""; // Invalid

    // Act
    bool canExecute = viewModel.SaveCommand.CanExecute(null);

    // Assert
    Assert.False(canExecute);
}

[Fact]
public async Task LongOperationCommand_WhenCancelled_StopsExecution()
{
    // Arrange
    var viewModel = new MyViewModel(/*...*/);
    var task = viewModel.LongOperationCommand.ExecuteAsync(null);

    // Act
    await Task.Delay(100);
    viewModel.LongOperationCommand.Cancel();

    // Assert
    await Assert.ThrowsAsync<OperationCanceledException>(() => task);
}
```

## Dependencies

- **I-Synergy.Framework.Core** - Core abstractions and services
- **Microsoft.Extensions.Logging** - Logging infrastructure

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.UI** - Base UI components
- **I-Synergy.Framework.UI.Maui** - MAUI UI implementation
- **I-Synergy.Framework.UI.WPF** - WPF UI implementation
- **I-Synergy.Framework.UI.Blazor** - Blazor UI implementation
- **I-Synergy.Framework.CQRS** - CQRS pattern integration

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).
