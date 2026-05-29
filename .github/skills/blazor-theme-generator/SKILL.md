---
name: blazor-theme-generator
description: Generate a complete Blazor app theme (--b-* CSS custom properties for dark + light mode) from a single accent color. Trigger when the user provides an accent color for a Blazor app, says "generate a Blazor theme", "update the Blazor theme colors", "create --b-* tokens", or asks to theme their Blazor UI. Handles both hsl(H, S%, L%) and #RRGGBB formats. Outputs the full :root block with color scale, semantic --b-* tokens, static tokens, and both body[data-app-theme="dark"] and body[data-app-theme="light"] blocks.
---

# Blazor Theme Generator

Generates a complete Blazor app theme from a single accent color. The saturated 11-step `--color-primary-*` scale is used for accents. Surface/background `--b-*` tokens are **desaturated** variants derived from the accent hue — saturated backgrounds are unreadable.

## Inputs

| Input | Required | Notes |
|-------|----------|-------|
| **Accent color** | Yes | HSL string like `hsl(149, 100%, 42%)` or hex like `#00D766` |

---

## Step 1: Parse the accent color

**If HSL string** — extract H (0–360), S (0–100).

**If hex** — parse R, G, B; then convert to HSL:
```
r' = R/255, g' = G/255, b' = B/255
max = max(r', g', b'), min = min(r', g', b'), Δ = max - min
L = (max + min) / 2
S = Δ / (1 - |2L - 1|)  (0 if Δ=0)
H = 60 * ((g'-b')/Δ mod 6) if max=r'
    60 * ((b'-r')/Δ + 2)  if max=g'
    60 * ((r'-g')/Δ + 4)  if max=b'
H = round(H) mod 360, S = round(S * 100)
```

Also compute R,G,B from the parsed/derived HSL for rgba tokens (see HSL→RGB in Step 3).

---

## Step 2: Generate the saturated color scale

Same as `css-theme-generator`. Fixed lightness stops with perceptual adjustments:

| Step | L% | H adj | S adj |
|------|----|-------|-------|
| 50   | 97 | +2    | -2    |
| 100  | 93 | -1    | +1    |
| 200  | 87 | 0     | -1    |
| 300  | 79 | 0     | -1    |
| 400  | 69 | 0     | 0     |
| 500  | 58 | 0     | 0     |
| 600  | 49 | 0     | 0     |
| 700  | 41 | 0     | 0     |
| 800  | 33 | 0     | 0     |
| 900  | 26 | 0     | 0     |
| 950  | 20 | 0     | 0     |

Hue wraps via `(H + adj + 360) % 360`. Saturation clamped to [0, 100].

**This saturated scale is for accents only — NOT for backgrounds.**

---

## Step 3: Compute desaturated surface colors

Surfaces use the accent hue at very low saturation. Compute as HSL then convert to hex.

### Dark mode surfaces

Near-black colors with subtle accent hue undertone. The hue shifts and saturation ratios match the reference theme:

| Token | H | S formula | L |
|-------|---|-----------|----|
| --b-sidebar-bg | H - 6  | max(4, round(S * 0.13)) | 8% |
| --b-bg | H - 11 | max(4, round(S * 0.11)) | 11% |
| --b-surface | H - 21 | max(4, round(S * 0.10)) | 14% |
| --b-surface-2 | H - 18 | max(4, round(S * 0.08)) | 18% |
| --b-surface-3 | H - 9  | max(4, round(S * 0.07)) | 22% |

Hue shifts are progressive — surfaces farther from the sidebar carry more hue drift, creating visual depth. At S=100%, surface saturation ranges from 7–13%.

### Light mode surfaces

Near-white colors with a subtle accent hue tint. Low saturation keeps them neutral enough for readability:

