# Reduced motion, haptics, and typography

Platform APIs, the `IMotionPreferences` abstraction, and the markup behind the
feedback, accessibility, and type rules in `SKILL.md`.

## Haptics

```csharp
HapticFeedback.Default.Perform(HapticFeedbackType.Click);      // snap, toggle
HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);  // pick up / commit
```

`HapticFeedback` (`Microsoft.Maui.Devices`) is a no-op where unsupported; check `HapticFeedback.Default.IsSupported`. For richer patterns use native Core Haptics / `VibrationEffect` via platform code.

## Reduced-motion settings

MAUI has no unified API, so wrap the platform settings:

```csharp
public interface IMotionPreferences
{
    bool ReduceMotion { get; }
    bool ReduceTransparency { get; }
    bool IncreaseContrast { get; }
}
```

Platform sources:
- **iOS/Mac Catalyst:** `UIAccessibility.IsReduceMotionEnabled`, `UIAccessibility.IsReduceTransparencyEnabled`, `UIAccessibility.DarkerSystemColorsEnabled`.
- **Android:** animations disabled when `Settings.Global.AnimatorDurationScale == 0`; high contrast text via `AccessibilityManager`.
- **Windows:** `new Windows.UI.ViewManagement.UISettings().AnimationsEnabled`, `HighContrast` via `AccessibilitySettings`.

Cross-fade instead of a spring when motion is reduced:

```csharp
void Show(View v) {
    if (_motion.ReduceMotion) { v.Opacity = 0; v.TranslationY = 0; _ = v.FadeToAsync(1, 200); }
    else _y.AnimateTo(v, 0, y => v.TranslationY = y, response: 0.4, damping: 1.0);
}
```

## Typography markup

```xml
<Label Text="Dashboard"
       FontSize="34" FontAttributes="Bold"
       CharacterSpacing="-0.7"   <!-- ≈ -0.02em at 34 -->
       LineHeight="1.05" />
<Label Text="Body copy that reads comfortably."
       FontSize="17" LineHeight="1.4" CharacterSpacing="0" />
```
