# Blazor State Management (Fluxor) & JavaScript Interop

Fluxor state/actions/reducer/effects registration and component usage, plus the JS-interop patterns and companion `interop.js`. Load this when adding global state or calling into JavaScript.

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
    public bool IsLoading { get; init; }
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
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static BudgetState ReduceLoadBudgetsSuccessAction(BudgetState state, LoadBudgetsSuccessAction action) =>
        state with { IsLoading = false, Budgets = action.Budgets };

    [ReducerMethod]
    public static BudgetState ReduceLoadBudgetsFailureAction(BudgetState state, LoadBudgetsFailureAction action) =>
        state with { IsLoading = false, ErrorMessage = action.ErrorMessage };
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
    @if (BudgetState.Value.IsLoading)
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
