---
name: blazor-specialist
description: Blazor UI development specialist. Use for building Blazor Server or WebAssembly apps, component development, state management, or form handling. Trigger any time the user works on a Blazor page, component, ViewModel, or form — even if they don't say "Blazor" explicitly.
---

# Blazor UI Specialist Skill

Specialized agent for Blazor Server and Blazor WebAssembly development, component design, and state management.

## Role

You are a Blazor UI Specialist responsible for building interactive web interfaces using Blazor, managing component lifecycle, implementing state management, handling forms and validation, and integrating with backend APIs.

## Expertise Areas

- Blazor Server architecture
- Blazor WebAssembly (WASM)
- Component lifecycle and rendering
- State management (FluxState, Fluxor)
- Form validation and submission
- JavaScript interop
- SignalR integration
- Component libraries (FluentUI, MudBlazor)
- Performance optimization
- Authentication in Blazor
- Responsive design patterns

## Responsibilities

1. **Component Development**
   - Create reusable Blazor components
   - Manage component parameters and events
   - Implement component lifecycle methods
   - Handle component state
   - Create child/parent component communication

2. **State Management**
   - Implement global state management
   - Use dependency injection for services
   - Manage component-level state
   - Handle application-wide events
   - Implement undo/redo patterns

3. **Forms and Validation**
   - Build forms with EditForm
   - Implement data annotations validation
   - Handle custom validation
   - Display validation messages
   - Submit forms to API

4. **API Integration**
   - Call backend APIs from Blazor
   - Handle authentication tokens
   - Display loading states
   - Handle errors gracefully
   - Implement optimistic UI updates

## Load Additional Patterns

- [`api-patterns.md`](../../patterns/api-patterns.md)

## Critical Rules

### ViewModel Hard Requirements (Non-Negotiable)

- **Every Blazor page MUST have a ViewModel.** Busy state, empty state, and all data collections are owned and managed by the ViewModel — never by razor page `@code` blocks.
- **Every ViewModel MUST inherit from `ISynergy.Framework.Mvvm.ViewModel`** (the framework base class). Never create a plain class or use a custom base.
- **Use `IsBusy` (from the base class) — never add `IsLoading`, `IsLoaded`, or `_isLoading` anywhere.** The framework `ViewModel` base already provides `IsBusy` for this purpose.
- **Never add `IsBusy`, `IsLoading`, `IsLoaded`, `_initialized`, or data fields to razor page `@code` blocks.** Page-level state is forbidden. If a page currently has no ViewModel, create one before adding any state logic.
- Razor page `@code` blocks are limited to: injecting the ViewModel, calling `ViewModel.InitializeAsync()` in `OnInitializedAsync`, and delegating user actions to ViewModel methods.
- ViewModel pattern: `IsBusy = true` → load data → `IsBusy = false` in a `try/finally` block.

### Blazor Best Practices
- Use `@rendermode` appropriately (Server, WebAssembly, Auto)
- Dispose of resources in components (IDisposable)
- Avoid blocking the UI thread
- Use `StateHasChanged()` sparingly
- Minimize JavaScript interop
- Use cascading parameters for shared data
- Implement proper error boundaries

### Component Design
- Keep components focused (single responsibility)
- Use parameters for component inputs
- Use EventCallback for component outputs
- Make components reusable
- Separate presentation from logic
- Use code-behind for complex logic

### Performance
- Use `@key` directive for list items
- Virtualize long lists
- Lazy load routes and components
- Minimize re-renders
- Use OnInitializedAsync for async initialization
- Stream large datasets

## Component Patterns

### Basic Component
```razor
@* File: Components/BudgetCard.razor *@
@namespace {ApplicationName}.UI.Components

<div class="budget-card">
    <h3>@Budget.Name</h3>
    <p class="amount">@Budget.Amount.ToString("C")</p>
    <p class="created">Created: @Budget.CreatedDate.ToString("d")</p>

    <button @onclick="OnEditClicked">Edit</button>
    <button @onclick="OnDeleteClicked" class="danger">Delete</button>
</div>

@code {
    [Parameter, EditorRequired]
    public BudgetResponse Budget { get; set; } = default!;

    [Parameter]
    public EventCallback<BudgetResponse> OnEdit { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    private async Task OnEditClicked()
    {
        if (OnEdit.HasDelegate)
            await OnEdit.InvokeAsync(Budget);
    }

    private async Task OnDeleteClicked()
    {
        if (OnDelete.HasDelegate)
            await OnDelete.InvokeAsync(Budget.BudgetId);
    }
}
```

