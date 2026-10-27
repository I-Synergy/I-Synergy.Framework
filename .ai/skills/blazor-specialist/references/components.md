# Blazor Component Patterns

The basic `BudgetCard` component with `EventCallback` outputs, and the code-behind page pattern with loading/error/empty states. Load this when creating or reviewing a component.

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

### Component with Code-Behind
```razor
@* File: Pages/Budgets/BudgetList.razor *@
@page "/budgets"
@namespace {ApplicationName}.UI.Pages.Budgets
@inherits BudgetListBase

<PageTitle>Budgets</PageTitle>

<div class="budget-list-page">
    <h1>Budgets</h1>

    @if (IsLoading)
    {
        <p>Loading budgets...</p>
    }
    else if (ErrorMessage is not null)
    {
        <div class="error">@ErrorMessage</div>
    }
    else if (!Budgets.Any())
    {
        <p>No budgets found. Create your first budget!</p>
    }
    else
    {
        <div class="budget-grid">
            @foreach (var budget in Budgets)
            {
                <BudgetCard
                    Budget="@budget"
                    OnEdit="@HandleEditBudget"
                    OnDelete="@HandleDeleteBudget" />
            }
        </div>
    }

    <button @onclick="HandleCreateBudget" class="primary">Create Budget</button>
</div>
```

```csharp
// File: Pages/Budgets/BudgetList.razor.cs
using Microsoft.AspNetCore.Components;
using {ApplicationName}.UI.Services;

namespace {ApplicationName}.UI.Pages.Budgets;

public class BudgetListBase : ComponentBase, IDisposable
{
    [Inject]
    protected IBudgetService BudgetService { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [Inject]
    protected ILogger<BudgetListBase> Logger { get; set; } = default!;

    protected List<BudgetResponse> Budgets { get; set; } = new();
    protected bool IsLoading { get; set; }
    protected string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadBudgetsAsync();
    }

    protected async Task LoadBudgetsAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            Budgets = await BudgetService.GetBudgetsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading budgets");
            ErrorMessage = "Failed to load budgets. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected void HandleCreateBudget()
    {
        Navigation.NavigateTo("/budgets/create");
    }

    protected void HandleEditBudget(BudgetResponse budget)
    {
        Navigation.NavigateTo($"/budgets/{budget.BudgetId}/edit");
    }

    protected async Task HandleDeleteBudget(Guid budgetId)
    {
        try
        {
            await BudgetService.DeleteBudgetAsync(budgetId);
            await LoadBudgetsAsync(); // Reload list
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting budget {BudgetId}", budgetId);
            ErrorMessage = "Failed to delete budget. Please try again.";
        }
    }

    public void Dispose()
    {
        // Clean up subscriptions, timers, etc.
    }
}
```
