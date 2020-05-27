using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Models;

namespace ISynergy.Models
{
    /// <summary>
    /// UserInfo model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public sealed class Profile : ModelBase, IProfile
    {
        /// <summary>
        /// Default Constructor with token parameter.
        /// </summary>
        /// <param name="token">The token.</param>
        public Profile(Token token)
        {
            Token = token;

            try
            {
                var securityToken = new JwtSecurityToken(token.IdToken);

                AccountId = Guid.Parse(securityToken.Claims.Where(q => q.Type == ClaimTypes.AccountIdType).Select(q => q.Value).Single());
                AccountDescription = securityToken.Claims.Where(q => q.Type == ClaimTypes.AccountDescriptionType).Select(q => q.Value).Single();
                TimeZoneId = securityToken.Claims.Where(q => q.Type == ClaimTypes.TimeZoneType).Select(q => q.Value).SingleOrDefault() ?? "W. Europe Standard Time";
                UserId = Guid.Parse(securityToken.Claims.Where(q => q.Type == ClaimTypes.UserIdType).Select(q => q.Value).Single());
                Username = securityToken.Claims.Where(q => q.Type == ClaimTypes.UserNameType).Select(q => q.Value).Single();
                Email = securityToken.Claims.Where(q => q.Type == ClaimTypes.UserNameType).Select(q => q.Value).Single();
                LicenseExpration = DateTimeOffset.Parse(securityToken.Claims.Where(q => q.Type == ClaimTypes.LicenseExprationType).Select(q => q.Value).Single(), CultureInfo.InvariantCulture);
                LicenseUsers = int.Parse(securityToken.Claims.Where(q => q.Type == ClaimTypes.LicenseUsersType).Select(q => q.Value).Single());
                Roles = securityToken.Claims.Where(q => q.Type == ClaimTypes.RoleType).Select(q => q.Value).ToList();
                Modules = securityToken.Claims.Where(q => q.Type == ClaimTypes.ModulesType).Select(q => q.Value).ToList();
                Expiration = securityToken.ValidTo.ToLocalTime();

                IsAuthenticated = true;
            }
            catch (Exception)
            {
                IsAuthenticated = false;
            }
        }

        /// <summary>
        /// Constructor based on default constructor with username and token.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The token.</param>
        public Profile(string username, Token token)
            : this(token)
        {
            Username = username;
        }

        /// <summary>
        /// Gets or sets the Account_Id property value.
        /// </summary>
        /// <value>The account identifier.</value>
        public Guid AccountId
        {
            get { return GetValue<Guid>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Account_Description property value.
        /// </summary>
        /// <value>The account description.</value>
        public string AccountDescription
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TimeZoneId property value.
        /// </summary>
        /// <value>The time zone identifier.</value>
        public string TimeZoneId
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the UserId property value.
        /// </summary>
        /// <value>The user identifier.</value>
        public Guid UserId
        {
            get { return GetValue<Guid>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Username property value.
        /// </summary>
        /// <value>The username.</value>
        public string Username
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Email property value.
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Roles property value.
        /// </summary>
        /// <value>The roles.</value>
        public List<string> Roles
        {
            get { return GetValue<List<string>>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Modules property value.
        /// </summary>
        /// <value>The modules.</value>
        public List<string> Modules
        {
            get { return GetValue<List<string>>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the License_Expration property value.
        /// </summary>
        /// <value>The license expration.</value>
        public DateTimeOffset LicenseExpration
        {
            get { return GetValue<DateTimeOffset>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the License_Users property value.
        /// </summary>
        /// <value>The license users.</value>
        public int LicenseUsers
        {
            get { return GetValue<int>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Token property value.
        /// </summary>
        /// <value>The token.</value>
        public Token Token
        {
            get { return GetValue<Token>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Expiration property value.
        /// </summary>
        /// <value>The expiration.</value>
        public DateTime Expiration
        {
            get { return GetValue<DateTime>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsAuthenticated property value.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// Determines whether [is in role] [the specified role].
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns><c>true</c> if [is in role] [the specified role]; otherwise, <c>false</c>.</returns>
        public bool IsInRole(string role) => Roles?.Contains(role) ?? false;
    }
}
