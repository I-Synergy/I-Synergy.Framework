using System.Security.Cryptography;
using System.Text;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class IntegerExtensions.
/// </summary>
public static class IntegerExtensions
{
    /// <summary>
    /// Converts to guid.
    /// </summary>
    /// <param name="_self">The value.</param>
    /// <returns>Guid.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Unsigned integer is greater than 24bit</exception>
    public static Guid ToGuid(this int _self)
    {
        var result = new byte[16];
        BitConverter.GetBytes(_self).CopyTo(result, 0);
        return new Guid(result);
    }

    /// <summary>
    /// Generates the alpha numeric key.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <returns>System.String.</returns>
    public static string GenerateAlphaNumericKey(this int _self)
    {
        if (_self <= 0)
            return string.Empty;
        const string rawChars = "23456789abcdefghjkmnpqrstuwvxyzABCDEFGHJKMNPQRSTUVWXYZ";
        var result = new StringBuilder(_self);
        for (var i = 0; i < _self; i++)
            result.Append(rawChars[RandomNumberGenerator.GetInt32(rawChars.Length)]);
        return result.ToString();
    }

    /// <summary>
    /// Generates the numeric key.
    /// </summary>
    /// <param name="_self">The self.</param>
    /// <returns>System.String.</returns>
    public static string GenerateNumericKey(this int _self)
    {
        if (_self <= 0)
            return string.Empty;
        const string rawChars = "0123456789";
        var result = new StringBuilder(_self);
        for (var i = 0; i < _self; i++)
            result.Append(rawChars[RandomNumberGenerator.GetInt32(rawChars.Length)]);
        return result.ToString();
    }

    /// <summary>
    /// Converts int to uint.
    /// </summary>
    /// <param name="_self">The value.</param>
    /// <returns>uint</returns>
    public static uint ToUInt(this int _self) =>
        unchecked((uint)(_self - int.MinValue));

    /// <summary>
    /// Converts uint to int.
    /// </summary>
    /// <param name="_self"></param>
    /// <returns>int</returns>
    public static int ToInt(this uint _self) =>
        unchecked((int)_self + int.MinValue);
}
