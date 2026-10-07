# API Input Validation Patterns

Data annotations, `IValidatableObject` custom validation, and file-upload validation for API boundaries. Load this when adding or reviewing request-model validation.

## Input Validation Patterns

### Data Annotations
```csharp
public sealed record CreateBudgetCommand(
    [Required]
    [StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Name contains invalid characters")]
    string Name,

    [Range(0.01, 1_000_000)]
    decimal Amount,

    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    string NotificationEmail
) : ICommand<CreateBudgetResponse>;
```

### Custom Validation
```csharp
public sealed record CreateBudgetCommand(
    string Name,
    decimal Amount,
    DateTimeOffset StartDate
) : ICommand<CreateBudgetResponse>, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Sanitize input
        if (Name.Contains("<script>", StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "Name contains invalid characters",
                new[] { nameof(Name) });
        }

        if (StartDate > DateTimeOffset.UtcNow.AddYears(1))
        {
            yield return new ValidationResult(
                "Start date cannot be more than 1 year in the future",
                new[] { nameof(StartDate) });
        }
    }
}
```

### File Upload Validation
```csharp
app.MapPost("/upload", async (IFormFile file) =>
{
    // Validate file
    if (file is null || file.Length == 0)
        return Results.BadRequest("No file uploaded");

    // Validate file size (10MB max)
    if (file.Length > 10 * 1024 * 1024)
        return Results.BadRequest("File too large");

    // Validate file type
    var allowedExtensions = new[] { ".pdf", ".jpg", ".png" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

    if (!allowedExtensions.Contains(extension))
        return Results.BadRequest("File type not allowed");

    // Validate content type
    var allowedContentTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
    if (!allowedContentTypes.Contains(file.ContentType))
        return Results.BadRequest("Invalid content type");

    // Validate file content (not just extension)
    using var stream = file.OpenReadStream();
    // Check file signature/magic bytes

    // Process file
    return Results.Ok();
});
```
