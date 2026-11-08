# Blazor Forms & API Services

`EditForm` with data-annotations validation and the typed HTTP service that the pages call. Load this when building a form, binding a model, or wiring an API call.

## Form Validation Pattern

```razor
@* File: Pages/{Entities}/Create{Entity}.razor *@
@page "/{entities}/create"
@namespace {ApplicationName}.UI.Pages.{Entities}

<PageTitle>Create {Entity}</PageTitle>

<div class="create-{entity}-page">
    <h1>Create {Entity}</h1>

    <EditForm Model="@Model" OnValidSubmit="@HandleValidSubmit" FormName="Create{Entity}Form">
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
    private I{Entity}Service {Entity}Service { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromForm]
    private Create{Entity}Model Model { get; set; } = new();

    private bool IsSubmitting { get; set; }
    private string? ErrorMessage { get; set; }

    private async Task HandleValidSubmit()
    {
        IsSubmitting = true;
        ErrorMessage = null;

        try
        {
            var command = new Create{Entity}Command(
                Model.Name,
                Model.Amount,
                Model.StartDate);

            var result = await {Entity}Service.Create{Entity}Async(command);

            Navigation.NavigateTo($"/{entities}/{result.{Entity}Id}");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to create {entity}. Please try again.";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/{entities}");
    }
}
```

```csharp
// File: Models/Create{Entity}Model.cs
using System.ComponentModel.DataAnnotations;

public class Create{Entity}Model
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
// File: Services/{Entity}Service.cs
namespace {ApplicationName}.UI.Services;

using System.Net.Http.Json;

public interface I{Entity}Service
{
    Task<List<{Entity}Response>> Get{Entities}Async(CancellationToken cancellationToken = default);
    Task<{Entity}Response> Get{Entity}ByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Create{Entity}Response> Create{Entity}Async(Create{Entity}Command command, CancellationToken cancellationToken = default);
    Task Update{Entity}Async(Guid id, Update{Entity}Command command, CancellationToken cancellationToken = default);
    Task Delete{Entity}Async(Guid id, CancellationToken cancellationToken = default);
}

public class {Entity}Service(
    HttpClient httpClient,
    ILogger<{Entity}Service> logger
) : I{Entity}Service
{
    public async Task<List<{Entity}Response>> Get{Entities}Async(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var {entities} = await httpClient.GetFromJsonAsync<List<{Entity}Response>>(
                "/{entities}",
                cancellationToken);

            return {entities} ?? new List<{Entity}Response>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching {entities}");
            throw;
        }
    }

    public async Task<{Entity}Response> Get{Entity}ByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var {entity} = await httpClient.GetFromJsonAsync<{Entity}Response>(
                $"/{entities}/{id}",
                cancellationToken);

            return {entity} ?? throw new InvalidOperationException("{Entity} not found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching {entity} {Entity}Id", id);
            throw;
        }
    }

    public async Task<Create{Entity}Response> Create{Entity}Async(
        Create{Entity}Command command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "/{entities}",
                command,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Create{Entity}Response>(
                cancellationToken: cancellationToken);

            return result ?? throw new InvalidOperationException("Failed to create {entity}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating {entity}");
            throw;
        }
    }

    public async Task Update{Entity}Async(
        Guid id,
        Update{Entity}Command command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync(
                $"/{entities}/{id}",
                command,
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating {entity} {Entity}Id", id);
            throw;
        }
    }

    public async Task Delete{Entity}Async(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync(
                $"/{entities}/{id}",
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting {entity} {Entity}Id", id);
            throw;
        }
    }
}

// Register service
builder.Services.AddScoped<I{Entity}Service, {Entity}Service>();
```
