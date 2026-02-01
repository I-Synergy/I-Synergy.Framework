namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class FreeAttribute. This class cannot be inherited.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class FreeAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether this application is free to use.
    /// </summary>
    /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
    public bool IsFree { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeAttribute" /> class.
    /// </summary>
    /// <param name="isFree">if set to <c>true</c> this application is free to use.</param>
    public FreeAttribute(bool isFree)
    {
        IsFree = isFree;
    }
}
