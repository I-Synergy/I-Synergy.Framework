---
name: xaml-theme-generator
description: Generate XAML ResourceDictionary theme files (WinUI, UWP, MAUI) from a single accent color. Trigger when the user provides an accent color for a XAML app, says "generate a XAML theme", "create WinUI theme resources", "update MAUI colors", "generate App.xaml theme", or asks to theme their WinUI/UWP/MAUI application. Handles both hsl(H, S%, L%) and #RRGGBB formats. Outputs complete ResourceDictionary XAML with primary color scale, light/dark theme dictionaries, and platform-specific key mappings.
---

# XAML Theme Generator

Generates XAML `ResourceDictionary` theme files for WinUI 3, UWP, and MAUI from a single accent color. Uses the same HSL color scale and desaturated surface algorithms as `css-theme-generator` and `blazor-theme-generator`.

## Inputs

| Input | Required | Notes |
|-------|----------|-------|
| **Accent color** | Yes | HSL string like `hsl(149, 100%, 42%)` or hex like `#00D766` |
| **Platform** | No | `maui`, `winui`, or `all` (default: `all`) |

---

## Step 1: Parse accent color

Same as `css-theme-generator`. Extract H (0–360), S (0–100), and R/G/B.

**If hex** — convert to HSL:
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

---

## Step 2: Generate the saturated primary scale

Same 11-step HSL scale with perceptual adjustments:

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

Convert each step to hex. This is the **saturated** brand scale — used for accents, buttons, links, focus visuals.

---

## Step 3: Compute desaturated surface colors

Same formulas as `blazor-theme-generator`. Convert to hex.

### Dark mode surfaces

| Resource key | H | S formula | L |
|-------------|---|-----------|----|
| SurfaceDarkest | H - 6  | max(4, round(S * 0.13)) | 8% |
| BackgroundDark | H - 11 | max(4, round(S * 0.11)) | 11% |
| SurfaceDark | H - 21 | max(4, round(S * 0.10)) | 14% |
| SurfaceDarkVariant | H - 18 | max(4, round(S * 0.08)) | 18% |
| SurfaceDarkAlt | H - 9  | max(4, round(S * 0.07)) | 22% |

### Light mode surfaces

| Resource key | H | S formula | L |
|-------------|---|-----------|----|
| SurfaceLight | H - 3  | max(3, round(S * 0.08)) | 88% |
| BackgroundLight | H - 3  | max(3, round(S * 0.10)) | 90% |
| SurfaceLightVariant | H - 3  | max(3, round(S * 0.08)) | 92% |
| SurfaceLightAlt | H - 2  | max(5, round(S * 0.12)) | 84% |
| SurfaceCard | — | 0% | 100% (`#FFFFFF`) |

---

## Step 4: Compute text colors

Same formulas. Convert to hex.

**Dark theme text:**
| Resource key | H | S | L |
|-------------|---|---|---|
| TextDark | 0 | 0% | 96% |
| TextDarkSecondary | accent_H | 4% | 65% |
| TextDarkTertiary | accent_H | 4% | 38% |

**Light theme text:**
| Resource key | H | S | L |
|-------------|---|---|---|
| TextLight | accent_H | 10% | 9% |
| TextLightSecondary | accent_H | 6% | 32% |
| TextLightTertiary | accent_H | 4% | 50% |

---

## Step 5: Compute accent RGB (for opacity variants)

**Accent** = scale-500: H₅₀₀, S₅₀₀, L=58%. Convert to R,G,B.

**AccentDark** = scale-700: H₇₀₀, S₇₀₀, L=41%. Convert to R,G,B.

Also generate low-opacity accent variants as hex with alpha:
- `AccentDimDark` = `#1A{R_dark:X2}{G_dark:X2}{B_dark:X2}` (10% opacity)
- `AccentGlowDark` = `#38{R_dark:X2}{G_dark:X2}{B_dark:X2}` (22% opacity)
- `AccentDimLight` = `#24{R_light:X2}{G_light:X2}{B_light:X2}` (14% opacity)
- `AccentGlowLight` = `#42{R_light:X2}{G_light:X2}{B_light:X2}` (26% opacity)

Where the alpha byte = round(opacity * 255). 0.10→26=0x1A, 0.22→56=0x38, 0.14→36=0x24, 0.26→66=0x42.

---

## Step 6: Platform-specific key mappings

### MAUI convention

```
Primary        → Primary500 (scale-500)
PrimaryDark    → Primary700 (scale-700)
PrimaryLight   → Primary300 (scale-300)
Secondary      → Primary400
Tertiary       → Primary300
Surface        → theme: SurfaceCard (light) / SurfaceDark (dark)
SurfaceVariant → theme: SurfaceLightVariant / SurfaceDarkVariant
Background     → theme: BackgroundLight / BackgroundDark
Error          → static #FF4757
OnPrimary      → #FFFFFF (always white — accent is dark enough)
OnSurface      → theme: TextLight / TextDark
OnBackground   → theme: TextLight / TextDark
Outline        → theme: border-light / border-dark
```

### WinUI / UWP convention

```
SystemAccentColor           → Primary500
SystemAccentColorLight1     → Primary300
SystemAccentColorLight2     → Primary200
SystemAccentColorLight3     → Primary100
SystemAccentColorDark1      → Primary700
SystemAccentColorDark2      → Primary800
SystemAccentColorDark3      → Primary900
SystemAccentColorComplementary → (H+180 mod 360, same S/L as Primary500)
```

