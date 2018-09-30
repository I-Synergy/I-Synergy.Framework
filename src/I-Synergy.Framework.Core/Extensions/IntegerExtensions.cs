using ISynergy.Encryption;
using ISynergy.Utilities;

namespace System
{
    public static class IntegerExtensions
    {
        public static Guid ToGuid(this uint value)
        {
            if (value >= Math.Pow(2, 24))
                throw new ArgumentOutOfRangeException("Unsigned integer is greater than 24bit");

            string strGuid = Guid.NewGuid().ToString().Remove("-{}"); //Remove any '-' and '{}' characters
            byte[] bytes = BitConverter.GetBytes(value);
            var encryptedarray = Cypher.EncryptDES(bytes, Cypher.Key);
            string EncryptedGuid = ByteUtility.WriteBytesToString(strGuid, encryptedarray, 9);
            Guid.TryParse(EncryptedGuid, out Guid outg);
            return outg;
        }

        public static string GenerateAlphaNumericKey(this int self)
        {
            string vRawChars = "23456789abcdefghjkmnpqrstuwvxyzABCDEFGHJKMNPQRSTUVWXYZ";
            System.Text.StringBuilder vResult = new System.Text.StringBuilder();

            for (int i = 1; i <= self; i++)
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

        public static string GenerateNumericKey(this int self)
        {
            string vRawChars = "0123456789";
            System.Text.StringBuilder vResult = new System.Text.StringBuilder();

            for (int i = 1; i <= self; i++)
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