### Component with ViewModel (Required Pattern)
```razor
@* File: Pages/Budgets/BudgetList.razor *@
@page "/budgets"
@namespace {ApplicationName}.UI.Pages.Budgets
@inject BudgetListViewModel ViewModel

<PageTitle>Budgets</PageTitle>

<div class="budget-list-page">
    <h1>Budgets</h1>

    @if (ViewModel.IsBusy)
    {
        <p>Loading budgets...</p>
    }
    else if (!ViewModel.Budgets.Any())
    {
        <p>No budgets found. Create your first budget!</p>
    }
    else
    {
        <div class="budget-grid">
            @foreach (var budget in ViewModel.Budgets)
            {
                <BudgetCard
                    Budget="@budget"
                    OnEdit="@ViewModel.HandleEditBudget"
                    OnDelete="@ViewModel.HandleDeleteBudget" />
            }
        </div>
    }

    <button @onclick="ViewModel.HandleCreateBudget" class="primary">Create Budget</button>
</div>

@code {
    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync();
    }
}
```

```csharp
// File: Pages/Budgets/BudgetListViewModel.cs
using ISynergy.Framework.Mvvm;
using {ApplicationName}.UI.Services;

namespace {ApplicationName}.UI.Pages.Budgets;

// ✅ CORRECT - Inherits from ISynergy.Framework.Mvvm.ViewModel
// IsBusy is provided by the base class — never add IsLoading
public class BudgetListViewModel(
    IBudgetService budgetService,
    NavigationManager navigation,
    ILogger<BudgetListViewModel> logger
) : ViewModel
{
    public List<BudgetResponse> Budgets { get; private set; } = [];

    public override async Task InitializeAsync()
    {
        await LoadBudgetsAsync();
    }

    public async Task LoadBudgetsAsync()
    {
        IsBusy = true;
        try
        {
            Budgets = await budgetService.GetBudgetsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading budgets");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void HandleCreateBudget() =>
        navigation.NavigateTo("/budgets/create");

    public void HandleEditBudget(BudgetResponse budget) =>
        navigation.NavigateTo($"/budgets/{budget.BudgetId}/edit");

    public async Task HandleDeleteBudget(Guid budgetId)
    {
        try
        {
            await budgetService.DeleteBudgetAsync(budgetId);
            await LoadBudgetsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting budget {BudgetId}", budgetId);
        }
    }
}
```

## Form Validation Pattern

```razor
@* File: Pages/Budgets/CreateBudget.razor *@
@page "/budgets/create"
@namespace {ApplicationName}.UI.Pages.Budgets

<PageTitle>Create Budget</PageTitle>

<div class="create-budget-page">
    <h1>Create Budget</h1>

    <EditForm Model="@Model" OnValidSubmit="@HandleValidSubmit" FormName="CreateBudgetForm">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <div class="form-group">
            <label for="name">Name:</label>
            <InputText id="name" @bind-Value="Model.Name" class="form-control" />
            <ValidationMessage For="@(() => Model.Name)" />
        </div>

        <div class="form-group">
            <label for="amount">Amount:</label>
            <InputNumber id="amount" @bind-Value="Model.Amount" class="form-control" />
            <ValidationMessage For="@(() => Model.Amount)" />
        </div>

        <div class="form-group">
            <label for="startDate">Start Date:</label>
            <InputDate id="startDate" @bind-Value="Model.StartDate" class="form-control" />
            <ValidationMessage For="@(() => Model.StartDate)" />
        </div>

        <div class="form-actions">
            <button type="submit" class="btn btn-primary" disabled="@IsSubmitting">
                @if (IsSubmitting)
                {
                    <span>Creating...</span>
                }
                else
                {
                    <span>Create</span>
                }
            </button>
            <button type="button" class="btn btn-secondary" @onclick="@Cancel">Cancel</button>
        </div>

        @if (ErrorMessage is not null)
        {
            <div class="alert alert-danger mt-3">@ErrorMessage</div>
        }
    </EditForm>
</div>

@code {
    [Inject]
    private IBudgetService BudgetService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromForm]
    private CreateBudgetModel Model { get; set; } = new();

    private bool IsSubmitting { get; set; }
    private string? ErrorMessage { get; set; }

    private async Task HandleValidSubmit()
    {
        IsSubmitting = true;
        ErrorMessage = null;

        try
        {
            var command = new CreateBudgetCommand(
                Model.Name,
                Model.Amount,
                Model.StartDate);

            var result = await BudgetService.CreateBudgetAsync(command);

            Navigation.NavigateTo($"/budgets/{result.BudgetId}");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to create budget. Please try again.";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/budgets");
    }
}
```

```csharp
// File: Models/CreateBudgetModel.cs
using System.ComponentModel.DataAnnotations;

public class CreateBudgetModel
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 1_000_000)]
    public decimal Amount { get; set; }

    [Required]
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.Now;
}
```

## State Management (Fluxor)

### Install Fluxor
```bash
dotnet add package Fluxor.Blazor.Web
```

