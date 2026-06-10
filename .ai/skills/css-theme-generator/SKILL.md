---
name: css-theme-generator
description: Generate a full CSS custom property theme (light + dark) from a single accent color in HSL or hex. Trigger when the user provides an accent color, says "generate a theme", "create CSS tokens from this color", "build a color scale", or asks to derive a design system palette from a brand color. Handles both hsl(H, S%, L%) and #RRGGBB formats.
---

# CSS Theme Generator

Generates an 11-step CSS custom property color scale (`--color-primary-50` through `--color-primary-950`) with light and dark mode from a single accent color. The algorithm extracts hue and saturation from the input, applies fixed perceptual lightness stops, and adds subtle hue/saturation corrections at the extremes for natural color perception.

## Inputs

| Input | Required | Notes |
|-------|----------|-------|
| **Accent color** | Yes | An HSL string like `hsl(142, 65%, 36%)` or a hex color like `#33CC8A` |

## Algorithm

### Step 1: Parse the input color to H and S

**If HSL string** (e.g., `hsl(142, 65%, 36%)`):
- Extract H, S, and L using a regex: `hsl\((\d+),\s*(\d+)%,\s*(\d+)%\)`
- H is an integer 0–360, S is an integer 0–100

**If hex** (e.g., `#33CC8A`):
- Parse R, G, B from the hex string
- Convert to HSL:
  ```
  r' = R / 255, g' = G / 255, b' = B / 255
  max = max(r', g', b'), min = min(r', g', b')
  Δ = max - min
  
  L = (max + min) / 2
  
  if Δ = 0: S = 0, H = 0
  else:
    S = Δ / (1 - |2L - 1|)
    if max = r': H = 60 * (((g' - b') / Δ) mod 6)
    if max = g': H = 60 * ((b' - r') / Δ + 2)
    if max = b': H = 60 * ((r' - g') / Δ + 4)
  
  H = round(H) mod 360
  S = round(S * 100)
  ```

### Step 2: Fixed lightness stops and perceptual adjustments

The 11 lightness stops are fixed. Adjustments are keyed to lightness level (not position), so they apply consistently whether the stop is in light or dark mode:

| Step | Lightness | Hue shift | Saturation shift |
|------|-----------|-----------|-----------------|
| 50   | 97%       | +2        | -2              |
| 100  | 93%       | -1        | +1              |
| 200  | 87%       | 0         | -1              |
| 300  | 79%       | 0         | -1              |
| 400  | 69%       | 0         | 0               |
| 500  | 58%       | 0         | 0               |
| 600  | 49%       | 0         | 0               |
| 700  | 41%       | 0         | 0               |
| 800  | 33%       | 0         | 0               |
| 900  | 26%       | 0         | 0               |
| 950  | 20%       | 0         | 0               |

Perceptual rules:
- At **97%** lightness (near-white): shift hue +2, reduce saturation -2 (prevents color from looking washed out or tinted)
- At **93%** lightness: shift hue -1, boost saturation +1 (adds warmth/depth to light tints)
- At **87%** and **79%** lightness: reduce saturation -1 (smooths the transition into midtones)

### Step 3: Generate light mode (`:root`)

For each step (50→950), compute:
```
H_step = (H + H_adj) mod 360
S_step = clamp(S + S_adj, 0, 100)
L_step = L_stop
```

Output ascending by step number (lightest to darkest):
```css
:root {
  --color-primary-50: hsl({H+2}, {S-2}%, 97%);
  --color-primary-100: hsl({H-1}, {S+1}%, 93%);
  --color-primary-200: hsl({H}, {S-1}%, 87%);
  --color-primary-300: hsl({H}, {S-1}%, 79%);
  --color-primary-400: hsl({H}, {S}%, 69%);
  --color-primary-500: hsl({H}, {S}%, 58%);
  --color-primary-600: hsl({H}, {S}%, 49%);
  --color-primary-700: hsl({H}, {S}%, 41%);
  --color-primary-800: hsl({H}, {S}%, 33%);
  --color-primary-900: hsl({H}, {S}%, 26%);
  --color-primary-950: hsl({H}, {S}%, 20%);
}
```

### Step 4: Generate dark mode (`.dark`)

Invert the lightness scale so the darkest light-mode stop becomes the lightest dark-mode stop. The same perceptual adjustments apply to the same lightness values:

```
Dark 50  ← Light 950 (L=20%,  no adjustment)
Dark 100 ← Light 900 (L=26%,  no adjustment)
Dark 200 ← Light 800 (L=33%,  no adjustment)
Dark 300 ← Light 700 (L=41%,  no adjustment)
Dark 400 ← Light 600 (L=49%,  no adjustment)
Dark 500 ← Light 500 (L=58%,  no adjustment)
Dark 600 ← Light 400 (L=69%,  no adjustment)
Dark 700 ← Light 300 (L=79%,  S-1)
Dark 800 ← Light 200 (L=87%,  S-1)
Dark 900 ← Light 100 (L=93%,  H-1, S+1)
Dark 950 ← Light 50  (L=97%,  H+2, S-2)
```

Output:
```css
.dark {
  --color-primary-50: hsl({H}, {S}%, 20%);
  --color-primary-100: hsl({H}, {S}%, 26%);
  --color-primary-200: hsl({H}, {S}%, 33%);
  --color-primary-300: hsl({H}, {S}%, 41%);
  --color-primary-400: hsl({H}, {S}%, 49%);
  --color-primary-500: hsl({H}, {S}%, 58%);
  --color-primary-600: hsl({H}, {S}%, 69%);
  --color-primary-700: hsl({H}, {S-1}%, 79%);
  --color-primary-800: hsl({H}, {S-1}%, 87%);
  --color-primary-900: hsl({H-1}, {S+1}%, 93%);
  --color-primary-950: hsl({H+2}, {S-2}%, 97%);
}
```

## Edge Cases

**Hue wrapping**: After applying H_adj, use `(H + H_adj + 360) % 360`. Negative shifts (like -1) wrap to 359; positive shifts beyond 360 wrap to 0.

**Saturation clamping**: `clamp(S + S_adj, 0, 100)`. Near-gray inputs (S < 3) will produce near-gray scales — this is expected and correct.

## Output Format

Present the computed values in a code block ready to copy into a CSS file. Include a brief summary of the source color and extracted H/S values before the output.

Example summary line:
```
Source: hsl(142, 65%, 36%) → H=142 S=65%
```
