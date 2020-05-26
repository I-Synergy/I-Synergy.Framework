namespace ISynergy.Framework.Core.Exceptions
{
    public class ApiException
    {
        public string Error { get; }
        public string Description { get; }

        public ApiException(string error, string description)
        {
            Error = error;
            Description = description;
        }
    }
}
