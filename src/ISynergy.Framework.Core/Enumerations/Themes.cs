using ISynergy.Framework.Core.Attributes;

namespace ISynergy.Framework.Core.Enumerations;

/// <summary>
/// Theme enumeration
/// </summary>
public enum Themes
{
    /// <summary>
    /// System default
    /// </summary>
    [LocalizedDescription("System")]
    Default = 0,
    /// <summary>
    /// Light theme.
    /// </summary>
    [LocalizedDescription("Light")]
    Light = 1,
    /// <summary>
    /// Dark theme.
    /// </summary>
    [LocalizedDescription("Dark")]
    Dark = 2
}
