using ISynergy.Framework.AspNetCore.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Authentication.Validators
{
    /// <summary>
    /// Class IdentityPasswordValidator.
    /// Implements the <see cref="PasswordValidator{TUser}" />
    /// </summary>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    /// <seealso cref="PasswordValidator{TUser}" />
    public class IdentityPasswordValidator<TUser> : PasswordValidator<TUser>
        where TUser : class
    {
        /// <summary>
        /// The options
        /// </summary>
        private readonly IdentityPasswordOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityPasswordValidator{TUser}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public IdentityPasswordValidator(IOptions<IdentityPasswordOptions> options)
        {
            this.options = options?.Value ?? new IdentityPasswordOptions();
        }

        /// <summary>
        /// validate as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns>IdentityResult.</returns>
        public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var result = await base.ValidateAsync(manager, user, password).ConfigureAwait(false);

            if (!result.Succeeded)
                return result;

            if (options.RequiredRegexMatch is null || options.RequiredRegexMatch.IsMatch(password))
                return IdentityResult.Success;

            // Todo: Move to IdentityErrorDescriber.
            var error = new IdentityError()
            {
                Code = "PasswordRequirementsFailed",
                Description = "Password does not comply with requirements."
            };
            return IdentityResult.Failed(error);
        }
    }
}
