using System.Text;

namespace ISynergy
{
    public static class Gratification
    {
        public static string Gratify()
        {
            StringBuilder gratification = new StringBuilder();
            gratification.AppendLine("بِسمِ اللَّهِ الرَّحمٰنِ الرَّحيمِ");
            gratification.AppendLine("الحَمدُ لِلَّهِ رَبِّ العالَمينَ");
            gratification.AppendLine("الرَّحمٰنِ الرَّحيمِ");
            gratification.AppendLine("مالِكِ يَومِ الدّينِ");
            gratification.AppendLine("إِيّاكَ نَعبُدُ وَإِيّاكَ نَستَعينُ");
            gratification.AppendLine("اهدِنَا الصِّراطَ المُستَقيمَ");
            gratification.AppendLine("صِراطَ الَّذينَ أَنعَمتَ عَلَيهِم غَيرِ المَغضوبِ عَلَيهِم وَلَا الضّالّينَ");

            return gratification.ToString();
        }
    }
}