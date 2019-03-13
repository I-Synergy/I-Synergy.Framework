using ISynergy.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ISynergy.Validators
{
    public class IdentityPasswordValidator<TUser> : PasswordValidator<TUser>
        where TUser : class
    {
        private readonly IdentityPasswordOptions options;

        public IdentityPasswordValidator(IOptions<IdentityPasswordOptions> options)
        {
            this.options = options?.Value ?? new IdentityPasswordOptions();
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var result = await base.ValidateAsync(manager, user, password);

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
