namespace ISynergy.Helpers
{
    public class EnviromentalSettings
    {
        public enum Environments
        {
            development,
            test,
            release
        }

        public Environments Environment { get; set; }
    }
}