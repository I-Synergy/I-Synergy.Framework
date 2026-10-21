# Spring solver, velocity handoff, and momentum

Base implementation for the fluid-motion rules in `SKILL.md`. Read this when you are writing gesture-driven motion: springs, release velocity, momentum projection, and soft boundaries.

## Behavior over animation: use springs

Animation is a conversation between you and the object. A spring responds to new input by changing its target; the motion stays continuous.

Apple's designer-friendly parameters:

- **Damping ratio:** `1.0` = critically damped (no bounce). `< 1.0` overshoots. Lower = bouncier.
- **Response:** the spring's period in seconds (lower = snappier). It is not a duration; settle time emerges from the parameters. Natural frequency: `omega = 2π / response`.

**Defaults:**
- Start with **damping `1.0`**. Add bounce (**~`0.8`**) **only when the gesture carried momentum** (a flick, a throw). Overshoot on a menu that just appeared feels wrong; on a flicked card it feels right.

| Interaction | Damping | Response |
| --- | --- | --- |
| Move / reposition | `1.0` | `0.4` |
| Rotation | `0.8` | `0.4` |
| Drawer / sheet | `0.8` | `0.3` |

### Reference implementation: `Spring`

Analytic (closed-form) solution, so it is exact for any frame rate, with initial velocity and re-targeting from live state. It uses MAUI's `Animation` purely as a display-synced clock (the platform ticker: CADisplayLink on iOS, Choreographer on Android).

```csharp
using Microsoft.Maui.Controls;

public sealed class Spring
{
    readonly string _name = "spring-" + Guid.NewGuid().ToString("N");

    public double Value { get; private set; }
    public double Velocity { get; private set; }   // units per second

    public Spring(double initial = 0) => Value = initial;

    /// Call during drags so the spring always knows the live state.
    public void Set(double value, double velocity = 0) { Value = value; Velocity = velocity; }

    public void Stop(IAnimatable owner) => owner.AbortAnimation(_name);

    public void AnimateTo(IAnimatable owner, double target, Action<double> onUpdate,
                          double response = 0.4, double damping = 1.0,
                          double? initialVelocity = null, Action? onSettled = null)
    {
        owner.AbortAnimation(_name);                       // interrupt cleanly

        double zeta = Math.Clamp(damping, 0.05, 1.0);
        double w    = 2 * Math.PI / response;
        double d0   = Value - target;                      // start from LIVE value
        double v0   = initialVelocity ?? Velocity;         // inherit LIVE velocity
        double wd   = w * Math.Sqrt(Math.Max(1 - zeta * zeta, 0));
        double B    = zeta < 1 ? (v0 + zeta * w * d0) / wd : 0;
        double len  = Math.Clamp(9.0 / (zeta * w), 0.2, 4.0);   // ~settled

        var anim = new Animation(p =>
        {
            double t = p * len, e = Math.Exp(-zeta * w * t), d, v;
            if (zeta >= 1)
            {
                double c = v0 + w * d0;
                d = (d0 + c * t) * e;
                v = (c - w * (d0 + c * t)) * e;
            }
            else
            {
                double cos = Math.Cos(wd * t), sin = Math.Sin(wd * t);
                d = e * (d0 * cos + B * sin);
                v = e * ((B * wd - zeta * w * d0) * cos + (-d0 * wd - zeta * w * B) * sin);
            }
            Value = target + d; Velocity = v;
            onUpdate(Value);
        });

        anim.Commit(owner, _name, 16, (uint)(len * 1000), Easing.Linear, (_, cancelled) =>
        {
            if (cancelled) return;                         // interrupted: keep live state
            Value = target; Velocity = 0; onUpdate(Value); onSettled?.Invoke();
        });
    }
}
```

Usage (one spring per axis, per element):

```csharp
readonly Spring _y = new();
_y.AnimateTo(card, target: 0, y => card.TranslationY = y, response: 0.4, damping: 1.0);
```

