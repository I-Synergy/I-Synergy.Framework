using System;

namespace ISynergy.Framework.Core.Extensions
{
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
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string GenerateAlphaNumericKey(this int self)
        {
            var vRawChars = "23456789abcdefghjkmnpqrstuwvxyzABCDEFGHJKMNPQRSTUVWXYZ";
            var vResult = new System.Text.StringBuilder();

            for (var i = 1; i <= self; i++)
            {
                vResult.Append(vRawChars.Trim().Substring(
                    Convert.ToInt32(
                            new Random().Next(int.MaxValue) * (vRawChars.Length - 1)
                            )
                        , 1)
                    );
            }

            return vResult.ToString();
        }

        /// <summary>
        /// Generates the numeric key.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string GenerateNumericKey(this int self)
        {
            var vRawChars = "0123456789";
            var vResult = new System.Text.StringBuilder();

            for (var i = 1; i <= self; i++)
            {
                vResult.Append(vRawChars.Trim().Substring(
                    Convert.ToInt32(
                        new Random().Next(int.MaxValue) * (vRawChars.Length - 1)
                        )
                    , 1)
                );
            }

            return vResult.ToString();
        }
    }
}
