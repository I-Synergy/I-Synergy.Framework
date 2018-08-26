namespace ISynergy.Exceptions
{
    public class DuplicateClaimException : ClaimAuthorizationException
    {
        public DuplicateClaimException(string claimType)
            : base($"Claim '{claimType}' is found multiple times, while it's only supported once.")
        {
        }
    }
}