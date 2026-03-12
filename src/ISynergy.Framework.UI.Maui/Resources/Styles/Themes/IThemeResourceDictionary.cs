namespace ISynergy.Framework.UI.Resources.Styles.Themes;

/// <summary>
/// Marker interface implemented by all theme <see cref="Microsoft.Maui.Controls.ResourceDictionary"/>
/// subclasses in this namespace. Used by <c>ApplicationExtensions.SetApplicationColor</c> to identify
/// theme dictionaries without a fragile <c>GetType().Namespace</c> string comparison, which is
/// robust under IL trimming.
/// </summary>
public interface IThemeResourceDictionary
{
}
