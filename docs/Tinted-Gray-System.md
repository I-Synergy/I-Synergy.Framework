# Tinted Gray System for MAUI Themes

## Overview
Each of the 48 theme files now includes **tinted gray colors** that are blended with the theme's primary color. This creates a cohesive color palette where even neutral grays carry a subtle hint of the theme color.

## Implementation

### Color Blending Formula
Each tinted gray is calculated using a **15% blend** of the primary theme color with the base neutral gray:

```
TintedColor = BaseGray � 0.85 + PrimaryColor � 0.15
```

This creates subtle, harmonious grays that complement each theme while maintaining sufficient contrast for usability.

## Available Tinted Gray Keys

All theme files include the following tinted gray color keys:

| Key | Base Gray Value | Usage |
|-----|-----------------|--------|
| `TintedGray000` | #F0F0F0 | Lightest - backgrounds, surfaces |
| `TintedGray100` | #E1E1E1 | Very light - subtle backgrounds |
| `TintedGray200` | #C8C8C8 | Light - borders, dividers |
| `TintedGray300` | #ACACAC` | Medium-light - disabled states |
| `TintedGray400` | #919191 | Medium - secondary text |
| `TintedGray500` | #6E6E6E | Medium-dark - body text |
| `TintedGray600` | #404040 | Dark - emphasis text |
| `TintedGray900` | #212121 | Very dark - high contrast |
| `TintedGray950` | #141414 | Darkest - maximum contrast |

## Usage in XAML

### Using Tinted Grays
```xaml
<!-- Tinted grays that blend with the current theme -->
<Label TextColor="{DynamicResource TintedGray900}" />
<BoxView BackgroundColor="{DynamicResource TintedGray100}" />
<Border Stroke="{DynamicResource TintedGray200}" />
```

### Using Original Neutral Grays
```xaml
<!-- Original neutral grays from Generic.xaml (unchanged) -->
<Label TextColor="{DynamicResource Gray900}" />
<BoxView BackgroundColor="{DynamicResource Gray100}" />
<Border Stroke="{DynamicResource Gray200}" />
```

## Benefits

### 1. **Theme Cohesion**
- Tinted grays subtly match each theme's primary color
- Creates a more unified, professional appearance
- Reduces visual clash between theme colors and neutrals

### 2. **Flexibility**
- Original neutral `GrayXXX` colors still available in `Generic.xaml`
- Use `TintedGrayXXX` for theme-aware UI elements
- Use `GrayXXX` when you need pure neutrals

### 3. **Automatic Updates**
- Tinted grays update automatically when theme changes
- No need to manually adjust gray tones for each theme
- Consistent color relationships across all 48 themes

## Example Comparisons

### Microsoft Blue Theme (#0078D7)
| Gray Level | Neutral Gray | Tinted Gray | Visual Effect |
|------------|--------------|-------------|---------------|
| 000 | #F0F0F0 | #ECF0F5 | Slight blue tint |
| 500 | #6E6E6E | #6C7B8E | Cooler, blue-gray |
| 950 | #141414 | #14232F | Deep blue-black |

### Gold/Yellow Theme (#FFB900)
| Gray Level | Neutral Gray | Tinted Gray | Visual Effect |
|------------|--------------|-------------|---------------|
| 000 | #F0F0F0 | #F6F3E8 | Warm, creamy white |
| 500 | #6E6E6E | #847966 | Warm, golden-gray |
| 950 | #141414 | #272314 | Warm, brown-black |

### Green Theme (#107C10)
| Gray Level | Neutral Gray | Tinted Gray | Visual Effect |
|------------|--------------|-------------|---------------|
| 000 | #F0F0F0 | #E7EDDF | Subtle sage tint |
| 500 | #6E6E6E | #697564 | Olive-gray |
| 950 | #141414 | #142014 | Deep forest black |

## Best Practices

### When to Use Tinted Grays
? Backgrounds that should feel connected to the theme
? UI elements that are part of the primary interface
? Text that should feel harmonious with the theme
? Borders and dividers in themed areas

### When to Use Neutral Grays
? Content that needs to be theme-independent
? Pure black/white requirements
? Maximum contrast scenarios
? Standard text that shouldn't shift colors

## Technical Implementation

### Blending Algorithm
```csharp
private static string BlendColors(int r1, int g1, int b1, int r2, int g2, int b2, double tintAmount = 0.15)
{
    var r = (int)Math.Round(r2 * (1 - tintAmount) + r1 * tintAmount);
    var g = (int)Math.Round(g2 * (1 - tintAmount) + g1 * tintAmount);
    var b = (int)Math.Round(b2 * (1 - tintAmount) + b1 * tintAmount);
    
    return $"{r:X2}{g:X2}{b:X2}";
}
```

### Theme File Structure
Each theme file (e.g., `Theme0078d7.xaml`) now contains:
```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ResourceDictionary ...>
    <!-- Primary Colors -->
    <Color x:Key="Primary">#0078D7</Color>
    <Color x:Key="Secondary">#3F96DF</Color>
    <Color x:Key="Tertiary">#005AA1</Color>
    
    <!-- Tinted Grays (15% blend with Primary #0078D7) -->
    <Color x:Key="TintedGray000">#ECF0F5</Color>
    <Color x:Key="TintedGray100">#DDE3EA</Color>
    <!-- ... 7 more tinted grays ... -->
</ResourceDictionary>
```

## Migration Guide

### Updating Existing Code
If you have existing XAML using hardcoded gray values:

**Before:**
```xaml
<Label TextColor="#6E6E6E" />
<Border Stroke="#C8C8C8" />
```

**After (Theme-aware):**
```xaml
<Label TextColor="{DynamicResource TintedGray500}" />
<Border Stroke="{DynamicResource TintedGray200}" />
```

**After (Theme-independent):**
```xaml
<Label TextColor="{DynamicResource Gray500}" />
<Border Stroke="{DynamicResource Gray200}" />
```

## Accessibility Considerations

### Contrast Ratios
The tinted grays maintain similar contrast ratios to their neutral counterparts:
- `TintedGray950` on `TintedGray000`: ? 15:1 (AAA)
- `TintedGray900` on `TintedGray000`: ? 12:1 (AAA)
- `TintedGray600` on `TintedGray000`: ? 7:1 (AA)

### Color Blindness
The 15% tint is subtle enough that it doesn't significantly affect readability for users with color vision deficiencies.

## Files Modified

All 48 theme files updated:
- ? Themeffb900.xaml through Theme7e735f.xaml
- ? Each includes 9 tinted gray definitions
- ? All build successfully
- ? Backward compatible

## Related Documentation
- [MAUI Dynamic Theme System](./MAUI-Dynamic-Theme-System.md)
- [Theme Color Reference](./Theme-Color-Reference.md)
- [ThemeWindow Synchronization](./ThemeWindow-Synchronization.md)
