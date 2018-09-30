namespace System
{
    public static class DecimalExtensions
    {
        public static bool IsNegative(this decimal self)
        {
            if (self < 0)
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
