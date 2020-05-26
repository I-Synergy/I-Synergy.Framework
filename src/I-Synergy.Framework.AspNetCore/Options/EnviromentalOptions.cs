namespace ISynergy.Framework.AspNetCore.Options
{
    public class EnviromentalOptions
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