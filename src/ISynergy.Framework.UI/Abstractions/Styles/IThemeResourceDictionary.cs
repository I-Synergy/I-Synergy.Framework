namespace ISynergy.Framework.UI.Abstractions.Styles;

/// <summary>
/// Used by ApplicationExtensions.SetApplicationColor to identify and remove existing theme
/// dictionaries without a fragile GetType().Namespace string comparison.
/// </summary>
public interface IThemeResourceDictionary { }
