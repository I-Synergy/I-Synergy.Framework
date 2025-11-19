# Available Theme Colors Reference

## Color Palette

| Color Code | Color Family | Theme Class | Preview |
|------------|--------------|-------------|---------|
| #FFB900 | Yellow/Gold | Themeffb900 | ?? Yellow-Gold |
| #FF8C00 | Orange | Themeff8c00 | ?? Dark Orange |
| #F7630C | Orange-Red | Themef7630c | ?? Orange-Red |
| #CA5010 | Red-Orange | Themeca5010 | ?? Red-Orange |
| #DA3B01 | Red | Themeda3b01 | ?? Bright Red |
| #EF6950 | Coral | Themeef6950 | ?? Coral |
| #D13438 | Red | Themed13438 | ?? Red |
| #FF4343 | Light Red | Themeff4343 | ?? Light Red |
| #E74856 | Rose | Themee74856 | ?? Rose |
| #E81123 | Red | Themee81123 | ?? Bright Red |
| #EA005E | Magenta | Themeea005e | ?? Magenta |
| #C30052 | Dark Magenta | Themec30052 | ?? Dark Magenta |
| #E3008C | Pink | Themee3008c | ?? Pink |
| #BF0077 | Purple-Pink | Themebf0077 | ?? Purple-Pink |
| #C239B3 | Orchid | Themec239b3 | ?? Orchid |
| #9A0089 | Purple | Theme9a0089 | ?? Purple |
| #0078D7 | Blue (Default) | Theme0078d7 | ?? Microsoft Blue |
| #0063B1 | Dark Blue | Theme0063b1 | ?? Dark Blue |
| #8E8CD8 | Light Purple | Theme8e8cd8 | ?? Light Purple |
| #6B69D6 | Purple-Blue | Theme6b69d6 | ?? Purple-Blue |
| #8764B8 | Violet | Theme8764b8 | ?? Violet |
| #744DA9 | Purple | Theme744da9 | ?? Purple |
| #B146C2 | Magenta-Purple | Themeb146c2 | ?? Magenta-Purple |
| #881798 | Purple | Theme881798 | ?? Purple |
| #0099BC | Cyan | Theme0099bc | ?? Cyan |
| #2D7D9A | Teal-Blue | Theme2d7d9a | ?? Teal-Blue |
| #00B7C3 | Light Cyan | Theme00b7c3 | ?? Light Cyan |
| #038387 | Teal | Theme038387 | ?? Teal |
| #00B294 | Turquoise | Theme00b294 | ?? Turquoise |
| #018574 | Teal-Green | Theme018574 | ?? Teal-Green |
| #00CC6A | Green-Cyan | Theme00cc6a | ?? Green-Cyan |
| #10893E | Green | Theme10893e | ?? Green |
| #7A7574 | Gray-Brown | Theme7a7574 | ? Gray-Brown |
| #5D5A58 | Dark Gray | Theme5d5a58 | ? Dark Gray |
| #68768A | Blue-Gray | Theme68768a | ? Blue-Gray |
| #515C6B | Slate | Theme515c6b | ? Slate |
| #567C73 | Sage | Theme567c73 | ? Sage |
| #486860 | Olive-Gray | Theme486860 | ? Olive-Gray |
| #498205 | Olive Green | Theme498205 | ?? Olive Green |
| #107C10 | Green | Theme107c10 | ?? Green |
| #767676 | Medium Gray | Theme767676 | ? Medium Gray |
| #4C4A48 | Charcoal | Theme4c4a48 | ? Charcoal |
| #69797E | Steel Gray | Theme69797e | ? Steel Gray |
| #4A5459 | Dark Steel | Theme4a5459 | ? Dark Steel |
| #647C64 | Moss | Theme647c64 | ?? Moss |
| #525E54 | Forest Gray | Theme525e54 | ? Forest Gray |
| #847545 | Tan | Theme847545 | ?? Tan |
| #7E735F | Taupe | Theme7e735f | ?? Taupe |

## Usage in Code

```csharp
// Apply a theme dynamically
if (Application.Current is Application app)
{
    // Microsoft Blue
    app.SetApplicationColor("#0078D7");
  
    // Or use the ThemeService
    var themeService = serviceProvider.GetRequiredService<IThemeService>();
    themeService.SetStyle();
}
```

## Usage in Settings

Users can select their preferred color from the settings, and the application will automatically load the corresponding theme at runtime.

## Color Variations

Each theme includes three color keys:
- **Primary**: The main accent color
- **Secondary**: A lighter variant (adds 25% luminosity)
- **Tertiary**: A darker variant (subtracts 25% luminosity)

These can be referenced in XAML:
```xaml
<Label TextColor="{DynamicResource Primary}" />
<BoxView Color="{DynamicResource Secondary}" />
<Border Stroke="{DynamicResource Tertiary}" />
```

## Default Theme

If no theme is specified or an invalid color is provided, the system falls back to **#FFB900** (Yellow-Gold).
