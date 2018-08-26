namespace ISynergy.Exceptions
{
    public class InvalidClaimValueException : ClaimAuthorizationException
    {
        public InvalidClaimValueException(string claimType)
            : base($"Claim '{claimType}' is found, but has an invalid value.")
        {
        }
    }
}