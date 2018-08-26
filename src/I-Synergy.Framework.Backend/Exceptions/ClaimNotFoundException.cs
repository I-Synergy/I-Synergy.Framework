namespace ISynergy.Exceptions
{
    public class ClaimNotFoundException : ClaimAuthorizationException
    {
        public ClaimNotFoundException(string claimType)
            : base($"Claim '{claimType}' not found.")
        {
        }
    }
}