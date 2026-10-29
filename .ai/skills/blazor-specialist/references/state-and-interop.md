# Blazor State Management (Fluxor) & JavaScript Interop

Fluxor state/actions/reducer/effects registration and component usage, plus the JS-interop patterns and companion `interop.js`. Load this when adding global state or calling into JavaScript.

## State Management (Fluxor)

### Install Fluxor
```bash
dotnet add package Fluxor.Blazor.Web
```

### Define State
```csharp
// File: Store/{Entity}State/{Entity}State.cs
namespace {ApplicationName}.UI.Store.{Entity}State;

public record {Entity}State
{
    public List<{Entity}Response> {Entities} { get; init; } = new();
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
```

### Define Actions
```csharp
// File: Store/{Entity}State/{Entity}Actions.cs
namespace {ApplicationName}.UI.Store.{Entity}State;

public record Load{Entities}Action;
public record Load{Entities}SuccessAction(List<{Entity}Response> {Entities});
public record Load{Entities}FailureAction(string ErrorMessage);
```

### Implement Reducer
```csharp
// File: Store/{Entity}State/{Entity}Reducers.cs
using Fluxor;

namespace {ApplicationName}.UI.Store.{Entity}State;

public static class {Entity}Reducers
{
    [ReducerMethod]
    public static {Entity}State ReduceLoad{Entities}Action({Entity}State state, Load{Entities}Action action) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static {Entity}State ReduceLoad{Entities}SuccessAction({Entity}State state, Load{Entities}SuccessAction action) =>
        state with { IsLoading = false, {Entities} = action.{Entities} };

    [ReducerMethod]
    public static {Entity}State ReduceLoad{Entities}FailureAction({Entity}State state, Load{Entities}FailureAction action) =>
        state with { IsLoading = false, ErrorMessage = action.ErrorMessage };
}
```

### Implement Effects
```csharp
// File: Store/{Entity}State/{Entity}Effects.cs
using Fluxor;

namespace {ApplicationName}.UI.Store.{Entity}State;

public class {Entity}Effects(I{Entity}Service {entity}Service)
{
    [EffectMethod]
    public async Task HandleLoad{Entities}Action(Load{Entities}Action action, IDispatcher dispatcher)
    {
        try
        {
            var {entities} = await {entity}Service.Get{Entities}Async();
            dispatcher.Dispatch(new Load{Entities}SuccessAction({entities}));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new Load{Entities}FailureAction(ex.Message));
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
@inject IState<{Entity}State> {Entity}State
@inject IDispatcher Dispatcher

<div>
    @if ({Entity}State.Value.IsLoading)
    {
        <p>Loading...</p>
    }
    else
    {
        @foreach (var {entity} in {Entity}State.Value.{Entities})
        {
            <{Entity}Card {Entity}="@({entity})" />
        }
    }
</div>

@code {
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Dispatcher.Dispatch(new Load{Entities}Action());
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
window.{entity}App = {
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
