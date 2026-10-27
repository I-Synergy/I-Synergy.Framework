# Blazor Forms & API Services

`EditForm` with data-annotations validation and the typed HTTP service that the pages call. Load this when building a form, binding a model, or wiring an API call.

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
