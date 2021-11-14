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
        /// <param name="_self">The self.</param>
        /// <returns>System.String.</returns>
        public static string GenerateAlphaNumericKey(this int _self)
        {
            var rawChars = "23456789abcdefghjkmnpqrstuwvxyzABCDEFGHJKMNPQRSTUVWXYZ";
            var result = new StringBuilder();

            for (var i = 1; i <= _self; i++)
            {
                result.Append(rawChars.Trim().Substring(
                    Convert.ToInt32(
                            new Random().Next(int.MaxValue) * (rawChars.Length - 1))
                        , 1)
                    );
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates the numeric key.
        /// </summary>
        /// <param name="_self">The self.</param>
        /// <returns>System.String.</returns>
        public static string GenerateNumericKey(this int _self)
        {
            var rawChars = "0123456789";
            var result = new StringBuilder();

            for (var i = 1; i <= _self; i++)
            {
                result.Append(rawChars.Trim().Substring(
                    Convert.ToInt32(
                        new Random().Next(int.MaxValue) * (rawChars.Length - 1))
                    , 1)
                );
            }

            return result.ToString();
        }
    }
}