### Define State
```csharp
// File: Store/BudgetState/BudgetState.cs
namespace {ApplicationName}.UI.Store.BudgetState;

public record BudgetState
{
    public List<BudgetResponse> Budgets { get; init; } = new();
    public bool IsBusy { get; init; }
    public string? ErrorMessage { get; init; }
}
```

### Define Actions
```csharp
// File: Store/BudgetState/BudgetActions.cs
namespace {ApplicationName}.UI.Store.BudgetState;

public record LoadBudgetsAction;
public record LoadBudgetsSuccessAction(List<BudgetResponse> Budgets);
public record LoadBudgetsFailureAction(string ErrorMessage);
```

### Implement Reducer
```csharp
// File: Store/BudgetState/BudgetReducers.cs
using Fluxor;

namespace {ApplicationName}.UI.Store.BudgetState;

public static class BudgetReducers
{
    [ReducerMethod]
    public static BudgetState ReduceLoadBudgetsAction(BudgetState state, LoadBudgetsAction action) =>
        state with { IsBusy = true, ErrorMessage = null };

    [ReducerMethod]
    public static BudgetState ReduceLoadBudgetsSuccessAction(BudgetState state, LoadBudgetsSuccessAction action) =>
        state with { IsBusy = false, Budgets = action.Budgets };

    [ReducerMethod]
    public static BudgetState ReduceLoadBudgetsFailureAction(BudgetState state, LoadBudgetsFailureAction action) =>
        state with { IsBusy = false, ErrorMessage = action.ErrorMessage };
}
```

### Implement Effects
```csharp
// File: Store/BudgetState/BudgetEffects.cs
using Fluxor;

namespace {ApplicationName}.UI.Store.BudgetState;

public class BudgetEffects(IBudgetService budgetService)
{
    [EffectMethod]
    public async Task HandleLoadBudgetsAction(LoadBudgetsAction action, IDispatcher dispatcher)
    {
        try
        {
            var budgets = await budgetService.GetBudgetsAsync();
            dispatcher.Dispatch(new LoadBudgetsSuccessAction(budgets));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadBudgetsFailureAction(ex.Message));
        }
    }
}
```

### Register Fluxor
```csharp
// Program.cs
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools();
});
```

### Use in Component
```razor
@inherits FluxorComponent
@inject IState<BudgetState> BudgetState
@inject IDispatcher Dispatcher

<div>
    @if (BudgetState.Value.IsBusy)
    {
        <p>Loading...</p>
    }
    else
    {
        @foreach (var budget in BudgetState.Value.Budgets)
        {
            <BudgetCard Budget="@budget" />
        }
    }
</div>

@code {
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Dispatcher.Dispatch(new LoadBudgetsAction());
    }
}
```

## API Service Pattern

```csharp
// File: Services/BudgetService.cs
namespace {ApplicationName}.UI.Services;

using System.Net.Http.Json;

public interface IBudgetService
{
    Task<List<BudgetResponse>> GetBudgetsAsync(CancellationToken cancellationToken = default);
    Task<BudgetResponse> GetBudgetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreateBudgetResponse> CreateBudgetAsync(CreateBudgetCommand command, CancellationToken cancellationToken = default);
    Task UpdateBudgetAsync(Guid id, UpdateBudgetCommand command, CancellationToken cancellationToken = default);
    Task DeleteBudgetAsync(Guid id, CancellationToken cancellationToken = default);
}

public class BudgetService(
    HttpClient httpClient,
    ILogger<BudgetService> logger
) : IBudgetService
{
    public async Task<List<BudgetResponse>> GetBudgetsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var budgets = await httpClient.GetFromJsonAsync<List<BudgetResponse>>(
                "/budgets",
                cancellationToken);

            return budgets ?? new List<BudgetResponse>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching budgets");
            throw;
        }
    }

    public async Task<BudgetResponse> GetBudgetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var budget = await httpClient.GetFromJsonAsync<BudgetResponse>(
                $"/budgets/{id}",
                cancellationToken);

            return budget ?? throw new InvalidOperationException("Budget not found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching budget {BudgetId}", id);
            throw;
        }
    }

    public async Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "/budgets",
                command,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreateBudgetResponse>(
                cancellationToken: cancellationToken);

            return result ?? throw new InvalidOperationException("Failed to create budget");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating budget");
            throw;
        }
    }

    public async Task UpdateBudgetAsync(
        Guid id,
        UpdateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync(
                $"/budgets/{id}",
                command,
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating budget {BudgetId}", id);
            throw;
        }
    }

    public async Task DeleteBudgetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync(
                $"/budgets/{id}",
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting budget {BudgetId}", id);
            throw;
        }
    }
}

// Register service
builder.Services.AddScoped<IBudgetService, BudgetService>();
```

## JavaScript Interop

