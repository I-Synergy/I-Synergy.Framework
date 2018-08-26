using ISynergy.Extensions;
using System;
using System.Collections.Generic;

namespace ISynergy.Common
{
    public static class General
    {
        public static int Age(DateTime date)
        {
            int result = DateTime.Now.Year - date.Year;

            if (DateTime.Now.Month < date.Month || (DateTime.Now.Month == date.Month && DateTime.Now.Day < date.Day))
                result--;

            return result;
        }

        public static int AgeInDays(DateTime date)
        {
            double result = (DateTime.Now - date).TotalDays;
            return Convert.ToInt32(Math.Floor(result));
        }

        public static bool IsFloat(string value)
        {
            try
            {
                float output;
                return float.TryParse(value, out output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsDecimal(string value)
        {
            try
            {
                decimal output;
                return decimal.TryParse(value, out output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsInteger(string value)
        {
            try
            {
                int output;
                return int.TryParse(value, out output);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string IncreaseString2Long(string source, long summand)
        {
            string numberString = string.Empty;
            int codePos = source.Length;

            // Go back from end of id to the begin while a char is a number
            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (Char.IsDigit(source.Substring(i, 1).ToCharArray()[0]))
                {
                    // if found char isdigit set the position one element back
                    codePos--;
                }
                else
                {
                    // if we found a char we can break up
                    break;
                }
            }

            if (codePos < source.Length)
            {
                // the for-loop has found at least one numeric char at the end
                numberString = source.Substring(codePos, source.Length - codePos);
            }

            if (numberString.Length == 0)
            {
                // no number was found at the and so we simply add the summand as string
                return source + summand;
            }
            else
            {
                long num = long.Parse(numberString) + summand;
                return source.Substring(0, codePos) + num.ToString();
            }
        }

        public static string WordWrap(string strText, int iWidth)
        {
            if (strText.Length <= iWidth)
                return strText;
            // dont need to do anything

            string sResult = strText;
            string sChar = null;
            // temp holder for current string char
            int iEn = 0;
            int iLineNO = iWidth;

            while (sResult.Length >= iLineNO)
            {
                // work backwards from the max len to 1 looking for a space
                for (iEn = iLineNO; iEn >= 1; iEn += -1)
                {
                    sChar = sResult.Substring(iEn, 1);
                    // found a space
                    if (sChar == " ")
                    {
                        sResult = sResult.Remove(iEn, 1);
                        // Remove the space
                        sResult = sResult.Insert(iEn, System.Environment.NewLine);
                        // insert a line feed here,
                        iLineNO += iWidth;
                        // increment
                        break;
                    }
                }
            }

            return sResult;
        }

        public static bool IsNegative(decimal decValue)
        {
            if (decValue < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int CovertString2Numeric(string strValue)
        {
            List<string> iChars = new List<string>();
            string strResult = string.Empty;

            foreach (string iChar in iChars.EnsureNotNull())
            {
                if (IsInteger(iChar) == true)
                {
                    strResult = strResult + iChar;
                }
            }

            return Convert.ToInt32(strResult);
        }

        public static string GenerateAlphaNumericKey(int iLength)
        {
            string vRawChars = "23456789abcdefghjkmnpqrstuwvxyzABCDEFGHJKMNPQRSTUVWXYZ";
            System.Text.StringBuilder vResult = new System.Text.StringBuilder();

            for (int i = 1; i <= iLength; i++)
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

        public static string GenerateNumericKey(int iLength)
        {
            string vRawChars = "0123456789";
            System.Text.StringBuilder vResult = new System.Text.StringBuilder();

            for (int i = 1; i <= iLength; i++)
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

        //Linked list style enumerator
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
    }
}