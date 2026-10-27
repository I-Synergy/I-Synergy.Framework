# API Secrets Management & Audit Logging

Azure Key Vault, user secrets, environment variables, and audit-logging patterns for command handlers. Load this when wiring secret storage or adding audit logs to a handler.

## Secrets Management

### Azure Key Vault Configuration
```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
    new DefaultAzureCredential());

// Access secrets
var dbConnectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
var apiKey = builder.Configuration["ExternalApi:ApiKey"];
```

### Local Development (User Secrets)
```bash
# Initialize user secrets
dotnet user-secrets init --project src/{ApplicationName}.Services.API

# Set secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;..." --project src/{ApplicationName}.Services.API
dotnet user-secrets set "OpenIddict:SigningKey" "base64-key" --project src/{ApplicationName}.Services.API
```

### Environment Variables
```csharp
// For containerized environments
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
    ?? throw new InvalidOperationException("DB_PASSWORD not set");
```

## Audit Logging

```csharp
public sealed class CreateBudgetCommandHandler(
    DataContext dataContext,
    ILogger<CreateBudgetCommandHandler> logger,
    IHttpContextAccessor httpContextAccessor
) : ICommandHandler<CreateBudgetCommand, CreateBudgetResponse>
{
    public async Task<CreateBudgetResponse> HandleAsync(
        CreateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        // Audit log
        logger.LogInformation(
            "User {UserId} from IP {IpAddress} creating Budget with Name {BudgetName}. CorrelationId: {CorrelationId}",
            userId, ipAddress, command.Name, Activity.Current?.Id);

        // Create budget
        var entity = new Entities.Budgets.Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = command.Name,
            Amount = command.Amount,
            CreatedBy = userId
        };

        dataContext.Budgets.Add(entity);
        var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);

        if (rowsAffected == 0)
            throw new InvalidOperationException("Failed to create budget");

        // Audit log success
        logger.LogInformation(
            "User {UserId} successfully created Budget {BudgetId}. CorrelationId: {CorrelationId}",
            userId, entity.BudgetId, Activity.Current?.Id);

        return new CreateBudgetResponse(entity.BudgetId);
    }
}
```
