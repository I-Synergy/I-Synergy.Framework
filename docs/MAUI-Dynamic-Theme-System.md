# Dynamic Theme System for .NET MAUI

## Overview
This implementation brings the dynamic theme system from WinUI to .NET MAUI, allowing runtime color theme switching with predefined theme resource dictionaries.

## Implementation Details

### 1. Theme Resource Dictionaries
Created 48 theme XAML files in `src\ISynergy.Framework.UI.Maui\Resources\Styles\Themes\` directory, each containing:
- **Primary Color**: The main accent color
- **Secondary Color**: A lighter variant (+25% luminosity)
- **Tertiary Color**: A darker variant (-25% luminosity)

#### Available Colors:
- ffb900, ff8c00, f7630c, ca5010, da3b01, ef6950, d13438, ff4343
- e74856, e81123, ea005e, c30052, e3008c, bf0077, c239b3, 9a0089
- 0078d7, 0063b1, 8e8cd8, 6b69d6, 8764b8, 744da9, b146c2, 881798
- 0099bc, 2d7d9a, 00b7c3, 038387, 00b294, 018574, 00cc6a, 10893e
- 7a7574, 5d5a58, 68768a, 515c6b, 567c73, 486860, 498205, 107c10
- 767676, 4c4a48, 69797e, 4a5459, 647c64, 525e54, 847545, 7e735f

### 2. ApplicationExtensions Class
Created `src\ISynergy.Framework.UI.Maui\Extensions\ApplicationExtensions.cs` with:

```csharp
public static Microsoft.Maui.Controls.Application SetApplicationColor(
    this Microsoft.Maui.Controls.Application application, 
   string color,
    ILogger? logger = null)
```

**Features:**
- Removes existing theme dictionaries before adding new ones
- Dynamically loads the appropriate theme based on hex color code
- Falls back to default theme (#FFB900) if color not found
- Includes logging for diagnostics
- Thread-safe and optimized for performance

### 3. Updated ThemeService
Enhanced `src\ISynergy.Framework.UI.Maui\Services\ThemeService.cs` to:
- Use `ApplicationExtensions.SetApplicationColor()` for dynamic theme loading
- Include structured logging throughout the theme application process
- Update platform-specific colors (Android & Windows)
- Send `StyleChangedMessage` to notify the application of theme changes
- Use guard clauses and null checking

**Key Methods:**
- `SetStyle()`: Main method to apply the selected theme
- `UpdatePlatformColors()`: Platform-specific color updates
- `UpdateOrAddResource()`: Helper to safely update resources

### 4. Architecture Benefits

#### Clean Code Principles:
- **Single Responsibility**: Each theme file manages only its color definitions
- **Open/Closed**: Easy to add new themes without modifying existing code
- **Dependency Inversion**: Uses interfaces and abstractions

#### SOLID Principles:
- Separation of concerns between theme definitions and application logic
- Dependency injection for services
- Immutable theme resources

#### Logging:
- Information-level logs for major operations
- Trace-level logs for detailed diagnostics
- Error logging with exception details

### 5. Usage Example

```csharp
// In your application startup or settings change
var themeService = serviceProvider.GetRequiredService<IThemeService>();
themeService.SetStyle(); // Applies theme from settings

// Or directly on the application
if (Application.Current is Application app)
{
    app.SetApplicationColor("#0078D7"); // Microsoft Blue
}
```

### 6. Platform Support
- **Android**: Updates `colorPrimary`, `colorAccent`, and `colorPrimaryDark`
- **iOS/MacCatalyst**: Uses standard MAUI color resources
- **Windows**: Updates `SystemAccentColor` and `SystemColorControlAccentColor`

### 7. Performance Considerations
- Theme dictionaries are loaded on-demand
- Old themes are removed before adding new ones to prevent memory leaks
- Uses pattern matching for efficient color lookup

### 8. Future Enhancements
Potential improvements:
1. Add support for custom color themes
2. Implement theme preview functionality
3. Add animation transitions between themes
4. Cache frequently used themes
5. Add theme validation

## Testing
All theme files compile successfully and the build passes without errors.

## Migration from Old System
The new system is backward compatible. Existing code will continue to work, but you can now:
1. Add more color variations easily
2. Get better logging and diagnostics
3. Have consistent theming across platforms

## Files Created
- 48 Theme XAML files (e.g., `Themeffb900.xaml`, `Theme0078d7.xaml`, etc.)
- 48 Theme code-behind files (`.xaml.cs`)
- 1 `ApplicationExtensions.cs` file
- Updated `ThemeService.cs`

## Compliance with Guidelines
? Uses SOLID principles
? Implements structured logging
? Follows clean code practices
? Uses guard clauses for null checking
? Modern C# features (pattern matching, expression-bodied members)
? Proper error handling
? XML documentation comments