| Token | H | S formula | L |
|-------|---|-----------|----|
| --b-sidebar-bg | H - 3  | max(3, round(S * 0.08)) | 88% |
| --b-bg | H - 3  | max(3, round(S * 0.10)) | 90% |
| --b-surface | — | 0% | 100% (#FFFFFF) |
| --b-surface-2 | H - 3  | max(3, round(S * 0.08)) | 92% |
| --b-surface-3 | H - 2  | max(5, round(S * 0.12)) | 84% |

Convert all surface HSL values to hex.

---

## Step 4: Compute accent RGB values (for rgba tokens)

**Dark accent** = scale-500: H₅₀₀, S₅₀₀, L=58%

**Light accent** = scale-700: H₇₀₀, S₇₀₀, L=41%

Convert both to RGB:
```
Given HSL (H, S, L) with S,L in [0,1]:
C = (1 - |2L - 1|) * S
X = C * (1 - |(H/60) mod 2 - 1|)
m = L - C/2
(R', G', B') = segment(H/60) → (C,X,0), (X,C,0), (0,C,X), (0,X,C), (X,0,C), (C,0,X)
R = round((R'+m) * 255), G = round((G'+m) * 255), B = round((B'+m) * 255)
```

---

## Step 5: Compute text colors

Text uses the accent hue at very low saturation for subtle cohesion:

**Dark mode text:**
| Token | H | S | L |
|-------|---|---|---|
| --b-text | 0 | 0% | 96% |
| --b-text-2 | accent_H | 4% | 65% |
| --b-text-3 | accent_H | 4% | 38% |

**Light mode text:**
| Token | H | S | L |
|-------|---|---|---|
| --b-text | accent_H | 10% | 9% |
| --b-text-2 | accent_H | 6% | 32% |
| --b-text-3 | accent_H | 4% | 50% |

Convert all text HSL to hex.

---

## Step 6: Assemble output

Use this exact template. `{...}` placeholders are replaced with computed values:

```css
:root {
    /* ── Color scale (HSL) ───────────────────────────────────────────── */
    --color-primary-50:  hsl({H₅₀},  {S₅₀}%,  97%);
    --color-primary-100: hsl({H₁₀₀}, {S₁₀₀}%, 93%);
    --color-primary-200: hsl({H₂₀₀}, {S₂₀₀}%, 87%);
    --color-primary-300: hsl({H₃₀₀}, {S₃₀₀}%, 79%);
    --color-primary-400: hsl({H₄₀₀}, {S₄₀₀}%, 69%);
    --color-primary-500: hsl({H₅₀₀}, {S₅₀₀}%, 58%);
    --color-primary-600: hsl({H₆₀₀}, {S₆₀₀}%, 49%);
    --color-primary-700: hsl({H₇₀₀}, {S₇₀₀}%, 41%);
    --color-primary-800: hsl({H₈₀₀}, {S₈₀₀}%, 33%);
    --color-primary-900: hsl({H₉₀₀}, {S₉₀₀}%, 26%);
    --color-primary-950: hsl({H₉₅₀}, {S₉₅₀}%, 20%);

    /* ── Static tokens ───────────────────────────────────────────────── */
    --b-danger:      #FF4757;
    --b-success:     #4ADE80;
    --b-warning:     #FBBF24;
    --b-header-h:    58px;
    --b-sidebar-w:   160px;
    --r:             12px;
    --r-sm:          8px;
    --r-pill:        50px;
}

/* ── Dark theme ──────────────────────────────────────────────────────── */
body[data-app-theme="dark"] {
    --b-sidebar-bg:  {dark_sidebar_bg};
    --b-bg:          {dark_bg};
    --b-surface:     {dark_surface};
    --b-surface-2:   {dark_surface_2};
    --b-surface-3:   {dark_surface_3};
    --b-accent:      var(--color-primary-500);
    --b-accent-dim:  rgba({R_dark}, {G_dark}, {B_dark}, 0.10);
    --b-accent-glow: rgba({R_dark}, {G_dark}, {B_dark}, 0.22);
    --b-text:        {text_dark};
    --b-text-2:      {text_dark_2};
    --b-text-3:      {text_dark_3};
    --b-border:      rgba(255, 255, 255, 0.08);
}

/* ── Light theme ─────────────────────────────────────────────────────── */
body[data-app-theme="light"] {
    --b-sidebar-bg:  {light_sidebar_bg};
    --b-bg:          {light_bg};
    --b-surface:     #FFFFFF;
    --b-surface-2:   {light_surface_2};
    --b-surface-3:   {light_surface_3};
    --b-accent:      var(--color-primary-700);
    --b-accent-dim:  rgba({R_light}, {G_light}, {B_light}, 0.14);
    --b-accent-glow: rgba({R_light}, {G_light}, {B_light}, 0.26);
    --b-text:        {text_light};
    --b-text-2:      {text_light_2};
    --b-text-3:      {text_light_3};
    --b-border:      rgba(0, 0, 0, 0.12);
}
```

---

## Summary format

Print before the CSS block:

```
Source: {input} → H={H} S={S}%

Dark surfaces:  {darkest hex} → {lightest hex}  (desaturated, L=8%→22%)
Dark accent:    scale-500 (L=58%, RGB: {R_dark}, {G_dark}, {B_dark})

Light surfaces: {lightest hex} → {darkest hex}  (desaturated, L=88%→100%)
Light accent:   scale-700 (L=41%, RGB: {R_light}, {G_light}, {B_light})
```