---

## Step 7: Assemble output

### Universal color scale (all platforms)

```xml
<!-- Primary color scale — place in Colors.xaml (MAUI) or App.xaml (WinUI/UWP) -->
<ResourceDictionary>
    <Color x:Key="Primary50">#FF{hex50}</Color>
    <Color x:Key="Primary100">#FF{hex100}</Color>
    <Color x:Key="Primary200">#FF{hex200}</Color>
    <Color x:Key="Primary300">#FF{hex300}</Color>
    <Color x:Key="Primary400">#FF{hex400}</Color>
    <Color x:Key="Primary500">#FF{hex500}</Color>
    <Color x:Key="Primary600">#FF{hex600}</Color>
    <Color x:Key="Primary700">#FF{hex700}</Color>
    <Color x:Key="Primary800">#FF{hex800}</Color>
    <Color x:Key="Primary900">#FF{hex900}</Color>
    <Color x:Key="Primary950">#FF{hex950}</Color>

    <!-- Static semantic colors -->
    <Color x:Key="Error">#FFFF4757</Color>
    <Color x:Key="Success">#FF4ADE80</Color>
    <Color x:Key="Warning">#FFFBBF24</Color>
</ResourceDictionary>
```

### Light theme dictionary

```xml
<ResourceDictionary x:Key="Light">
    <!-- Surfaces -->
    <Color x:Key="Surface">{hex_SurfaceCard}</Color>
    <Color x:Key="SurfaceVariant">{hex_SurfaceLightVariant}</Color>
    <Color x:Key="SurfaceAlt">{hex_SurfaceLightAlt}</Color>
    <Color x:Key="Background">{hex_BackgroundLight}</Color>
    <Color x:Key="SidebarBackground">{hex_SurfaceLight}</Color>

    <!-- Accent -->
    <Color x:Key="Accent">{hex_Primary700}</Color>
    <Color x:Key="AccentDim">{hex_AccentDimLight}</Color>
    <Color x:Key="AccentGlow">{hex_AccentGlowLight}</Color>

    <!-- Text -->
    <Color x:Key="Text">{hex_TextLight}</Color>
    <Color x:Key="TextSecondary">{hex_TextLightSecondary}</Color>
    <Color x:Key="TextTertiary">{hex_TextLightTertiary}</Color>

    <!-- Border -->
    <Color x:Key="Border">#1F000000</Color>
</ResourceDictionary>
```

### Dark theme dictionary

```xml
<ResourceDictionary x:Key="Dark">
    <!-- Surfaces -->
    <Color x:Key="Surface">{hex_SurfaceDark}</Color>
    <Color x:Key="SurfaceVariant">{hex_SurfaceDarkVariant}</Color>
    <Color x:Key="SurfaceAlt">{hex_SurfaceDarkAlt}</Color>
    <Color x:Key="Background">{hex_BackgroundDark}</Color>
    <Color x:Key="SidebarBackground">{hex_SurfaceDarkest}</Color>

    <!-- Accent -->
    <Color x:Key="Accent">{hex_Primary500}</Color>
    <Color x:Key="AccentDim">{hex_AccentDimDark}</Color>
    <Color x:Key="AccentGlow">{hex_AccentGlowDark}</Color>

    <!-- Text -->
    <Color x:Key="Text">{hex_TextDark}</Color>
    <Color x:Key="TextSecondary">{hex_TextDarkSecondary}</Color>
    <Color x:Key="TextTertiary">{hex_TextDarkTertiary}</Color>

    <!-- Border -->
    <Color x:Key="Border">#14FFFFFF</Color>
</ResourceDictionary>
```

---

## Platform integration instructions

### MAUI — `Resources/Styles/Colors.xaml`

Merge all three dictionaries into one file. Reference with `{AppThemeBinding Light={StaticResource Surface}, Dark={StaticResource Surface}}` in styles, or use separate light/dark style dictionaries.

### WinUI 3 / UWP — `App.xaml`

Wrap light and dark dictionaries in `<ResourceDictionary.ThemeDictionaries>`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- Primary scale + statics -->
            <ResourceDictionary Source="Colors.xaml"/>
        </ResourceDictionary.MergedDictionaries>

        <ResourceDictionary.ThemeDictionaries>
            <ResourceDictionary x:Key="Light">
                <!-- Light theme resources -->
            </ResourceDictionary>
            <ResourceDictionary x:Key="Dark">
                <!-- Dark theme resources -->
            </ResourceDictionary>
        </ResourceDictionary.ThemeDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

---

## Summary format

Print before the XAML output:

```
Source: {input} → H={H} S={S}%

Scale:
  Light end:  Primary50  = {hex50}
  Mid:        Primary500 = {hex500}
  Dark end:   Primary950 = {hex950}

Dark theme:
  Surfaces: {darkest} → {lightest}  (S=7–13%, L=8–22%)
  Accent:   Primary500  ({hex500})
  Text:     {TextDark} / {TextDarkSecondary} / {TextDarkTertiary}

Light theme:
  Surfaces: {lightest} → {darkest}  (S=8–12%, L=84–100%)
  Accent:   Primary700  ({hex700})
  Text:     {TextLight} / {TextLightSecondary} / {TextLightTertiary}
```
