/**
 * Applies the application theme to the document element.
 * Sets the data-theme attribute and CSS custom properties for accent colors.
 * @param {string} theme - The theme to apply: 'light' or 'dark'
 * @param {string} accentColor - The accent color as a hex string (e.g., '#0078D7')
 */
export function applyTheme(theme, accentColor) {
    if (!document?.documentElement) {
        console.warn('Document element not available for theme application');
        return;
    }

    // Set the data-theme attribute on the document element
    document.documentElement.setAttribute('data-theme', theme || 'light');

    // Apply accent color CSS custom properties
    if (accentColor) {
        // Normalize hex color (remove # if present)
        const normalizedColor = accentColor.startsWith('#') ? accentColor : '#' + accentColor;

        // Set the primary accent color
        document.documentElement.style.setProperty('--accent-color', normalizedColor);

        // Optionally set derived shades (light and dark variants)
        // These can be calculated via CSS calc() or computed via JavaScript
        // For now, we just set the primary color and let CSS handle the variants

        console.debug(`Theme applied: ${theme}, accent color: ${normalizedColor}`);
    }
}

/**
 * Gets the current theme from the document element's data-theme attribute.
 * @returns {string} The current theme ('light' or 'dark') or 'light' if not set
 */
export function getCurrentTheme() {
    return document?.documentElement?.getAttribute('data-theme') || 'light';
}
