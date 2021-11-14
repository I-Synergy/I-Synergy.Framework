namespace ISynergy.Framework.Financial
{
    /// <summary>
    /// Calculations regarding banking.
    /// </summary>
    public static class Banking
    {
        /// <summary>
        /// If the outcome is equal to zero, this means that the division by number 11 did not yield a residual value.
        /// If there is a residual value(so something remains left after all 11 parts are fully distributed), this number does not meet the eleven test.
        /// </summary>
        /// <param name="nineCharactersLongNumber">The nine characters long number.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ElevenTest(string nineCharactersLongNumber)
        {
            if (nineCharactersLongNumber.Length != 9)
            {
                return false;
            }

            if (!nineCharactersLongNumber.IsInteger())
            {
                return false;
            }

            var NumTotal = 0;

            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(0, 1)) * 9;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(1, 1)) * 8;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(2, 1)) * 7;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(3, 1)) * 6;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(4, 1)) * 5;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(5, 1)) * 4;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(6, 1)) * 3;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(7, 1)) * 2;
            NumTotal += Convert.ToInt32(nineCharactersLongNumber.Substring(8, 1)) * 1;

            double CheckNum = NumTotal % 11;

            if (CheckNum != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
