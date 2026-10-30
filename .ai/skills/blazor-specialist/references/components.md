# Blazor Component Patterns

The basic `{Entity}Card` component with `EventCallback` outputs, and the code-behind page pattern with loading/error/empty states. Load this when creating or reviewing a component.

## Component Patterns

### Basic Component
```razor
@* File: Components/{Entity}Card.razor *@
@namespace {ApplicationName}.UI.Components

<div class="{entity}-card">
    <h3>@({Entity}.Name)</h3>
    <p class="amount">@({Entity}.Amount.ToString("C"))</p>
    <p class="created">Created: @({Entity}.CreatedDate.ToString("d"))</p>

    <button @onclick="OnEditClicked">Edit</button>
    <button @onclick="OnDeleteClicked" class="danger">Delete</button>
</div>

@code {
    [Parameter, EditorRequired]
    public {Entity}Response {Entity} { get; set; } = default!;

    [Parameter]
    public EventCallback<{Entity}Response> OnEdit { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    private async Task OnEditClicked()
    {
        if (OnEdit.HasDelegate)
            await OnEdit.InvokeAsync({Entity});
    }

    private async Task OnDeleteClicked()
    {
        if (OnDelete.HasDelegate)
            await OnDelete.InvokeAsync({Entity}.{Entity}Id);
    }
}
```

### Component with Code-Behind
```razor
@* File: Pages/{Entities}/{Entity}List.razor *@
@page "/{entities}"
@namespace {ApplicationName}.UI.Pages.{Entities}
@inherits {Entity}ListBase

<PageTitle>{Entities}</PageTitle>

<div class="{entity}-list-page">
    <h1>{Entities}</h1>

    @if (IsLoading)
    {
        <p>Loading {entities}...</p>
    }
    else if (ErrorMessage is not null)
    {
        <div class="error">@ErrorMessage</div>
    }
    else if (!{Entities}.Any())
    {
        <p>No {entities} found. Create your first {entity}!</p>
    }
    else
    {
        <div class="{entity}-grid">
            @foreach (var {entity} in {Entities})
            {
                <{Entity}Card
                    {Entity}="@({entity})"
                    OnEdit="@HandleEdit{Entity}"
                    OnDelete="@HandleDelete{Entity}" />
            }
        </div>
    }

    <button @onclick="HandleCreate{Entity}" class="primary">Create {Entity}</button>
</div>
```

```csharp
// File: Pages/{Entities}/{Entity}List.razor.cs
using Microsoft.AspNetCore.Components;
using {ApplicationName}.UI.Services;

namespace {ApplicationName}.UI.Pages.{Entities};

public class {Entity}ListBase : ComponentBase, IDisposable
{
    [Inject]
    protected I{Entity}Service {Entity}Service { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [Inject]
    protected ILogger<{Entity}ListBase> Logger { get; set; } = default!;

    protected List<{Entity}Response> {Entities} { get; set; } = new();
    protected bool IsLoading { get; set; }
    protected string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Load{Entities}Async();
    }

    protected async Task Load{Entities}Async()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            {Entities} = await {Entity}Service.Get{Entities}Async();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading {entities}");
            ErrorMessage = "Failed to load {entities}. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected void HandleCreate{Entity}()
    {
        Navigation.NavigateTo("/{entities}/create");
    }

    protected void HandleEdit{Entity}({Entity}Response {entity})
    {
        Navigation.NavigateTo($"/{entities}/{entity}.{Entity}Id}/edit");
    }

    protected async Task HandleDelete{Entity}(Guid {entity}Id)
    {
        try
        {
            await {Entity}Service.Delete{Entity}Async({entity}Id);
            await Load{Entities}Async(); // Reload list
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting {entity} {Entity}Id", {entity}Id);
            ErrorMessage = "Failed to delete {entity}. Please try again.";
        }
    }

    public void Dispose()
    {
        // Clean up subscriptions, timers, etc.
    }
}
```