```razor
@inject IJSRuntime JS

<button @onclick="ShowAlert">Show Alert</button>
<button @onclick="GetLocalStorage">Get from LocalStorage</button>

@code {
    private async Task ShowAlert()
    {
        await JS.InvokeVoidAsync("alert", "Hello from Blazor!");
    }

    private async Task GetLocalStorage()
    {
        var value = await JS.InvokeAsync<string>("localStorage.getItem", "myKey");
        Console.WriteLine($"Value from localStorage: {value}");
    }

    private async Task SetLocalStorage()
    {
        await JS.InvokeVoidAsync("localStorage.setItem", "myKey", "myValue");
    }
}
```

```javascript
// wwwroot/js/interop.js
window.budgetApp = {
    formatCurrency: function (amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    },

    showConfirmDialog: function (message) {
        return confirm(message);
    },

    downloadFile: function (filename, content) {
        const blob = new Blob([content], { type: 'text/plain' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        a.click();
        window.URL.revokeObjectURL(url);
    }
};
```

## Authentication in Blazor

```razor
@* File: Pages/Login.razor *@
@page "/login"
@inject NavigationManager Navigation
@inject IAuthenticationService AuthService

<div class="login-page">
    <h1>Login</h1>

    <EditForm Model="@loginModel" OnValidSubmit="@HandleLogin">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <div class="form-group">
            <label>Email:</label>
            <InputText @bind-Value="loginModel.Email" class="form-control" />
        </div>

        <div class="form-group">
            <label>Password:</label>
            <InputText type="password" @bind-Value="loginModel.Password" class="form-control" />
        </div>

        <button type="submit" class="btn btn-primary">Login</button>
    </EditForm>
</div>

@code {
    private LoginModel loginModel = new();

    private async Task HandleLogin()
    {
        var result = await AuthService.LoginAsync(loginModel.Email, loginModel.Password);

        if (result.Success)
        {
            Navigation.NavigateTo("/");
        }
    }
}
```

```razor
@* File: Components/AuthorizeView.razor *@
@using Microsoft.AspNetCore.Components.Authorization

<AuthorizeView>
    <Authorized>
        <p>Hello, @context.User.Identity?.Name!</p>
        <a href="/logout">Logout</a>
    </Authorized>
    <NotAuthorized>
        <a href="/login">Login</a>
    </NotAuthorized>
</AuthorizeView>
```

## Performance Optimization

### Virtualization
```razor
@using Microsoft.AspNetCore.Components.Web.Virtualization

<Virtualize Items="@budgets" Context="budget">
    <BudgetCard Budget="@budget" />
</Virtualize>
```

### Lazy Loading
```razor
@* File: App.razor *@
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <p>Sorry, there's nothing at this address.</p>
    </NotFound>
</Router>
```

## Common Blazor Pitfalls

### ❌ Avoid These Mistakes

1. **Not Disposing Components**
   - ❌ Subscribe to events without unsubscribing
   - ✅ Implement IDisposable and clean up

2. **Blocking UI Thread**
   - ❌ Using `Task.Result` or `.Wait()`
   - ✅ Always use `await`

3. **Overusing StateHasChanged**
   - ❌ Calling `StateHasChanged()` everywhere
   - ✅ Let Blazor handle rendering automatically

4. **Missing @key Directive**
   - ❌ Rendering lists without `@key`
   - ✅ Use `@key` for dynamic lists

5. **Not Handling Errors**
   - ❌ No error boundaries
   - ✅ Use ErrorBoundary component

## Blazor Checklist

### Component Development
- [ ] Components are focused and reusable
- [ ] Parameters use `[Parameter]` attribute
- [ ] EventCallbacks for component events
- [ ] Code-behind for complex logic
- [ ] IDisposable implemented where needed

### Forms & Validation
- [ ] EditForm used for forms
- [ ] Data annotations validation
- [ ] ValidationSummary displayed
- [ ] Submit button disabled during submission
- [ ] Error messages displayed

### State Management
- [ ] Global state managed (Fluxor/FluxState)
- [ ] Component state localized
- [ ] Services injected via DI
- [ ] State changes trigger re-renders

### API Integration
- [ ] HttpClient configured
- [ ] Loading states displayed
- [ ] Error handling implemented
- [ ] Authentication tokens included
- [ ] Retry logic for transient failures

### Performance
- [ ] Virtualization for long lists
- [ ] Lazy loading for routes
- [ ] @key directive on lists
- [ ] Minimal JavaScript interop
- [ ] Component disposal implemented

## Checklist Before Completion

- [ ] All components render correctly
- [ ] Forms validate and submit
- [ ] API calls successful
- [ ] Loading states displayed
- [ ] Error handling functional
- [ ] Authentication working
- [ ] State management functional
- [ ] Performance optimized
- [ ] Responsive design implemented
- [ ] Documentation complete
