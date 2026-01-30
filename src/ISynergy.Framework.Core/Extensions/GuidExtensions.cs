namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class GuidExtensions.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    /// Converts to uint.
    /// </summary>
    /// <param name="_self">The unique identifier.</param>
    /// <returns>System.UInt32.</returns>
    public static int ToInt(this Guid _self)
    {
        var value = _self.ToByteArray();
        return BitConverter.ToInt32(value, 0);
    }
}
