# Materials, depth, and translucency

Platform implementations and markup for the material rules in `SKILL.md`. Read this
when a surface needs blur, acrylic, a scrim, or Apple's continuous corners.

## Backdrop blur per platform

MAUI has no cross-platform backdrop blur. Build one abstraction (`BlurView : ContentView`) and implement per platform via a handler, with a graceful translucent fallback.

- **iOS / Mac Catalyst:** insert a `UIVisualEffectView` behind the content.

```csharp
#if IOS || MACCATALYST
var blur = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.SystemMaterial))
{
    Frame = platformView.Bounds,
    AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
};
platformView.InsertSubview(blur, 0);
#endif
```

  On the newest OS versions you can use the system's glass effect (`UIGlassEffect`) behind the same abstraction, guarded by an OS version check.
- **Windows:** use WinUI `AcrylicBrush` on the native panel via the handler.
- **Android:** true backdrop blur is not generally available; fall back to a semi-transparent surface color (alpha ~0.85) with a subtle top border. Don't fake it with a blurred screenshot unless you accept the cost.

## Translucent surface markup

```xml
<Border Background="{AppThemeBinding Light=#99FFFFFF, Dark=#99000000}"
        Stroke="{AppThemeBinding Light=#66FFFFFF, Dark=#33FFFFFF}"
        StrokeThickness="1"
        StrokeShape="RoundRectangle 24">
    <Border.Shadow>
        <Shadow Brush="Black" Opacity="0.18" Radius="24" Offset="0,8" />
    </Border.Shadow>
    <!-- content -->
</Border>
```