Notes:
- Animate **transform properties only** (`TranslationX/Y`, `Scale`, `Rotation`, `Opacity`). Animating `WidthRequest`, `HeightRequest`, `Margin` or `Padding` triggers layout every frame and will not hold 60/120 fps.
- `AnchorX`/`AnchorY` (0..1) are MAUI's transform-origin. Use them for spatial consistency (see "Spatial consistency" in `SKILL.md`).
- For ProMotion/120 Hz displays, don't hard-code frame math; the solver above is time-based, so it is frame-rate independent.

## Velocity handoff: the seam between drag and animation

When the gesture ends, the spring must continue at the finger's exact velocity so there is no visible seam.

Track the last ~100 ms of samples and compute velocity at release:

```csharp
public sealed class VelocityTracker
{
    readonly Queue<(long ts, double pos)> _s = new();
    public void Reset() => _s.Clear();
    public void Add(double pos)
    {
        long now = Stopwatch.GetTimestamp();
        _s.Enqueue((now, pos));
        while (_s.Count > 2 && (now - _s.Peek().ts) > Stopwatch.Frequency / 10) _s.Dequeue(); // 100 ms
    }
    public double Velocity()   // units per second
    {
        if (_s.Count < 2) return 0;
        var first = _s.Peek(); var last = _s.Last();
        double dt = (last.ts - first.ts) / (double)Stopwatch.Frequency;
        return dt <= 0 ? 0 : (last.pos - first.pos) / dt;
    }
}
```

Pass `tracker.Velocity()` as `initialVelocity` to `Spring.AnimateTo` (it takes absolute units/s, so no normalization by remaining distance is needed).

## Momentum projection: animate to where the gesture is going

Do not snap to the nearest boundary from the release point. Project the resting position from velocity, then pick the snap point nearest the projection.

```csharp
// decelerationRate 0.998 = normal scroll feel, 0.99 = snappier
static double Project(double velocityPerSec, double rate = 0.998)
    => (velocityPerSec / 1000.0) * rate / (1 - rate);

double projected = card.TranslationY + Project(velocity);
double target    = NearestSnapPoint(projected);
_y.AnimateTo(card, target, y => card.TranslationY = y,
             response: 0.35, damping: 0.8, initialVelocity: velocity);  // bounce: a flick preceded it
```

Use the exponential-decay form above (what Apple ships), not `v²/(2·decel)`.

A full drag-release handler:

```csharp
double _startY; readonly VelocityTracker _vt = new();

void OnPan(object? s, PanUpdatedEventArgs e)
{
    switch (e.StatusType)
    {
        case GestureStatus.Started:
            _y.Stop(card);                       // grab: interrupt immediately
            _startY = card.TranslationY;         // start from the LIVE value
            _vt.Reset();
            break;

        case GestureStatus.Running:
            double y = Rubberband(_startY + e.TotalY, minY, maxY);
            card.TranslationY = y;
            _y.Set(y);                           // keep spring state in sync
            _vt.Add(y);
            break;

        case GestureStatus.Completed:
        case GestureStatus.Canceled:
            double v = _vt.Velocity();
            double target = NearestSnapPoint(card.TranslationY + Project(v));
            _y.AnimateTo(card, target, y => card.TranslationY = y,
                         response: 0.3, damping: 0.8, initialVelocity: v);
            break;
    }
}
```

Decide reverse-vs-commit with the **sign of velocity**, not just position.

## Rubber-banding: soft boundaries

```csharp
static double Rubberband(double value, double min, double max, double dimension = 400, double c = 0.55)
{
    if (value >= min && value <= max) return value;
    double over = value < min ? value - min : value - max;
    double damped = (over * dimension * c) / (dimension + c * Math.Abs(over));
    return (value < min ? min : max) + damped;
}
```

Resistance increases the further past the bound; on release, spring back to the bound with the release velocity.
