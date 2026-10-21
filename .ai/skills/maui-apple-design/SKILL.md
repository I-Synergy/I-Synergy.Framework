---
name: maui-apple-design
description: >-
  Apple's interface-design and fluid-motion approach, translated for .NET MAUI
  (C#/XAML). USE FOR: gesture-driven UI, spring animations, drag/swipe/sheet
  interactions, momentum projection, interruptible transitions, translucent
  materials and depth, Dynamic Type/tracking/leading, reduced-motion, haptics, and
  the design foundations (feedback, spatial consistency, restraint) behind
  Apple-style interfaces. DO NOT USE FOR: XAML layout, binding or state bugs, or
  Shell routes (use maui-ui-patterns), `CollectionView` loading or handler internals
  (use maui-controls-deep-dive), screen-reader audits (use maui-accessibility),
  startup/jank measurement, trim or NativeAOT work (use maui-performance), or native
  services and manifests (use maui-platform-invoke).
---

# MAUI Apple Design

How Apple builds interfaces that stop feeling like a computer and start feeling like an extension of you. The knowledge comes from Apple's WWDC design talks, chiefly *Designing Fluid Interfaces* (WWDC 2018), distilled and translated into .NET MAUI: gesture recognizers, `Animation`/`AbortAnimation`, transform properties, handlers for native platform views, and Essentials APIs.

The through-line: **an interface feels alive when motion starts from the current on-screen value, inherits the user's velocity, projects momentum forward, and can be grabbed and reversed at any instant.** Springs are the tool that makes this natural, because they are inherently interruptible and velocity-aware.

> MAUI ships duration/easing animations only (`TranslateToAsync`, `Easing.SpringOut`, ...). These are fixed-length and cannot be re-targeted with velocity. For anything the user can touch, use the small spring solver in `references/spring-solver.md` instead.
>
> **Version floor:** the `*ToAsync` view extensions (`TranslateToAsync`, `ScaleToAsync`, `FadeToAsync`, `RotateToAsync`, ...) exist from **.NET MAUI 10**; on MAUI 9 the same methods are `TranslateTo`, `ScaleTo`, `FadeTo`, `RotateTo`. Everything else here — gesture recognizers, `Animation`/`AbortAnimation`, transform properties, handlers, Essentials — is target-framework-neutral.

## The Core Idea

An interface is fluid when it behaves like the physical world: things respond instantly, move continuously, carry momentum, resist at boundaries, and can be redirected mid-motion.

Apple frames design as serving four human needs: **safety/predictability, understanding, achievement, and joy.**

## Response: kill latency

The moment lag appears, the feeling of directness falls off a cliff.

- **Respond on press-down, not on release.** `Button` exposes `Pressed` and `Released` (use `Pressed` for the highlight). For arbitrary views, `PointerGestureRecognizer` (`PointerPressed`) works on Windows/Mac Catalyst only; on iOS/Android use `TouchBehavior` from CommunityToolkit.Maui or a handler-level touch hook.
- **Audit latency on the input path:** no `Task.Delay`, no debounce on touch, no awaiting animations before acting on the next input.
- **Feedback is continuous during the interaction**, not just at the end. For a drag, update `TranslationX/Y` 1:1 with the pan on every `PanUpdated`.
- **Keep touch handlers cheap.** Never do layout-affecting work, I/O or binding-heavy updates in `PanUpdated`.

```csharp
button.Pressed  += async (_, _) => { button.CancelAnimations(); await button.ScaleToAsync(0.97, 100, Easing.CubicOut); };
button.Released += async (_, _) => { button.CancelAnimations(); await button.ScaleToAsync(1.0, 200, Easing.CubicOut); };
```

(For non-interruptible micro-feedback like this, built-in animations are fine; always `CancelAnimations()` first so presses never queue up.)

## Direct manipulation: 1:1 tracking

Touch and content move together, and the offset from where the user grabbed is preserved.

- Use `PanGestureRecognizer`. `TotalX`/`TotalY` are cumulative since the gesture started, so record the element's translation at `Started` and add the totals: `Translation = start + Total`. That preserves the grab offset automatically.
- **Android caveat:** if the recognizer is on the same view you are translating, `TotalX/Y` can jitter because the view moves under the finger. Attach the recognizer to a **stationary parent/overlay** and translate the child, or use a native handler (`UIPanGestureRecognizer` / `GestureDetector`).
- `PanUpdated` gives **no velocity**. Track your own short sample history (see `references/spring-solver.md`), or read native velocity from a handler (`UIPanGestureRecognizer.VelocityInView`, Android `VelocityTracker`).
- Always handle `GestureStatus.Canceled` like `Completed`; the system can steal the touch (scroll views, back-swipe).

## Interruptibility: the single most important principle

Every animation must be interruptible and redirectable at any moment. A user must be able to grab a moving element mid-flight and reverse it.

- **Never lock out input during a transition.** No `IsEnabled = false` or `InputTransparent = true` while animating, and no `await`-ing an animation before accepting the next gesture.
- **Always animate from the live value.** Read `view.TranslationY` (or your spring's `Value`) at the moment of interruption, never the logical/target value.
- **Do not use `TranslateToAsync` and friends for gesture-driven motion.** They are fixed-duration, can't inherit velocity, and a cancelled animation leaves a velocity discontinuity (a "brick wall").
- **Keep the spring state (`Value`, `Velocity`) outside the view** so a re-target blends velocity instead of hard-cutting it.
- **Decompose 2D motion into independent X and Y springs.**
- Cancel in-flight work on grab: `spring.Stop()` / `view.AbortAnimation(name)` in `GestureStatus.Started`.

## Behavior over animation: use springs

Animation is a conversation between you and the object. A spring responds to new input by changing its target; the motion stays continuous.

- **Damping ratio:** `1.0` = critically damped (no bounce). `< 1.0` overshoots. Lower = bouncier.
- **Response:** the spring's period in seconds (lower = snappier). It is not a duration; settle time emerges from the parameters. Natural frequency: `omega = 2π / response`.

Start with **damping `1.0`**. Add bounce (**~`0.8`**) **only when the gesture carried momentum** (a flick, a throw). Overshoot on a menu that just appeared feels wrong; on a flicked card it feels right.

| Interaction | Damping | Response |
| --- | --- | --- |
| Move / reposition | `1.0` | `0.4` |
| Rotation | `0.8` | `0.4` |
| Drawer / sheet | `0.8` | `0.3` |

Built-in `*ToAsync` animations are fixed-duration and cannot inherit velocity, so anything the user can touch needs an analytic spring solved from the live `Value`/`Velocity`. The full `Spring` class — closed-form, frame-rate independent, re-targetable mid-flight — is in [`references/spring-solver.md`](references/spring-solver.md). One spring per axis, per element:

```csharp
readonly Spring _y = new();
_y.AnimateTo(card, target: 0, y => card.TranslationY = y, response: 0.4, damping: 1.0);
```

Animate **transform properties only** (`TranslationX/Y`, `Scale`, `Rotation`, `Opacity`); animating `WidthRequest`, `HeightRequest`, `Margin` or `Padding` triggers layout every frame. `AnchorX`/`AnchorY` (0..1) are MAUI's transform-origin.

## Velocity handoff: the seam between drag and animation

When the gesture ends, the spring must continue at the finger's exact velocity so there is no visible seam. `PanUpdated` reports no velocity, so sample the last ~100 ms yourself (or read `UIPanGestureRecognizer.VelocityInView` / Android `VelocityTracker`) and pass the result as `initialVelocity` — it is in absolute units/s, so no normalization by remaining distance is needed. The `VelocityTracker` and a complete drag-release handler are in [`references/spring-solver.md`](references/spring-solver.md).

## Momentum projection: animate to where the gesture is going

Do not snap to the nearest boundary from the release point. Project the resting position from velocity, snap to the point nearest the projection, and decide reverse-vs-commit with the **sign of velocity**, not just position. Use Apple's exponential-decay form `pos + (v/1000)·d/(1−d)` with `d = 0.998` (normal feel) or `0.99` (snappier), not `v²/(2·decel)` — the `Project` helper is in [`references/spring-solver.md`](references/spring-solver.md).

## Spatial consistency: symmetric paths, anchored origins

- **Enter and exit along the same path.** A panel that slides in from the right dismisses to the right.
- **Anchor to the source.** Set `AnchorX/AnchorY` from the trigger's position (normalized to the animated view) so menus and popovers grow out of the button that opened them, not from the center.
- **Mirror easing on reversible transitions.**
- Don't break platform navigation: keep the iOS edge-swipe-back working in `Shell`/`NavigationPage`. Don't put full-width horizontal pan recognizers over it.

## Hint in the direction of the gesture

Intermediate frames should telegraph the outcome: scale/translate a control toward the finger, reveal the destination as the drag progresses (drive `Opacity`/`Scale` from drag progress = `translation / distance`), rather than interpolating blindly at the end.

## Rubber-banding: soft boundaries

Resistance has to increase the further the finger goes past the bound, and on release the element springs back to the bound carrying the release velocity. Use the damping curve `(over · dimension · c) / (dimension + c · |over|)` with `dimension ≈ 400`, `c ≈ 0.55` — the `Rubberband` helper is in [`references/spring-solver.md`](references/spring-solver.md).

## Gesture design details (the "feel" checklist)

- **Tap:** highlight on press-down, commit on release, ~10 dp hysteresis; allow cancel by dragging away.
- **Hit targets:** minimum **44×44 dp**. Use `MinimumWidthRequest/HeightRequest` or padding rather than enlarging the visuals.
- **Drag/swipe:** wait for ~10 dp of movement before committing to an axis, then track 1:1.
- **Prefer `PanGestureRecognizer` over `SwipeGestureRecognizer`.** Swipe only reports a final direction and throws away continuous feedback and velocity.
- **Nested scrolling:** pan recognizers inside `ScrollView`/`CollectionView` compete with scrolling. Lock to the dominant axis after the threshold, and for complex cases (sheets over lists) move to a native handler.
- **Minimize disambiguation delays:** a double-tap recognizer delays single taps; only add it where double-tap exists.

## Frame-level smoothness

- Animate only `TranslationX/Y`, `Scale`, `Rotation`, `Opacity`. Avoid animating `Shadow`, `CornerRadius`, `BackgroundColor` gradients and layout properties during a drag.
- Keep visual trees shallow in animated elements; flatten nested `Grid`/`StackLayout`. Prefer `Border` over `Frame` (Frame is obsolete) and avoid `CollectionView` item templates that re-layout per frame.
- Rasterize/cache heavy static content (images pre-sized, no `Opacity` on large nested trees if avoidable).
- Profile on a **real mid-range Android device in Release/AOT**, not a debug emulator; jank that only shows in Debug is not a real defect, jank in Release is.
- Don't log, allocate lists, or capture closures inside per-frame callbacks.

## Materials & depth: translucency conveys hierarchy

MAUI has no cross-platform backdrop blur. Build one abstraction (`BlurView : ContentView`) and implement it per platform through a handler, with a graceful translucent fallback. The platform implementations (iOS `UIVisualEffectView`, the `UIGlassEffect` variant, Windows `AcrylicBrush`, the Android alpha fallback) and the translucent `Border` markup are in [`references/materials-and-depth.md`](references/materials-and-depth.md).

- **Material weight encodes hierarchy:** heavier/darker for structural regions, lighter for interactive elements. **Never stack a light translucent surface on another.**
- **Bigger surfaces read thicker:** stronger blur and a deeper `Shadow` than small chips.
- **Dim to focus** for modal tasks (scrim + push the background back with a small `Scale` < 1); **separate without a scrim** for parallel panels.
- **Edge effects instead of hard dividers:** a short gradient (`LinearGradientBrush`) where content meets floating chrome, instead of a 1 px `BoxView` under a header.
- **Materialize, don't just fade:** animate scale and opacity together on enter/exit.
- Use `Border` with `StrokeShape="RoundRectangle"`; on iOS, set `CALayer.CornerCurve = CACornerCurve.Continuous` in a handler for Apple's continuous ("squircle") corners.
- Respect safe areas (notch, home indicator); let translucent bars extend under them and inset content instead.

## Multimodal feedback: motion + sound + haptics

1. **Causality:** trigger on the actual causal event (toggle flipped, item snapped home), matching the physicality of the action.
2. **Harmony:** visual, sound and haptic fire on the **same frame**; don't `await` an animation before the haptic.
3. **Utility:** only where it earns its place (success, error, commit, snap). Over-feedback gets ignored.

```csharp
HapticFeedback.Default.Perform(HapticFeedbackType.Click);      // snap, toggle
HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);  // pick up / commit
```

`HapticFeedback` (`Microsoft.Maui.Devices`) is a no-op where unsupported; check `HapticFeedback.Default.IsSupported`. For richer patterns use native Core Haptics / `VibrationEffect` through platform code.

## Reduced motion & accessibility

Reduced motion means a gentler, non-vestibular equivalent, not no feedback. MAUI has no unified API, so wrap the platform settings (`UIAccessibility.IsReduceMotionEnabled`, Android `AnimatorDurationScale`, WinUI `UISettings.AnimationsEnabled`) behind an `IMotionPreferences` abstraction — the interface, the per-platform sources and the cross-fade snippet are in [`references/reduced-motion-and-typography.md`](references/reduced-motion-and-typography.md).

- **ReduceMotion:** replace slides/springs/parallax with a short `Opacity` cross-fade (~200 ms); drop overshoot (force damping 1.0 or skip the spring and set the final value); stop looping/auto-playing animations.
- **ReduceTransparency:** swap `BlurView` for an opaque `Border` background.
- **IncreaseContrast:** near-solid backgrounds and a visible border.
- Avoid full-screen moving backgrounds, slow looping oscillations (~0.2 Hz), and abrupt brightness jumps (animate theme changes).
- Set `SemanticProperties.Description`/`Hint` on custom gesture-driven controls and provide a non-gesture alternative (a button) for every swipe/drag action. Custom gestures are invisible to screen readers.

## Typography: Dynamic Type, tracking, leading

- **Use the system font** (leave `FontFamily` unset): SF on iOS, Roboto on Android, Segoe UI on Windows. It ships optical sizing and tuned metrics. Override only with a reason; if you ship a custom font, register it in `ConfigureFonts` and test weights on every platform.
- **Weights:** `FontAttributes` only gives Bold/Italic. For Semibold/Medium use named font families or, on iOS, a handler mapping to `UIFont.SystemFontOfSize(size, UIFontWeight.Semibold)`.
- **Respect the user's text size.** `FontAutoScalingEnabled` is `true` by default on `Label`/`Entry`/`Button`; keep it on, size containers with `Auto` rows and no fixed heights, and test at the largest accessibility size.
- **Tracking is size-specific:** `Label.CharacterSpacing` should shrink (negative) as text grows and stay ~0 (slightly positive when tiny) for body. Never one value for all sizes.
- **Leading tracks size inversely:** use `Label.LineHeight` (a multiplier): tight on large headings (~1.05-1.15), looser for body (~1.3-1.5).
- Build hierarchy from **weight + size + leading** together.

Rule of thumb for display text: `CharacterSpacing ≈ -0.02 × FontSize`. The markup these rules produce is in [`references/reduced-motion-and-typography.md`](references/reduced-motion-and-typography.md).

## Design foundations: the eight principles

1. **Purpose.** Decide what *not* to build. Every feature costs the user's time, attention and trust.
2. **Agency.** Keep people in control and offer easy undo (snackbar/toast with Undo) for slips; reserve confirmation dialogs (`DisplayAlert`) for genuinely destructive actions. Overuse trains people to click through.
3. **Responsibility.** Ask for permissions (`Permissions.RequestAsync<T>()`) at the moment of need, with a clear reason, and request only what you use. Anticipate misuse, especially with AI features.
4. **Familiarity.** Follow platform conventions: back navigation, tab bar placement (`Shell` TabBar), pull-to-refresh (`RefreshView`), native pickers and share sheets. Things that look the same behave the same. Only break a pattern if you can prove it's better.
5. **Flexibility.** Adapt to device, orientation, window size (iPad/desktop multi-window), accessibility settings and language (RTL via `FlowDirection`, localized resources). Let people personalize.
6. **Simplicity, not minimalism.** Strip the unnecessary; keep the common path first, advanced options one level deeper. Use hierarchy (order, spacing, contrast) so the most important thing is most obvious.
7. **Craft.** Every spacing, timing and alignment value is deliberate. Define spacing/radius/duration tokens once (`ResourceDictionary` + C# constants) and reuse them. Jittery scrolling or misaligned icons read as carelessness.
8. **Delight.** The result of getting the other seven right; decide the emotion (calm, confident, excited) and reinforce it everywhere.

Tactical rules:
- **Feedback comes in four kinds:** status, completion, warning, error. Validate inline (`Entry` + validation behaviors), not only on submit.
- **Wayfinding.** Every screen answers: Where am I? Where can I go? What's here? How do I get out? Never trap the user.
- **Grouping & mapping.** Proximity implies relationship; put a control near what it affects. If a control needs a label to explain it, the mapping is weak.
- **Direct, specific labels** ("Progress", "Library") beat generic ones ("Home").
- **Dark mode and theming:** use `AppThemeBinding` and semantic color resources; never hard-code a color that only works on one theme. Ease theme transitions.

## MAUI-specific pitfalls (check before shipping)

| Pitfall | Do instead |
| --- | --- |
| `TranslateToAsync` for drag release | `Spring.AnimateTo` with release velocity |
| Awaiting an animation, then reacting to input | Stay input-live; abort on `Started` |
| Pan recognizer on the view being translated (Android jitter) | Recognizer on a stationary parent, or native handler |
| Animating `WidthRequest`/`Margin` | Animate `Scale`/`Translation` |
| `SwipeGestureRecognizer` for custom drags | `PanGestureRecognizer` + velocity |
| Hard-coded fonts and sizes | System font, auto-scaling, relative layout |
| Blur via screenshots on Android | Translucent fallback surface |
| Gesture-only actions | Add a button alternative + `SemanticProperties` |
| Judging performance in Debug | Test Release/AOT on real devices |
| Frame (obsolete) with shadows | `Border` + `Shadow` |

## Process

- **Prototype interactively.** A working prototype on a real device beats a thousand static mockups; you discover the interface by playing with it. Use Hot Reload for visuals, but judge motion on hardware.
- **Design interaction and visuals together.** Motion isn't a layer added after the pixels.
- **Test with real people in real context**, and review motion with fresh eyes: slow it down (temporarily scale `len`/response) to catch what's invisible at full speed.
- **Test each platform separately.** Apple-feel on iOS does not automatically mean the Android/Windows build should mimic it; keep the principles (response, interruptibility, continuity) and respect each platform's conventions.

## Quick Reference

| Need | Technique | MAUI value |
| --- | --- | --- |
| Default UI spring | Critically damped | `damping 1.0`, `response 0.3-0.4` |
| Momentum / flick spring | Slight bounce | `damping ~0.8`, `response 0.3-0.4` |
| Gesture → spring velocity | Hand off release velocity | `VelocityTracker.Velocity()` → `initialVelocity` |
| Flick landing point | Project momentum | `pos + (v/1000)·d/(1−d)`, `d = 0.998` |
| Interrupt cleanly | Start from live value + velocity | `Spring.Value/Velocity`, abort on `Started` |
| Reversible transition | Same path, mirrored easing | symmetric translate, `AnchorX/Y` at trigger |
| 1:1 drag | Pan + start offset | `start + TotalX/Y` (stationary recognizer host) |
| Feedback | On press-down, continuous | `Button.Pressed`, `PanUpdated` |
| Boundary | Rubber-band | `Rubberband(value, min, max)` |
| Translucent chrome | Platform blur via handler | `UIVisualEffectView` / Acrylic / alpha fallback |
| Type tracking | Size-specific | `CharacterSpacing ≈ -0.02·FontSize` on large text |
| Leading | Inverse to size | `LineHeight` 1.05-1.15 (headings), 1.3-1.5 (body) |
| Reduced motion | Cross-fade, no springs | `IMotionPreferences.ReduceMotion` |
| Haptics | Same frame as visual | `HapticFeedback.Default.Perform(...)` |
| Tap target | ≥ 44×44 dp | `MinimumWidthRequest/HeightRequest` |

## Sources

Deep detail for the sections above — load only what the task needs, relative to this skill's directory:

- [Spring solver, velocity handoff, and momentum](references/spring-solver.md)
- [Materials, depth, and translucency](references/materials-and-depth.md)
- [Reduced motion, haptics, and typography](references/reduced-motion-and-typography.md)
