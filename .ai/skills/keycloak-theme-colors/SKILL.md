---
name: keycloak-theme-colors
description: Update the accent colors in an I-Synergy Keycloak login theme from a single base hex color. Trigger when the user provides a hex color code (like "#0078D7") for a theme, says "align colors with the logo", "update the theme colors", "set the accent to #XXXXXX for [theme]", or shows a logo image and wants the login theme to match it. Automatically derives hover, dim, and dark-mode variants and rewrites all CSS color tokens in one pass.
---

# Keycloak Theme Colors

Rewrites all CSS color tokens in an I-Synergy Keycloak login theme from a single base hex color.
One command in, everything updated — no manual hex math.

## Inputs

Collect these before proceeding (infer from context where possible):

| Input | Required | Notes |
|-------|----------|-------|
| **Base hex color** | Yes | The brand's primary color, e.g. `#0078D7`. If the user shows a logo/icon, read it visually and identify the dominant brand color. |
| **Theme name** | Yes | Folder name under `i-synergy-{theme}`, e.g. `business`, `pointofsales`, `budgets`. Infer from context if obvious. |
| **Logo images** | Optional | If provided, place in the `img/` folder (see below) and optionally adjust `--b-logo-w`/`--b-logo-h`. |

---

## Color derivation

Given **base hex `#RRGGBB`** — parse to integers R, G, B (0–255):

### Light mode tokens

| Token | Derivation | Example: `#0078D7` (R=0, G=120, B=215) |
|-------|-----------|------------------------------------------|
| `--b-accent` | base hex | `#0078D7` |
| `--b-accent-hover` | Darken 20 %: `round(channel × 0.80)` each | R=0, G=96, B=172 → `#0060AC` |
| `--b-accent-dim` | `rgba(R, G, B, 0.10)` | `rgba(0, 120, 215, 0.10)` |

### Dark mode tokens (accent must be readable on `#0A0A0A`)

| Token | Derivation | Example |
|-------|-----------|---------|
| `--b-accent` | Lighten 30 %: `round(channel + (255 − channel) × 0.30)` | R=77, G=157, B=243 → `#4D9DF3` |
| `--b-accent-hover` | base hex | `#0078D7` |
| `--b-accent-dim` | `rgba(R_light, G_light, B_light, 0.10)` | `rgba(77, 157, 243, 0.10)` |

### PatternFly variables (both PF4 and PF5)

Mirror the accent/hover pair for each mode:

```
--pf-v5-global--primary-color--100  = accent
--pf-v5-global--primary-color--200  = hover
--pf-v5-global--link--Color         = accent
--pf-v5-global--link--Color--hover  = hover

--pf-global--primary-color--100     = accent   (PF4, same values)
--pf-global--primary-color--200     = hover
--pf-global--link--Color            = accent
--pf-global--link--Color--hover     = hover
```

---

## CSS file location

```
src/ISynergy.AppHost/config/keycloak/themes/i-synergy-{theme}/login/resources/css/login.css
```

---

## Update procedure

1. **Compute all derived colors** — show a brief summary so the user can verify:
   ```
   Base:         #0078D7  (R=0, G=120, B=215)
   Light accent: #0078D7
   Light hover:  #0060AC
   Light dim:    rgba(0, 120, 215, 0.10)
   Dark accent:  #4D9DF3
   Dark hover:   #0078D7
   Dark dim:     rgba(77, 157, 243, 0.10)
   ```

2. **Read the CSS file** to confirm its current structure.

3. **Edit only the two token blocks** — leave everything below them untouched:
   - `:root { }` — lines ~12–47 (light mode)
   - `@media (prefers-color-scheme: dark) :root { }` — lines ~54–88 (dark mode)

4. **Update the comment on `--b-accent`** in `:root` to name the brand and hex, e.g.:
   ```css
   --b-accent: #0078D7;   /* I-Synergy Business brand blue */
   ```

5. **Report** what changed:
   ```
   Theme:        i-synergy-{theme}
   Base color:   #XXXXXX
   Light mode:   accent #XXXXXX · hover #XXXXXX
   Dark mode:    accent #XXXXXX · hover #XXXXXX
   File updated: src/ISynergy.AppHost/config/keycloak/themes/i-synergy-{theme}/login/resources/css/login.css
   ```

---

## Handling logo/icon images

If the user provides image files:

- **Identify the brand color**: Read each image visually. The dominant non-white, non-grey color in the icon/swirl is the base hex.
- **Logo files** belong in:
  ```
  src/ISynergy.AppHost/config/keycloak/themes/i-synergy-{theme}/login/resources/img/
  ```
  Expected filenames: `logo_light.png` (dark text, light bg), `logo_dark.png` (light text, dark bg), `icon.png`.
- **Logo dimensions**: Current default is `280px × 35px`. If the new logo has significantly different proportions, adjust `--b-logo-w` and `--b-logo-h` accordingly (or ask the user).

---

## Full token reference (both blocks for copy-reference)

### `:root` (light mode) — only color tokens change:
```css
--b-accent:       {light-accent};   /* {Brand} brand color */
--b-accent-dim:   {light-dim};
--b-accent-hover: {light-hover};
/* PatternFly 5 */
--pf-v5-global--primary-color--100:  {light-accent};
--pf-v5-global--primary-color--200:  {light-hover};
--pf-v5-global--link--Color:         {light-accent};
--pf-v5-global--link--Color--hover:  {light-hover};
/* PatternFly 4 */
--pf-global--primary-color--100:     {light-accent};
--pf-global--primary-color--200:     {light-hover};
--pf-global--link--Color:            {light-accent};
--pf-global--link--Color--hover:     {light-hover};
```

### `@media (prefers-color-scheme: dark) :root` — only color tokens change:
```css
--b-accent:       {dark-accent};
--b-accent-dim:   {dark-dim};
--b-accent-hover: {dark-hover};
/* PatternFly 5 */
--pf-v5-global--primary-color--100:  {dark-accent};
--pf-v5-global--primary-color--200:  {dark-hover};
--pf-v5-global--link--Color:         {dark-accent};
--pf-v5-global--link--Color--hover:  {dark-hover};
/* PatternFly 4 */
--pf-global--primary-color--100:     {dark-accent};
--pf-global--primary-color--200:     {dark-hover};
--pf-global--link--Color:            {dark-accent};
--pf-global--link--Color--hover:     {dark-hover};
```
