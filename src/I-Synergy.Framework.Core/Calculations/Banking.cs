using System;

namespace ISynergy.Calculations
{
    public class Bank
    {
        public static bool CheckAccount(string accountnumber)
        {
            if (accountnumber.Length != 9)
            {
                return false;
            }

            if (!accountnumber.IsInteger())
            {
                return false;
            }

            int NumTotal = 0;

            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(0, 1)) * 9;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(1, 1)) * 8;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(2, 1)) * 7;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(3, 1)) * 6;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(4, 1)) * 5;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(5, 1)) * 4;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(6, 1)) * 3;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(7, 1)) * 2;
            NumTotal = NumTotal + Convert.ToInt32(accountnumber.Substring(8, 1)) * 1;

            double CheckNum = NumTotal % 11;

            //is de uitkomst gelijk aan nul dan betekent dit dat de deling door het Num 11 geen restwaarde opleverde.
            //is er wel een rest waarde (dus blijft er nog iets over nadat alle 11 delen volledig zijn verdeeld), dan voldoet
            //dit bank rekening nummer niet aan de elf proef.

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