namespace ISynergy.Framework.Core.Models.Accounts
{
    public class RegistrationResult
    {
        public string UserId { get; }
        public string Account { get; }
        public string Email { get; }
        public string Token { get; }

        public RegistrationResult(string userId, string account, string email, string token)
        {
            UserId = userId;
            Account = account;
            Email = email;
            Token = token;
        }
    }
}
