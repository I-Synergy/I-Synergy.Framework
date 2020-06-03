using System;

namespace ISynergy.Framework.Core.Models.Accounts
{
    /// <summary>
    /// Class RegistrationResult.
    /// </summary>
    public class RegistrationResult
    {
        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public Guid UserId { get; }
        /// <summary>
        /// Gets the account.
        /// </summary>
        /// <value>The account.</value>
        public string Account { get; }
        /// <summary>
        /// Gets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; }
        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationResult"/> class.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="account">The account.</param>
        /// <param name="email">The email.</param>
        /// <param name="token">The token.</param>
        public RegistrationResult(Guid userId, string account, string email, string token)
        {
            UserId = userId;
            Account = account;
            Email = email;
            Token = token;
        }
    }
}
