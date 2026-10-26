/**
 * Applies the application theme to the document element.
 * Sets the data-theme attribute and CSS custom properties for accent colors.
 * Supports the 'system' preference, which follows the operating system's color scheme.
 * @param {string} theme - The theme preference: 'light', 'dark' or 'system'
 * @param {string} accentColor - The accent color as a hex string (e.g., '#0078D7')
 * @returns {string} The resolved theme ('light' or 'dark')
 */
export function applyTheme(theme, accentColor) {
    currentPreference = theme || 'dark';
    currentAccentColor = accentColor || currentAccentColor;

    const resolvedTheme = resolveTheme(currentPreference);

    if (!document?.documentElement) {
        console.warn('Document element not available for theme application');
        return resolvedTheme;
    }

    applyResolvedTheme(resolvedTheme, currentAccentColor);

    console.debug(`Theme applied: ${currentPreference} (${resolvedTheme})`);

    return resolvedTheme;
}

/**
 * Gets the theme currently applied to the document element.
 * @returns {string} The resolved theme ('light' or 'dark') or 'light' if not set
 */
export function getCurrentTheme() {
    return document?.documentElement?.getAttribute('data-theme') || 'light';
}

/**
 * Watches the operating system color scheme while the 'system' preference is active.
 * Re-applies the palette and notifies .NET so the application state can follow.
 * @param {object} dotNetReference - A DotNetObjectReference with an OnSystemThemeChanged(bool) method
 */
export function watchSystemTheme(dotNetReference) {
    dotNetReferenceValue = dotNetReference;

    if (systemThemeQuery || !window.matchMedia) {
        return;
    }

    systemThemeQuery = window.matchMedia('(prefers-color-scheme: dark)');

    systemThemeListener = () => {
        if (currentPreference !== 'system') {
            return;
        }

        const resolvedTheme = systemThemeQuery.matches ? 'dark' : 'light';
        applyResolvedTheme(resolvedTheme, currentAccentColor);

        if (dotNetReferenceValue) {
            dotNetReferenceValue
                .invokeMethodAsync('OnSystemThemeChanged', resolvedTheme === 'dark')
                .catch(() => { /* the circuit is gone; nothing to notify */ });
        }
    };

    systemThemeQuery.addEventListener('change', systemThemeListener);
}

let currentPreference = 'dark';
let currentAccentColor = null;
let systemThemeQuery = null;
let systemThemeListener = null;
let dotNetReferenceValue = null;

function resolveTheme(preference) {
    if (preference === 'system') {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
            ? 'dark'
            : 'light';
    }

    return preference === 'light' ? 'light' : 'dark';
}

function applyResolvedTheme(theme, accentColor) {
    document.documentElement.setAttribute('data-theme', theme || 'light');

    if (accentColor) {
        // Normalize hex color (remove # if present)
        const normalizedColor = accentColor.startsWith('#') ? accentColor : '#' + accentColor;

        // Set the primary accent color
        document.documentElement.style.setProperty('--accent-color', normalizedColor);

        // Optionally set derived shades (light and dark variants)
        // These can be calculated via CSS calc() or computed via JavaScript
        // For now, we just set the primary color and let CSS handle the variants
    }
}
