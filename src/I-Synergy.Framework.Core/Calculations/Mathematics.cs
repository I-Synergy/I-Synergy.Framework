namespace ISynergy.Calculations
{
    public static class Mathematics
    {
        /// <summary>
        /// Checkt of het getal even is
        /// </summary>
        /// <param name="number"></param>
        /// <returns>Boolean</returns>
        /// <remarks></remarks>
        public static bool IsEven(int number)
        {
            if (number % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}