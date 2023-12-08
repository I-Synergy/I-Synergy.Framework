using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Abstractions.Base;

/// <summary>
/// Interface IProperty
/// </summary>
public interface IProperty
{
    /// <summary>
    /// Occurs when [value changed].
    /// </summary>
    event EventHandler ValueChanged;

    /// <summary>
    /// Resets the changes.
    /// </summary>
    void ResetChanges();

    /// <summary>
    /// Marks as clean.
    /// </summary>
    void MarkAsClean();

    /// <summary>
    /// Gets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
    [JsonIgnore]
    bool IsDirty { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is original set.
    /// </summary>
    /// <value><c>true</c> if this instance is original set; otherwise, <c>false</c>.</value>
    [JsonIgnore]
    bool IsOriginalSet { get; }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    [JsonIgnore]
    string Name { get; }
}
